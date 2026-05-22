# TODO — MyNet

Date: 2026-05-22

Ce fichier regroupe les tâches restantes, priorités et critères d'acceptation pour terminer les améliorations du système `MyNet`, en particulier le module `MyNet.Observable` et la préparation à la publication des packages.

## Checklist rapide
- [ ] Ajouter les behaviors essentiels au système de behaviors (voir liste et priorités ci‑dessous)
- [ ] Mettre à jour les `README` (racine + projets dans `src/`)
- [ ] Mettre à jour les icônes dans `assets/` et les références dans les README/nuget
- [ ] Mettre à jour les descriptions et métadonnées des packages (csproj / Directory.Packages.props)
- [ ] Ajouter tests unitaires pour les nouveaux behaviors et la logique d'observabilité
- [ ] Mettre à jour le `CHANGELOG.md` / notes de version
- [ ] Vérifier CI / pipeline (build / tests / pack / publish)
- [ ] Documenter exemples d’usage (Quick Start) pour `MyNet.Observable`

---

## 1) Behaviors — liste & priorités

But : enrichir `BehaviorRegistry` pour couvrir besoins courants d’objets observables (validation, undo/redo, logging, change tracking, etc.).

Recommandation d'emplacement : `src/MyNet.Observable/Behaviors/`

Priorité "Must"
- LoggingBehavior (Must)
  - Journalise chaque mutation (PropertyChanging / PropertyChanged) avec : sender, nom de propriété, old/new, timestamp
  - Utiliser `ILogger` si présent ; niveau configurable (Info/Debug)
  - Tests unitaires qui vérifient les appels au logger
- ChangeTrackingBehavior (Must)
  - Fournit `IsDirty`, `GetChanges()` et `Reset()`
  - Tests : vérifier marquage des propriétés modifiées
- UndoRedoBehavior (Must)
  - Pile d’actions pour `Undo()` / `Redo()` (stocke old/new)
  - API : `CanUndo`, `CanRedo`
  - Tests : annulation/rétablissement corrects
- UIThreadMarshallingBehavior (Must pour usage UI)
  - Assure dispatch des notifications (`PropertyChanged`) sur le thread UI via `SynchronizationContext`

Priorité "Should"
- Throttling/CoalescingBehavior
  - Regroupe notifications fréquentes et émet en batch après délai configurable
- AuditTrailBehavior
  - Historique détaillé exportable
- SerializationBehavior
  - Aide snapshots/restauration des états

Nice-to-have
- AutoResetBehavior, LazyLoadBehavior, CollectionSyncBehavior, RxIntegrationBehavior

Pour chaque behavior :
- créer tests unitaires dans `tests/`
- ajouter exemples d’utilisation dans `docs/` ou `src/.../README.md`
- documenter intégration via DI (méthode d’extension `IServiceCollection` si pertinent)

---

## 2) Mettre à jour les README

Fichiers ciblés :
- `README.md` racine
- `src/*/README.md` pour chaque sous-projet (au moins `MyNet.Observable`, `MyNet.Utilities`)
- `docs/` (ajouter Quick Start, exemples d’usage)

Checklist README :
- Titre & courte description
- Installation (NuGet, versions compatibles)
- Quick Start (ex : classe héritant `ObservableObject`, enregistrement d’un behavior)
- API highlights (ProcessPropertyChanging, NotifyPropertyChanged with before/after, Behaviors.Register)
- Contribution / build instructions (dotnet sdk, commandes)
- Licence, contact, badges (CI, tests, nuget)

Commandes à montrer :
```powershell
dotnet restore
dotnet build -c Release
dotnet test
dotnet pack -c Release -o artifacts
```

Critères d’acceptation : README clair, exemple compilable, badges visibles.

---

## 3) Icônes

Emplacement : `assets/`

Tâches :
- Vérifier format et résolutions (préférer SVG + PNG variants : 16, 32, 128, 256, 512)
- Générer variantes et conserver dans `assets/icons/` ou `assets/` avec nom cohérent
- Mettre à jour références dans `csproj` / `nuspec` / `README`

Critères : icônes disponibles, référencées dans les packages, rendu correct sur nuget.org/local

---

## 4) Descriptions & métadonnées des packages (NuGet)

Fichiers ciblés :
- `src/*/*.csproj`
- `Directory.Packages.props`, `package.props`

Champs à vérifier : `<Description>`, `<PackageTags>`, `<Authors>`, `<RepositoryUrl>`, `<PackageIcon>`, `<PackageLicenseExpression>`

Checklist :
- Courte description (<= 250 chars)
- Long description (README.md inclus dans le package si possible)
- Tags pertinents (observable; propertychanged; mvvm; validation)
- S’assurer que l’icône référencée est bien packée
- Mettre à jour la version et CHANGELOG

Exemple snippet (à ajouter dans .csproj) :
```xml
<Description>Bibliothèque MyNet.Observable — helpers pour objets observables et behaviors</Description>
<PackageTags>observable;propertychanged;mvvm;validation</PackageTags>
<RepositoryUrl>https://github.com/sandre58/MyNet2</RepositoryUrl>
```

---

## 5) Tests & CI

Actions :
- Ajouter tests unitaires pour behaviors (Logging, Validation, ChangeTracking, UndoRedo)
- Tests d’intégration minimal pour vérifier interaction `ObservableObject` + behaviors
- Mettre à jour pipeline CI (GitHub Actions / Azure DevOps) pour build/test/pack

Commandes utiles :
```powershell
dotnet test --no-build
dotnet pack src/MyNet.Observable/MyNet.Observable.csproj -c Release -o artifacts
```

---

## 6) Documentation / exemples

Créer dossier `samples/` ou `examples/` contenant :
- Petit projet console montrant :
  - définition d’une classe héritant `ObservableObject`
  - enregistrement de `LoggingBehavior` et `ChangeTrackingBehavior`
  - modification de propriété, affichage des notifications, undo
- Guides : "How to add a behavior", "How to implement validation rules"

Critères : exemples compilables et exécutables

---

## 7) Release / packaging

Étapes :
- Mettre à jour `CHANGELOG.md`
- Bump versions (Directory.Packages.props ou csproj)
- Préparer notes de release
- Tester packing localement, puis publier

---

## 8) Qualité & règles de style

Vérifier :
- `stylecop.json` respecté
- Build sans warnings bloquants (ou lister warnings acceptés)
- Accessibilité des docs (alt text images, liens) 

---

## Modèle de ticket (exemples)

- Tâche : Implémenter `LoggingBehavior`
  - Estimation : 2-4h
  - Critères : tests unitaires, README snippet, injection via DI

- Tâche : Implémenter `ValidationBehavior`
  - Estimation : 4-8h
  - Critères : support rules, tests, exemple

- Tâche : README par package
  - Estimation : 1-2h / projet

- Tâche : Générer icônes
  - Estimation : 1-3h

---

## Exemples / snippets rapides

Extrait d’utilisation d’un behavior dans le constructeur d’un objet :
```csharp
// dans la classe dérivée de ObservableObject
this.Behaviors.Register(new LoggingBehavior(logger));
```

Exemple conceptuel de règle de validation :
```csharp
validationBehavior.AddRule(nameof(MyProp), value => value != null && ((string)value).Length > 0, "MyProp must not be empty");
```

---
