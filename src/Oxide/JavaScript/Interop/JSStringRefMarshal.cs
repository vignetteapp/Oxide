// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Oxide.Javascript.Interop
{
    internal unsafe class JSStringRefMarshal : ICustomMarshaler
    {
        private readonly bool destroy;

        public static ICustomMarshaler GetInstance(string cookie)
            => new JSStringRefMarshal(cookie);

        public JSStringRefMarshal(string cookie = null)
        {
            destroy = cookie != @"DoNotDestroy";
        }

        public void CleanUpManagedData(object managedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            if (destroy)
                JSCore.JSStringRelease(pNativeData);
        }

        public int GetNativeDataSize() => -1;

        public IntPtr MarshalManagedToNative(object managedObj)
        {
            if (managedObj is not string str)
                throw new MarshalDirectiveException($"Cannot marshal {managedObj.GetType().Name} to ULString.");

            fixed (byte* ptr = Encoding.UTF8.GetBytes(str))
                return JSCore.JSStringCreateWithUTF8CString((IntPtr)ptr);
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            uint copied = 0;
            uint length = JSCore.JSStringGetMaximumUTF8CStringSize(pNativeData);
            Span<byte> buffer = new byte[(int)length];

            fixed (byte* pointer = buffer)
                copied = JSCore.JSStringGetUTF8CString(pNativeData, pointer, length);

            return Encoding.UTF8.GetString(buffer[..((int)copied - 1)]);
        }
    }
}
