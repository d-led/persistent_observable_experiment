using DiskQueue;
using System;

namespace persistentrx_test
{
    class PersistentQueueWrapper<T> : IDisposable, IQueue<T> where T : class
    {
        readonly IPersistentQueue queue;

        public PersistentQueueWrapper(string filename)
        {
            queue = new PersistentQueue(filename);
        }

        public T Dequeue()
        {
            using (var session = queue.OpenSession())
            {
                var bytes = session.Dequeue();
                session.Flush();
                return bytes.FromBytes<T>();
            }
        }

        public void Enqueue(T item)
        {
            using (var session = queue.OpenSession())
            {
                session.Enqueue(item.ToBytes());
                session.Flush();
            }
        }

        #region IDisposable Support
        bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    queue?.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
