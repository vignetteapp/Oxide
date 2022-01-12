using System;
using Oxide.Interop;
using NUnit.Framework;

namespace Oxide.Tests
{
    public class StringMarshalTests
    {
        [Test]
        public void TestMarshalStringToNative()
        {
            var marshaler = new ULStringMarshaler();

            string oldString = "Hello World";
            IntPtr ptr = marshaler.MarshalManagedToNative(oldString);

            string newString = (string)marshaler.MarshalNativeToManaged(ptr);
            marshaler.CleanUpNativeData(ptr);

            Assert.Equals(oldString, newString);
        }
    }
}
