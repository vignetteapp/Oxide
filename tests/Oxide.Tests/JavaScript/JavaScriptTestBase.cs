// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using NUnit.Framework;
using Oxide.Javascript;

namespace Oxide.Tests.Javascript
{
    public abstract class JavascriptTestBase
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
