namespace Cored.Exception
{
    using System;
    using Interface;
    public class ExceptionHandler : IExceptionHandler
    {
        #region Implementation of IExceptionHandler

        /// <inheritdoc />
        public void HandleError(Exception exception)
        {
            // Log it
            // TODO: Log the exception
        }

        #endregion
    }
}