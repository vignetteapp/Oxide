// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Oxide.Apps;
using Oxide.Platforms;

namespace Oxide.Examples.FileSystem
{
    public class Program
    {
        public static void Main()
        {
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            AppCore.EnablePlatformFileSystem(Path.Combine(basePath, "assets"));
            AppCore.EnableDefaultLogger(Path.Combine(basePath, "ultralight.log"));
            AppCore.EnablePlatformFontLoader();

            Platform.FileSystem = new AssemblyBackedFileSystem(Assembly.GetExecutingAssembly());

            Renderer renderer;
            View view;

            using var config = new Config { CachePath = Path.Combine(basePath, "cache") };
            using var viewConfig = new ViewConfig { IsAccelerated = false };

            renderer = new Renderer(config);
            view = new View(renderer, 500, 500, viewConfig)
            {
                URL = @"file:///app.html"
            };

            while (view.IsLoading)
            {
                renderer.Update();
                renderer.Render();
            }

            view.Surface.Bitmap.SaveAsPNG(Path.Combine(basePath, "screenshot.png"));
        }
    }

    public class AssemblyBackedFileSystem : IFileSystem
    {
        private readonly Assembly assembly;
        private readonly Dictionary<uint, Stream> handles = new Dictionary<uint, Stream>();
        private uint current;

        public AssemblyBackedFileSystem(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public void CloseFile(uint handle)
        {
            handles[handle].Dispose();
            handles.Remove(handle);
        }

        public bool FileExists(string path)
            => assembly.GetManifestResourceStream(pathToResourceName(path)) != null;

        public bool GetFileSize(uint handle, out long result)
        {
            result = 0;

            if (!handles.TryGetValue(handle, out var stream))
                return false;

            result = stream.Length;

            return true;
        }

        public bool GetMimeType(string path, out string type)
        {
            type = "text/html";
            return true;
        }

        public uint OpenFile(string path)
        {
            Interlocked.Increment(ref current);
            handles[current] = assembly.GetManifestResourceStream(pathToResourceName(path));
            return current;
        }

        public long ReadFile(uint handle, Span<byte> data)
            => handles[handle].Read(data);

        private string pathToResourceName(string path) => $"{assembly.GetName().Name}.{path.Replace('/', '.')}";
    }
}
