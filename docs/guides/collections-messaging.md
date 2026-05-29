# Collections & messaging

Observable collections with efficient range change notifications, and a **weak-reference messenger** for in-process MVVM messaging.

## Packages

| Package | Responsibility |
|---------|----------------|
| [MyNet.Collections](../../src/MyNet.Collections/README.md) | Range collections, keyed collections, synchronizers |
| [MyNet.Messaging](../../src/MyNet.Messaging/README.md) | `Messenger`, weak actions, typed messages |

---

## MyNet.Collections

### ObservableRangeCollection

Use when the UI binds to a list that receives **batch** updates (better than resetting `ObservableCollection`).

```csharp
using MyNet.Collections;

var items = new ObservableRangeCollection<string>();
items.AddRange(["a", "b", "c"]);
items.ReplaceRange(0, 1, ["A", "B"]); // replace 1 item at index 0 with two items
items.RemoveRange(0, 2);
```

Implements `IObservableRangeCollection<T>` with `INotifyCollectionChanged` + `INotifyPropertyChanged`.

### Keyed collections

`ObservableKeyedCollection<TKey, T>` — dictionary-backed observable collection with stable keys.

`ReadOnlyObservableKeyedCollection<TKey, T>` — read-only projection for UI binding.

### Synchronizers

`ICollectionSynchronizer` / `ICollectionSorter<T>` — keep a master list and a view list in sync (sort/filter at collection level before Observable's `ExtendedCollection`).

Use when you need collection-level sync without DynamicData. For rich filter/sort pipelines, prefer [Observable → Extended collections](observable.md#extended-collections-filter-sort-group-selection).

### UI thread dispatch

From Observable package, range collections can be scheduled to the UI thread:

```csharp
using MyNet.Observable.Collections.Extensions;
using MyNet.Utilities.Collections;

var uiList = new ObservableRangeCollection<Row>().Scheduled(Scheduler.CurrentThread);
```

---

## MyNet.Messaging

Lightweight **pub/sub** inspired by MVVM messengers: weak references so subscribers can be collected when views disappear.

### Register and send

```csharp
using MyNet.Messaging;

public sealed class OrderPlacedMessage(int OrderId) : MessageBase;

public sealed class OrderSummaryViewModel
{
    public OrderSummaryViewModel()
    {
        Messenger.Default.Register<OrderPlacedMessage>(this, OnOrderPlaced);
    }

    private void OnOrderPlaced(OrderPlacedMessage msg) =>
        RefreshTotal(msg.OrderId);
}

// Publisher (anywhere):
Messenger.Default.Send(new OrderPlacedMessage(42));
```

### Cleanup

```csharp
Messenger.Default.Unregister(this); // in IDisposable.Dispose
```

### Custom messenger instance

`Messenger.Default` is a process-wide singleton. Create `new Messenger()` for isolated scopes (e.g. per window).

### Property-changed messages

`PropertyChangedMessage<T>` — optional typed wrapper when relaying property changes across modules.

### Threading

Handlers run on the publisher's thread unless you marshal inside the handler. Pair with UI thread helpers from Utilities/Observable for view updates.

---

## Used by

| Consumer | Usage |
|----------|--------|
| **MyNet.Observable** | `ExtendedCollection<T>`, selection |
| **MyNet.UI** | Optional cross-VM events; shell does not require Messenger |

---

## Testing

- `tests/MyNet.Collections.Tests` — range and keyed collections
- `tests/MyNet.Messaging.Tests` — register, send, weak cleanup

## Related

- [Observable models](observable.md)
- [UI presentation layer](ui.md)
