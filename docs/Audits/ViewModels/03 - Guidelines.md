# 📘 Guidelines & Patterns pour Futurs ViewModels

**Complément aux analyses : Standards de conception pour la couche ViewModel MyNet.UI**

---

## 🎯 Principes Cardinaux

### 1. Single Responsibility (SRP)

**Chaque ViewModel = UNE responsabilité claire**

✅ **BON**:
```csharp
// WorkspaceViewModel: Gère titre, mode, refresh, navigation
public class WorkspaceViewModel : ViewModelBase 
{
    public string? Title { get; set; }
    public ScreenMode Mode { get; set; }
    public Task RefreshAsync();
    public Task ResetAsync();
    public event EventHandler<NavigationRequestEventArgs> NavigationRequested;
}

// DialogViewModel: Gère fermeture et résultat
public class DialogViewModel<TResult> : ViewModelBase
{
    public ICommand CloseCommand { get; }
    public TResult? Result { get; }
    public Task<bool> CanCloseAsync();
}

// ListViewModelBase: Gère pipeline filtrage/tri/groupement/pagination
public class ListViewModelBase<T> : ViewModelBase
{
    public IFiltersViewModel<T>? Filters { get; }
    public ISortingViewModel<T>? Sorting { get; }
    public IGroupingViewModel<T>? Grouping { get; }
    public IPagingViewModel? Paging { get; }
}
```

❌ **MAUVAIS**:
```csharp
// ❌ MultiViewModel fait 7+ choses
public class MultiViewModel : ViewModelBase
{
    // Workspace stuff
    public string? Title { get; set; }
    public Task RefreshAsync();
    
    // List stuff
    public IReadOnlyList<Item> Items { get; }
    public Task FilterAsync(Func<Item, bool> predicate);
    
    // Dialog stuff
    public ICommand CloseCommand { get; }
    
    // Selection stuff
    public Item? SelectedItem { get; set; }
    public void Select(Item item);
    
    // ❌ Trop de responsabilités!
}
```

---

### 2. Composition over Inheritance

**Préférer composition pour configurer, héritage pour étendre comportement**

✅ **BON** (composition):
```csharp
// Configuration injectée, pas héritage
public class ListViewModelBase<T> : ViewModelBase
{
    protected IListDataProvider<T> Provider { get; }  // Injecté
    
    public ListViewModelBase(
        IListDataProvider<T> provider,
        IFiltersViewModel<T>? filters = null,        // Config composée
        ISortingViewModel<T>? sorting = null,        // Config composée
        IGroupingViewModel<T>? grouping = null,      // Config composée
        IPagingViewModel? paging = null,             // Config composée
        IBusyService? busyService = null,
        IScheduler? scheduler = null)
        : base(busyService)
    {
        Provider = provider;
        Filters = filters;
        Sorting = sorting;
        Grouping = grouping;
        Paging = paging;
    }
}

// Sélection comme composition
public class SelectableListViewModel<T> : ListViewModelBase<T>
{
    private readonly ISelectionManager<T> _manager;  // Composée!
    
    public SelectableListViewModel(
        IListDataProvider<T> provider,
        ISelectionManager<T> manager,               // Injecté
        ListViewModelOptions<T>? options = null)
        : base(provider, options?.Filters, ...)
    {
        _manager = manager;
    }
    
    public IReadOnlyList<T> SelectedItems => _manager.Selected;
    public void Select(T item) => _manager.Select(item);
}
```

❌ **MAUVAIS** (héritage excessif):
```csharp
// ❌ 3 niveaux héritage
public class Base<T> : ViewModelBase { }
public class List<T> : Base<T> { }
public class SelectionList<T> : List<T> { }  // ❌ Trop profond

// ❌ Chaque niveau ajoute couplage
public class SelectionList<T, TCollection> : WrapperList<T, Selection<T>, TCollection>
    where TCollection : ExtendedWrapperCollection<T, SelectedWrapper<T>> { }
    // ❌ Type complexes, impossible à mocker
```

