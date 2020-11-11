namespace Cored.Fabric
{
    using Microsoft.Extensions.DependencyInjection;
    using Construction;

    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Used in a hosted environment when using an existing set of services and configuration,
        /// such as an ASP.NET Core environment.
        /// </summary>
        /// <param name="services">The services to use</param>
        /// <returns>Fabric Construction</returns>
        public static FabricConstruction AddFabric(this IServiceCollection services)
        {
            // Add the services into the fabric
            Fabric.Construction.UseHostedServices(services);

            // Return construction for chaining
            return Fabric.Construction;
        }
    }
}