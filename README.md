# Durable queues as IObservables experiment for fun

[![Build status](https://ci.appveyor.com/api/projects/status/l82q9ukingqcb4j9/branch/master?svg=true)](https://ci.appveyor.com/project/d-led/persistent-observable-experiment/branch/master)

## Queue to IObservable

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

```text
131473231919577487
131473231921304305
No more items
```

## Making IObservable durable

```c#
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
```

## Using

* [DiskQueue](https://github.com/i-e-b/DiskQueue) for persistent queue
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) for serialization

later, using [sharpqueue](https://github.com/sharptools/sharpqueue)

## Ideas

* Hide the queue lifetime from the observables
* Use another queue as example
