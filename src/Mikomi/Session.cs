using System;
using System.Runtime.InteropServices;
using Mikomi.Graphics;

namespace Mikomi
{
    public class Session : DisposableObject
    {
        /// <summary>
        /// Whether or not is persistent (backed to disk).
        /// </summary>
        public bool Persistent => Ultralight.ulSessionIsPersistent(Handle);

        /// <summary>
        /// Unique numeric Id for the session.
        /// </summary>
        public ulong ID => Ultralight.ulSessionGetId(Handle);

        /// <summary>
        /// Unique name identifying the session (used for unique disk path).
        /// </summary>
        public string Name => Ultralight.ulSessionGetName(Handle);

        /// <summary>
        /// The disk path to write to (used by persistent sessions only).
        /// </summary>
        public string DiskPath => Ultralight.ulSessionGetDiskPath(Handle);

        internal Session(IntPtr handle)
            : base(handle, false)
        {
        }

        public Session(Renderer renderer, bool persistent, string name)
            : base(Ultralight.ulCreateSession(renderer.Handle, persistent, name))
        {
        }

        protected override void DisposeUnmanaged()
            => Ultralight.ulDestroySession(Handle);
    }

#pragma warning disable CA2101 // Custom marshaler is used

    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateSession(
            IntPtr renderer,
            [MarshalAs(UnmanagedType.I1)] bool isPersistent,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string name);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroySession(IntPtr session);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulSessionIsPersistent(IntPtr session);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler), MarshalCookie = "DoNotDestroy")]
        internal static extern string ulSessionGetName(IntPtr session);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulSessionGetId(IntPtr session);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler), MarshalCookie = "DoNotDestroy")]
        internal static extern string ulSessionGetDiskPath(IntPtr session);
    }

#pragma warning restore CA2101

}
