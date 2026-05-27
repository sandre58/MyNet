# UI Architecture (MyNet.UI)

Vue d’ensemble des briques UI partagées dans `MyNet.UI`.

## Locators (View / ViewModel)

Le dossier [`src/MyNet.UI/Locators`](../src/MyNet.UI/Locators) fournit la résolution ViewModel ↔ View et l’instanciation MVVM.

Documentation détaillée : **[Locators README](../src/MyNet.UI/Locators/README.md)**.

Résumé :

- `AddViewLocators()` enregistre `SuffixConvention` par défaut, `ITypeResolver`, `IViewLocator`, `IViewModelLocator`, `IViewFactory`.
- Les **views** peuvent être créées via DI ou `Activator` ; les **view models** exigent un enregistrement DI.
- Les conventions s’évaluent dans l’ordre d’enregistrement ; `ITypeResolver.Register` est prioritaire.
- Les échecs de résolution lèvent `ViewResolutionException`.

```csharp
services.AddViewLocators(r => r.Register(typeof(MainViewModel), typeof(MainView)));
```

## Autres modules

Voir les guides dédiés dans `docs/` (dialogs, notifications, traduction, etc.) au fur et à mesure de leur documentation.
