// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Oxide.Interop
{
    internal unsafe class ULStringMarshaler : ICustomMarshaler
    {
        private readonly bool destroy;

        public static ICustomMarshaler GetInstance(string cookie)
            => new ULStringMarshaler(cookie);

        public ULStringMarshaler(string cookie = null)
        {
            destroy = cookie != @"DoNotDestroy";
        }

        public void CleanUpManagedData(object managedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            if (destroy)
                Ultralight.ulDestroyString(pNativeData);
        }

        public int GetNativeDataSize() => -1;

        public IntPtr MarshalManagedToNative(object managedObj)
        {
            if (managedObj is not string str)
                throw new MarshalDirectiveException($"Cannot marshal {managedObj.GetType().Name} to ULString.");

            fixed (byte* ptr = Encoding.Unicode.GetBytes(str))
                return Ultralight.ulCreateStringUTF16((IntPtr)ptr, (uint)str.Length);
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
            => Encoding.Unicode.GetString(new Span<byte>((void*)Ultralight.ulStringGetData(pNativeData), ((int)Ultralight.ulStringGetLength(pNativeData)) * 2));
    }
}
