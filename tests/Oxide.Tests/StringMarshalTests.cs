// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using NUnit.Framework;
using Oxide.Interop;
using Oxide.Javascript.Interop;

namespace Oxide.Tests
{
    public class StringMarshalTests
    {
        [Test]
        public void TestMarshalULStringToNative()
        {
            var marshaler = new ULStringMarshaler();

            string oldString = "Hello World";
            IntPtr ptr = marshaler.MarshalManagedToNative(oldString);

            string newString = (string)marshaler.MarshalNativeToManaged(ptr);
            marshaler.CleanUpNativeData(ptr);

            Assert.AreEqual(oldString, newString);
        }

        [Test]
        public void TestMarshalJSStringToNative()
        {
            var marshaler = new JSStringRefMarshal();

            string oldString = "Hello World";
            IntPtr ptr = marshaler.MarshalManagedToNative(oldString);

            string newString = (string)marshaler.MarshalNativeToManaged(ptr);
            marshaler.CleanUpManagedData(ptr);

            Assert.AreEqual(oldString, newString);
        }
    }
}
