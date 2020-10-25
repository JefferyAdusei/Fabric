namespace Cored.Environment
{
    /// <summary>
    /// Details about the current fabric environment.
    /// </summary>
    public interface IFabricEnvironment
    {
        /// <summary>
        /// Gets the configuration of the environment, typically Development or Production
        /// </summary>
        string Configuration { get; }

        /// <summary>
        /// Gets a value indicating whether application is running in development or production environment
        /// </summary>
        bool IsDevelopment { get; }

        /// <summary>
        /// Gets a value indicating whether the application is running in a mobile platform.
        /// </summary>
        bool IsMobile { get; }
    }
}