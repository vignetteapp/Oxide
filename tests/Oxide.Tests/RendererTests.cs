// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using NUnit.Framework;
using Oxide.Apps;

namespace Oxide.Tests
{
    [Ignore("Cannot run in headless")]
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
