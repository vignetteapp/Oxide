using System;
using System.Runtime.InteropServices;

namespace Oxide.JavaScript.Interop
{
    internal class JSStringRefMarshal : ICustomMarshaler
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
                Ultralight.ulDestroyString(pNativeData);
        }

        public int GetNativeDataSize() => -1;

        public IntPtr MarshalManagedToNative(object managedObj)
        {
            if (managedObj is not string str)
                throw new MarshalDirectiveException($"Cannot marshal {managedObj.GetType().Name} to ULString.");

            return JSCore.JSStringCreateWithUTF8CString(str);
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
            => Marshal.PtrToStringUni(JSCore.JSStringGetCharactersPtr(pNativeData));
    }
}

namespace Oxide.JavaScript
{
    public partial class JSCore
    {
        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSStringCreateWithUTF8CString([MarshalAs(UnmanagedType.LPUTF8Str)] string ptr);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void ulDestroyString(IntPtr str);

        [DllImport(LIB_WEBCORE, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr JSStringGetCharactersPtr(IntPtr str);
    }
}
