using NUnit.Framework;
using Oxide.JavaScript;

namespace Oxide.Tests.JavaScript
{
    public abstract class JavaScriptTestBase
    {
        protected JSContextGroup Group { get; private set; }
        protected JSContext Context { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Group = new JSContextGroup();
            Context = Group.CreateContext();
        }

        [TearDown]
        public void TearDown()
        {
            Group?.Dispose();
        }
    }
}
