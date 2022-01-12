using System;
using Mikomi.Apps;
using NUnit.Framework;

namespace Mikomi.Tests
{
    public class RendererTests : RendererTestBase
    {
        [Test]
        public void TestAttemptCreateSecondRenderer()
        {
            var localConfig = new Config();

            Assert.Throws<InvalidOperationException>(() => new Renderer(localConfig));
            Renderer.Dispose();

            Assert.DoesNotThrow(() => Renderer = new Renderer(localConfig));

            localConfig.Dispose();
        }

        [Test]
        public void TestAttemptCreateAppWhileRendererExists()
        {
            App app = null;
            Assert.Throws<InvalidOperationException>(() => new App());
            Renderer.Dispose();

            Assert.DoesNotThrow(() => app = new App());
            app.Dispose();
        }
    }
}
