# Changelog

All notable changes to this project will be documented in this file.  
This file is generated automatically based on commit history and tags.




## [v14.1.0] - 2025-11-21


### 🚀 Features

- Refactor the logging mechanism by replacing the `TraceLevel` enum with a new `PerformanceTraceLevel` enum, adding more granular logging levels such as `Console`, `Information`, `Warning`, and `Error`. Update the `LogManager` class to use this new enum and overload the `MeasureTime` method to dynamically determine logging levels based on elapsed time. *(commit by **Stéphane ANDRE (E104915)** in [6abbc930](https://github.com/sandre58/MyNet/commit/6abbc93096869ed9e1dca0d6e62226a310c4c9bf))*










**Full Changelog:** [compare v14.0.4...v14.1.0](https://github.com/sandre58/MyNet/compare/v14.0.4...v14.1.0)


---
