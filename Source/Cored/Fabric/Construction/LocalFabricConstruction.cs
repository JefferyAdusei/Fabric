namespace Cored.Fabric.Construction
{
    using System;
    using Microsoft.Extensions.Configuration;

    /// <inheritdoc />
    /// <summary>
    /// Creates a default framework construction containing all 
    /// the default configuration and services
    /// </summary>
    /// <example>
    /// <para>
    ///     This is an example setup code for building a Dna Framework Construction
    /// </para>
    /// <code>
    ///     // Build the framework adding any required services
    ///     Framework.Construct&lt;DefaultFrameworkConstruction&gt;()
    ///             .AddFileLogger()
    ///             .Build();
    /// </code>
    /// </example>
    public class LocalFabricConstruction : FabricConstruction
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFabricConstruction"/> class.
        /// </summary>
        public LocalFabricConstruction()
        {
            // Configure...
            this.AddLocalConfiguration()
                // And add default services
                .AddLocalServices();
        }

        public LocalFabricConstruction(Action<IConfigurationBuilder> configureAction)
        {
            // Configure...
            this.AddLocalConfiguration(configureAction)
                // And add default services
                .AddLocalServices();
        }

        #endregion
    }
}