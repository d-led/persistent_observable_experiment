using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;

namespace persistentrx_test
{
    public static class PersistentQueueExtensions
    {
        public static byte[] ToBytes<T>(this T what)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(what));
        }

        public static T FromBytes<T>(this byte[] bytes)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }

        public static IObservable<T> ToObservableItems<T>(this IQueue<T> queue, TimeSpan sleep_for, TimeSpan max_wait)
        {
            return Observable.Create<T>(observer =>
            {
                var sw = new Stopwatch();
                sw.Start();

                while (true)
                {
                    try
                    {
                        var item = queue.Dequeue();
                        observer.OnNext(item);
                        sw.Restart();
                    }
                    catch (NoElementsInQueueException)
                    {

                        if (sw.Elapsed > max_wait)
                        {
                            observer.OnCompleted();
                            return Disposable.Empty;
                        }

                        Thread.Sleep(sleep_for);

                        continue;
                    }
                }
            });
        }
    }
}
