// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System.Collections.Generic;
using NUnit.Framework;
using Oxide.Javascript;

namespace Oxide.Tests.Javascript
{
    public class HostObjectTests : JavascriptTestBase
    {
        [Test]
        public void TestConvertHostObject()
        {
            var myInst = new TestClass();

            Context.Global.myInst = myInst;
            Assert.IsInstanceOf<TestClass>(Context.Global.myInst);
            Assert.AreEqual(myInst, Context.Global.myInst);
        }

        [Test]
        public void TestField()
        {
            var myField = new TestClassWithField();

            Context.Global.myField = myField;
            Context.Evaluate("myField.TestField = true");
            Assert.IsTrue((bool)Context.Evaluate("myField.TestField"));
            Assert.Throws<JavascriptException>(() => Context.Evaluate("myField.TestFieldReadOnly = true"));
        }

        [Test]
        public void TestProperty()
        {
            var myProp = new TestClassWithProperty();

            Context.Global.myProp = myProp;
            Context.Evaluate("myProp.TestPropertyGetSet = true");
            Assert.IsTrue((bool)Context.Evaluate("myProp.TestPropertyGetSet"));

            Context.Evaluate("myProp.TestPropertySet = true");
            Assert.IsTrue((bool)Context.Evaluate("myProp.TestPropertyGet"));

            Assert.Throws<JavascriptException>(() => Context.Evaluate("myProp.TestPropertyGet = false"));
        }

        [Test]
        public void TestMethod()
        {
            var myMethod = new TestClassWithMethod();

            Context.Global.myMethod = myMethod;
            Context.Evaluate("myMethod.TestMethod(\"Hello World\")");
            Assert.AreEqual("Hello World", myMethod.TestString);
            Assert.AreEqual("Lorem Ipsum", Context.Evaluate("myMethod.TestMethodWithReturn(\"Lorem Ipsum\")"));
            Assert.Throws<JavascriptException>(() => Context.Evaluate("myMethod.TestMethod()"));
        }

        [Test]
        public void TestConstructor()
        {
            Context.RegisterHostType<TestClass>();
            var myClass = Context.Evaluate("new TestClass()");
            Assert.IsInstanceOf<TestClass>(myClass);

            Context.RegisterHostType<TestClassWithConstructorParameters>();
            var myClassParams = Context.Evaluate("new TestClassWithConstructorParameters('Hello World')");
            Assert.IsInstanceOf<TestClassWithConstructorParameters>(myClassParams);
            Assert.AreEqual("Hello World", ((TestClassWithConstructorParameters)myClassParams).Param);
        }

        [Test]
        public void TestListGeneric()
        {
            Context.RegisterHostType<List<string>>("TestList");
            var myList = Context.Evaluate("new TestList()");
            Assert.IsInstanceOf<List<string>>(myList);

            Context.Global.myList = new List<string>();
            Context.Evaluate("myList.Add('hello world')");
            Assert.AreEqual("hello world", Context.Evaluate("myList[0]"));
        }

        public class TestClass
        {
        }

        public class TestClassWithConstructorParameters
        {
            private readonly string param;

            public string Param => param;

            public TestClassWithConstructorParameters(string param)
            {
                this.param = param;
            }
        }

        public class TestClassWithField
        {
            public bool TestField = false;

            public readonly bool TestFieldReadOnly = false;
        }

        public class TestClassWithProperty
        {
            public bool TestPropertyGetSet { get; set; }

            private bool testPrivate;

            public bool TestPropertySet
            {
                set => testPrivate = value;
            }

            public bool TestPropertyGet => testPrivate;
        }

        public class TestClassWithMethod
        {
            public string TestString { get; private set; }

            public void TestMethod(string value) => TestString = value;
            public string TestMethodWithReturn(string value) => TestString = value;
        }
    }
}
