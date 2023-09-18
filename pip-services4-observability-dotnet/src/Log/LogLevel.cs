namespace PipServices4.Observability.Log
{
	/// <summary>
	/// Standard log levels.
	/// Logs at debug and trace levels are usually captured
	/// only locally for troubleshooting and never sent to consolidated log services.
	/// </summary>
	public enum LogLevel
	{
		/** Nothing to log */
		None = 0,
		/** Log only fatal errors that cause processes to crash */
		Fatal = 1,
		/** Log all errors. */
		Error = 2,
		/** Log errors and warnings */
		Warn = 3,
		/** Log errors and important information messages */
		Info = 4,
		/** Log everything except traces */
		Debug = 5,
		/** Log everything. */
		Trace = 6
	}
}