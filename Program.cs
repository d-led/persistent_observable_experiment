using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace persistentrx_test
{
    public struct WorkItem
    {
        public long WorkId { get; set; }
    }

    static class Program
    {
        static void Main()
        {
            try
            {
                Example();
                QueueAsObservableExample();
                DurableObservableExample();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        static void DurableObservableExample()
        {
            PrintTitle(nameof(DurableObservableExample));
            // post some items
            using (var queue = new PersistentQueueWrapper<WorkItem>("q1"))
            {
                Observable
                    .Range(0, 2)
                    .Select(_ => NewWorkItem())
                    .Persistent(queue, TimeSpan.FromMilliseconds(0));
            }

            // post some more items and observe the previously enqueued ones
            using (var queue = new PersistentQueueWrapper<WorkItem>("q1"))
            {
                Observable
                    .Interval(TimeSpan.FromSeconds(1))
                    .Take(3)
                    .Select(_ => NewWorkItem())
                    .Persistent(queue, TimeSpan.FromMilliseconds(100))
                    .Subscribe(item => Console.WriteLine(item.WorkId))
                    ;
            }
        }

        static void PrintTitle(string title)
        {
            Console.WriteLine($"----{title}----");
        }

        static void Example()
        {
            PrintTitle(nameof(Example));
            using (var queue = new PersistentQueueWrapper<WorkItem>("q2"))
            {
                queue.Enqueue(new WorkItem { WorkId = DateTime.Now.ToFileTimeUtc() });
            }

            using (var queue = new PersistentQueueWrapper<WorkItem>("q2"))
            {
                queue.Enqueue(NewWorkItem());
                queue
                   .ToObservableItems(
                       sleep_for: TimeSpan.FromSeconds(0.3),
                       max_wait: TimeSpan.FromSeconds(1)
                   )
                   .Subscribe(
                       x => Console.WriteLine($"{x.WorkId}"),
                       e => Console.Error.WriteLine(e),
                       () => Console.WriteLine("No more items")
                   );
            }
        }

        static WorkItem NewWorkItem()
        {
            return new WorkItem { WorkId = DateTime.Now.ToFileTimeUtc() };
        }

        static void QueueAsObservableExample()
        {
            PrintTitle(nameof(QueueAsObservableExample));
            using (var queue = new PersistentQueueWrapper<WorkItem>("q3"))
            {
                // produce some in the background
                Task.Run(() =>
                {
                    Observable
                        .Interval(TimeSpan.FromSeconds(0.5))
                        .Select(_ => new WorkItem { WorkId = DateTime.Now.ToFileTimeUtc() })
                        .Take(5)
                        .Subscribe(item => queue.Enqueue(item));
                    Console.WriteLine("Press any key to stop producing");
                });

                // consume with a timeout
                queue
                    .ToObservableItems(
                        sleep_for: TimeSpan.FromSeconds(0.3),
                        max_wait: TimeSpan.FromSeconds(1)
                    )
                    .Subscribe(
                        x => Console.WriteLine($"{x.WorkId}"),
                        e => Console.Error.WriteLine(e),
                        () => Console.WriteLine("No more items")
                    );
            }
        }
    }
}
