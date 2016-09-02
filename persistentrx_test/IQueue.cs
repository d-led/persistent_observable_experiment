namespace persistentrx_test
{
    public interface IQueue<T> where T : class
    {
        void Enqueue(T item);
        T Dequeue();
    }
}
