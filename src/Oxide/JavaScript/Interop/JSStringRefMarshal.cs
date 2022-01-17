using System;
using System.Runtime.InteropServices;
using System.Text;

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
                JSCore.JSStringRelease(pNativeData);
        }

        public int GetNativeDataSize() => -1;

        public IntPtr MarshalManagedToNative(object managedObj)
        {
            if (managedObj is not string str)
                throw new MarshalDirectiveException($"Cannot marshal {managedObj.GetType().Name} to ULString.");

            return JSCore.JSStringCreateWithUTF8CString(str);
        }

        public unsafe object MarshalNativeToManaged(IntPtr pNativeData)
        {
            uint copied = 0;
            uint length = JSCore.JSStringGetMaximumUTF8CStringSize(pNativeData);
            byte[] data = new byte[(int)length];

            fixed (byte* pointer = data)
            {
                copied = JSCore.JSStringGetUTF8CString(pNativeData, pointer, length);
            }

            Array.Resize(ref data, (int)copied - 1);
            return Encoding.UTF8.GetString(data);
        }
    }
}
