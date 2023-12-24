using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PipServices4.Expressions.Calculator.Functions;
using PipServices4.Expressions.Calculator.Parsers;
using PipServices4.Expressions.Calculator.Variables;
using PipServices4.Expressions.Tokenizers;
using PipServices4.Expressions.Variants;

namespace PipServices4.Expressions.Calculator
{
    /// <summary>
    /// Implements an expression calculator class.
    /// </summary>
    public class ExpressionCalculator
    {
        private IVariableCollection _defaultVariables = new VariableCollection();
        private IFunctionCollection _defaultFunctions = new DefaultFunctionCollection();
        private IVariantOperations _variantOperations = new TypeUnsafeVariantOperations();
        private ExpressionParser _parser = new ExpressionParser();
        private bool _autoVariables = true;

        /// <summary>
        /// Constructs this class with default parameters.
        /// </summary>
        public ExpressionCalculator()
        {
        }

        /// <summary>
        /// Constructs this class and assigns expression string.
        /// </summary>
        /// <param name="expression">The expression string.</param>
        public ExpressionCalculator(string expression)
        {
            Expression = expression;
        }

        public ExpressionCalculator(IList<Token> originalTokens)
        {
            OriginalTokens = originalTokens;
        }

        /// <summary>
        /// The expression string.
        /// </summary>
        public string Expression
        {
            get
            {
                return _parser.Expression;
            }
            set
            {
                _parser.Expression = value;
                if (_autoVariables)
                {
                    CreateVariables(_defaultVariables);
                }
            }
        }

        public IList<Token> OriginalTokens
        {
            get { return _parser.OriginalTokens; }
            set
            {
                _parser.OriginalTokens = value;
                if (_autoVariables)
                {
                    CreateVariables(_defaultVariables);
                }
            }
        }

        /// <summary>
        /// Flag to turn on auto creation of variables for specified expression.
        /// </summary>
        public Boolean AutoVariables
        {
            get { return _autoVariables; }
            set { _autoVariables = value; }
        }

        /// <summary>
        /// Manager for operations on variant values.
        /// </summary>
        public IVariantOperations VariantOperations
        {
            get { return _variantOperations; }
            set { _variantOperations = value; }
        }

        /// <summary>
        /// The list with default variables.
        /// </summary>
        public IVariableCollection DefaultVariables
        {
            get { return _defaultVariables; }
        }

        /// <summary>
        /// The list with default functions.
        /// </summary>
        public IFunctionCollection DefaultFunctions
        {
            get { return _defaultFunctions; }
        }

        /// <summary>
        /// The list of original expression tokens.
        /// </summary>
        public IList<ExpressionToken> InitialTokens
        {
            get { return _parser.InitialTokens; }
        }

        /// <summary>
        /// The list of processed expression tokens.
        /// </summary>
        public IList<ExpressionToken> ResultTokens
        {
            get { return _parser.ResultTokens; }
        }

        /// <summary>
        /// Populates the specified variables list with variables from parsed expression.
        /// </summary>
        /// <param name="variables">The list of variables to be populated.</param>
        public void CreateVariables(IVariableCollection variables)
        {
            foreach (string variableName in _parser.VariableNames)
            {
                if (variables.FindByName(variableName) == null)
                {
                    variables.Add(new Variable(variableName));
                }
            }
        }

        /// <summary>
        /// Cleans up this calculator from all data.
        /// </summary>
        public void Clear()
        {
            _parser.Clear();
            _defaultVariables.Clear();
        }

        /// <summary>
        /// Evaluates this expression using default variables and functions.
        /// </summary>
        /// <returns>An evaluated expression value.</returns>
        public Task<Variant> EvaluateAsync()
        {
            return EvaluateUsingVariablesAndFunctionsAsync(null, null);
        }

        /// <summary>
        /// Evaluates this expression using specified variables.
        /// </summary>
        /// <param name="variables">The list of variables</param>
        /// <returns>An evaluated expression value.</returns>
        public Task<Variant> EvaluateUsingVariablesAsync(IVariableCollection variables)
        {
            return EvaluateUsingVariablesAndFunctionsAsync(variables, null);
        }

