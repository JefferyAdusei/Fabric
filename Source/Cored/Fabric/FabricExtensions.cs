namespace Cored.Fabric
{
    using System;
    using System.IO;
    using System.Reflection;
    using Exception;
    using Exception.Interface;
    using Construction;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Extension methods for the Cored Fabric
    /// </summary>
    public static class FabricExtensions
    {
        #region Local Configuration

        /// <summary>
        /// Configures the fabric construction in the default way
        /// </summary>
        /// <param name="construction">The construction to configure</param>
        /// <param name="configure">The custom configuration action</param>
        /// <returns>Fabric construction for chaining</returns>
        public static FabricConstruction AddLocalConfiguration(this FabricConstruction construction,
            Action<IConfigurationBuilder> configure = null)
        {
            // Create configuration source
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                // Add environment variables
                .AddEnvironmentVariables();

            // If we are not on a mobile platform...
            if (!construction.Environment.IsMobile)
            {
                // Add file based configuration.

                // Set base path for JSON files as the startup location of the application
                configurationBuilder.SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                // Add application settings json files
                configurationBuilder.AddJsonFile("appsettings.json", true, true);
                configurationBuilder.AddJsonFile($"appsettings.{construction.Environment.Configuration}.json", true, true);
            }

            // Let custom configuration happen
            configure?.Invoke(configurationBuilder);

            // Inject configuration into services
            IConfiguration configuration = configurationBuilder.Build();
            construction.ServiceCollection.AddSingleton(configuration);

            // Set the construction configuration
            construction.UseConfiguration(configuration);

            // Chain the construction
            return construction;
        }

        /// <summary>
        /// Configures a fabric construction using the provided configuration
        /// </summary>
        /// <param name="construction"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static FabricConstruction AddConfiguration(this FabricConstruction construction,
            IConfiguration configuration)
        {
            // Add specific configuration
            construction.UseConfiguration(configuration);

            // Add configuration to services
            construction.ServiceCollection.AddSingleton(configuration);

            // Chain the construction
            return construction;
        }

        #endregion

        #region Local Exception Handler

        /// <summary>
        /// Inject the default exception handler into the fabric construction
        /// </summary>
        /// <param name="construction">The fabric construction</param>
        /// <returns>The Fabric construction for chaining</returns>
        public static FabricConstruction AddLocalExceptionHandler(this FabricConstruction construction)
        {
            // Bind a static instance of the ExceptionHandler
            construction.ServiceCollection.AddSingleton<IExceptionHandler>(new ExceptionHandler());

            // Chain the construction
            return construction;
        }

        #endregion

        #region Local Logger

        /// <summary>
        /// Injects the default logger into the fabric construction.
        /// </summary>
        /// <param name="construction">The construction</param>
        /// <returns>The construction for chaining</returns>
        public static FabricConstruction AddLocalLogger(this FabricConstruction construction)
        {
            // Add logging as default
            construction.ServiceCollection.AddLogging(options =>
            {
                // Default to debug level
                options.SetMinimumLevel(LogLevel.Debug);

                // Setup loggers from configuration
                options.AddConfiguration(construction.Configuration.GetSection("Logging"));

                // Add console logger
                options.AddConsole();

                // Add debug logger
                options.AddDebug();
            });

            // Adds a default logger so that we can get a non-generic ILogger
            // that will have the category name of "Fabric"
            construction.ServiceCollection.AddTransient(provider => provider?.GetService<ILoggerFactory>()?.CreateLogger("Fabric"));

            // Chain the construction
            return construction;
        }

        #endregion

        #region Local Services

        /// <summary>
        /// Injects all of the default services used by Fabric for a quicker and
        /// cleaner setup
        /// </summary>
        /// <param name="construction">The fabric construction</param>
        /// <returns>The fabric construction for further chaining</returns>
        public static FabricConstruction AddLocalServices(this FabricConstruction construction)
        {
            // Add exception handler
            construction.AddLocalExceptionHandler();

            // Add default logger
            construction.AddLocalLogger();

            // Chain the construction
            return construction;
        }

        #endregion
    }
}