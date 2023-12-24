using System;
using System.Collections.Generic;
using System.Text;

using PipServices4.Expressions.Mustache.Parsers;
using PipServices4.Expressions.Tokenizers;

namespace PipServices4.Expressions.Mustache
{
    public class MustacheTemplate
    {
        private Dictionary<string, dynamic> _defaultVariables = new Dictionary<string, dynamic>();
        private MustacheParser _parser = new MustacheParser();
        private bool _autoVariables = true;

        /// <summary>
        /// Constructs this class and assigns mustache template.
        /// </summary>
        /// <param name="template">The mustache template.</param>
        public MustacheTemplate(string template = null)
        {
            if (template != null)
                Template = template;
        }

        /// <summary>
        /// The mustache template.
        /// </summary>
        public string Template
        {
            get { return _parser.Template; }
            set
            {
                _parser.Template = value;
                if (_autoVariables)
                    CreateVariables(_defaultVariables);
            }
        }

        public IList<Token> OriginalTokens
        {
            get { return _parser.OriginalTokens; }
            set
            {
                _parser.OriginalTokens = value;
                if (_autoVariables)
                    CreateVariables(_defaultVariables);
            }
        }

        /// <summary>
        /// Gets the flag to turn on auto creation of variables for specified mustache.
        /// </summary>
        public bool AutoVariables
        {
            get { return _autoVariables; }
            set { _autoVariables = value; }
        }

        /// <summary>
        /// The list with default variables.
        /// </summary>
        public Dictionary<string, dynamic> DefaultVariables
        {
            get { return _defaultVariables; }
        }

        /// <summary>
        /// The list of original mustache tokens.
        /// </summary>
        public IList<MustacheToken> InitialTokens
        {
            get { return _parser.InitialTokens; }
        }

        /// <summary>
        /// The list of processed mustache tokens.
        /// </summary>
        public IList<MustacheToken> ResultTokens
        {
            get { return _parser.ResultTokens; }
        }

        /// <summary>
        /// Gets a variable value from the collection of variables
        /// </summary>
        /// <param name="variables">a collection of variables.</param>
        /// <param name="name">a variable name to get.</param>
        /// <returns>a variable value or <code>null</code></returns>
        public dynamic GetVariable(Dictionary<string, dynamic> variables, string name)
        {
            if (variables == null || name == null) return null;

            name = name.ToLower();
            string result = null;

            foreach (string propName in variables.Keys)
            {
                if (propName.ToLower() == name)
                {
                    result = variables[propName] != null ? Convert.ToString(variables[propName]) : result;
                }
            }

            return result;
        }

        /// <summary>
        /// Populates the specified variables list with variables from parsed mustache.
        /// </summary>
        /// <param name="variables">The list of variables to be populated.</param>
        public void CreateVariables(Dictionary<string, dynamic> variables)
        {
            if (variables == null) return;

            foreach (string variableName in _parser.VariableNames)
            {
                bool found = GetVariable(variables, variableName) != null;
                if (!found)
                {
                    variables[variableName] = null;
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
        /// Evaluates this mustache template using default variables.
        /// </summary>
        /// <returns>the evaluated template</returns>
        public string Evaluate()
        {
            return EvaluateWithVariables(null);
        }

        /// <summary>
        /// Evaluates this mustache using specified variables.
        /// </summary>
        /// <param name="variables">The collection of variables</param>
        /// <returns>the evaluated template</returns>
        public string EvaluateWithVariables(Dictionary<string, dynamic> variables)
        {
            variables = variables != null ? variables : _defaultVariables;

            return EvaluateTokens(_parser.ResultTokens, variables);
        }

        private string EscapeString(string value)
        {
            return value.Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("/", "\\/")
                        .Replace("\b", "\\b")
                        .Replace("\f", "\\f")
                        .Replace("\n", "\\n")
                        .Replace("\r", "\\r")
                        .Replace("\t", "\\t");
        }

        private bool IsDefinedVariable(Dictionary<string, dynamic> variables, string name)
        {
            var value = GetVariable(variables, name);

            return value != null && value != "False" && Convert.ToString(value) != "" && Convert.ToInt32(value) != 0;
        }

        private string EvaluateTokens(IList<MustacheToken> tokens, Dictionary<string, dynamic> variables)
        {
            if (tokens == null) return null;

            StringBuilder result = new StringBuilder();

            foreach (MustacheToken token in tokens)
            {
                switch (token.Type)
                {
                    case MustacheTokenType.Comment:
                        // Skip;
                        break;
                    case MustacheTokenType.Value:
                        result.Append(token.Value != null ? token.Value : "");
                        break;
                    case MustacheTokenType.Variable:
                        var value1 = GetVariable(variables, token.Value);
                        result.Append(value1 != null ? value1 : "");
                        break;
                    case MustacheTokenType.EscapedVariable:
                        var value2 = GetVariable(variables, token.Value);
                        value2 = EscapeString(value2);
                        result.Append(value2 != null ? value2 : "");
                        break;
                    case MustacheTokenType.Section:
                        var defined1 = IsDefinedVariable(variables, token.Value);
                        if (defined1 && token.Tokens != null)
                        {
                            result.Append(EvaluateTokens(token.Tokens, variables));
                        }
                        break;
                    case MustacheTokenType.InvertedSection:
                        var defined2 = IsDefinedVariable(variables, token.Value);
                        if (!defined2 && token.Tokens != null)
                        {
                            result.Append(EvaluateTokens(token.Tokens, variables));
                        }
                        break;
                    case MustacheTokenType.Partial:
                        throw new MustacheException(null, "PARTIALS_NOT_SUPPORTED", "Partials are not supported", token.Line, token.Column);
                    default:
                        throw new MustacheException(null, "INTERNAL", "Internal error", token.Line, token.Column);
                }
            }

            return result.ToString();
        }
    }
}
