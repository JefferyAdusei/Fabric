namespace Cored.Exception
{
    using System;
    using Interface;
    using Logging;
    using static Fabric.Di.FabricDi;
    public class ExceptionHandler : IExceptionHandler
    {
        #region Implementation of IExceptionHandler

        /// <inheritdoc />
        public void HandleError(Exception exception)
        {
            // Log it
            Logger.LogErrorSource("An Exception was thrown", exception:exception);
        }

        #endregion
    }
}