---

### 3. Dépendance d'Injection (explicite)

**Jamais new, toujours injecter**

✅ **BON**:
```csharp
public class MyListViewModel : ListViewModelBase<Item>
{
    private readonly IItemService _service;
    
    public MyListViewModel(
        IListDataProvider<Item> provider,
        IItemService service,              // ✅ Injecté
        ListViewModelOptions<Item>? options = null)
        : base(provider, options)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }
    
    public async Task RefreshAsync()
    {
        var items = await _service.GetItemsAsync();  // Utilise service
        // Update list
    }
}
```

❌ **MAUVAIS**:
```csharp
public class MyListViewModel : ListViewModelBase<Item>
{
    private ItemService _service = new();  // ❌ new!
    
    public async Task RefreshAsync()
    {
        var items = await _service.GetItemsAsync();
    }
}
```

---

### 4. Asynchrone Explicite

**Utiliser ExecuteStateAsync pour Load, ExecuteAsync pour Refresh**

✅ **BON**:
```csharp
public class MyViewModel : WorkspaceViewModel
{
    private readonly IDataService _service;
    
    protected override async Task OnLoadAsync(CancellationToken cancellationToken)
    {
        // Load une fois, gère State automatiquement
        Data = await _service.GetAsync(cancellationToken);
    }
    
    protected override async Task OnRefreshAsync(CancellationToken cancellationToken)
    {
        // Refresh, ne change pas State au succès
        Data = await _service.GetAsync(cancellationToken);
    }
    
    protected override void OnExecutionError(Exception exception)
    {
        // Gérer à la classe niveau
        Logger.Error(exception);
        // Important: ne pas relancer
    }
}

// Usage depuis UI:
await viewModel.LoadAsync();    // State → Loading → Loaded (ou Error)
await viewModel.RefreshAsync(); // Recharge, gère BusyService
```

❌ **MAUVAIS**:
```csharp
public async Task LoadAsync()
{
    await Task.Delay(1000);  // ❌ Pas de GetAsync
    Data = GetDataSync();     // ❌ Sync dans async!
}

public async Task RefreshAsync()
{
    State = LoadState.Loading;  // ❌ Manual state management
    try { ... }
    catch { throw; }            // ❌ Pas d'error handling
}
```

---

### 5. Gestion Dispose & Cleanup

**Enregistrer TOUT dans Disposables, nettoyer dans Cleanup()**

✅ **BON**:
```csharp
public class MyViewModel : ViewModelBase
{
    private IDisposable? _subscription;
    
    public MyViewModel(IObservable<Item> source)
    {
        // Enregistrer subscription dans Disposables
        _subscription = source.Subscribe(item =>
        {
            // Handle item
        });
        Disposables.Add(_subscription);  // ✅ Auto-cleanup!
    }
    
    // OU utiliser RX Observable pattern
    public MyViewModel(IObservable<Item> source)
    {
        Disposables.Add(
            source.Subscribe(item =>
            {
                // Handle
            })
        );  // ✅ Implicit cleanup via Disposables collection
    }
    
    [SuppressMessage("Usage", "CA2213")]  // Disposed in Cleanup
    private readonly SemaphoreSlim _lock = new(1, 1);
    
    protected override void Cleanup()
    {
        // ✅ Cleanup explicite si nécessaire
        _lock?.Dispose();
        
        // Event unsubscribe si pas via Disposables
        ItemService.ItemChanged -= HandleItemChanged;
        
        base.Cleanup();  // ✅ Toujours appeler!
    }
}
```

❌ **MAUVAIS**:
```csharp
public class MyViewModel : ViewModelBase
{
    private IDisposable _subscription;
    
    public MyViewModel(IObservable<Item> source)
    {
        _subscription = source.Subscribe(...);
        // ❌ Pas ajouté à Disposables = memory leak!
    }
    
    // ❌ Pas de Cleanup()
    
    // ❌ Event subscription sans unsubscribe
    public MyViewModel()
    {
        ItemService.ItemChanged += HandleItemChanged;  // ❌ Leak
    }
}
```

