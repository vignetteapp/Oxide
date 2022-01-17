// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

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
