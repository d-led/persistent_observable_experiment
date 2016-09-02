# Durable queues as IObservables experiment for fun

```c#
using (var queue = new PersistentQueueWrapper<WorkItem>("q1")) {
  queue.Enqueue(new WorkItem { WorkId = DateTime.Now.ToFileTimeUtc() });
}

using (var queue = new PersistentQueueWrapper<WorkItem>("q1")) {
 // enqueue
 queue.Enqueue(new WorkItem { WorkId = DateTime.Now.ToFileTimeUtc() });

 // dequeue
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
```

&darr;

```
131473231919577487
131473231921304305
No more items
```