---

### 6. Événements vs Propriétés

**Events pour notifications, propriétés pour état**

✅ **BON**:
```csharp
// État → Propriété (observable)
public string? Title { get; set; }
public LoadState State { get; private set; }
public IReadOnlyList<Item> Items { get; }

// Notifications → Événement
public event EventHandler<FiltersChangedEventArgs<T>>? FiltersChanged;
public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;
public event EventHandler<CloseRequestedEventArgs>? CloseRequested;

// Usage:
vm.Title = "New Title";  // Binding synchrone
vm.FiltersChanged += (s, e) => { /* React */ };  // Subscription
```

❌ **MAUVAIS**:
```csharp
// ❌ State exposé via event
public event EventHandler<StateChangedEventArgs>? StateChanged;
private LoadState _state;
public LoadState State
{
    get => _state;
    private set
    {
        _state = value;
        StateChanged?.Invoke(this, new(_state));  // ❌ Prop + event
    }
}

// ❌ Notification manquée
public void ApplyFilters()
{
    // ❌ Pas d'événement → UI ne sait pas update
}
```

---

## 🏗️ Patterns Recommandés

### Pattern 1: ViewModelBase + Configuration

```csharp
// Base unifiée pour tous
public abstract class ViewModelBase(IBusyService? busyService = null) 
    : EditableObject, IIdentifiable<Guid>
{
    public Guid Id { get; } = Guid.NewGuid();
    public LoadState State { get; private set; } = LoadState.NotLoaded;
    public IBusyService BusyService { get; } = busyService ?? EmptyBusyService.Instance;
    
    protected Task LoadAsync(CancellationToken ct) 
        => ExecuteStateAsync<IndeterminateBusy>(...);
}

// Configurations composées
public class ListViewModelOptions<T>
{
    public IFiltersViewModel<T>? Filters { get; init; }
    public ISortingViewModel<T>? Sorting { get; init; }
    public IGroupingViewModel<T>? Grouping { get; init; }
    public IPagingViewModel? Paging { get; init; }
    public IBusyService? BusyService { get; init; }
    public IScheduler? Scheduler { get; init; }
}

// Utilisation:
var options = new ListViewModelOptions<Item>
{
    Sorting = SortingViewModel<Item>.CreateBuilder()
        .AddProperty(x => x.Name)
        .Build(),
    Paging = new PagingViewModel(pageSize: 50)
};

var vm = new ListViewModelBase<Item>(provider, options);
```

### Pattern 2: Builders pour Configuration Complex

```csharp
// Fluent builder
var sorting = SortingViewModel<Item>.CreateBuilder()
    .AddProperty(x => x.Name, cfg => cfg.AsDefault().Ascending())
    .AddProperty(x => x.Date)
    .Build();

var filtering = FiltersViewModel<Item>.CreateBuilder()
    .AddCondition(x => x.Status, "Status")
    .AddCondition(x => x.Priority, "Priority")
    .Build();

// Tout injecté dans ListViewModelBase
var vm = new ListViewModelBase<Item>(
    provider,
    filters: filtering,
    sorting: sorting
);
```

### Pattern 3: Interfaces pour Abstraction

