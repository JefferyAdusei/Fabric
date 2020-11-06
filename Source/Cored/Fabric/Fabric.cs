namespace Cored.Fabric
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Construction;
    using Logging;
    using static Di.FabricDi;

    /// <summary>
    /// The main entry point of the Fabric library.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         To use the Cored.Fabric, you need to create a new <see cref="FabricConstruction"/>
    ///         such as <see cref="LocalFabricConstruction"/> and then add your services
    ///         then finally <see cref="Fabric.Build(FabricConstruction, bool)"/> For example
    ///     </para>
    ///     <code>
    ///         // Create the default framework and build it
    ///         Fabric.Construct&lt;DefaultFrameworkConstruction&gt;().Build();
    ///     </code>
    /// </remarks>
    public static class Fabric
    {
        #region Public Properties

        /// <summary>
        /// The fabric construction used in this application.
        /// NOTE: This should be set by the consuming application at the very start of the program
        /// </summary>
        /// <example>
        ///     <code>
        ///         Fabric.Construct&lt;LocalFabricConstruction&gt;();
        ///     </code>
        /// </example>
        public static FabricConstruction Construction { get; private set; }

        /// <summary>
        /// Gets the dependency service provider for public consumption.
        /// </summary>
        public static IServiceProvider ServiceProvider => Construction?.ServiceProvider;

        #endregion

        #region Extension Methods

        /// <summary>
        /// Should be called once a Fabric Construction is finished and we want to build
        /// it and start the application.
        /// </summary>
        /// <param name="construction"><inheritdoc cref="FabricConstruction"/></param>
        /// <param name="shouldLog">Indicates whether the fabric started message should be logged</param>
        public static void Build(this FabricConstruction construction, bool shouldLog = true)
        {
            // Build the service provider
            construction.Build();

            // Log the startup complete
            if (shouldLog)
            {
                Logger.LogCriticalSource($"Fabric has started in{FabricEnvironment.Configuration}...");
            }
        }

        /// <summary>
        /// The initial call to setting up and using Fabric
        /// </summary>
        /// <typeparam name="T">The type of the construction to use</typeparam>
        /// <returns>Construction for chaining</returns>
        public static FabricConstruction Construct<T>()
            where T : FabricConstruction, new()
        {
            Construction = new T();

            // Return construction for chaining
            return Construction;
        }

        /// <summary>
        /// The initial call to setting up and using Fabric
        /// </summary>
        /// <typeparam name="T">The type of the construction to use</typeparam>
        /// <param name="constructionInstance">The instance of the construction to use</param>
        /// <returns>Construction for chaining</returns>
        public static FabricConstruction Construct<T>(T constructionInstance)
            where T : FabricConstruction
        {
            // Set construction
            Construction = constructionInstance;

            // Return construction for chaining
            return Construction;
        }

        /// <summary>
        /// Shortcut to Fabric.Provider.GetService to get an injected service of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of the service to get</typeparam>
        /// <returns>The service provider</returns>
        public static T Service<T>() => ServiceProvider.GetService<T>();

        #endregion
    }
}