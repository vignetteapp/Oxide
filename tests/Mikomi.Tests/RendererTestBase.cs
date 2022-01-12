using System;
using System.IO;
using System.Reflection;
using Mikomi.Apps;
using NUnit.Framework;

namespace Mikomi.Tests
{
    public abstract class RendererTestBase
    {
        protected Renderer Renderer { get; set; }
        protected Session Session { get; set; }
        protected View View { get; set; }
        protected ViewConfig ViewConfig { get; set; }
        protected Config Config { get; set; }
        private static readonly string base_path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static RendererTestBase()
        {
            AppCore.EnablePlatformFontLoader();
            AppCore.EnablePlatformFileSystem(Path.Combine(base_path, "assets"));
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            Config = new Config { CachePath = Path.Combine(base_path, "cache") };
            Renderer = new Renderer(Config);
            Session = new Session(Renderer, false, $"test-{Guid.NewGuid()}");
            ViewConfig = new ViewConfig { IsAccelerated = false };
            View = new View(Renderer, 100, 100, ViewConfig, Session);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            View?.Dispose();
            ViewConfig?.Dispose();
            Session?.Dispose();
            Renderer?.Dispose();
            Config?.Dispose();
        }
    }
}