```csharp
// Chaque concept = interface
public interface IFiltersViewModel<T>
{
    IFilter<T>? CurrentFilter { get; }
    bool HasActiveFilters { get; }
    void Apply();
    void Clear();
    void Reset();
    event EventHandler<FiltersChangedEventArgs<T>>? FiltersChanged;
}

public interface IListDataProvider<T>
{
    IReadOnlyList<T> Source { get; }
    IReadOnlyList<T> Items { get; }
    void SetFilter(IFilter<T>? filter);
    void SetSorting(params ISortingProperty<T>[] sorting);
    void SetGrouping(params IGroupingProperty<T>[] grouping);
}

public interface ISelectionManager<T>
{
    SelectionMode Mode { get; set; }
    IReadOnlyList<T> Selected { get; }
    void Select(T item);
    void Toggle(T item);
    void SetSelection(IEnumerable<T> items);
    void ClearSelection();
}

// Composition:
public class ListViewModelBase<T>
{
    protected IListDataProvider<T> Provider { get; }
    public IFiltersViewModel<T>? Filters { get; }
    public ISortingViewModel<T>? Sorting { get; }
    // ... etc
}
```

### Pattern 4: Events pour Cascades

```csharp
private void SubscribeToConfigurationEvents()
{
    var deferrer = new Deferrer(ApplyAllConfigurationChanges);
    
    // Tous les changements déclenchent un seul refresh
    Filters?.FiltersChanged += (_, e) =>
    {
        CurrentFilter = e.Filter;
        deferrer.Request();  // ← Batch!
    };
    Sorting?.SortingChanged += (_, e) =>
    {
        CurrentSorting = e.Sorting;
        deferrer.Request();  // ← Batch!
    };
    Grouping?.GroupingChanged += (_, e) =>
    {
        CurrentGrouping = e.Grouping;
        deferrer.Request();  // ← Batch!
    };
    Paging?.PagingChanged += (_, _) =>
    {
        deferrer.Request();  // ← Batch!
    };
}

private void ApplyAllConfigurationChanges()
{
    // ✅ Une seule fois, même si 4 sources changent
    ApplyFilter();
    ApplySorting();
    ApplyGrouping();
    UpdatePaging();
}
```

### Pattern 5: Mediator pour Commandes Globales (Advanced)

```csharp
// Interface médiateur
public interface IViewModelMediator
{
    void Send<TRequest, TResponse>(TRequest request) 
        where TRequest : IRequest<TResponse>;
    
    event EventHandler<NavigationRequestedEventArgs>? NavigationRequested;
    event EventHandler<DialogRequestedEventArgs>? DialogRequested;
}

// Implémentation
public class ViewModelMediator : IViewModelMediator
{
    public void Send<TRequest, TResponse>(TRequest request)
        where TRequest : IRequest<TResponse>
    {
        // Route commandes
        if (request is EditItemRequest editReq)
            NavigationRequested?.Invoke(this, new EditItemArgs(editReq.Item));
        else if (request is DeleteItemRequest delReq)
            DialogRequested?.Invoke(this, new ConfirmDeleteArgs(delReq.Item));
    }
}

// Usage dans ViewModel
public class ItemListViewModel : ListViewModelBase<Item>
{
    private readonly IViewModelMediator _mediator;
    
    public ItemListViewModel(
        IListDataProvider<Item> provider,
        IViewModelMediator mediator)
        : base(provider)
    {
        _mediator = mediator;
    }
    
    public void EditItem(Item item)
    {
        // Deléguer au médiateur
        _mediator.Send(new EditItemRequest(item));
    }
}
```

---

## 🧪 Checklist Design Nouveau ViewModel

