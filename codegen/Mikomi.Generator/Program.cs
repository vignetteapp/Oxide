using System.IO;
using CppSharp;
using CppSharp.AST;

namespace Mikomi.Generator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConsoleDriver.Run(new Generator(args[0]));
        }
    }

    public class Generator : ILibrary
    {
        private readonly string basePath;

        public Generator(string basePath)
        {
            this.basePath = basePath;
        }

        public void Postprocess(Driver driver, ASTContext ctx)
        {
        }

        public void Preprocess(Driver driver, ASTContext ctx)
        {
        }

        public void Setup(Driver driver)
        {
            driver.Options.OutputDir = @"src\Mikomi.Generated\";
            driver.ParserOptions.Verbose = true;

            string includeDir = Path.Combine(basePath, @"include");
            string libraryDir = Path.Combine(basePath, @"lib");

            var ultralight = driver.Options.AddModule("Ultralight");
            ultralight.IncludeDirs.Add(includeDir);
            ultralight.LibraryDirs.Add(libraryDir);
            ultralight.Headers.Add("Ultralight/CAPI.h");
            ultralight.Libraries.Add("Ultralight.lib");
            ultralight.Libraries.Add("UltralightCore.lib");
            ultralight.Defines.Add("ULTRALIGHT_IMPLEMENTATION");

            var webcore = driver.Options.AddModule("JavascriptCore");
            webcore.IncludeDirs.Add(includeDir);
            webcore.LibraryDirs.Add(libraryDir);
            webcore.Headers.Add("JavaScriptCore/JSContextRef.h");
            webcore.Headers.Add("JavaScriptCore/JSObjectRef.h");
            webcore.Headers.Add("JavaScriptCore/JSObjectRefPrivate.h");
            webcore.Headers.Add("JavaScriptCore/JSStringRef.h");
            webcore.Headers.Add("JavaScriptCore/JSValueRef.h");
            webcore.Headers.Add("JavaScriptCore/JSTypedArray.h");
            webcore.Libraries.Add("WebCore.lib");
            webcore.Defines.Add("BUILDING_JavaScriptCore");

            var appcore = driver.Options.AddModule("AppCore");
            appcore.IncludeDirs.Add(includeDir);
            appcore.LibraryDirs.Add(libraryDir);
            appcore.Headers.Add("AppCore/CAPI.h");
            appcore.Libraries.Add("AppCore.lib");
            appcore.Defines.Add("APPCORE_IMPLEMENTATION");
        }

        public void SetupPasses(Driver driver)
        {
        }
    }
}
