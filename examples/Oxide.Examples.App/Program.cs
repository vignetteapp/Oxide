// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System.Numerics;
using System.Reflection;
using Oxide;
using Oxide.Apps;

string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

AppCore.EnableDefaultLogger(Path.Combine(basePath, "ultralight.log"));
AppCore.EnablePlatformFileSystem(Path.Combine(basePath, "assets"));
AppCore.EnablePlatformFontLoader();

using var myConfig = new Config { CachePath = Path.Combine(basePath, "cache"), OverrideRAMSize = 128 };
using var myAppConfig = new AppConfig { ForceCPURenderer = false };
using var myApp = new App(myAppConfig, myConfig);

using var myWindow = myApp.Monitor.CreateWindow(500, 500, false, WindowFlags.Titled | WindowFlags.Resizable);
using var myOverlay = myWindow.CreateOverlay();

myOverlay.View.URL = @"https://google.com/";
myOverlay.View.OnDOMReady += (_, __) => myOverlay.View.GetJSContext(ctx => myWindow.Title = ctx.Global.document.title);

myWindow.OnClose += (_, __) => myApp.Quit();
myWindow.OnResize += (_, args) => myOverlay.Size = new Vector2(args.Width, args.Height);

myApp.Run();
