using System;
using System.Runtime.InteropServices;

namespace Mikomi
{
    internal class ULStringMarshaler : ICustomMarshaler
    {
        private readonly bool destroy;

        public static ICustomMarshaler GetInstance(string cookie)
            => new ULStringMarshaler(cookie);

        public ULStringMarshaler(string cookie)
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

            return Ultralight.ulCreateStringUTF16(str, (uint)str.Length);
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
            => Ultralight.ulStringGetData(pNativeData);
    }

#pragma warning disable CA2101 // Custom marshaler is used

    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = true, ThrowOnUnmappableChar = true)]
        internal static extern IntPtr ulCreateStringUTF16([MarshalAs(UnmanagedType.LPWStr)] string str, uint length);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyString(IntPtr str);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulStringGetLength(IntPtr str);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = true, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        internal static extern string ulStringGetData(IntPtr str);
    }

#pragma warning restore CA2101

}
