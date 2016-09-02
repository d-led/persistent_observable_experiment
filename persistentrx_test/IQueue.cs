using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace persistentrx_test
{
    public interface IQueue<T> where T : class
    {
        void Enqueue(T item);
        T Dequeue();
    }

}
