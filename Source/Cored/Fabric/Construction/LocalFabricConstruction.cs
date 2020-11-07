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
    ///     Framework.Construct&lt;LocalFabricConstruction&gt;()
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

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFabricConstruction"/> class
        /// which allows user to define and configure their own <see cref="IConfigurationBuilder"/>
        /// </summary>
        /// <param name="configureAction"><see cref="IConfigurationBuilder"/> for user configuration</param>
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