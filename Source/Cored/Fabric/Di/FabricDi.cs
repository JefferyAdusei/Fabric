namespace Cored.Fabric.Di
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Environment;
    using Exception.Interface;

    /// <summary>
    /// The core services that could be available in the Core Fabric for quick
    /// and easy access anywhere in code.
    /// </summary>
    /// <example>
    ///     <code>
    ///         using static FabricDi
    ///
    ///         Logger.Log(Configuration["SomeSection"]);
    ///     </code>
    /// </example>
    public static class FabricDi
    {
        /// <summary>
        /// Gets the shortcut to access the configuration
        /// </summary>
        public static IConfiguration Configuration => Fabric.ServiceProvider?.GetService<IConfiguration>();

        /// <summary>
        /// Gets the shortcut to access the registered or default logger.
        /// </summary>
        public static ILogger Logger => Fabric.ServiceProvider?.GetService<ILogger>();

        /// <summary>
        /// Gets the shortcut to access the logger factory for creating loggers.
        /// </summary>
        public static ILoggerFactory LoggerFactory => Fabric.ServiceProvider?.GetService<ILoggerFactory>();

        /// <summary>
        /// Gets the shortcut to access the fabric framework environment.
        /// </summary>
        public static IFabricEnvironment FabricEnvironment => Fabric.ServiceProvider?.GetService<IFabricEnvironment>();

        /// <summary>
        /// Gets the shortcut to access the default exception handler.
        /// </summary>
        public static IExceptionHandler ExceptionHandler => Fabric.ServiceProvider?.GetService<IExceptionHandler>();
    }
}