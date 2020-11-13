namespace Cored.Environment.WebHosted
{
    using System;
    using Microsoft.AspNetCore.Hosting;
    using Fabric;
    using Fabric.Construction;

    /// <summary>
    /// Extension methods for <see cref="IWebHostBuilder"/>
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Adds the Fabric construct to the ASP.NET Core application
        /// </summary>
        /// <param name="builder">The web host builder</param>
        /// <param name="configure">Custom action to configure Fabric's construction</param>
        /// <returns></returns>
        public static IWebHostBuilder UseFabric(this IWebHostBuilder builder,
            Action<FabricConstruction> configure = null)
        {
            builder.ConfigureServices((context, services) =>
            {
                // Construct a hosted Fabric for the hosted environment
                Fabric.Construct<HostedFabricConstruction>();

                // Setup this service collection to be used by Fabric
                services.AddFabric()
                    .AddConfiguration(context.Configuration)
                    .AddLocalServices();

                // Invoke construction configuration if specified
                configure?.Invoke(Fabric.Construction);

                /*
                 * NOTE: Fabric will bo .Build() from the Startup.cs configure call
                 *      app.UseFabric();
                 */
            });

            // Return builder for chaining
            return builder;
        }
    }
}