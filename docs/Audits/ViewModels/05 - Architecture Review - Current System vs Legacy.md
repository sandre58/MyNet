# 🧠 Revue complète de l’architecture ViewModel

**Projet**: `MyNet.UI`  
**Date**: Mai 2026  
**Périmètre**: Nouveau système de ViewModels + dossier `Legacy` (hors UI concrète)  
**Objectif**: Évaluer la pertinence architecturale du socle MVVM en vue d’une librairie de contrôles Avalonia publiable sur NuGet, avec perspective multi-UI à moyen terme.

> Ce document reflète l’état **actuel** du code inspecté dans `src/MyNet.UI`, `src/MyNet.Observable` et `src/MyNet.UI/Legacy`. Il complète et, sur plusieurs points, corrige des constats plus anciens présents dans les documents précédents du dossier `docs/ViewModels`.

---

## Sommaire

1. [Synthèse exécutive](#synthèse-exécutive)
2. [Méthode et périmètre](#méthode-et-périmètre)
3. [Forces actuelles à conserver](#forces-actuelles-à-conserver)
4. [Faiblesses structurelles majeures](#faiblesses-structurelles-majeures)
5. [Analyse détaillée par axe](#analyse-détaillée-par-axe)
6. [Analyse du Legacy](#analyse-du-legacy)
7. [Architecture cible recommandée](#architecture-cible-recommandée)
8. [Roadmap priorisée](#roadmap-priorisée)
9. [Conclusion globale](#conclusion-globale)

---

## Synthèse exécutive

### Verdict court

Le nouveau système va **dans la bonne direction**, mais il est encore **au milieu d’une refonte**, pas au niveau d’un socle MVVM NuGet totalement stabilisé.

En une phrase :

> Les fondations sont prometteuses (`IDialogService`, `DialogResult<T>`, `IListDataProvider<T>`, `ISelectionManager<T>`, builders sorting/grouping), mais l’architecture reste trop centrée sur l’héritage, encore trop XAML-oriented, et plusieurs responsabilités transverses sont mal placées.

### Évaluation synthétique

| Axe | Niveau actuel | Verdict |
|---|---:|---|
| Cohérence globale | 7/10 | Bonne direction, refonte incomplète |
| Testabilité | 7/10 | Bonne sur les listes, moyenne sur workflows d’édition/dialog |
| Découplage UI | 6/10 | Correct pour Avalonia, insuffisant pour un vrai multi-UI |
| Extensibilité | 7/10 | Bon potentiel si la hiérarchie est réduite |
| Maintenabilité long terme | 6/10 | Fragile si les bases héritées restent centrales |
| Qualité API publique | 6/10 | Intuitive par endroits, encore confuse sur plusieurs concepts |

### Recommandation forte

Ne pas continuer à enrichir les grosses classes de base.

La bonne direction est maintenant :

1. **réduire les bases ViewModel** ;
2. **extraire les comportements en composants/compositions** ;
3. **stabiliser un core de présentation indépendant du host UI** ;
4. **laisser Avalonia vivre dans une couche d’adaptation**.

---

## Méthode et périmètre

### Code inspecté

Analyse réalisée sur les composants suivants, entre autres :

- `src/MyNet.UI/ViewModels/...`
- `src/MyNet.UI/Dialogs/...`
- `src/MyNet.UI/Commands/...`
- `src/MyNet.Observable/...`
- `src/MyNet.UI/Legacy/ViewModels/...`
- tests `MyNet.UI.Tests` et `MyNet.Observable.Tests`

### Vérifications effectuées

- cartographie des hiérarchies de ViewModels ;
- comparaison nouveau système vs `Legacy` ;
- revue du pipeline listes / wrappers / sélection ;
- revue du workflow édition / save / cancel / fermeture ;
- revue du système de dialogs et résultats typés ;
- revue des commandes et du `CanExecute` ;
- revue du couplage UI et de la trajectoire multi-UI ;
- lecture de tests existants pour identifier les contrats implicites.

### Note importante sur l’état du dépôt

Lors de la validation, les tests ciblés n’ont pas pu être exécutés complètement car le build est actuellement bloqué par des erreurs dans `src/MyNet.Observable/ObservableObject.cs`.

Ce point n’invalide pas la revue architecturale, mais il a une conséquence forte :

> la base observable qui soutient tout le système ViewModel n’est pas entièrement saine dans l’instantané actuellement analysé.

---

## Forces actuelles à conserver

## 1. Fin progressive des managers statiques legacy

Le passage du modèle ancien basé sur des accès globaux (`DialogManager`, `ToasterManager`, `AppBusyManager`, etc.) vers des services injectés est une excellente décision.

À conserver absolument :

- `IDialogService`
- `IContentDialogService`
- `INotificationPublisher`
- la logique de stratégies de dialog (`IDialogStrategy`)

### Pourquoi c’est important

Cela améliore :

- le découplage ;
- la testabilité ;
- la réutilisabilité ;
- la capacité à fournir plusieurs hosts UI.

---

## 2. `DialogResult<T>` est une vraie amélioration

Le nouveau modèle typé avec :

- `Success`
- `Cancelled`
- `Dismissed`

est bien meilleur que les conventions booléennes ou nullable bool du legacy.

### Bénéfices

- résultat explicite ;
- meilleure lisibilité de l’API ;
- meilleure compatibilité avec des hosts multiples ;
- comportement plus prévisible en cas de fermeture implicite.

---

## 3. `IListDataProvider<T>` est une des meilleures abstractions du nouveau système

La couche liste a déjà franchi un cap important grâce à l’introduction d’une abstraction dédiée pour le pipeline de données.

### Ce que cela apporte

- séparation entre pipeline de données et ViewModel ;
- meilleur mocking en tests ;
- moindre couplage aux collections concrètes ;
- possibilité de spécialiser la source plus tard (cache, remote, virtualisation réelle, pagination serveur).

### Conclusion

C’est une brique structurante à conserver comme noyau de la future architecture liste.

---

## 4. La sélection évolue enfin vers la composition

Le couple :

- `SelectableListViewModel<T>`
- `ISelectionManager<T>`

va dans la bonne direction.

C’est une amélioration nette par rapport aux hiérarchies de ViewModels de sélection du legacy.

---

## 5. Les builders de sorting et grouping sont une bonne décision produit

Les builders :

- `SortingViewModelBuilder<T>`
- `GroupingViewModelBuilder<T>`

améliorent l’ergonomie de l’API publique.

### Pourquoi c’est bien

- la configuration est plus lisible ;
- l’intention est plus claire ;
- l’API est plus discoverable pour un développeur externe.

Ils constituent une bonne base pour une API NuGet plus propre.

---

## 6. Le socle actuel n’est pas directement couplé à Avalonia

Le package `MyNet.UI` ne dépend pas directement d’Avalonia dans les ViewModels analysés.

### C’est un vrai point fort

Cela signifie que le système est aujourd’hui :

- compatible avec Avalonia ;
- relativement indépendant du framework visuel ;
- mieux préparé qu’un système WPF-historique.

### Limite

Cela ne suffit pas encore à qualifier le socle de « réellement UI-agnostique » au sens Blazor / MAUI / WinUI.

---

## Faiblesses structurelles majeures

## 1. `ViewModelBase` concentre trop de responsabilités

`ViewModelBase` hérite de `EditableObject`, ce qui rend presque tous les ViewModels implicitement :

- validables ;
- modifiables ;
- localisables ;
- disposables ;
- porteurs d’un `BusyService` ;
- porteurs d’un `LoadState`.

### Pourquoi c’est un problème

Un ViewModel n’a pas toujours besoin de tous ces comportements.

Exemples :

- une liste n’est pas nécessairement modifiable ;
- un pager n’est pas un objet de dirty tracking ;
- un simple host de dialog n’a pas forcément besoin de validation ;
- un ViewModel d’affichage n’a pas toujours besoin d’un cycle `Load/Refresh/Reset`.

### Conséquences

- classes de base de type « god object » ;
- contrats implicites trop larges ;
- hiérarchies plus difficiles à faire évoluer ;
- API publique moins lisible.

### Recommandation

Ramener `ViewModelBase` à un noyau minimal, puis composer les comportements transverses.

---

## 2. `EditionViewModel` mélange des responsabilités orthogonales

`EditionViewModel` gère en même temps :

- `Save` / `SaveAsync` ;
- `SaveAndClose` ;
- validation ;
- dirty tracking ;
- confirmation avant fermeture ;
- interaction avec le dialog ;
- publication de notifications d’erreur ;
- logique de résultat du dialog.

### Pourquoi c’est problématique

L’objet n’est plus seulement un ViewModel d’édition :

- c’est aussi un orchestrateur de workflow ;
- un contrôleur de fermeture ;
- un point d’intégration UI ;
- un adaptateur de notifications.

### Conséquence principale

L’édition est **couplée structurellement** au fait d’être hébergée dans un dialog, car `EditionViewModel` hérite de `DialogViewModel<bool>`.

Cela pénalise immédiatement :

- l’édition en page ;
- l’édition inline ;
- l’édition dans un panneau latéral ;
- l’édition Blazor ;
- les scénarios non-dialog.

### Recommandation

Sortir le workflow d’édition de la hiérarchie de dialog et le transformer en **session/composant d’édition**.

---

## 3. Deux modèles d’édition coexistent sans être réellement alignés

Le système contient deux axes proches mais distincts :

- `EditionViewModel`
- `ItemEditionViewModel<T>`

### `ItemEditionViewModel<T>` gère

- `Item`
- `OriginalItem`
- `IsDirty`
- `Apply`
- `Reset`

### `EditionViewModel` gère

- validation ;
- save ;
- cancel ;
- fermeture ;
- résultat de dialog ;
- notifications.

### Problème

Ces deux modèles devraient être **orthogonaux** mais sont aujourd’hui séparés d’une manière confuse.

### Conséquences

- duplication conceptuelle ;
- modèle mental difficile à expliquer ;
- surcharge de la hiérarchie ;
- extension plus complexe.

---

## 4. Le système de commandes est inachevé sur un point critique

Les commandes implémentent `IRaiseCanExecuteChanged`, mais la revue du code ne montre pas d’usage réel de `RaiseCanExecuteChanged()` dans les workflows ViewModel inspectés.

### Pourquoi c’est problématique

Les commandes conditionnelles risquent de ne pas être rafraîchies correctement quand l’état change.

Exemples concernés :

- `CanSave()` ;
- `CanValidate()` ;
- `CanApply()` ;
- `HasNextPage` / `HasPreviousPage` ;
- `SelectedCount`.

### Conséquences possibles

- boutons qui restent activés/inactivés à tort ;
- logique correcte côté code mais UX incohérente ;
- difficulté de maintenance sur les écrans complexes.

### Recommandation

Introduire un vrai modèle de commande moderne avec :

- invalidation de `CanExecute` ;
- état d’exécution ;
- politique de concurrence ;
- annulation ;
- gestion d’erreurs.

---

## 5. La sélection est mieux découplée… mais reste conceptuellement incohérente

Le système actuel mélange :

- une sélection réelle sur les items via `SelectionEngine<T>` ;
- une projection wrapper `SelectedWrapper<T>` avec propriété `IsSelected`.

### Problème

Le wrapper porte un état de sélection, mais la sélection réelle est pilotée ailleurs.

Cela crée deux représentations concurrentes de la même information.

### Conséquences

- duplication conceptuelle ;
- risque d’incohérence UI ;
- wrapper « sélectionnable » ambigu ;
- API confuse pour les développeurs externes.

### Recommandation

Choisir une seule source de vérité :

- soit la sélection vit dans un état séparé ;
- soit le wrapper devient réellement la source de vérité.

Le mélange actuel est à éviter.

---

## 6. Le système est XAML-agnostique, pas encore vraiment UI-agnostique

Même sans dépendance directe à Avalonia, l’architecture repose encore fortement sur des concepts desktop/XAML :

- `ICommand`
- `ObservableCollection`
- `ReadOnlyObservableCollection`
- `INotifyPropertyChanged`
- `INotifyCollectionChanged`

### Conclusion

Le système est aujourd’hui bien placé pour :

- Avalonia ;
- WinUI ;
- MAUI XAML.

Mais il n’est pas encore réellement adapté à une réutilisation naturelle côté Blazor.

### Recommandation

Préparer une séparation entre :

- un **core de présentation** ;
- et une **couche d’adaptation XAML**.

---

## 7. La politique de cycle de vie / disposal n’est pas totalement claire

Les dialogs et listes possèdent des objets disposables, des subscriptions Rx et des collections bindables.

Or la politique globale de fermeture / disposal n’est pas encore clairement homogène à l’échelle du système.

### Risque

- fuites mémoire ;
- handlers encore accrochés après fermeture ;
- tests qui passent localement mais dérivent en production dans des scénarios d’ouverture/fermeture répétée.

---

## Analyse détaillée par axe

## 1. Hiérarchie des ViewModels

### Hiérarchie observée

- `ViewModelBase`
  - `WorkspaceViewModel`
    - `DialogViewModel`
      - `EditionViewModel`
      - `ListDialogViewModel`
        - `SelectionDialogViewModel`
    - `ItemViewModel<T>`
      - `ItemEditionViewModel<T>`
    - `TabWorkspaceViewModel`
- `ListViewModelBase<T>`
  - `ListViewModel<T>`
  - `CrudListViewModel<T>`
  - `ListViewModelBase<T, TCollection>`
    - `WrapperListViewModel<...>`
      - `SelectableListViewModel<T>`

### Diagnostic

- la branche `WorkspaceViewModel -> DialogViewModel -> EditionViewModel` est trop verticale ;
- la branche listes est plus saine, mais encore couplée à certaines structures de collection ;
- trop de comportements transverses restent portés par héritage.

### Recommandation

Réserver l’héritage à un socle minimal, et basculer les comportements métiers d’écran vers la composition.

---

## 2. Composition vs héritage

### Constat

L’architecture utilise encore trop l’héritage pour :

- l’édition ;
- la validation ;
- le dirty tracking ;
- la fermeture sécurisée ;
- la navigation de workspace ;
- certains comportements liste.

### Recommandation forte

Transformer progressivement ces comportements en composants :

- `IChangeTracker`
- `IValidator`
- `ISaveHandler`
- `ICloseGuard`
- `ISelectionState<T>`
- `IListPipelineState<T>`
- `IPagingState`

---

## 3. `DialogViewModel`

### Ce qui est pertinent

- `DialogResult<T>` ;
- `IDialogService` ;
- `IContentDialogService` ;
- `IDialogStrategy`.

### Ce qui reste discutable

- `RequestClose()` exposé côté VM ;
- `CloseRequested` comme mécanisme central ;
- `CloseCommand` avec paramètre `bool?` ;
- `Owner` typé en `object` dans `DialogOptions` ;
- coexistence de plusieurs notions de résultat.

### Analyse

Le contenu métier du dialog et son hébergement UI restent encore trop entremêlés.

### Recommandation

Faire du contenu du dialog un objet qui exprime une intention de fermeture et un résultat, tandis que le host UI gère la mécanique réelle d’ouverture/fermeture.

---

## 4. `EditableViewModel` / `EditionViewModel`

### Workflow actuel

Le workflow supporte déjà :

- save sync/async ;
- save-and-close ;
- cancel ;
- validation ;
- confirmation avant fermeture ;
- publication d’erreurs.

### Points positifs

- l’intention fonctionnelle est claire ;
- la confirmation avant fermeture existe ;
- l’usage des services injectés est meilleur que dans le legacy.

### Problèmes

- trop de responsabilités au même endroit ;
- couplage à `MessageBoxResult` ;
- couplage à `IDialogService` ;
- couplage à `INotificationPublisher` ;
- `Save()` sync sur une API async ;
- restes de hooks peu cohérents (`OnClosingWithNoResult`, `OnClosingWithCancelResult`).

### Recommandation forte

Remplacer `EditionViewModel` par une composition autour de :

- une session d’édition ;
- un validateur ;
- un guard de fermeture ;
- une politique de notification ;
- un host de dialog optionnel.

---

## 5. ListViewModels

### Partie saine

- `IListDataProvider<T>` ;
- `ListViewModelBase<T>` ;
- pipeline filtre / tri / regroupement ;
- `DeferRefresh()` ;
- tests associés sur debouncing et disposal.

### Partie à clarifier

- `PagingViewModel` représente surtout un **état de pagination**, pas une pagination réellement appliquée aux données ;
- `VirtualizedListViewModel<T>` est une fenêtre `Skip/Take`, pas une virtualisation au sens fort ;
- `WrapperListViewModel` reste assez lié au modèle wrapper et reconstruit ses groupes de manière brutale ;
- `ListViewModelFactory` et `ListViewModelOptions<T>` commencent à concentrer trop de variantes.

### Recommandation

Conserver le pipeline côté provider / collections et réduire le ViewModel à une façade bindable légère.

---

## 6. Sélection

### Points positifs

- `SelectionEngine<T>` dans `MyNet.Observable` est une bonne brique pure ;
- `ISelectionManager<T>` améliore la composition ;
- des tests existent déjà sur le moteur et la composition.

### Manques / problèmes

- interface trop minimale ;
- pas de `Unselect` exposé ;
- pas d’API `Contains` ;
- ambiguïté avec `SelectedWrapper<T>` ;
- pas d’événement riche avec delta de sélection.

### Recommandation

Faire de la sélection un état explicite, riche et autonome, puis brancher les wrappers/UI par adaptation.

---

## 7. Commandes

### Forces

- `ICommandFactory` injectable ;
- séparation sync / async ;
- helpers `CreateRequired<T>` pratiques.

### Faiblesses

- invalidation `CanExecute` non systématisée ;
- pas d’état `IsRunning` exposé ;
- pas d’annulation ;
- politique de concurrence implicite seulement ;
- erreurs non structurées.

### Recommandation

Introduire une abstraction de commande plus moderne dans le core, puis exposer `ICommand` dans une couche d’adaptation XAML.

---

## 8. Interfaces

### Diagnostic

Plusieurs interfaces restent trop larges ou trop liées à la forme UI actuelle.

Exemples :

- `IWorkspaceViewModel` mélange identité, état, mode, titre et chargement ;
- `IListViewModel<T>` mélange état de liste et configuration du pipeline ;
- `IPagingViewModel` expose directement des commandes UI ;
- `IDialogService` agrège plusieurs domaines (content dialogs, message box, file dialogs).

### Recommandation

Découper les interfaces par capacités plus fines.

---

## 9. Factories / Builders / Extensions

### État actuel

- builders sorting/grouping : pertinents ;
- factory liste : utile, mais commence à s’étendre ;
- options de liste : pratiques mais trop générales.

### Problèmes

- multiplication des variantes ;
- certaines options sont hétérogènes ;
- l’API publique expose déjà quelques détails de structure interne.

### Recommandation

Garder les builders là où ils ajoutent vraiment de la valeur, mais limiter les factories statiques aux cas simples.

---

## 10. Couplage avec l’UI

### Nouveau système

Le nouveau système a fortement réduit le couplage direct à un framework UI concret.

### Mais il reste lié à l’univers XAML/desktop via

- `ICommand` ;
- `ObservableCollection` ;
- `MessageBoxResult` ;
- `DialogOptions.Owner` ;
- des patterns d’interaction adaptés d’abord à un host desktop.

### Recommandation

Préparer une séparation de packages :

- un noyau de présentation ;
- un package XAML ;
- un package Avalonia.

---

## 11. Réactivité / Rx / DynamicData

### Points positifs

- usage réel de Rx/DynamicData ;
- présence de tests de pipeline ;
- usage d’un `Deferrer` pour coalescer certaines mises à jour ;
- attention portée au nettoyage dans plusieurs classes.

### Risques détectés

- politique threading encore floue ;
- `CurrentThreadScheduler` comme fallback UI par défaut, ce qui n’est pas un vrai dispatcher UI ;
- usage de `ConfigureAwait(false)` dans des couches qui mutent aussi de l’état bindable ;
- reconstructions parfois peu incrémentales (`WrapperListViewModel`).

### Recommandation

Définir une stratégie threading claire et homogène pour les mutations bindables.

---

## 12. API publique NuGet

### État actuel

L’API est globalement compréhensible, mais pas encore suffisamment stable ni suffisamment nette pour une large consommation externe.

### Noms ou concepts à revoir

- `EditionViewModel` : trop gros et trop chargé conceptuellement ;
- `VirtualizedListViewModel` : nom trompeur ;
- `PagingViewModel` : représente un état, pas une pagination complète ;
- `WorkspaceViewModel.Mode` / `ScreenMode` : fortement orientés écran ;
- `ListDialogViewModel` : comportement plus large que son nom ne le suggère.

### Recommandation

Avant stabilisation NuGet, clarifier les noms et réduire les concepts ambiguës.

---

## Analyse du Legacy

## Objectif de cette comparaison

Le but n’est pas de reproduire le legacy, mais de :

- récupérer les bonnes idées ;
- supprimer les mauvaises abstractions ;
- moderniser ;
- simplifier ;
- améliorer la maintenabilité.

---

## Ce qu’il faut conserver conceptuellement

### 1. Les workflows d’import

Le legacy contenait de vraies idées utiles :

- import multi-sources ;
- chargement à la demande ;
- sélection des éléments à importer ;
- validation avant confirmation.

### Verdict

À réintégrer, mais sous forme moderne et modulaire, pas via une hiérarchie de ViewModels lourde.

---

### 2. Les éditeurs de collections spécialisées

Exemples legacy pertinents :

- `StringListEditionViewModel`
- `EditableRulesViewModel<T>`

Ces classes révèlent des besoins réels :

- édition de collections ;
- ajout/suppression ;
- tri manuel / move up / move down ;
- validation de contenu.

### Verdict

Les concepts sont utiles. Les bases legacy ne le sont pas.

---

### 3. Certains modules applicatifs spécialisés

Exemples :

- recent files ;
- préférences ;
- règles éditables ;
- shell applicatif.

### Verdict

Utile comme feature pack ou module métier, pas comme fondation du framework ViewModel.

---

## Ce qu’il faut jeter architecturalement

- `DialogManager` statique ;
- `ToasterManager` statique ;
- `Messenger.Default` omniprésent ;
- services globaux d’orchestration d’écran ;
- hiérarchies d’édition monolithiques ;
- couplage direct VM -> UI feedback ;
- conventions de fermeture implicites.

---

## Fonctionnalités perdues qui méritent une réintégration moderne

### 1. Import multi-sources

À reconstruire comme module composé autour de :

- `IImportSource<T>` ;
- `ImportSession<T>` ;
- `ImportItemState` ;
- `ImportValidationSummary`.

---

### 2. Édition de collections réordonnables

À reconstruire autour d’un composant de type :

- `EditableCollectionController<T>` ;
- `ReorderableCollectionEditor<T>`.

---

### 3. Modules applicatifs réutilisables

Recent files, notifications shell, préférences, etc. peuvent devenir des packages ou modules optionnels, mais ne doivent plus polluer le socle MVVM cœur.

---

## Matrice Legacy : à conserver / à jeter / à réintégrer

| Élément Legacy | Intérêt conceptuel | Pattern à conserver ? | Décision |
|---|---|---:|---|
| `Legacy/ViewModels/Edition/EditionViewModel.cs` | Moyen | Non | Jeter l’architecture, conserver l’intention fonctionnelle |
| `Legacy/ViewModels/Edition/ItemEditionViewModel.cs` | Élevé | Partiellement | Réintroduire le concept de snapshot d’édition autrement |
| `Legacy/ViewModels/Import/ImportDialogViewModel.cs` | Élevé | Partiellement | Refaire comme module d’import moderne |
| `Legacy/ViewModels/Import/ImportBySourcesDialogViewModel.cs` | Élevé | Partiellement | Refaire avec orchestration par services/composants |
| `Legacy/ViewModels/Import/ImportablesListViewModel.cs` | Moyen | Non | Garder l’idée métier, pas la hiérarchie |
| `Legacy/ViewModels/Rules/EditableRulesViewModel.cs` | Élevé | Oui, partiellement | Réintroduire comme composant d’édition de collection |
| `Legacy/ViewModels/Edition/StringListEditionViewModel.cs` | Moyen | Oui, conceptuellement | Refaire sous forme générique réutilisable |
| `Legacy/ViewModels/FileHistory/RecentFilesViewModel.cs` | Moyen | Partiellement | Bon exemple de feature pack, pas de base framework |
| `Services/AppBusyManager.cs` | Faible | Non | À supprimer du modèle cible |
| `DialogManager` / `ToasterManager` | Faible | Non | À bannir définitivement |

---

## Architecture cible recommandée

## Principes directeurs

1. **Moins d’héritage, plus de composition**.
2. **Séparer le contenu métier de l’hébergement UI**.
3. **Ne pas faire des dialogs un concept central de l’édition**.
4. **Séparer le core de présentation des adaptateurs XAML/Avalonia**.
5. **Faire de la sélection, validation, dirty tracking et save des comportements explicites**.

---

## Découpage cible par couches

### Couche 1 — Core Presentation

Sans dépendance aux concepts XAML spécifiques.

Responsabilités :

- dirty tracking ;
- validation ;
- save workflow ;
- close guard ;
- sélection ;
- état de liste ;
- état de pagination ;
- résultats de fermeture.

### Couche 2 — XAML Adapter

Responsabilités :

- `ObservableObject` ;
- adaptateurs `ICommand` ;
- collections observables ;
- ViewModel shells légers.

### Couche 3 — Avalonia Adapter

Responsabilités :

- stratégies de dialog Avalonia ;
- intégration file dialogs ;
- scheduler/dispatcher Avalonia ;
- comportements d’overlay/fenêtrage.

---

## Exemple d’architecture cible pour l’édition

```csharp
public interface IEditSession
{
    bool IsDirty { get; }
    ValueTask<ValidationSummary> ValidateAsync(CancellationToken ct = default);
    ValueTask<SaveResult> SaveAsync(CancellationToken ct = default);
    ValueTask RevertAsync(CancellationToken ct = default);
}
```

Puis composition autour de :

- `IEditSession`
- `IUnsavedChangesGuard`
- `IValidationReporter`
- `ISaveCoordinator`

Le dialog devient un host optionnel, pas le centre de l’édition.

---

## Exemple d’architecture cible pour la sélection

```csharp
public interface ISelectionState<T>
{
    SelectionMode Mode { get; }
    IReadOnlyList<T> SelectedItems { get; }
    T? SelectedItem { get; }
    int SelectedCount { get; }
    bool Contains(T item);

    event EventHandler<SelectionChangedEventArgs<T>> Changed;

    void Select(T item);
    void Unselect(T item);
    void Toggle(T item);
    void Clear();
    void Set(IEnumerable<T> items);
}
```

---

## Exemple d’architecture cible pour les dialogs

```csharp
public interface IDialogContent<TResult>
{
    ValueTask<DialogCloseDecision<TResult>> RequestCloseAsync(CancellationToken ct = default);
}
```

Le host UI :

- décide comment afficher ;
- gère l’owner ;
- gère la modalité ;
- gère la fermeture physique ;
- traduit le résultat pour la plateforme.

---

## Roadmap priorisée

## P0 — avant stabilisation NuGet sérieuse

1. **Sortir l’édition de `DialogViewModel`**.
2. **Corriger le modèle de commandes et la propagation de `CanExecute`**.
3. **Résoudre l’incohérence entre sélection réelle et `SelectedWrapper<T>`**.
4. **Définir une politique explicite de disposal/cycle de vie des dialogs et listes**.
5. **Supprimer ou isoler les dépendances legacy restantes encore actives**.
6. **Réparer la base `ObservableObject` actuellement cassée au build**.

---

## P1 — consolidation architecturale

1. réduire `ViewModelBase` ;
2. réduire `WorkspaceViewModel` ;
3. clarifier `Paging` vs pagination réelle ;
4. renommer `VirtualizedListViewModel` ;
5. scinder les interfaces trop larges.

---

## P2 — trajectoire produit / plateforme

1. séparer le package core du package XAML ;
2. créer une couche d’adaptation Avalonia dédiée ;
3. préparer un chemin crédible vers Blazor si cet objectif est conservé ;
4. retransformer les concepts utiles du legacy en modules propres.

---

## Conclusion globale

## Cohérence actuelle

**Bonne, mais encore inachevée.**

Le système n’est plus un legacy maquillé ; il y a de vraies améliorations structurelles. Mais les choix centraux ne sont pas encore suffisamment stabilisés pour constituer une base MVVM NuGet haut de gamme sans dette forte.

## Maintenabilité

**Correcte à court terme, moyenne à long terme** si les grosses classes de base restent le pivot du système.

## Scalabilité

**Bonne sur la partie listes/pipeline**, plus fragile sur les workflows d’édition/dialog.

## Flexibilité

**Bonne pour Avalonia et l’écosystème XAML**, insuffisante pour une ambition réellement multi-UI tant qu’un noyau de présentation indépendant n’est pas isolé.

## Qualité architecturale

Le système possède :

- de bonnes intuitions ;
- plusieurs briques déjà modernes ;
- un potentiel élevé ;
- mais encore un centre de gravité trop basé sur l’héritage et les ViewModels « orchestrateurs ».

## Jugement final

> Le système actuel est un **excellent socle de refonte**, mais pas encore un framework MVVM NuGet totalement abouti.

La priorité n’est plus d’ajouter des bases abstraites.

La priorité est maintenant de :

> **réduire les bases, extraire les comportements, stabiliser un core clair, puis construire les adaptateurs UI par-dessus.**

---

## Annexe — constats de validation

### Tests / build

Une tentative de validation ciblée a échoué à cause d’erreurs de compilation dans `src/MyNet.Observable/ObservableObject.cs`.

### Implication

Le diagnostic architectural ci-dessus reste valide, mais toute phase de refactoring devrait commencer par la stabilisation de cette base observable.

