namespace persistentrx_test
{
    public interface IQueue<T>
    {
        void Enqueue(T item);
        T Dequeue();
    }
}
