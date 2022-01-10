using System;
using System.Runtime.InteropServices;

namespace Mikomi
{
    internal class ULString : DisposableObject
    {
        public string Data => Ultralight.ulStringGetData(Handle);
        public int Length => (int)Ultralight.ulStringGetLength(Handle);

        public ULString(IntPtr handle, bool owned = true)
            : base(handle, owned)
        {
        }

        public ULString(string str)
            : base(Ultralight.ulCreateStringUTF16(str, (uint)str.Length))
        {
        }

        protected override void DisposeUnmanaged()
            => Ultralight.ulDestroyString(Handle);
    }

    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CharSet = CharSet.Unicode, BestFitMapping = true, ThrowOnUnmappableChar = true)]
        internal static extern IntPtr ulCreateStringUTF16([MarshalAs(UnmanagedType.LPWStr)] string str, uint length);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyString(IntPtr str);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulStringGetLength(IntPtr str);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CharSet = CharSet.Unicode, BestFitMapping = true, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        internal static extern string ulStringGetData(IntPtr str);
    }
}
