namespace Cored.Exception
{
    using System;
    using Interface;
    using Logging;
    using static Fabric.Di.FabricDi;

    /// <inheritdoc />
    public class ExceptionHandler : IExceptionHandler
    {
        #region Implementation of IExceptionHandler

        /// <inheritdoc />
        public void HandleError(Exception exception)
        {
            // Log it
            Logger.LogErrorSource("An Exception occured", exception: exception);
        }

        #endregion
    }
}