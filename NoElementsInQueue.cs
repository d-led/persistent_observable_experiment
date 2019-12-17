using System;
using System.Runtime.Serialization;

namespace persistentrx_test
{
    [Serializable]
    class NoElementsInQueueException : Exception
    {
    }
}