```markdown
ARCHITECTURE
☐ Hérite de ViewModelBase (ou ItemViewModel, DialogViewModel)
☐ Implémente une interface claire (IWorkspaceViewModel, etc.)
☐ Responsabilité unique et bien définie
☐ Pas d'héritage profond (max 2 niveaux)
☐ Composition pour les configurations

DÉPENDANCES
☐ DI d'interfaces, pas new
☐ IBusyService optionnel (via paramètre)
☐ IScheduler optionnel (via paramètre)
☐ Services métier injectés

ASYNCHRONISME
☐ ExecuteStateAsync<TBusy>() pour Load
☐ ExecuteAsync() pour Refresh/Reset
☐ CancellationToken partout
☐ OnExecutionError() implémenta pour logging

GESTION RESSOURCES
☐ EventHandler unsub ou via Disposables.Add()
☐ IDisposable fields ajoutés à Disposables
☐ Cleanup() override appelant base.Cleanup()
☐ SemaphoreSlim/Locks disposés

ÉTAT & PROPRIÉTÉS
☐ LoadState exposé via State
☐ Propriétés avec backing fields si complexe
☐ OnPropertyChanged via Fody ou EditableObject
☐ Pas mutable state sans protection

ÉVÉNEMENTS
☐ EventHandler<TArgs> pour notifications
☐ Noms suivant convention [Subject]Changed
☐ Args héritants EventArgs
☐ Documentation XML sur chaque event

COMMANDES
☐ ICommand exposées si UI binding nécessaire
☐ Via ICommandFactory injecté (Create / CreateNotNull)
☐ Pas de Commands si optionnelles
☐ CanExecute callbacks clairs

TESTABILITÉ
☐ Dépendances mockables
☐ Interfaces claires pour mock
☐ Pas de UI coupling
☐ Pas de threads/delays

DOCUMENTATION
☐ XML docs sur classe et members publics
☐ Exemples usage si complexe
☐ Remarks pour comportement non évident
☐ Obsolete attr si dépréated

PERFORMANCE
☐ Pas de LINQ chaud (hot path)
☐ Pas d'allocations inutiles
☐ Debouncing/throttling si cascades
☐ Lazy<T> pour initialization coûteuse
```

---

## 🎓 Exemples Complets

### Exemple 1: ViewModel Simple (ItemViewModel)

```csharp
/// <summary>
/// Provides a view model for displaying a single item.
/// </summary>
public abstract class ItemViewModel<T> : ViewModelBase, IItemViewModel<T>
{
    /// <summary>
    /// Gets or sets the current item.
    /// </summary>
    public T? Item { get; protected set; }

    /// <summary>
    /// Sets the current item directly.
    /// </summary>
    public virtual void SetItem(T? item) => Item = item;

    /// <summary>
    /// Handles item changes (unsubscribe old, subscribe new).
    /// </summary>
    protected virtual void OnItemChanged(T? oldValue, T? newValue)
    {
        UnsubscribeFromItem(oldValue);
        SubscribeToItem(newValue);
        HandleItemChanged();
    }

    private void SubscribeToItem(T? item)
    {
        if (item is INotifyPropertyChanged notifyPropertyChanged)
            notifyPropertyChanged.PropertyChanged += HandleItemPropertyChanged;
    }

    private void UnsubscribeFromItem(T? item)
    {
        if (item is INotifyPropertyChanged notifyPropertyChanged)
            notifyPropertyChanged.PropertyChanged -= HandleItemPropertyChanged;
    }

    private void HandleItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        => HandleCurrentItemPropertyChanged(e);

    /// <summary>
    /// Override to react to item property changes.
    /// </summary>
    protected virtual void HandleCurrentItemPropertyChanged(PropertyChangedEventArgs e) { }

    /// <summary>
    /// Override to react to item being set/changed.
    /// </summary>
    protected virtual void HandleItemChanged() { }

    protected override void Cleanup()
    {
        UnsubscribeFromItem(Item);
        base.Cleanup();
    }
}

// Usage:
public class PersonViewModel : ItemViewModel<Person>
{
    protected override void HandleCurrentItemPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Person.FirstName))
            OnPropertyChanged(nameof(DisplayName));  // Update derived property
    }
    
    public string DisplayName => $"{Item?.FirstName} {Item?.LastName}";
}
```

### Exemple 2: ViewModel avec List + Selection

