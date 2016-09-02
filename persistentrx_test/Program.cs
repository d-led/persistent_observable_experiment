﻿using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace persistentrx_test
{
    public class WorkItem
    {
        public long WorkId { get; set; }
    }

    static class Program
    {
        static void Main()
        {
            try
            {
                Run();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        static void Run()
        {
            IQueue<WorkItem> queue = new PersistentQueueWrapper<WorkItem>("q1");

            // produce some in the background
            Task.Run(() =>
            {
                var key_pressed = Observable
                    .Timer(TimeSpan.FromSeconds(1))
                    .Select(_ => Console.ReadKey());

                Observable
                    .Interval(TimeSpan.FromSeconds(0.5))
                    .Select(_ => new WorkItem { WorkId = DateTime.Now.ToFileTimeUtc() })
                    .TakeUntil(key_pressed)
                    .Subscribe(item => queue.Enqueue(item));
                Console.WriteLine("Press any key to stop producing");
            });

            // consume with a timeout
            queue
                .ToObservableItems(
                    sleep_for: TimeSpan.FromMilliseconds(10),
                    max_wait: TimeSpan.FromSeconds(2)
                )
                .Subscribe(
                    x => Console.WriteLine($"{x.WorkId}"),
                    e => Console.Error.WriteLine(e),
                    () => Console.WriteLine("No more items")
                );
        }
    }
}