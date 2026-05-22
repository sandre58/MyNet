# 📋 PLAN D'ACTION - MyNet.Utilities Audit

**Date**: 11 Mai 2026  
**Priorité**: CRITIQUE - Blocking Path to NuGet.org Release  
**Timeline**: 2-3 semaines recommandé

---

## 🚨 PHASE 1: BLOCKERS (Jours 1-2) - CRITIQUE

### 1️⃣ Erreur CS0103: RandomNumber n'existe pas

**Fichiers affectés**:
- `src/MyNet.Utilities.Generator.Extensions/Internet.cs` (65-72)
- `src/MyNet.Utilities.Generator.Extensions/AddressGenerator.cs` (38)

**Erreur exacte**:
```csharp
// ❌ CS0103: The name 'RandomNumber' does not exist in the current context
RandomNumber  
```

**Actions requises**:

1. **Investigation (1h)**:
   ```bash
   # Chercher où RandomNumber est défini ou devrait être
   cd "D:\repos\github\sandre58\MyNet2"
   grep -r "RandomNumber" src/ --include="*.cs"
   grep -r "class RandomNumber" src/ --include="*.cs"
   grep -r "public.*RandomNumber" src/ --include="*.cs"
   ```

2. **Hypothèses et solutions**:

   **Hypothèse A**: C'est une extension method qui manque
   ```csharp
   // À chercher dans Generator.Extensions
   public static int RandomNumber(...) => RandomGenerator.Service.Number(...)
   ```
   
   **Hypothèse B**: C'est la classe RandomGeneratorService
   ```csharp
   // À chercher dans MyNet.Utilities.Generator
   public class RandomGeneratorService : IRandomGeneratorService
   {
       public int Number(int min, int max) => ...
   }
   ```

3. **Solutions possibles**:
   
   **Option 1**: Ajouter extension method dans Internet.cs
   ```csharp
   namespace MyNet.Utilities.Generator.Extensions;
   
   internal static class RandomNumberExtensions
   {
       public static int RandomNumber(this int max) 
           => RandomGenerator.Service.Number(0, max);
       
       public static int RandomNumber(this (int min, int max) range)
           => RandomGenerator.Service.Number(range.min, range.max);
   }
   ```

   **Option 2**: Utiliser RandomGenerator.Service directement
   ```csharp
   // Avant (erreur):
   public int SomeNumber = RandomNumber();
   
   // Après (correct):
   public int SomeNumber = RandomGenerator.Service.Number(0, 100);
   ```

4. **Recommandation**: 
   - [ ] Vérifier quelle syntax est utilisée dans le reste du code
   - [ ] Si RandomNumber est style attendu, implémenter extension method
   - [ ] Documenter dans CONTRIBUTING.md les conventions pour Generator.Extensions

**Propriétaire**: Dev qui connaît RandomGenerator intent

---

### 2️⃣ Erreur CS0117: TranslationService.RegisterResources n'existe pas

**Fichiers affectés**:
- `src/MyNet.Utilities.Generator.Extensions/ResourceLocator.cs` (25-27)

**Erreur exacte**:
```csharp
// ❌ CS0117: 'TranslationService' does not contain a definition for 'RegisterResources'
TranslationService.RegisterResources(...)
```

**Actions requises**:

1. **Investigation (1h)**:
   ```bash
   grep -r "class TranslationService" src/
   grep -r "RegisterResources" src/
   grep -r "Localization" src/MyNet.Utilities/
   ```

2. **Contexte probable**:
   ```csharp
   // ResourceLocator.cs utilise likely:
   // - AddressResources.Designer.cs (auto-generated)
   // - InternetResources.Designer.cs (auto-generated)
   // - Besoin de les enregistrer auprès de TranslationService
   ```

3. **Solutions possibles**:

   **Option A**: TranslationService a changé d'API
   ```csharp
   // Vérifier la signature actuelle:
   public class TranslationService
   {
       // Quelle méthode utiliser pour enregistrer ressources?
       // RegisterResources -> ? ou autre?
       // ResourceProvider -> ?
   }
   ```

   **Option B**: Implémenter RegisterResources si manquant
   ```csharp
   public static class TranslationService
   {
       public static void RegisterResources(
           Type resourceType, 
           string cultureName = "")
       {
           // Enregistrer ressources pour localization
       }
   }
   ```

4. **Fichier à examiner**:
   - Chercher définition complète de `TranslationService`
   - Vérifier interfaces implémentées ou classes dérivées
   - Regarder les usages corrects de TranslationService dans le codebase

**Propriétaire**: Dev maintenir Localization.Extensions

---

