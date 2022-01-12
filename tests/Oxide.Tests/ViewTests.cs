using NUnit.Framework;

namespace Oxide.Tests
{
    public class ViewTests : RendererTestBase
    {
        [Test]
        public void TestLoadHTML()
        {
            View.LoadHTML("<p>Hello World</p>");

            Assert.True(View.IsLoading);

            while (View.IsLoading)
            {
                Renderer.Update();
                Renderer.Render();
            }

            Assert.False(View.IsLoading);
        }

        [Test]
        public void TestLoadFile()
        {
            string url = @"file:///test.html";
            View.URL = url;

            Assert.True(View.IsLoading);

            while (View.IsLoading)
            {
                Renderer.Update();
                Renderer.Render();
            }

            Assert.True(View.URL == url);
        }

        [Test]
        public void TestViewLoadEvents()
        {
            bool beginLoad = false;
            bool finishLoad = false;
            bool domReady = false;

            View.OnBeginLoading += (_, __) => beginLoad = true;
            View.OnFinishLoading += (_, __) => finishLoad = true;
            View.OnDOMReady += (_, __) => domReady = true;

            View.URL = @"file:///test.html";

            while (View.IsLoading)
            {
                Renderer.Update();
                Renderer.Render();
            }

            Assert.True(beginLoad);
            Assert.True(finishLoad);
            Assert.True(domReady);
        }

        [Test]
        public void TestLoadURL()
        {
            // TODO: run local web server
            string url = @"https://httpbin.org/get";
            View.URL = url;

            Assert.True(View.IsLoading);

            while (View.IsLoading)
            {
                Renderer.Update();
                Renderer.Render();
            }

            Assert.True(View.URL == url);
        }
    }
}
