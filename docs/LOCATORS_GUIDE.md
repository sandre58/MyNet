# Locators (View / ViewModel)

Résolution des paires ViewModel ↔ View et instanciation dans une architecture MVVM.

## Enregistrement DI

```csharp
services.AddViewLocators(configureResolver: resolver =>
{
    // Mappings manuels (prioritaires sur les conventions)
    resolver.Register(typeof(SettingsViewModel), typeof(SettingsView));
});
```

Conventions supplémentaires (opt-in, avant ou après `AddViewLocators` selon la priorité voulue) :

```csharp
services
    .AddNamespaceConvention()           // segment ViewModels/Views, suffixe *View uniquement
    .AddParentNamespaceConvention()     // {Parent}.Views / {Parent}.ViewModels
    .AddAssemblyRootConvention("UI.Views") // {Assembly}.UI.Views.{Name}View
    .AddViewLocators();
```

## Composants

| Service | Rôle |
|---------|------|
| `ITypeResolver` | Résout le **type** de vue à partir d’un ViewModel (conventions + `Register`) |
| `IViewLocator` | Instancie une **vue** : DI d’abord, sinon `Activator` (constructeur sans paramètre) |
| `IViewModelLocator` | Instancie un **ViewModel** : **DI strict** (doit être enregistré) |
| `IViewFactory` | `ViewModel` → instance de vue (`Resolve` + `Get`) |

## Ordre des conventions

`TypeResolver` parcourt les conventions dans l’**ordre d’enregistrement DI** et s’arrête à la première qui retourne un type.

Par défaut, seule `SuffixConvention` est enregistrée (swap `ViewModels`/`Views` + essai de plusieurs suffixes : `View`, `Control`, `Page`, etc.).

Les mappings manuels via `resolver.Register(source, target)` **priment toujours** sur les conventions.

## Politique d’instanciation

- **Views** : enregistrer dans DI pour l’injection de dépendances ; sinon instanciation par réflexion.
- **ViewModels** : toujours enregistrés dans DI (`AddTransient` / `AddScoped`, etc.).

## Création d’une vue depuis un ViewModel

```csharp
public class MyShell(IViewFactory viewFactory)
{
    public void Show<TViewModel>()
        where TViewModel : class
    {
        var view = viewFactory.CreateView(typeof(TViewModel));
        // ou : viewFactory.CreateView<TViewModel, MyView>();
    }
}
```

En cas d’échec (pas de mapping, type incompatible, instanciation impossible), `ViewResolutionException` est levée.

## Layouts de projet supportés

| Convention | ViewModel | View |
|------------|-----------|------|
| `SuffixConvention` (défaut) | `MyApp.ViewModels.PersonViewModel` | `MyApp.Views.PersonView` (ou `PersonPage`, etc.) |
| `NamespaceConvention` | idem | `MyApp.Views.PersonView` uniquement |
| `ParentNamespaceConvention` | `MyApp.Features.ViewModels.XViewModel` | `MyApp.Features.Views.XView` |
| `AssemblyRootConvention` | `Any.Namespace.FooViewModel` | `{Assembly}.UI.Views.FooView` |

## Fichiers

- `Conventions/` — stratégies de nommage (`TypeNamingConventionBase`, helpers)
- `Factories/ViewFactory.cs` — orchestration
- `ViewLocator.cs`, `ViewModelLocator.cs` — instanciation
- `ServiceCollectionExtensions.cs` — enregistrement DI
