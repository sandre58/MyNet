# Changelog

All notable changes to this project will be documented in this file.  
This file is generated automatically based on commit history and tags.




## [v19.0.0] - 2026-05-29


### ♻️ Code Refactoring

- Update README and project files for improved package documentation and structure *(commit by **Stéphane ANDRE (E104915)** in [53247973](https://github.com/sandre58/MyNet/commit/532479733407c7042508ed56c730ad463342fbfa))*

- Update project descriptions and tags for clarity and improved searchability *(commit by **Stéphane ANDRE (E104915)** in [6c74d6e2](https://github.com/sandre58/MyNet/commit/6c74d6e2c6e3c279afe548bfdb2ab2a0cc7ab3f2))*

- Update method names to include 'Async' suffix and simplify lambda expressions *(commit by **Stéphane ANDRE (E104915)** in [43de5085](https://github.com/sandre58/MyNet/commit/43de508588494f620ce22d1880a5cf9091b393f2))*

- Simplify command creation in view models and update legacy keys for display modes *(commit by **Stéphane ANDRE (E104915)** in [5b0370ca](https://github.com/sandre58/MyNet/commit/5b0370ca2cf20d931d7409e97450435ce9a110d3))*

- Introduce display mode view models and column options for enhanced display management *(commit by **Stéphane ANDRE (E104915)** in [5f54f8c1](https://github.com/sandre58/MyNet/commit/5f54f8c123a1f672f2f9f9fd6f1b28eabaed9b04))*

- Introduce IEditionStateViewModel for improved edition state tracking and add CollectionEditionViewModel for editable collections *(commit by **Stéphane ANDRE (E104915)** in [7ffd83d0](https://github.com/sandre58/MyNet/commit/7ffd83d06054de88ab95ef6fb356173b0caa973a))*

- Add culture service support to view models for improved localization management *(commit by **Stéphane ANDRE (E104915)** in [15534ecd](https://github.com/sandre58/MyNet/commit/15534ecdccf1d3e9e5306677d24a05a31038caf2))*

- Enhance dialog service with improved lifecycle management and cancellation support *(commit by **Stéphane ANDRE (E104915)** in [80010784](https://github.com/sandre58/MyNet/commit/800107846acaf861b4fa078cdd4015308979fb3e))*

- Simplify property initializations and improve code readability *(commit by **Stéphane ANDRE (E104915)** in [c30c03d4](https://github.com/sandre58/MyNet/commit/c30c03d425103145a9251489e33d3c7329065c26))*

- Introduce interfaces and event args for activable and closable view models *(commit by **Stéphane ANDRE (E104915)** in [c0dce162](https://github.com/sandre58/MyNet/commit/c0dce162636ee2e401dd313bf073a3fef0f153a2))*

- Update theming services and improve theme management integration *(commit by **Stéphane ANDRE (E104915)** in [6b5176f1](https://github.com/sandre58/MyNet/commit/6b5176f1b4042af5f212f6e471c5731cc1f84675))*

- Improve navigation interfaces and enhance documentation clarity *(commit by **Stéphane ANDRE (E104915)** in [c430afba](https://github.com/sandre58/MyNet/commit/c430afbab27aeb5258b220e931de02ef82e933e7))*

- Enhance view resolution logic and improve naming conventions for view models *(commit by **Stéphane ANDRE (E104915)** in [ce5437fd](https://github.com/sandre58/MyNet/commit/ce5437fde64ba82ab966d1ba51ece2ed78022f26))*

- Remove unused busy service references and improve ViewModel constructors *(commit by **Stéphane ANDRE (E104915)** in [d88724f4](https://github.com/sandre58/MyNet/commit/d88724f4d224aaf8e5036232ffed612e18bf6d32))*

- Enhance notification handling and validation logic in toast and file extension attributes *(commit by **Stéphane ANDRE (E104915)** in [eed0c74e](https://github.com/sandre58/MyNet/commit/eed0c74e1e0f82a1c6a59987b0940aa437d3129c))*

- Update property setters to use SetProperty for better change tracking *(commit by **Stéphane ANDRE (E104915)** in [b9522ce9](https://github.com/sandre58/MyNet/commit/b9522ce97ed0553b9f5e07410ca25bb844b8be13))*

- Remove unused using directives and improve code clarity *(commit by **Stéphane ANDRE (E104915)** in [42b9f0f0](https://github.com/sandre58/MyNet/commit/42b9f0f010d4f69414b0a1ebb9fd5ad8d745b823))*

- Improve test class structure and dispose pattern in Messenger tests *(commit by **Stéphane ANDRE (E104915)** in [13674a2a](https://github.com/sandre58/MyNet/commit/13674a2a0aa9495c478ce3d47600307e6a371208))*

- Rename and reorganize namespaces from MyNet.Utilities to MyNet.Primitives for improved clarity *(commit by **Stéphane ANDRE (E104915)** in [9a60d7fa](https://github.com/sandre58/MyNet/commit/9a60d7faeef6c78d13b2bc2144deeb66d44dd919))*


### ✅ Tests

- Refactor RandomEnumTests to use DefaultRandomGenerator and add AssemblyInfo for test configuration *(commit by **Stéphane ANDRE (E104915)** in [17480642](https://github.com/sandre58/MyNet/commit/17480642c26e18b62bb5fc7efa55f27cd2af6f76))*

- Add unit tests for CancelledFileDialogService, CompositeToastFilter, ExportFileType, IoHelper, NavigationParametersExtensions, NotificationModel, and NotificationPublisherExtensions *(commit by **Stéphane ANDRE (E104915)** in [9675190d](https://github.com/sandre58/MyNet/commit/9675190d22a5fea583742db3cc800aacaa8a00ae))*

- Add unit tests for Expression and conversion extensions *(commit by **Stéphane ANDRE (E104915)** in [56b5e800](https://github.com/sandre58/MyNet/commit/56b5e800576ca039df3287f1fbe1defca0414c7b))*

- Add unit tests for MailKitService and related classes *(commit by **Stéphane ANDRE (E104915)** in [2bb13f56](https://github.com/sandre58/MyNet/commit/2bb13f5651c63c423de7c1b9fe423b6285ed1350))*

- Refactor unit tests for clarity and consistency *(commit by **Stéphane ANDRE (E104915)** in [cf679b6b](https://github.com/sandre58/MyNet/commit/cf679b6babd1ef4d3d49266c2743bd79f278f868))*


### 🐛 Bug Fixes

- Correct copyright name spelling and update localization strings for French *(commit by **Stéphane ANDRE (E104915)** in [b0f81616](https://github.com/sandre58/MyNet/commit/b0f81616963eaaa3f0e0a45fc81ed57b986b263c))*


### 👷 CI/CD

- Fix test arguments format in CI configuration *(commit by **Stéphane ANDRE (E104915)** in [2ad8847e](https://github.com/sandre58/MyNet/commit/2ad8847e1165c7a3cf288db43dceabfe34520319))*


### 🔧 Chores

- Update assembly thresholds and improve MessengerExtensions registration logic; add unit tests for PeriodExtensions and Registry services *(commit by **Stéphane ANDRE (E104915)** in [679755d2](https://github.com/sandre58/MyNet/commit/679755d2e6ae7583c9aae27d06d2ba09e711473e))*


### 🚀 Features

- Add package icon generator tool and update project files for new dependencies *(commit by **Stéphane ANDRE (E104915)** in [dd7685a0](https://github.com/sandre58/MyNet/commit/dd7685a0dc8dae95ff12402af7135146e5a96dfa))*

- Add coverage management scripts and configuration for threshold verification *(commit by **Stéphane ANDRE (E104915)** in [b618acca](https://github.com/sandre58/MyNet/commit/b618acca85c8503f0c1339008b623246bf0cdf42))*

- Refactor shell view models to improve organization and introduce new interfaces for shell services *(commit by **Stéphane ANDRE (E104915)** in [c35e7006](https://github.com/sandre58/MyNet/commit/c35e700605318b31ff52ed8e81a58593ce8beb8c))*

- Introduce shell drawer management with notifications and file menu support *(commit by **Stéphane ANDRE (E104915)** in [80742010](https://github.com/sandre58/MyNet/commit/80742010fef03dc83c0153ebe0639ff63bc59bcf))*

- Add recent files management with view models and operations *(commit by **Stéphane ANDRE (E104915)** in [f07c8a63](https://github.com/sandre58/MyNet/commit/f07c8a63da6326462c3eb2dd06eda26089957bd8))*

- Implement export functionality with customizable columns and validation *(commit by **Stéphane ANDRE (E104915)** in [84002bfc](https://github.com/sandre58/MyNet/commit/84002bfcc77fbcc4490c7e9cf8a6660d060fc595))*

- Add MailKit integration with SMTP options validation and unit tests *(commit by **Stéphane ANDRE (E104915)** in [f2503950](https://github.com/sandre58/MyNet/commit/f25039500f2839a85ac4f53ab531f9a6c1187fe5))*

- Add HTTP exception handling and parser with unit tests *(commit by **Stéphane ANDRE (E104915)** in [9d12300f](https://github.com/sandre58/MyNet/commit/9d12300f7ac494a8b9ff4fcaa5a9c2d1b533d8e7))*

- Migrate geography-related code from MyNet.Utilities to MyNet.Geography and introduce new Google Maps integration *(commit by **Stéphane ANDRE (E104915)** in [51f5d390](https://github.com/sandre58/MyNet/commit/51f5d390bb8f44f3bc8087df2e4b88988a6cf7bf))*

- Update interfaces and classes to use IObservableValue for display names and improve localization support *(commit by **Stéphane ANDRE (E104915)** in [cfb31dae](https://github.com/sandre58/MyNet/commit/cfb31daef78c1c9cf2c6a6cb29877839082d1cb7))*

- Refactor MergeManyEx and related tests for improved clarity and performance *(commit by **Stéphane ANDRE (E104915)** in [569602d2](https://github.com/sandre58/MyNet/commit/569602d240cae201094ed6e78ec5a2b782fddf5f))*

- Enhance validation localization and improve error messages for clarity *(commit by **Stéphane ANDRE (E104915)** in [e23fba05](https://github.com/sandre58/MyNet/commit/e23fba050605d03ed9472f8e48cca39c2af233b5))*

- Implement SingleTaskDeferrer and associated tests for deferred task execution *(commit by **Stéphane ANDRE (E104915)** in [4c942ce8](https://github.com/sandre58/MyNet/commit/4c942ce8fb014b24177e06fa509869484f75aad7))*

- Update collection statistics and improve test clarity *(commit by **Stéphane ANDRE (E104915)** in [b68247f2](https://github.com/sandre58/MyNet/commit/b68247f23e2e709f0d13477271e4b41af63a2ab5))*

- Enhance ExtendedCollection and related classes for improved performance and clarity *(commit by **Stéphane ANDRE (E104915)** in [1ab5aa36](https://github.com/sandre58/MyNet/commit/1ab5aa36e0abe7bfbb4cad9a2d61b642a49d389c))*

- Remove unused wrapper projection methods and clean up project files *(commit by **Stéphane ANDRE (E104915)** in [97bb2a4a](https://github.com/sandre58/MyNet/commit/97bb2a4a6849d9d86ad37fc1ed731685c2518f28))*

- Refactor property setters to use SetProperty and improve metadata handling *(commit by **Stéphane ANDRE (E104915)** in [cfaef54d](https://github.com/sandre58/MyNet/commit/cfaef54d4fb09f3a3692e1d80e94c9dc5144acc5))*

- Refactor property changing logic to support cancellation and improve observable property handling *(commit by **Stéphane ANDRE (E104915)** in [868e3e82](https://github.com/sandre58/MyNet/commit/868e3e820f63c79f06fc712148bbc5dc0dd8184e))*

- Refactor metadata bootstrap tests for improved clarity and structure *(commit by **Stéphane ANDRE (E104915)** in [462c803a](https://github.com/sandre58/MyNet/commit/462c803a3ef94ba5f8e11d6c75168215e86e6e44))*

- Implement lazy metadata bootstrap and refactor generator to remove ModuleInitializer *(commit by **Stéphane ANDRE (E104915)** in [6bff7a8d](https://github.com/sandre58/MyNet/commit/6bff7a8d6032032b752de813c9cdaf9df6232b1e))*

- Enhance notification suspension with coalescing and initial snapshot capabilities *(commit by **Stéphane ANDRE (E104915)** in [011c106c](https://github.com/sandre58/MyNet/commit/011c106c27153036fa3eaf2018a5e5d0c51f2acb))*

- Update behavior replacement method to use Register instead of Replace *(commit by **Stéphane ANDRE (E104915)** in [106f9e55](https://github.com/sandre58/MyNet/commit/106f9e556a571a8de587066ed149094ed7b86ca5))*

- Update behavior registration methods to use BehaviorRegistry facade *(commit by **Stéphane ANDRE (E104915)** in [8719a874](https://github.com/sandre58/MyNet/commit/8719a8749c5b9fa368333981df5c3889db5c71e5))*

- Refactor property access and modification tracking for improved performance *(commit by **Stéphane ANDRE (E104915)** in [7d59a68e](https://github.com/sandre58/MyNet/commit/7d59a68ea2f650cae58e230a71075929b62f0e62))*

- Implement metadata-driven behavior application and refactor related methods *(commit by **Stéphane ANDRE (E104915)** in [13dca7eb](https://github.com/sandre58/MyNet/commit/13dca7ebd6abc6b7f1a80913fc6c195ea0714605))*

- Add new text transformation interfaces and update related namespaces *(commit by **Stéphane ANDRE (E104915)** in [f32370af](https://github.com/sandre58/MyNet/commit/f32370afe09f1ab69c5e8dc870cd356b72e31334))*










**Full Changelog:** [compare v18.0.0...v19.0.0](https://github.com/sandre58/MyNet/compare/v18.0.0...v19.0.0)


---