### 3️⃣ Erreur CS1061: string.Translate() n'existe pas

**Fichiers affectés**:
- `src/MyNet.Utilities.Generator.Extensions/NameGenerator.cs` (38, 41, 43, 45)

**Erreur exacte**:
```csharp
// ❌ CS1061: 'string' does not contain a definition for 'Translate'
var translated = name.Translate()
```

**Actions requises**:

1. **Investigation (1h)**:
   ```bash
   grep -r "\.Translate\(" src/ --include="*.cs" | head -10
   grep -r "public.*Translate" src/ --include="*.cs"
   find src/MyNet.Utilities.Localization* -type f
   ```

2. **Pattern probable**:
   ```csharp
   // NameGenerator.cs utilise pattern:
   var frenchName = someName.Translate();  // Translate using default culture?
   var translatedName = someName.Translate("fr");  // Translate to specific culture?
   ```

3. **Solutions**:

   **Option A**: Extension method Translate manquante
   ```csharp
   // Ajouter dans NameGenerator.cs ou source partagée:
   namespace MyNet.Utilities.Generator.Extensions;
   
   internal static class TranslationExtensions
   {
       public static string Translate(this string key, string culture = "")
       {
           // Chercher translation dans ResourceProvider
           return TranslationService.GetString(key, culture) ?? key;
       }
   }
   ```

   **Option B**: Utiliser API TranslationService directement
   ```csharp
   // Avant:
   var translated = name.Translate();
   
   // Après:
   var translated = TranslationService.GetString(name) ?? name;
   ```

4. **Vérifier aussi**:
   - [ ] Impact sur d'autres générateurs qui utilisent Translate
   - [ ] Si c'est pattern cohérent dans suite ou cas special
   - [ ] Documentation sur comment traduire noms générés

**Propriétaire**: Dev maintenir Generator.Extensions

---

## 📋 PHASE 2: DOCUMENTATION SYNTAXE C# 14 EXTENSION(...) (Jour 2-3)

**Status**: ✅ Syntaxe C# 14 officielle
```csharp
public static class StringExtensions
{
    extension(string? value)  // ✅ C# 14 official extension syntax
    {
        public string OrEmpty() => value ?? string.Empty;
    }
    
    extension(string value)   // ✅ C# 14 extension method
    {
        public string Random(char separator = Separator) { ... }
    }
}
```

**Actions requises**:

1. **Ajouter documentation XML**:
   ```csharp
   /// <summary>
   /// Extension methods for string types using C# 14 syntax.
   /// </summary>
   /// <remarks>
   /// This file uses the C# 14 extension method syntax which provides
   /// a cleaner, more readable approach compared to traditional
   /// <c>this T parameter</c> syntax.
   /// </remarks>
   public static class StringExtensions
   {
       extension(string? value)
       {
           public string OrEmpty() => value ?? string.Empty;
       }
   }
   ```

2. **Update CONTRIBUTING.md**:
   - [ ] Add C# 14 features section
   - [ ] Document extension(...) syntax with examples
   - [ ] Link to official documentation

3. **Update README.md**:
   - [ ] Mention C# 14 requirement
   - [ ] Add language version to prerequisites

**Propriétaire**: Dev team / Documentation

---

## 🔧 PHASE 3: ISSUES MAJEURS (Semaine 1-2)

### 4️⃣ CA1859 Performance Warning

**Fichier**: `src/MyNet.Utilities/Messaging/Messenger.cs:544`

**Issue**:
```csharp
private readonly record struct WeakActionAndToken
{
    public IExecuteWithObject? Action { get; init; }  // ❌ Interface générique
    public object? Token { get; init; }
}
```

**Solution (30 min Implementation)**:

```csharp
// Avant (avec warning):
private readonly record struct WeakActionAndToken
{
    public IExecuteWithObject? Action { get; init; }  // Interface générique
    public object? Token { get; init; }
}

// Après (type concret):
private readonly record struct WeakActionAndToken<TMessage>
{
    public WeakAction<TMessage>? Action { get; init; }  // ❌ Peu complexe
    public object? Token { get; init; }
}
```

**Implémentation détaillée**:
- [ ] Refactoriser SendToTargetOrType pour utiliser generic struct
- [ ] Mettre à jour dictionnaires recipients pour utiliser struct générique
- [ ] Vérifier toutes les utilisations de WeakActionAndToken
- [ ] Tests pour vérifier pas de regression

**Complexité**: Moyenne  
**Timeline**: 2-3h  
**Propriétaire**: Dev Messaging

---

### 5️⃣ Dépendances Preview