```csharp
/// <summary>
/// Displays and manages a selectable list of items.
/// </summary>
public class SelectableItemsViewModel : ListViewModelBase<Item>
{
    private readonly IItemService _service;
    private readonly ISelectionManager<Item> _selection;

    public SelectableItemsViewModel(
        IListDataProvider<Item> provider,
        IItemService service,
        ISelectionManager<Item> selection,
        ListViewModelOptions<Item>? options = null)
        : base(provider, options?.Filters, options?.Sorting, 
               options?.Grouping, options?.Paging, 
               options?.BusyService, options?.Scheduler)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _selection = selection ?? throw new ArgumentNullException(nameof(selection));
        
        _selection.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ISelectionManager<Item>.Selected))
                OnPropertyChanged(nameof(SelectedItems));
        };
    }

    public IReadOnlyList<Item> SelectedItems => _selection.Selected;
    public Item? SelectedItem => _selection.SelectedItem;
    public int SelectedCount => _selection.SelectedCount;

    public void Select(Item item) => _selection.Select(item);
    public void Toggle(Item item) => _selection.Toggle(item);
    public void ClearSelection() => _selection.ClearSelection();

    protected override async Task OnLoadAsync(CancellationToken cancellationToken)
    {
        var items = await _service.GetAllAsync(cancellationToken).ConfigureAwait(false);
        // Update provider with items
    }

    protected override void Cleanup()
    {
        _selection?.Dispose();
        base.Cleanup();
    }
}
```

### Exemple 3: ViewModel Workspace avec Dialog

```csharp
/// <summary>
/// Workspace containing a list and edit dialog.
/// </summary>
public class ItemManagementWorkspace : WorkspaceViewModel
{
    private readonly IItemService _service;

    public ItemManagementWorkspace(IItemService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        
        ListViewModel = new SelectableItemsViewModel(
            provider: ...,
            service: service);
    }

    public SelectableItemsViewModel ListViewModel { get; }

    public DialogViewModel<Item>? EditDialog { get; private set; }

    protected override async Task OnLoadAsync(CancellationToken cancellationToken)
    {
        await ListViewModel.LoadAsync(cancellationToken).ConfigureAwait(false);
    }

    protected override async Task OnRefreshAsync(CancellationToken cancellationToken)
    {
        await ListViewModel.RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    public void EditItem(Item item)
    {
        EditDialog = new ItemEditionDialog(item, _service);
        RequestNavigation(EditDialog);  // Notify host
    }

    protected override string? CreateTitle() => "Item Management";
}

public class ItemEditionDialog : DialogViewModel<Item>
{
    private readonly ItemEditionViewModel _vm;

    public ItemEditionDialog(Item item, IItemService service)
    {
        _vm = new ItemEditionViewModel(service);
        _vm.SetOriginalItem(item);
        
        Title = "Edit Item";
    }

    protected override bool CanClose(bool? force) => force.GetValueOrDefault() || !_vm.IsDirty;
}
```

---

## 📊 Métriques Qualité

### Bonnes pratiques scoring

```
Pour chaque nouveau ViewModel, évaluer:

✅ Excellent (90-100%)
  - Héritage 0-1 niveaux
  - 1 responsabilité claire
  - 100% DI
  - Tests unitaires >80%
  - Cleanup systématique
  
✅ Très Bon (80-89%)
  - Héritage max 2 niveaux
  - 2 responsabilités liées
  - DI 100%
  - Tests >60%
  - Cleanup complet
  
⚠️ Bon (70-79%)
  - Héritage max 3 niveaux
  - 2-3 responsabilités
  - DI >80%
  - Tests >40%
  
❌ À Améliorer (<70%)
  - Héritage >3 niveaux
  - >3 responsabilités
  - DI <80%
  - Tests <40%
  - Leaks mémoire
```

---

## 🚀 Prochaines Étapes

1. **Immédiat** (cette sprint): Implémenter P0 blocker (2-4 semaines)
2. **Court terme** (1 mois): Refactor SelectionListViewModel → SelectableListViewModel
3. **Moyen terme** (3 mois): CRUD services, virtualisation
4. **Long terme** (6+ mois): Mediator, advanced patterns

---

**Document**: ViewModel Guidelines  
**Statut**: Reference Standard  
**Version**: 1.0  
**Last Updated**: Mai 2026

