// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using NUnit.Framework;
using Oxide.Javascript;

namespace Oxide.Tests.Javascript
{
    public class ConversionTests : JavascriptTestBase
    {
        [Test]
        public void TestConvertNumber()
        {
            var result = Context.Evaluate("1 + 1");
            Assert.IsInstanceOf<double>(result);
            Assert.AreEqual(result, 2);
        }

        [Test]
        public void TestConvertBoolean()
        {
            var result = Context.Evaluate("1 === 1");
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(result, true);
        }

        [Test]
        public void TestConvertString()
        {
            var result = Context.Evaluate("'hello ' + 'world'");
            Assert.IsInstanceOf<string>(result);
            Assert.AreEqual(result, "hello world");
        }

        [Test]
        public void TestConvertNull()
        {
            var result = Context.Evaluate("null");
            Assert.AreEqual(result, null);
        }

        [Test]
        public void TestConvertUndefined()
        {
            var result = Context.Evaluate("undefined");
            Assert.IsInstanceOf<Undefined>(result);
            Assert.AreEqual(result, Undefined.Value);
        }

        [Test]
        public void TestConvertObject()
        {
            dynamic result = Context.Evaluate("({ myProp: true, myFunc: () => true })");
            Assert.IsInstanceOf<JSObject>(result);
            Assert.IsInstanceOf<bool>(result.myProp);
            Assert.AreEqual(result.myProp, true);
            Assert.AreEqual(result.myFunc(), true);
        }

        [Test]
        public void TestConvertArray()
        {
            dynamic result = Context.Evaluate("([ 'hello', 'world', 42 ])");
            Assert.IsInstanceOf<JSObject>(result);
            Assert.IsInstanceOf<string>(result[0]);
            Assert.IsInstanceOf<string>(result[1]);
            Assert.IsInstanceOf<double>(result[2]);
            Assert.AreEqual(result[0], "hello");
            Assert.AreEqual(result[1], "world");
            Assert.AreEqual(result[2], 42);
        }

        [Test]
        public void TestConvertFunction()
        {
            dynamic func1 = Context.Evaluate("(function() { return true })");
            Assert.IsInstanceOf<JSObject>(func1);
            Assert.IsTrue(func1());

            dynamic func2 = Context.Evaluate("(function(msg) { return msg + ' World' })");
            Assert.IsInstanceOf<JSObject>(func2);
            Assert.AreEqual(func2("Hello"), "Hello World");
        }
    }
}
