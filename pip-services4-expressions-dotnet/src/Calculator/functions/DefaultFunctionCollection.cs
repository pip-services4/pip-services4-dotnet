using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PipServices4.Expressions.Variants;
using System.Linq;

namespace PipServices4.Expressions.Calculator.Functions
{
    /// <summary>
    /// Implements a list filled with standard functions.
    /// </summary>
    public class DefaultFunctionCollection : FunctionCollection
    {
        private static Random _random = new Random();

        /// <summary>
        /// Constructs this list and fills it with the standard functions.
        /// </summary>
        public DefaultFunctionCollection()
        {
            Add(new DelegatedFunction("Ticks", (FunctionCalculator)TicksFunctionCalculatorAsync));
            Add(new DelegatedFunction("TimeSpan", (FunctionCalculator)TimeSpanFunctionCalculatorAsync));
            Add(new DelegatedFunction("TimeOfDay", (FunctionCalculator)TimeOfDayFunctionCalculatorAsync));
            Add(new DelegatedFunction("Now", (FunctionCalculator)NowFunctionCalculatorAsync));
            Add(new DelegatedFunction("Date", (FunctionCalculator)DateFunctionCalculatorAsync));
            Add(new DelegatedFunction("DayOfWeek", (FunctionCalculator)DayOfWeekFunctionCalculatorAsync));
            Add(new DelegatedFunction("Min", (FunctionCalculator)MinFunctionCalculatorAsync));
            Add(new DelegatedFunction("Max", (FunctionCalculator)MaxFunctionCalculatorAsync));
            Add(new DelegatedFunction("Sum", (FunctionCalculator)SumFunctionCalculatorAsync));
            Add(new DelegatedFunction("If", (FunctionCalculator)IfFunctionCalculatorAsync));
            Add(new DelegatedFunction("Choose", (FunctionCalculator)ChooseFunctionCalculatorAsync));
            Add(new DelegatedFunction("E", (FunctionCalculator)EFunctionCalculatorAsync));
            Add(new DelegatedFunction("Pi", (FunctionCalculator)PiFunctionCalculatorAsync));
            Add(new DelegatedFunction("Rnd", (FunctionCalculator)RndFunctionCalculatorAsync));
            Add(new DelegatedFunction("Random", (FunctionCalculator)RndFunctionCalculatorAsync));
            Add(new DelegatedFunction("Abs", (FunctionCalculator)AbsFunctionCalculatorAsync));
            Add(new DelegatedFunction("Acos", (FunctionCalculator)AcosFunctionCalculatorAsync));
            Add(new DelegatedFunction("Asin", (FunctionCalculator)AsinFunctionCalculatorAsync));
            Add(new DelegatedFunction("Atan", (FunctionCalculator)AtanFunctionCalculatorAsync));
            Add(new DelegatedFunction("Exp", (FunctionCalculator)ExpFunctionCalculatorAsync));
            Add(new DelegatedFunction("Log", (FunctionCalculator)LogFunctionCalculatorAsync));
            Add(new DelegatedFunction("Ln", (FunctionCalculator)LogFunctionCalculatorAsync));
            Add(new DelegatedFunction("Log10", (FunctionCalculator)Log10FunctionCalculatorAsync));
            Add(new DelegatedFunction("Ceil", (FunctionCalculator)CeilFunctionCalculatorAsync));
            Add(new DelegatedFunction("Ceiling", (FunctionCalculator)CeilFunctionCalculatorAsync));
            Add(new DelegatedFunction("Floor", (FunctionCalculator)FloorFunctionCalculatorAsync));
            Add(new DelegatedFunction("Round", (FunctionCalculator)RoundFunctionCalculatorAsync));
            Add(new DelegatedFunction("Trunc", (FunctionCalculator)TruncFunctionCalculatorAsync));
            Add(new DelegatedFunction("Truncate", (FunctionCalculator)TruncFunctionCalculatorAsync));
            Add(new DelegatedFunction("Cos", (FunctionCalculator)CosFunctionCalculatorAsync));
            Add(new DelegatedFunction("Sin", (FunctionCalculator)SinFunctionCalculatorAsync));
            Add(new DelegatedFunction("Tan", (FunctionCalculator)TanFunctionCalculatorAsync));
            Add(new DelegatedFunction("Sqr", (FunctionCalculator)SqrtFunctionCalculatorAsync));
            Add(new DelegatedFunction("Sqrt", (FunctionCalculator)SqrtFunctionCalculatorAsync));
            Add(new DelegatedFunction("Empty", (FunctionCalculator)EmptyFunctionCalculatorAsync));
            Add(new DelegatedFunction("Null", (FunctionCalculator)NullFunctionCalculatorAsync));
            Add(new DelegatedFunction("Contains", (FunctionCalculator)ContainsFunctionCalculatorAsync));
            Add(new DelegatedFunction("Array", (FunctionCalculator)ArrayFunctionCalculatorAsync));
        }