**Problème**: 5 packages en preview
```xml
<PackageVersion Include="Microsoft.Extensions.DependencyInjection" 
    Version="11.0.0-preview.3.26207.106" />  <!-- Preview -->
<PackageVersion Include="System.Reactive" 
    Version="7.0.0-preview.1" />  <!-- Preview -->
<PackageVersion Include="StyleCop.Analyzers" 
    Version="1.2.0-beta.556" />  <!-- Beta -->
```

**Solutions**:

1. **Attendre Releases (Recommandé)**:
   ```
   Target: Fin Mai 2026 (estimation .NET)
   Timeline: Attendre 2-4 semaines
   Risque: Bas (versions stables compatibles généralement)
   Action: Mettre dans backlog "Post v1.0.0"
   ```

2. **Pinner versions compatibles (Acceptable)**:
   ```xml
   <!-- Si versions RT ne sont pas dispo bientôt: -->
   <PackageVersion Include="Microsoft.Extensions.DependencyInjection.Abstractions" 
       Version="10.0.0" />  <!-- LTS compatible -->
   <PackageVersion Include="System.Reactive" 
       Version="6.0.0" />  <!-- Stable antérieure -->
   ```

3. **Mix stratégie (Pragmatique)**:
   ```xml
   <!-- Runtime deps: toujours stable ou LTS -->
   <PackageVersion Include="Microsoft.Extensions.DependencyInjection" 
       Version="10.0.0" />  <!-- Dernier stable -->
   
   <!-- Analyzer deps: OK en preview (pas dans NuGet package) -->
   <PackageVersion Include="StyleCop.Analyzers" 
       Version="1.2.0-beta.556" />  <!-- PrivateAssets="all" -->
   ```

**Recommandation**:
- [ ] **Reclasser deps** en Runtime vs Development vs Analyzer
- [ ] **Utiliser versions 10.0.x** pour DependencyInjection (stable)
- [ ] **Attendre** System.Reactive 7.0.0 ou downgrade à 6.0.0
- [ ] **Documenter** dans README que v1.0.0 cible .NET 10 preview (inclure caveat)

**Propriétaire**: Dev Lead / PM

---

## 📊 PHASE 4: QUALITY IMPROVEMENTS (Semaine 2-3)

### 6️⃣ Code Coverage Reporting

**Statut**: Coverage configuré mais reports manquants

**Solution**:

1. **Générer reports**:
   ```bash
   # Terminal dans workspace
   cd "D:\repos\github\sandre58\MyNet2"
   
   # Construire avec coverage
   dotnet test tests/MyNet.Utilities.Tests/MyNet.Utilities.Tests.csproj \
     -c Release \
     --collect:"XPlat Code Coverage"
   
   # Résultats dans TestResults/
   ```

2. **Installer ReportGenerator** (optionel mais recommandé):
   ```bash
   dotnet tool install -g dotnet-reportgenerator-globaltool
   
   # Générer rapport HTML
   reportgenerator \
     -reports:"TestResults/**/coverage.cobertura.xml" \
     -targetdir:"coverage_reports" \
     -reporttypes:HtmlInline
   ```

3. **Publier reports**:
   - GitHub Pages (sandre58.github.io/MyNet/)
   - Badge dans README

4. **Target**: >= 80% coverage
   - [ ] Vérifier couverture actuelle
   - [ ] Ajouter tests manquants pour Generator.Extensions
   - [ ] Configurer CI pour bloquer < 80%

**Timeline**: 1j  
**Propriétaire**: QA / Dev

---

### 7️⃣ Google Maps API Security

**Fichier**: `src/MyNet.Utilities/Google/Maps/Constants.cs`

**Issue**:
```csharp
// ❌ Pas de scheme explicite
public const string ApiRegionFromLatLong = 
    "maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false";
```

**Solution**:

```csharp
// ✅ Ajouter https://
public const string ApiRegionFromLatLong = 
    "https://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false";

public const string ApiLatLongFromAddress = 
    "https://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false";

public const string ApiDirections = 
    "https://maps.googleapis.com/maps/api/directions/xml?origin={0}&destination={1}&sensor=false";
```

**Validation**:
- [ ] Tests passent avec https://
- [ ] Pas d'erreurs certificat SSL
- [ ] Documenter requirement pour Google Maps API Key

**Timeline**: 30min  
**Propriétaire**: Dev Google Services

---

### 8️⃣ Documentation et Examples

**Actions**:

