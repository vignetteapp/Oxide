using System.Numerics;
using System.Reflection;
using Mikomi;
using Mikomi.Apps;

string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

AppCore.EnableDefaultLogger(Path.Combine(basePath, "ultralight.log"));
AppCore.EnablePlatformFileSystem(Path.Combine(basePath, "assets"));
AppCore.EnablePlatformFontLoader();

App myApp;

using (var myConfig = new Config { CachePath = Path.Combine(basePath, "cache") })
{
    using var myAppConfig = new AppConfig { ForceCPURenderer = false };
    myApp = new App(myAppConfig, myConfig);
}

var myWindow = myApp.Monitor.CreateWindow(500, 500, false, WindowFlags.Titled | WindowFlags.Resizable);
myWindow.Title = "My Example App";

var myOverlay = myWindow.CreateOverlay();
myOverlay.View.URL = @"https://google.com/";

myWindow.OnClose += (_, __) => myApp.Quit();
myWindow.OnResize += (_, args) => myOverlay.Size = new Vector2(args.Width, args.Height);

myApp.Run();

myOverlay.Dispose();
myWindow.Dispose();
myApp.Dispose();
