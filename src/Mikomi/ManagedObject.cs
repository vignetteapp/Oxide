using System;

namespace Mikomi
{
    public abstract class ManagedObject : IDisposable
    {
        private bool isDisposed;

        internal IntPtr Handle { get; }

        protected ManagedObject(IntPtr handle)
        {
            Handle = handle;
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

            DisposeUnmanaged();

            isDisposed = true;
        }

        ~ManagedObject()
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
