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
        public static byte[] ToBytes<T>(this T what) where T : class
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(what));
        }

        public static T FromBytes<T>(this byte[] bytes) where T : class
        {
            if (bytes == null)
                return null;

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }

        public static IObservable<T> ToObservableItems<T>(this IQueue<T> queue, TimeSpan sleep_for, TimeSpan max_wait) where T : class
        {
            return Observable.Create<T>(observer =>
            {
                var sw = new Stopwatch();
                sw.Start();

                while (true)
                {
                    var item = queue.Dequeue();

                    if (item == null)
                    {
                        if (sw.Elapsed > max_wait)
                        {
                            observer.OnCompleted();
                            return Disposable.Empty;
                        }

                        Thread.Sleep(sleep_for);

                        continue;
                    }

                    observer.OnNext(item);
                    sw.Restart();
                }
            });
        }
    }
}
