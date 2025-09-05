# GitHub Pages Deployment

Ce repository inclut un workflow automatique pour déployer l'application Avalonia Browser Demo sur GitHub Pages.

## 🚀 Déploiement automatique

Le workflow `deploy-pages.yml` se déclenche automatiquement :
- À chaque push sur la branche `main`
- Manuellement via l'interface GitHub Actions

## 🔧 Configuration requise

Pour activer GitHub Pages sur votre repository :

1. **Aller dans Settings** → **Pages**
2. **Source** : Sélectionner "GitHub Actions"
3. **Save** les paramètres

## 📦 Structure du déploiement

```
GitHub Pages Site
├── index.html          # Page d'accueil avec navigation
└── app/                # Application Avalonia WebAssembly
    ├── index.html       # Point d'entrée de l'app Avalonia
    ├── main.js          # Bootstrap JavaScript
    └── _framework/      # Runtime .NET WebAssembly
        ├── dotnet.js
        ├── dotnet.wasm
        ├── *.wasm       # Assemblies .NET
        └── ...
```

## 🌐 URLs

- **Site principal** : `https://[username].github.io/[repository]/`
- **Application demo** : `https://[username].github.io/[repository]/app/`

## 🛠️ Build local

Pour tester localement l'application WebAssembly :

```bash
# Installer les workloads nécessaires
dotnet workload install wasm-tools

# Naviguer vers le projet Browser
cd demos/MyNet.Avalonia.Demo.Browser

# Build et publish
dotnet publish -c Release -f net10.0-browser

# Servir localement (exemple avec Python)
cd bin/Release/net10.0-browser/publish/wwwroot
python -m http.server 8000

# Ouvrir http://localhost:8000 dans le navigateur
```

## 🔍 Debugging

En cas de problème :

1. **Vérifier les logs** dans GitHub Actions
2. **Tester le build local** avec les commandes ci-dessus
3. **Vérifier les workloads** .NET WebAssembly installées
4. **Console du navigateur** pour les erreurs JavaScript/WASM

## ⚡ Optimisations

- Les fichiers sont automatiquement compressés (gzip)
- Le workflow utilise la mise en cache .NET
- Build optimisé pour la production (Release)
