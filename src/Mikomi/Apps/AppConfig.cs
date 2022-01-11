using System;
using System.Runtime.InteropServices;
using Mikomi.Interop;

namespace Mikomi.Apps
{
    public class AppConfig : DisposableObject
    {
        private string developerName;

        /// <summary>
        /// Set the name of the developer of this app.
        /// <br/>
        /// This is used to generate a unique path to store local application data
        /// on the user's machine.
        /// <br/>
        /// (Default = "MyCompany")
        /// </summary>
        public string DeveloperName
        {
            get => developerName;
            set => AppCore.ulSettingsSetDeveloperName(Handle, developerName = value);
        }

        private string name;

        /// <summary>
        /// Set the name of this app.
        /// <br/>
        /// This is used to generate a unique path to store local application data
        /// on the user's machine.
        /// <br/>
        /// (Default = "MyApp")
        /// </summary>
        public string Name
        {
            get => name;
            set => AppCore.ulSettingsSetAppName(Handle, name = value);
        }

        private string assetPath;

        /// <summary>
        /// Set the root file path for our file system, you should set this to the
        /// relative path where all of your app data is.
        /// <br/>
        /// This will be used to resolve all file URLs, eg file:///page.html
        /// <br/>
        /// (Default = "./assets/")
        /// <br/>
        /// This relative path is resolved using the following logic:
        /// <br/>
        /// - Windows: relative to the executable path
        /// <br/>
        /// - Linux:   relative to the executable path
        /// <br/>
        /// - macOS:   relative to YourApp.app/Contents/Resources/
        /// </summary>
        public string AssetPath
        {
            get => assetPath;
            set => AppCore.ulSettingsSetFileSystemPath(Handle, assetPath = value);
        }

        private bool loadShadersFromFileSystem;

        /// <summary>
        /// Set whether or not we should load and compile shaders from the file system
        /// (eg, from the /shaders/ path, relative to file_system_path).
        /// <br/>
        /// If this is false (the default), we will instead load pre-compiled shaders
        /// from memory which speeds up application startup time.
        /// </summary>
        public bool LoadShadersFromFileSystem
        {
            get => loadShadersFromFileSystem;
            set => AppCore.ulSettingsSetLoadShadersFromFileSystem(Handle, loadShadersFromFileSystem = value);
        }

        private bool forceCPURenderer;

        /// <summary>
        /// We try to use the GPU renderer when a compatible GPU is detected.
        /// <br/>
        /// Set this to true to force the engine to always use the CPU renderer.
        /// </summary>
        public bool ForceCPURenderer
        {
            get => forceCPURenderer;
            set => AppCore.ulSettingsSetForceCPURenderer(Handle, forceCPURenderer = value);
        }

        public AppConfig()
            : base(AppCore.ulCreateSettings())
        {
        }

        protected override void DisposeUnmanaged()
            => AppCore.ulDestroySettings(Handle);
    }

#pragma warning disable CA2101

    public partial class AppCore
    {
        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern IntPtr ulCreateSettings();

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern IntPtr ulDestroySettings(IntPtr settings);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulSettingsSetDeveloperName(
            IntPtr settings,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string name
        );

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulSettingsSetAppName(
            IntPtr settings,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string name
        );

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulSettingsSetFileSystemPath(
            IntPtr settings,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string path
        );

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulSettingsSetLoadShadersFromFileSystem(
            IntPtr settings,
            [MarshalAs(UnmanagedType.I1)] bool enabled
        );

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulSettingsSetForceCPURenderer(
            IntPtr settings,
            [MarshalAs(UnmanagedType.I1)] bool forceCpu
        );
    }

#pragma warning restore CA2101

}
