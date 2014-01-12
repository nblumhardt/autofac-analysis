using System;
using System.Threading;

namespace Autofac.Analysis.Transport.Util
{
    /// <summary>
    /// Base class for disposable objects.
    /// </summary>
    public class Disposable : IDisposable
    {
        int _isDisposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            var isDisposed = _isDisposed;
            Interlocked.CompareExchange(ref _isDisposed, 1, isDisposed);
            if (isDisposed == 0)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