        /// <summary>
        /// Checks if parameters contains the correct number of function parameters (must be stored on the top of the parameters).
        /// </summary>
        /// <param name="parameters">A list with function parameters.</param>
        /// <param name="expectedParamCount">The expected number of function parameters.</param>
        protected static void CheckParamCount(IList<Variant> parameters, int expectedParamCount)
        {
            int paramCount = parameters.Count;
            if (expectedParamCount != paramCount)
            {
                throw new ExpressionException(null, "WRONG_PARAM_COUNT",
                    String.Format("Expected {0} parameters but was found {1}",
                    expectedParamCount, paramCount));
            }
        }

        /// <summary>
        /// Gets function parameter by it's index.
        /// </summary>
        /// <param name="parameters">A list with function parameters.</param>
        /// <param name="paramIndex">Index for the function parameter (0 for the first parameter).</param>
        /// <returns>Function parameter value.</returns>
        protected static Variant GetParameter(IList<Variant> parameters, int paramIndex)
        {
            return parameters[paramIndex];
        }

        private Task<Variant> TicksFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 0);
            Variant result = new Variant((System.DateTime.Now.Ticks - 621355968000000000) / 10000);
            return Task.FromResult(result);
        }

        private Task<Variant> TimeSpanFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            int paramCount = parameters.Count;
            if (paramCount != 1 && paramCount != 3 && paramCount != 4 && paramCount != 5)
            {
                throw new ExpressionException(null, "WRONG_PARAM_COUNT", "Expected 1, 3, 4 or 5 parameters");
            }

            Variant result = new Variant();

            if (paramCount == 1)
            {
                Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Long);
                result.AsTimeSpan = new TimeSpan(value.AsLong);
            }
            else if (paramCount > 2)
            {
                Variant value1 = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Integer);
                Variant value2 = variantOperations.Convert(GetParameter(parameters, 1), VariantType.Integer);
                Variant value3 = variantOperations.Convert(GetParameter(parameters, 2), VariantType.Integer);
                Variant value4 = paramCount > 3 ? variantOperations.Convert(GetParameter(parameters, 3), VariantType.Integer) : Variant.FromInteger(0);
                Variant value5 = paramCount > 4 ? variantOperations.Convert(GetParameter(parameters, 4), VariantType.Integer) : Variant.FromInteger(0);

                result.AsTimeSpan = new TimeSpan(value1.AsInteger, value2.AsInteger, value3.AsInteger, value4.AsInteger, value5.AsInteger);
            }
         
            return Task.FromResult(result);
        }

        private Task<Variant> TimeOfDayFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            int paramCount = parameters.Count;
            if (!(paramCount == 1 || paramCount == 3))
            {
                throw new ExpressionException(null, "WRONG_PARAM_COUNT", "Expected 1 or 3 parameters");
            }

            Variant result = new Variant();

            if (paramCount == 1)
            {
                Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.DateTime);
                result.AsTimeSpan = value.AsDateTime.TimeOfDay;
            }
            else if (paramCount == 3)
            {
                Variant value1 = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Integer);
                Variant value2 = variantOperations.Convert(GetParameter(parameters, 1), VariantType.Integer);
                Variant value3 = variantOperations.Convert(GetParameter(parameters, 2), VariantType.Integer);

                result.AsTimeSpan = new TimeSpan(value1.AsInteger, value2.AsInteger, value3.AsInteger);
            }

            return Task.FromResult(result);
        }

        private Task<Variant> NowFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 0);
            Variant result = new Variant(System.DateTime.Now);
            return Task.FromResult(result);
        }

        private Task<Variant> DateFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            int paramCount = parameters.Count;
            if (paramCount < 1 || paramCount > 7)
            {
                throw new ExpressionException(null, "WRONG_PARAM_COUNT", "Expected from 1 to 7 parameters");
            }

            if (paramCount == 1)
            {
                Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Long);
                Variant result1 = Variant.FromDateTime(new DateTime(value.AsLong));
                return Task.FromResult(result1);
            }

            Variant value1 = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Integer);
            Variant value2 = paramCount > 1 ? variantOperations.Convert(GetParameter(parameters, 1), VariantType.Integer) : Variant.FromInteger(1);
            Variant value3 = paramCount > 2 ? variantOperations.Convert(GetParameter(parameters, 2), VariantType.Integer) : Variant.FromInteger(1);
            Variant value4 = paramCount > 3 ? variantOperations.Convert(GetParameter(parameters, 3), VariantType.Integer) : Variant.FromInteger(0);
            Variant value5 = paramCount > 4 ? variantOperations.Convert(GetParameter(parameters, 4), VariantType.Integer) : Variant.FromInteger(0);
            Variant value6 = paramCount > 5 ? variantOperations.Convert(GetParameter(parameters, 5), VariantType.Integer) : Variant.FromInteger(0);
            Variant value7 = paramCount > 6 ? variantOperations.Convert(GetParameter(parameters, 6), VariantType.Integer) : Variant.FromInteger(0);

            DateTime date = new DateTime(value1.AsInteger, value2.AsInteger, value3.AsInteger,
                value4.AsInteger, value5.AsInteger, value6.AsInteger, value7.AsInteger);
            Variant result = Variant.FromDateTime(date);
            return Task.FromResult(result);
        }

        private Task<Variant> DayOfWeekFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.DateTime);
            Variant result = Variant.FromInteger((int)value.AsDateTime.DayOfWeek);
            return Task.FromResult(result);
        }

        private Task<Variant> MinFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            int paramCount = parameters.Count;
            if (paramCount < 2)
            {
                throw new ExpressionException(null, "WRONG_PARAM_COUNT", "Expected at least 2 parameters");
            }
            Variant result = GetParameter(parameters, 0);
            for (int i = 1; i < paramCount; i++)
            {
                Variant value = GetParameter(parameters, i);
                if (variantOperations.More(result, value).AsBoolean)
                {
                    result = value;
                }
            }
            return Task.FromResult(result);
        }

        private Task<Variant> MaxFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            int paramCount = parameters.Count;
            if (paramCount < 2)
            {
                throw new ExpressionException(null, "WRONG_PARAM_COUNT", "Expected at least 2 parameters");
            }
            Variant result = GetParameter(parameters, 0);
            for (int i = 1; i < paramCount; i++)
            {
                Variant value = GetParameter(parameters, i);
                if (variantOperations.Less(result, value).AsBoolean)
                {
                    result = value;
                }
            }
            return Task.FromResult(result);
        }

        private Task<Variant> SumFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            int paramCount = parameters.Count;
            if (paramCount < 2)
            {
                throw new ExpressionException(null, "WRONG_PARAM_COUNT", "Expected at least 2 parameters");
            }
            Variant result = GetParameter(parameters, 0);
            for (int i = 1; i < paramCount; i++)
            {
                Variant value = GetParameter(parameters, i);
                result = variantOperations.Add(result, value);
            }
            return Task.FromResult(result);
        }

        private Task<Variant> IfFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 3);
            Variant value1 = GetParameter(parameters, 0);
            Variant value2 = GetParameter(parameters, 1);
            Variant value3 = GetParameter(parameters, 2);
            Variant condition = variantOperations.Convert(value1, VariantType.Boolean);
            Variant result = condition.AsBoolean ? value2 : value3;
            return Task.FromResult(result);
        }

        private Task<Variant> ChooseFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            int paramCount = parameters.Count;
            if (paramCount < 3)
            {
                throw new ExpressionException(null, "WRONG_PARAM_COUNT", "Expected at least 3 parameters");
            }

            Variant value1 = GetParameter(parameters, 0);
            Variant condition = variantOperations.Convert(value1, VariantType.Integer);
            int paramIndex = condition.AsInteger;

            if (paramCount < paramIndex + 1)
            {
                throw new ExpressionException(null, "WRONG_PARAM_COUNT", string.Format("Expected at least {0} parameters",
                    paramIndex + 1));
            }

            Variant result = GetParameter(parameters, paramIndex);
            return Task.FromResult(result);
        }

        private Task<Variant> EFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 0);
            Variant result = new Variant(System.Math.E);
            return Task.FromResult(result);
        }

        private Task<Variant> PiFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 0);
            Variant result = new Variant(System.Math.PI);
            return Task.FromResult(result);
        }

        private Task<Variant> RndFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 0);
            Variant result = new Variant(_random.NextDouble());
            return Task.FromResult(result);
        }

        private Task<Variant> AbsFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = GetParameter(parameters, 0);
            Variant result = new Variant();
            switch (value.Type)
            {
                case VariantType.Integer:
                    result.AsInteger = System.Math.Abs(value.AsInteger);
                    break;
                case VariantType.Long:
                    result.AsLong = System.Math.Abs(value.AsLong);
                    break;
                case VariantType.Float:
                    result.AsFloat = System.Math.Abs(value.AsFloat);
                    break;
                case VariantType.Double:
                    result.AsDouble = System.Math.Abs(value.AsDouble);
                    break;
                default:
                    value = variantOperations.Convert(value, VariantType.Double);
                    result.AsDouble = System.Math.Abs(value.AsDouble);
                    break;
            }
            return Task.FromResult(result);
        }

        private Task<Variant> AcosFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Double);
            Variant result = new Variant(System.Math.Acos(value.AsDouble));
            return Task.FromResult(result);
        }

        private Task<Variant> AsinFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Double);
            Variant result = new Variant(System.Math.Asin(value.AsDouble));
            return Task.FromResult(result);
        }

        private Task<Variant> AtanFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Double);
            Variant result = new Variant(System.Math.Atan(value.AsDouble));
            return Task.FromResult(result);
        }

        private Task<Variant> ExpFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Double);
            Variant result = new Variant(System.Math.Exp(value.AsDouble));
            return Task.FromResult(result);
        }

        private Task<Variant> LogFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Double);
            Variant result = new Variant(System.Math.Log(value.AsDouble));
            return Task.FromResult(result);
        }

        private Task<Variant> Log10FunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Double);
            Variant result = new Variant(System.Math.Log10(value.AsDouble));
            return Task.FromResult(result);
        }

        private Task<Variant> CeilFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Double);
            Variant result = new Variant(System.Math.Ceiling(value.AsDouble));
            return Task.FromResult(result);
        }

        private Task<Variant> FloorFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Double);
            Variant result = new Variant(System.Math.Floor(value.AsDouble));
            return Task.FromResult(result);
        }

        private Task<Variant> RoundFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Double);
            Variant result = new Variant(System.Math.Round(value.AsDouble));
            return Task.FromResult(result);
        }

        private Task<Variant> TruncFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Double);
            Variant result = new Variant((int)value.AsDouble);
            return Task.FromResult(result);
        }

        private Task<Variant> CosFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Double);
            Variant result = new Variant(System.Math.Cos(value.AsDouble));
            return Task.FromResult(result);
        }

        private Task<Variant> SinFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Double);
            Variant result = new Variant(System.Math.Sin(value.AsDouble));
            return Task.FromResult(result);
        }

        private Task<Variant> TanFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Double);
            Variant result = new Variant(System.Math.Tan(value.AsDouble));
            return Task.FromResult(result);
        }

        private Task<Variant> SqrtFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = variantOperations.Convert(GetParameter(parameters, 0), VariantType.Double);
            Variant result = new Variant(System.Math.Sqrt(value.AsDouble));
            return Task.FromResult(result);
        }

        private Task<Variant> EmptyFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 1);
            Variant value = GetParameter(parameters, 0);
            Variant result = new Variant(value.IsEmpty());
            return Task.FromResult(result);
        }

        private Task<Variant> NullFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 0);
            Variant result = new Variant();
            return Task.FromResult(result);
        }

        private Task<Variant> ContainsFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            CheckParamCount(parameters, 2);
            Variant containerstr = variantOperations.Convert(GetParameter(parameters, 0), VariantType.String);
            Variant substring = variantOperations.Convert(GetParameter(parameters, 1), VariantType.String);

            if (containerstr.IsEmpty() || containerstr.IsNull())
            {
                return Task.FromResult(new Variant(false));
            }

            Variant result = new Variant(containerstr.AsString.IndexOf(substring.AsString) >= 0);
            return Task.FromResult(result);
        }

        private Task<Variant> ArrayFunctionCalculatorAsync(IList<Variant> parameters, IVariantOperations variantOperations)
        {
            return Task.FromResult(new Variant(parameters.ToArray()));
        }
    }
}
