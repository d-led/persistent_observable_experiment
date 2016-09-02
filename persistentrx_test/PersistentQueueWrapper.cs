using DiskQueue;

namespace persistentrx_test
{
    class PersistentQueueWrapper<T> : IQueue<T> where T : class
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
    }
}
