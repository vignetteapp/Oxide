using System;

namespace Oxide
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
        // public string Name => Ultralight.ulSessionGetName(Handle);

        /// <summary>
        /// The disk path to write to (used by persistent sessions only).
        /// </summary>
        // public string DiskPath => Ultralight.ulSessionGetDiskPath(Handle);

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
}
