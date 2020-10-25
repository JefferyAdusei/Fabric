namespace Cored.Fabric
{
    using Microsoft.Extensions.DependencyInjection;
    using Construction;

    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static FabricConstruction AddFabric(this IServiceCollection services)
        {
            // Add the services into the fabric
            Fabric.Construction.UseHostedServices(services);

            // Return construction for chaining
            return Fabric.Construction;
        }
    }
}