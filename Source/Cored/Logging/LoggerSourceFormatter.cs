namespace Cored.Logging
{
    using System;
    using System.IO;

    /// <summary>
    /// Contains extensions that formats the message including
    /// the source information pulled out of the state
    /// </summary>
    public static class LoggerSourceFormatter
    {
        /// <summary>
        /// Formats the message including the source information pulled out of the state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string Format(object[] state, Exception exception)
        {
            // Get the values from the state
            string origin = state[0] as string;
            string filepath = state[1] as string;
            int lineNumber = (int) state[2];
            string message = state[3] as string;

            // Get any exception message
            string exceptionMessage = exception?.Message;

            if (exception != null)
            {
                exceptionMessage = $"\r\n{exception}";
            }

            return $"{message} [{Path.GetFileName(filepath)} > {origin} () > Line {lineNumber}] {exceptionMessage}";
        }
    }
}