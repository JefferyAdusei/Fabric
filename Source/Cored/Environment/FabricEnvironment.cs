namespace Cored.Environment
{
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <inheritdoc />
    public class FabricEnvironment : IFabricEnvironment
    {
        #region Public Properties

        /// <inheritdoc />
        public bool IsDevelopment =>
            Assembly.GetEntryAssembly()?
                .GetCustomAttribute<DebuggableAttribute>()?
                .IsJITTrackingEnabled == true;

        /// <inheritdoc />
        public string Configuration => IsDevelopment ? "Development" : "Production";

        /// <inheritdoc />
        /// This is a temporary, fragile check until it is officially supported
        /// https://github.com/dotnet/corefx/issues/27417
        public bool IsMobile => RuntimeInformation.FrameworkDescription?.ToLower().Contains("mono") == true;

        /// <inheritdoc />
        public bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        #endregion
    }
}