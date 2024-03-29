namespace PipServices4.Data.Validate
{
    /// <summary>
    /// Result generated by schema validation
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Creates a new instance of validation result.
        /// </summary>
        public ValidationResult() { }

        /// <summary>
        /// Creates a new instance of validation result and sets its values.
        /// </summary>
        /// <param name="path">a dot notation path of the validated element.</param>
        /// <param name="type">a type of the validation result: Information, Warning, or Error.</param>
        /// <param name="code">an error code.</param>
        /// <param name="message">a human readable message.</param>
        /// <param name="expected">an value expected by schema validation.</param>
        /// <param name="actual">an actual value found by schema validation.</param>
        public ValidationResult(string path, ValidationResultType type,
            string code, string message, object expected, object actual)
        {
            Path = path;
            Type = type;
            Code = code;
            Message = message;
        }

        public string Path { get; set; }
        public ValidationResultType Type { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public object Expected { get; set; }
        public object Actual { get; set; }
    }
}
