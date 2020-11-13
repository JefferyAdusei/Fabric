namespace Cored.Environment.WebHosted
{
    using Microsoft.AspNetCore.Builder;
    using Fabric;

    /// <summary>
    /// Extension methods for <see cref="IApplicationBuilder"/>
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Builds the Fabric construct and sets the Fabric.Provider from the <see cref="IApplicationBuilder"/> interface.
        /// </summary>
        /// <param name="app">The application builder from the hosted environment eg. ASP.NET Core</param>
        /// <returns></returns>
        public static IApplicationBuilder UseFabric(this IApplicationBuilder app)
        {
            // Build the framework as at this point we know the provider is available
            Fabric.Construction.Build(app.ApplicationServices);

            // Return app for chaining
            return app;
        }
    }
}