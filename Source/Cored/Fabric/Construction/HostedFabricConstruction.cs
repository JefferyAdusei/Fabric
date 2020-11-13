namespace Cored.Fabric.Construction
{
    /// <inheritdoc />
    ///  <summary>
    ///  Creates a default fabric construction containing all
    ///  the default configuration and services, when used inside
    ///  a project that has it's own service provider such as an
    ///  ASP.NET Core website
    ///  </summary>
    ///  <example>
    ///  <para>
    ///      This is an example setup code for building a Fabric Construction
    ///      if you include the Cored.Fabric NuGet package
    ///  </para>
    ///  <code>
    ///      //  Program.cs (in BuildWebHost)
    ///      // ------------------------------
    ///          return WebHost.CreateDefaultBuilder()
    ///              // Merge Cored Fabric into ASP.Net Core environment
    ///              .UseCoredFabric(construct =&gt;
    ///              {
    ///                  // Add file logger
    ///                  construct.AddFileLogger();
    ///                  //
    ///                  // NOTE: If you want to configure anything in ConfigurationBuilder just use
    ///                  //       ConfigureAppConfiguration(builder =&gt; {}) and then you  have
    ///                  //       access to Cored.Fabric.Environment and Construction at that point
    ///                  //       like the normal flow of Cored Fabric setup
    ///                  //
    ///                  // The last step is inside Startup Configure method to call
    ///              })
    ///              .UseStartup&lt;Setup&gt;()
    ///              .Build();
    ///      //  Startup.cs (in Configure)
    ///      // ---------------------------
    ///          // Use Cored Fabric
    ///          app.UseFabric();
    ///  </code>
    ///  </example>
    public class HostedFabricConstruction : FabricConstruction
    {
        #region Constructor

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cored.Fabric.Construction.HostedFabricConstruction" /> class.
        /// </summary>
        public HostedFabricConstruction() : base(false)
        {
        }

        #endregion
    }
}