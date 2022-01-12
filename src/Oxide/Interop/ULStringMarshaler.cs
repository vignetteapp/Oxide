using System;
using System.Runtime.InteropServices;

namespace Oxide.Interop
{
    internal class ULStringMarshaler : ICustomMarshaler
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

            return Ultralight.ulCreateStringUTF8(str, (uint)str.Length);
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
            => Marshal.PtrToStringUni(Ultralight.ulStringGetData(pNativeData));
    }
}

namespace Oxide
{

#pragma warning disable CA2101

    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateStringUTF8([MarshalAs(UnmanagedType.LPUTF8Str)] string ptr, uint length);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyString(IntPtr str);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulStringGetLength(IntPtr str);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr ulStringGetData(IntPtr str);
    }

#pragma warning restore CA2101

}
