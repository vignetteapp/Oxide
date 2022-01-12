using System;

namespace Oxide
{
    public abstract class DisposableObject : IDisposable
    {
        private bool isDisposed;
        private readonly bool owned;

        internal IntPtr Handle { get; }

        protected DisposableObject(IntPtr handle, bool owned = true)
        {
            Handle = handle;
            this.owned = owned;
        }

        protected virtual void DisposeUnmanaged()
        {
        }

        protected virtual void DisposeManaged()
        {
        }

        protected void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            if (disposing)
                DisposeManaged();

            if (owned)
                DisposeUnmanaged();

            isDisposed = true;
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