1. **Ajouter examples dans XML comments**:
   ```csharp
   /// <summary>
   /// Sends a message to registered recipients.
   /// </summary>
   /// <example>
   /// <code>
   /// // Enregistrer un recipient
   /// var messenger = Messenger.Default;
   /// messenger.Register&lt;MyMessage&gt;(this, message => 
   /// {
   ///     Console.WriteLine($"Message reçu: {message.Text}");
   /// });
   /// 
   /// // Envoyer un message
   /// messenger.Send(new MyMessage { Text = "Hello" });
   /// </code>
   /// </example>
   public virtual void Send<TMessage>(TMessage message) 
       => SendToTargetOrType(message, null, null);
   ```

2. **Améliorer READMEs**:
   ```markdown
   ## Quick Start Examples
   
   ### Authentication
   ```csharp
   var service = new WindowsAuthenticationService();
   var user = service.AuthenticateCurrentUser();
   ```
   
   ### Messaging Pattern
   ```csharp
   messenger.Register<MyMessage>(recipient, msg => HandleMessage(msg));
   messenger.Send(new MyMessage());
   ```
   ```

3. **Documenter syntaxe customisée** (si applicable)

**Timeline**: 2-3j  
**Propriétaire**: Dev + Tech Writer

---

## 🚀 PHASE 5: RELEASE READINESS (Jour 1 récurrent)

### Checklist de Publication v1.0.0

```markdown
## Build & Test
- [ ] dotnet build -c Release /p:TreatWarningsAsErrors=true  (0 errors/warnings)
- [ ] dotnet test succeeds avec >80% coverage
- [ ] Pas de issues StyleCop/Roslyn/Roslynator
- [ ] IntelliSense works dans VS2022
- [ ] Nuget package builds: dotnet pack -c Release

## Nuget Metadata
- [ ] Version = 1.0.0 (pas preview)
- [ ] Description complets + examples
- [ ] PackageIcon existe et valide
- [ ] README.md inclus et formaté
- [ ] License = MIT
- [ ] Repository = GitHub URL correct
- [ ] Tags = searchable et appropriés

## Documentation
- [ ] Changelog.md mis à jour
- [ ] README.md packages = English + French (ou decision)
- [ ] API docs générés (XML → HTML via Sandcastle ou DocFX)
- [ ] Guides utilisateurs prêts

## Security
- [ ] Pas de CVE connus (validate_cves)
- [ ] Pas de hardcoded secrets
- [ ] Google APIs require auth (documented)
- [ ] Cryptography patterns reviewed

## Compatibility
- [ ] Tests sur .NET 8, 9, 10  (si cible .NET 8)
- [ ] Tests sur Windows/Linux
- [ ] Tests breaking changes vs v0.x
- [ ] Semver confrmed (major/minor/patch)

## Metrics
- [ ] Code coverage >= 80%
- [ ] Test count >= 100 pour core package
- [ ] API surface finalized
- [ ] Performance benchmarks baseline (optional)
```

---

## 📆 TIMELINE PROPOSÉE

```
Jour 1-2 (ASAP)
├─ Investigation 3 blockers (RandomNumber, TranslationService, Translate)
├─ Identifier syntaxe extension(...)
└─ Créer issues GitHub pour chaque blocker

Semaine 1 (Days 3-5)
├─ Implémenter fixes RandomNumber
├─ Implémenter fixes TranslationService
├─ Implémenter fixes Translate
└─ Tester compilation: dotnet build -c Release

Semaine 2 (Days 6-10)
├─ ✅ Corriger CA1859 warning
├─ ✅ Dépendances: décider attendre vs pinner
├─ ✅ Générer code coverage reports
├─ ✅ Google Maps: ajouter https://
└─ ✅ Tests: >80% coverage

Semaine 3 (Days 11-15)
├─ ✅ Code review de tous les fixes
├─ ✅ Ajouter examples XML comments
├─ ✅ Améliorer READMEs
├─ ✅ Final security review
├─ ✅ Nuget package test build
└─ ✅ Publication test (pre-release)

Jour 16
└─ ✅ v1.0.0 Release to NuGet.org 🎉
```

---

## 📝 FOLLOW-UP QUESTIONS

Pour l'équipe de développement:

1. **Blockers**:
   - [ ] Où RandomNumber devrait être définit?
   - [ ] TranslationService API courante?
   - [ ] Are you targeting .NET 8+ for C# 14 support?

2. **Strategy**:
   - [ ] Attendre dépendances RTM ou downgrade?
   - [ ] .NET 10 preview OK pour v1.0.0?
   - [ ] Coverage target 80% ou plus?

3. **Infrastructure**:
   - [ ] CI/CD pipeline pour validations?
   - [ ] Access NuGet.org pour publish?
   - [ ] CodeCov.io ou autre coverage tracker?

---

**Fin du Plan d'Action**  
*Next: Assigner issues et commencer Phase 1*

