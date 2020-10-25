namespace Cored.Fabric.Construction
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Environment;

    /// <summary>
    /// The construction information when starting up and configuring Cored.Fabric
    /// </summary>
    public class FabricConstruction
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricConstruction"/> class.
        /// </summary>
        /// <param name="createServiceCollection">Indicates whether to create a new <see cref="IServiceCollection"/></param>
        public FabricConstruction(bool createServiceCollection = true)
        {
            // Create environment details
            Environment = new FabricEnvironment();

            // If we should create the service collection
            if (createServiceCollection)
            {
                // Create a new list of dependencies
                ServiceCollection = new ServiceCollection();
            }
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// The services that will get used and compiled once the fabric is built.
        /// </summary>
        protected IServiceCollection Services;

        #endregion

        #region Public Properties

        /// <summary>
        /// The environment to use.
        /// </summary>
        public IFabricEnvironment Environment { get; protected set; }

        /// <summary>
        /// The configuration to use.
        /// </summary>
        public IConfiguration Configuration { get; protected set; }

        /// <summary>
        /// The dependency injection service provider
        /// </summary>
        public IServiceProvider ServiceProvider { get; protected set; }

        /// <summary>
        /// The services that will get used and compiled once the framework is built.
        /// </summary>
        public IServiceCollection ServiceCollection
        {
            get => Services;

            set
            {
                // Set services
                Services = value;

                // If we have some...
                // Inject environment into services
                Services?.AddSingleton(Environment);
            }
        }

        #endregion

        #region Build Method

        /// <summary>
        /// Builds the service collection into a service provider
        /// </summary>
        /// <param name="provider">The given provider by the user</param>
        public void Build(IServiceProvider provider = null)
        {
            // Use given provider or build it
            ServiceProvider = provider ?? ServiceCollection.BuildServiceProvider();
        }

        #endregion

        #region Hosted Environment Methods

        /// <summary>
        /// Uses the given service collection in the framework.
        /// Typically used in an ASP.NET Core environment where the ASP.NET
        /// server has its own collection
        /// </summary>
        /// <param name="serviceCollection">The provided service collection</param>
        /// <returns>This fabric construction for chaining</returns>
        public FabricConstruction UseHostedServices(IServiceCollection serviceCollection)
        {
            // Set services
            ServiceCollection = serviceCollection;

            // Return self for chaining
            return this;
        }

        /// <summary>
        /// Uses the given configuration in the framework
        /// </summary>
        /// <param name="configuration">The configuration to use</param>
        /// <returns>This fabric construction for chaining</returns>
        public FabricConstruction UseConfiguration(IConfiguration configuration)
        {
            // Set configuration
            Configuration = configuration;

            // Return self for chaining
            return this;
        }

        #endregion
    }
}