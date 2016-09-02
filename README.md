# Durable queues as IObservables experiment for fun

```c#
// create a queue
using (var queue = new PersistentQueueWrapper<WorkItem>("q1")) {
  queue.Enqueue(new WorkItem { WorkId = DateTime.Now.ToFileTimeUtc() });
}
// and lose it

// reopen the durable queue
using (var queue = new PersistentQueueWrapper<WorkItem>("q1")) {
 // enqueue
 queue.Enqueue(new WorkItem { WorkId = DateTime.Now.ToFileTimeUtc() });

 // dequeue until timed out
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

Using
-----

* [DiskQueue](https://github.com/i-e-b/DiskQueue) for persistent queue
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) for serialization

later, using [sharpqueue](https://github.com/sharptools/sharpqueue)