        /// <summary>
        /// Evaluates this expression using specified variables and functions.
        /// </summary>
        /// <param name="variables">The list of variables</param>
        /// <param name="functions">The list of functions.</param>
        /// <returns>An evaluated expression value.</returns>
        public async Task<Variant> EvaluateUsingVariablesAndFunctionsAsync(
            IVariableCollection variables, IFunctionCollection functions)
        {
            CalculationStack stack = new CalculationStack();
            variables = variables ?? _defaultVariables;
            functions = functions ?? _defaultFunctions;

            foreach (ExpressionToken token in ResultTokens)
            {
                if (EvaluateConstant(token, stack)) { }
                else if (EvaluateVariable(token, stack, variables)) { }
                else if (await EvaluateFunctionAsync(token, stack, functions)) { }
                else if (EvaluateLogical(token, stack)) { }
                else if (EvaluateArithmetical(token, stack)) { }
                else if (EvaluateBoolean(token, stack)) { }
                else if (EvaluateOther(token, stack)) { }
                else
                {
                    throw new ExpressionException(null, "INTERNAL", "Internal error.");
                }
            }

            if (stack.Length != 1)
            {
                throw new ExpressionException(null, "INTERNAL", "Internal error.");
            }
            return stack.Pop();
        }

        private static bool EvaluateConstant(ExpressionToken token, CalculationStack stack)
        {
            if (token.Type == ExpressionTokenType.Constant)
            {
                stack.Push(token.Value);
                return true;
            }
            return false;
        }

        private static bool EvaluateVariable(
            ExpressionToken token, CalculationStack stack, IVariableCollection variables)
        {
            if (token.Type == ExpressionTokenType.Variable)
            {
                IVariable variable = variables.FindByName(token.Value.AsString);
                if (variable == null)
                {
                    throw new ExpressionException(null, "VAR_NOT_FOUND",
                        String.Format("Variable {0} was not found.", token.Value.AsString));
                }
                stack.Push(variable.Value);
                return true;
            }
            return false;
        }

        private async Task<bool> EvaluateFunctionAsync(
            ExpressionToken token, CalculationStack stack, IFunctionCollection functions)
        {
            if (token.Type == ExpressionTokenType.Function)
            {
                IFunction function = functions.FindByName(token.Value.AsString);
                if (function == null)
                {
                    throw new ExpressionException(null, "FUNC_NOT_FOUND",
                        String.Format("Function {0} was not found.", token.Value.AsString));
                }

                // Prepare parameters
                var parameters = new List<Variant>();
                var paramCount = stack.Pop().AsInteger;
                while (paramCount > 0)
                {
                    parameters.Insert(0, stack.Pop());
                    paramCount--;
                }

                Variant functionResult = await function.CalculateAsync(parameters, _variantOperations);

                stack.Push(functionResult);

                return true;
            }
            return false;
        }

        private bool EvaluateLogical(ExpressionToken token, CalculationStack stack)
        {
            switch (token.Type)
            {
                case ExpressionTokenType.And:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.And(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.Or:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.Or(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.Xor:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.Xor(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.Not:
                    {
                        stack.Push(_variantOperations.Not(stack.Pop()));
                        return true;
                    }
            }
            return false;
        }

        private bool EvaluateArithmetical(ExpressionToken token, CalculationStack stack)
        {
            switch (token.Type)
            {
                case ExpressionTokenType.Plus:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.Add(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.Minus:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.Sub(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.Star:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.Mul(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.Slash:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.Div(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.Procent:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.Mod(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.Power:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.Pow(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.Unary:
                    {
                        stack.Push(_variantOperations.Negative(stack.Pop()));
                        return true;
                    }
                case ExpressionTokenType.ShiftLeft:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.Lsh(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.ShiftRight:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.Rsh(value1, value2));
                        return true;
                    }
            }
            return false;
        }

        private bool EvaluateBoolean(ExpressionToken token, CalculationStack stack)
        {
            switch (token.Type)
            {
                case ExpressionTokenType.Equal:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.Equal(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.NotEqual:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.NotEqual(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.More:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.More(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.Less:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.Less(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.EqualMore:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.MoreEqual(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.EqualLess:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.LessEqual(value1, value2));
                        return true;
                    }
            }
            return false;
        }

        private bool EvaluateOther(ExpressionToken token, CalculationStack stack)
        {
            switch (token.Type)
            {
                case ExpressionTokenType.In:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.In(value2, value1));
                        return true;
                    }
                case ExpressionTokenType.NotIn:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(new Variant(!_variantOperations.In(value2, value1).AsBoolean));
                        return true;
                    }
                case ExpressionTokenType.Element:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.GetElement(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.IsNull:
                    {
                        stack.Push(new Variant(stack.Pop().IsNull()));
                        return true;
                    }
                case ExpressionTokenType.IsNotNull:
                    {
                        stack.Push(new Variant(!stack.Pop().IsNull()));
                        return true;
                    }
                case ExpressionTokenType.Like:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(_variantOperations.Like(value1, value2));
                        return true;
                    }
                case ExpressionTokenType.NotLike:
                    {
                        Variant value2 = stack.Pop();
                        Variant value1 = stack.Pop();
                        stack.Push(new Variant(!_variantOperations.Like(value1, value2).AsBoolean));
                        return true;
                    }
            }
            return false;
        }

    }
}
