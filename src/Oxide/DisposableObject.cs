using System;
using System.Threading;

namespace Oxide
{
    public abstract class DisposableObject : IDisposable
    {
        protected bool IsDisposed { get; private set; }
        protected readonly bool IsOwned;

        private volatile int disposeSignaled = 0;
        private readonly IntPtr handle;

        internal IntPtr Handle
        {
            get
            {
                if (IsDisposed && IsOwned)
                    throw new ObjectDisposedException(GetType().Name);

                if (handle == IntPtr.Zero)
                    throw new InvalidOperationException(@"This object has yet to be initialized.");

                return handle;
            }
        }

        protected DisposableObject(IntPtr handle, bool owned = true)
        {
            IsOwned = owned;
            this.handle = handle;
        }

        protected virtual void DisposeUnmanaged()
        {
        }

        protected virtual void DisposeManaged()
        {
        }

        protected void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref disposeSignaled, 1) != 0)
                return;

            if (IsDisposed)
                return;

            if (disposing)
                DisposeManaged();

            if (IsOwned)
                DisposeUnmanaged();

            IsDisposed = true;
        }

        ~DisposableObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
