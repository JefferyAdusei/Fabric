namespace Cored.Environment.Hosted
{
    using Fabric;
    using Fabric.Construction;
    using Microsoft.Extensions.Hosting;
    using System;

    /// <summary>
    /// Extension methods for <see cref="IHostBuilder"/>
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Adds the Fabric construct to the Hosted application like ASP.NET CORE and Background worker applications
        /// </summary>
        /// <param name="builder">The program initializing abstraction</param>
        /// <param name="construction">Custom action to configure Fabric's construction</param>
        /// <returns></returns>
        public static IHostBuilder UseFabric(this IHostBuilder builder, Action<FabricConstruction> construction = null)
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
                construction?.Invoke(Fabric.Construction);

                /*
                 * NOTE: Fabric will do .Build() from the Startup.cs configure call in ASP.NET CORE
                 *      app.UseFabric();
                 *
                 * In any other hosted environment, .Build should be called inside UseFabric
                 *      Host.CreateDefaultBuilder(args)
                 *          .UseFabric(construct =>
                 *              {
                 *                  all other content here
                 *                  construct.Build();
                 *              })
                 */
            });

            // return builder for chaining
            return builder;
        }
    }
}