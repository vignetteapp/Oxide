using System.Runtime.InteropServices;
using Mikomi.Interop;

namespace Mikomi.Apps
{
    public partial class AppCore
    {
        internal const string LIB_APPCORE = @"AppCore";

        /// <summary>
        /// This is only needed if you are not creating your own app.
        /// <br/>
        /// Initializes the platform font loader and sets it as the current FontLoader.
        /// </summary>
        public static void EnablePlatformFontLoader() => ulEnablePlatformFontLoader();

        /// <summary>
        /// This is only needed if you are not creating your own app.
        /// <br/>
        /// Initializes the platform file system (needed for loading file:/// URLs) and
        /// sets it as the current FileSystem.
        /// <br/>
        /// You can specify a base directory path to resolve relative paths against.
        /// </summary>
        public static void EnablePlatformFileSystem(string basePath) => ulEnablePlatformFileSystem(basePath);

        /// <summary>
        /// This is only needed if you are not creating your own app.
        /// <br/>
        /// Initializes the default logger (writes the log to a file).
        /// <br/>
        /// You should specify a writable log path to write the log to
        /// for example "./ultralight.log".
        /// </summary>
        public static void EnableDefaultLogger(string logPath) => ulEnableDefaultLogger(logPath);

#pragma warning disable CA2101

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulEnablePlatformFontLoader();

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulEnablePlatformFileSystem(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string basePath
        );

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulEnableDefaultLogger(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string logPath
        );
    }

#pragma warning restore CA2101

}
