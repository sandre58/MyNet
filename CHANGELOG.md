# Changelog for v13.0.0

All notable changes to this project will be documented in this file.  
This file is generated automatically based on commit history and tags.




## [v13.0.0] - 2025-10-16


### ♻️ Code Refactoring

- add app-projects input to CI build job configuration *(commit by **Stéphane ANDRE (E104915)** in [9f6d3895](https://github.com/sandre58/MyNet/commit/9f6d38959adffccda28dcc94bbf49060a9eac068))*

- simplify tag.yml workflow inputs by changing type to boolean and removing unnecessary options *(commit by **Stéphane ANDRE (E104915)** in [f5bc21aa](https://github.com/sandre58/MyNet/commit/f5bc21aa683e6e7107b02af84720ebb02ed6f8a5))*

- remove unused permissions and environment variables from CI workflows *(commit by **Stéphane ANDRE (E104915)** in [9b527638](https://github.com/sandre58/MyNet/commit/9b5276387ce371660ab9bd7a5ff3d42d712c66cf))*

- simplify tooling and workflows *(commit by **Stéphane ANDRE (E104915)** in [d8bbb51f](https://github.com/sandre58/MyNet/commit/d8bbb51f91a5bc0af4c46d68e7d76174b20929f7))*

- streamline CI workflows and enhance versioning logic in compute-version script *(commit by **Stéphane ANDRE (E104915)** in [0fd6234d](https://github.com/sandre58/MyNet/commit/0fd6234df6d695dd0610edb1c94ef7e8278a1fb5))*

- update project file paths and remove unused settings in solution *(commit by **Stéphane ANDRE (E104915)** in [8320c834](https://github.com/sandre58/MyNet/commit/8320c834b7d9c7f881487640893ff64fbcb83792))*

- improve version computation and output handling in CI workflows *(commit by **Stéphane ANDRE (E104915)** in [d5b06a04](https://github.com/sandre58/MyNet/commit/d5b06a045dc5ed88ce312cf6850a202893608f0c))*

- enhance version handling in CI workflow for improved project versioning *(commit by **Stéphane ANDRE (E104915)** in [ad84feca](https://github.com/sandre58/MyNet/commit/ad84feca9bd91b551cd13cefe9017aa8d9686139))*

- remove Avalonia and WPF *(commit by **Stéphane ANDRE (E104915)** in [2b1ab768](https://github.com/sandre58/MyNet/commit/2b1ab768c567897ebbd8e1b2a5d920f4d8a59e35))*

- add versioning and new CI ([#1](https://github.com/sandre58/MyNet/issues/1)) *(commit by **Stéphane ANDRE** in [81852b2d](https://github.com/sandre58/MyNet/commit/81852b2d63ece675b59e57a9497bec3fd444f95b))*


### 🐛 Bug Fixes

- minify JSON output for PROJECTS_VERSIONS in CI workflow *(commit by **Stéphane ANDRE (E104915)** in [cde0af00](https://github.com/sandre58/MyNet/commit/cde0af001e77bbd6f99bceba9b2f5ef4186cdb6b))*

- correct JSON handling in projects_versions assignment in CI workflow *(commit by **Stéphane ANDRE (E104915)** in [8ece9758](https://github.com/sandre58/MyNet/commit/8ece9758286814a5c271f8b78b9b6b143515d7e8))*

- update nbgv set-version command to use project directory variable *(commit by **Stéphane ANDRE (E104915)** in [547ef95d](https://github.com/sandre58/MyNet/commit/547ef95df3da5726e1248bb693d68e9533791d44))*

- correct string interpolation in git describe command for last tag retrieval *(commit by **Stéphane ANDRE (E104915)** in [91bba1fc](https://github.com/sandre58/MyNet/commit/91bba1fcfc287e5d0088a1a400c247754f24be2c))*

- add a line break before the full changelog link in RELEASE template *(commit by **Stéphane ANDRE (E104915)** in [c53c000d](https://github.com/sandre58/MyNet/commit/c53c000d44e906f78860d76ef0345105560c4f0d))*

- correct author reference formatting in changelog templates *(commit by **Stéphane ANDRE (E104915)** in [1371d556](https://github.com/sandre58/MyNet/commit/1371d5562698d9e129cf99b4652e7de68e203d2d))*

- update changelog templates to enhance author reference formatting and remove documentation type *(commit by **Stéphane ANDRE (E104915)** in [36b26823](https://github.com/sandre58/MyNet/commit/36b26823986e83514e0f0db4735b555ba86c62aa))*

- simplify commit reference formatting in changelog templates *(commit by **Stéphane ANDRE (E104915)** in [c0281a0b](https://github.com/sandre58/MyNet/commit/c0281a0b97f8cc52f1c8eb4415e059c3f3b79dd8))*

- improve author reference formatting in changelog templates *(commit by **Stéphane ANDRE (E104915)** in [01dd79f4](https://github.com/sandre58/MyNet/commit/01dd79f40af7c2540cf38ad862335cfa33402211))*

- standardize author reference formatting in changelog templates *(commit by **Stéphane ANDRE (E104915)** in [51ecd786](https://github.com/sandre58/MyNet/commit/51ecd78674a2667561ff7fc313f319a4887bcdeb))*

- update changelog templates for improved clarity and consistency in commit references *(commit by **Stéphane ANDRE (E104915)** in [4bfb48f4](https://github.com/sandre58/MyNet/commit/4bfb48f42f8403b2850832cf74672b7f016789db))*

- simplify changelog template structure and improve readability *(commit by **Stéphane ANDRE (E104915)** in [6e4a7c1b](https://github.com/sandre58/MyNet/commit/6e4a7c1b44e5f51df6830ca1a4dd52468d1edf51))*

- exclude markdown files from change detection in tagging workflow *(commit by **Stéphane ANDRE (E104915)** in [450ffa49](https://github.com/sandre58/MyNet/commit/450ffa49f6f21838dd983448867c79ea20ad2f68))*

- correct template logic for base path determination in RELEASE.tpl.md *(commit by **Stéphane ANDRE (E104915)** in [d8f206d1](https://github.com/sandre58/MyNet/commit/d8f206d1ba01a39c4abf5fe5624a9006bef232e5))*

- enhance changelog templates for clarity and structure *(commit by **Stéphane ANDRE (E104915)** in [dd7a7bbe](https://github.com/sandre58/MyNet/commit/dd7a7bbe56554a10baaa5441e17124deb8776f75))*

- update changelog templates for consistency and improve release workflow *(commit by **Stéphane ANDRE (E104915)** in [8092ac3d](https://github.com/sandre58/MyNet/commit/8092ac3d9abb9cf684965104a49cebf6c82bb1ac))*

- remove breaking changes section from changelog template *(commit by **Stéphane ANDRE (E104915)** in [0fb20be8](https://github.com/sandre58/MyNet/commit/0fb20be84c723dbece6a54e7728b84dc6dc44ab8))*

- simplify changelog and release templates by removing redundant version information *(commit by **Stéphane ANDRE (E104915)** in [884c315f](https://github.com/sandre58/MyNet/commit/884c315fc9d05c9af96da349609f682f3338e969))*

- update changelog template to consistently reference the current version tag *(commit by **Stéphane ANDRE (E104915)** in [40b68e0f](https://github.com/sandre58/MyNet/commit/40b68e0f44dccd55ff7d225bc4960b75d7115888))*

- remove build information from changelog template *(commit by **Stéphane ANDRE (E104915)** in [b7b10e1c](https://github.com/sandre58/MyNet/commit/b7b10e1c4f5ece584039528f5f2ff8c355bde30b))*

- update changelog link formatting for consistency *(commit by **Stéphane ANDRE (E104915)** in [41e5df44](https://github.com/sandre58/MyNet/commit/41e5df44ac530a1f7c3480710778c65888067dee))*

- update changelog and release templates for consistent version formatting *(commit by **Stéphane ANDRE (E104915)** in [a514ada0](https://github.com/sandre58/MyNet/commit/a514ada074c604953c1af419089202f8b7817e6d))*

- update changelog templates to consistently format version tags *(commit by **Stéphane ANDRE (E104915)** in [869620d6](https://github.com/sandre58/MyNet/commit/869620d63884516deaf59ed8bd31cf7d8b05d234))*

- update release workflow to show tag in summary after parsing *(commit by **Stéphane ANDRE (E104915)** in [6a210118](https://github.com/sandre58/MyNet/commit/6a2101180d54203e60eb712a726d38fe096920ba))*

- update changelog templates to consistently use tag information *(commit by **Stéphane ANDRE (E104915)** in [ef194a52](https://github.com/sandre58/MyNet/commit/ef194a52fd386db440fde63dd28c6451fbb95d1b))*

- update changelog templates to use tag information for version and release date *(commit by **Stéphane ANDRE (E104915)** in [deeb3955](https://github.com/sandre58/MyNet/commit/deeb39559a3e2fdc58d584c566dec1dc55be677e))*

- update changelog templates to use version info consistently *(commit by **Stéphane ANDRE (E104915)** in [8da91589](https://github.com/sandre58/MyNet/commit/8da9158907dbe6cff35ba8468335baafa6cd228a))*

- include tag output in workflow for changelog generation and updates *(commit by **Stéphane ANDRE (E104915)** in [d9775775](https://github.com/sandre58/MyNet/commit/d9775775e78e5df6eabb99344645fb7c94fd838b))*

- standardize fetch steps in release workflow and add tag commit display *(commit by **Stéphane ANDRE (E104915)** in [0e765897](https://github.com/sandre58/MyNet/commit/0e7658979797a9d5b1e39d3b3b48decdaf1c3659))*

- add tag listing step in release workflow for better visibility of available tags *(commit by **Stéphane ANDRE (E104915)** in [2eebf803](https://github.com/sandre58/MyNet/commit/2eebf80335f4abec72d7d988fe2f381138aeb802))*

- enhance release workflow by adding tag summary display and ensuring all tags are fetched *(commit by **Stéphane ANDRE (E104915)** in [dafb4d7a](https://github.com/sandre58/MyNet/commit/dafb4d7a5a050b0221d272e40df34fea2982212f))*

- add ref parameter to checkout steps in release workflow for consistent context *(commit by **Stéphane ANDRE (E104915)** in [22308fc0](https://github.com/sandre58/MyNet/commit/22308fc0aa4d431f9ace7e4e7d014b5bbf385c9a))*

- update paths for git-chglog configuration files in release workflow *(commit by **Stéphane ANDRE (E104915)** in [38e7ee68](https://github.com/sandre58/MyNet/commit/38e7ee683c60058a599cb25be5e439b31cb35bf7))*

- remove redundant restore step and standardize variable naming in WPF and core project build steps *(commit by **Stéphane ANDRE (E104915)** in [1e8b34b9](https://github.com/sandre58/MyNet/commit/1e8b34b953ebd98111fa5cf52d7c5f2d59a3490c))*

- replace dotnet pack with dotnet build for WPF and core projects in release workflow *(commit by **Stéphane ANDRE (E104915)** in [4f361c72](https://github.com/sandre58/MyNet/commit/4f361c72417a0dad970255e7e2b39f3cd403317a))*

- enhance tag workflow to trigger release on tag creation and push tags individually *(commit by **Stéphane ANDRE (E104915)** in [0fb49e0a](https://github.com/sandre58/MyNet/commit/0fb49e0a090c576e5adc24341d1978d3aed473fd))*

- update publicReleaseRefSpec and change firstUnstableTag to beta *(commit by **Stéphane ANDRE (E104915)** in [19940b8c](https://github.com/sandre58/MyNet/commit/19940b8ce602fd9b6277f7fabc1fbc8f34a8f164))*

- update default dry_run value and improve project detection logic in tag workflow *(commit by **Stéphane ANDRE (E104915)** in [0d734b39](https://github.com/sandre58/MyNet/commit/0d734b39bc05da148fbecbc848fa59e2fa08f056))*

- Remove unused Android and iOS demo projects from solution *(commit by **Stéphane ANDRE (E104915)** in [c582c1b7](https://github.com/sandre58/MyNet/commit/c582c1b76d233bb59b7a7954bc1d3ac40cf8dbad))*

- Update paths in project files to use MSBuildThisFileDirectory for consistency *(commit by **Stéphane ANDRE (E104915)** in [fcbd6e07](https://github.com/sandre58/MyNet/commit/fcbd6e079b99a2b0874de8bd77cc46b6831f9cf9))*

- Add condition to import statement in Directory.Build.props for Avalonia and WPF projects *(commit by **Stéphane ANDRE (E104915)** in [5add6ea8](https://github.com/sandre58/MyNet/commit/5add6ea84e487a9c329bd969cde46932747d82d9))*


### 👷 CI/CD

- Simplify secrets and update input syntax *(commit by **Stéphane ANDRE (E104915)** in [7e18160b](https://github.com/sandre58/MyNet/commit/7e18160b8fff26fc4b384de9a188328e5068fbc3))*

- add workflow_dispatch trigger and rename job to build *(commit by **Stéphane ANDRE (E104915)** in [2f869c3e](https://github.com/sandre58/MyNet/commit/2f869c3ede4e86d5a56b15e7eb5d6a6b5208c814))*

- update tool installation steps in CI workflows *(commit by **Stéphane ANDRE (E104915)** in [93704084](https://github.com/sandre58/MyNet/commit/93704084773d2d2dfaa921a39bfef398ccc67819))*

- display project version in build output *(commit by **Stéphane ANDRE (E104915)** in [23fed9e3](https://github.com/sandre58/MyNet/commit/23fed9e3fe691e85ec60906ef2a833c784905c6d))*

- refactor version injection and build process in CI workflow *(commit by **Stéphane ANDRE (E104915)** in [5f31b461](https://github.com/sandre58/MyNet/commit/5f31b461083e3f89e56cfe61128cd442248e895f))*

- update default version fallback in compute-version script to 0.0.0 *(commit by **Stéphane ANDRE (E104915)** in [d1ffd6ec](https://github.com/sandre58/MyNet/commit/d1ffd6ec1dbfa9f37adb1d8a58732367c441e3fe))*

- refactor compute-version script for improved readability and consistency *(commit by **Stéphane ANDRE (E104915)** in [7dc38519](https://github.com/sandre58/MyNet/commit/7dc3851989a03938543e050fbe50c5a7e239af78))*

- update package generation settings and fix version parsing logic in compute-version script *(commit by **Stéphane ANDRE (E104915)** in [646c7a5b](https://github.com/sandre58/MyNet/commit/646c7a5b09a8078218a17de2817bc90ef8e1a821))*

- update logger to use console.error for improved error handling *(commit by **Stéphane ANDRE (E104915)** in [ae8d102d](https://github.com/sandre58/MyNet/commit/ae8d102d41e6a1175ac6a79abcf171ffcfa61302))*

- enhance commit analysis by adding logging functionality *(commit by **Stéphane ANDRE (E104915)** in [b10dd9e4](https://github.com/sandre58/MyNet/commit/b10dd9e492f55fa726436b2926b1ad8b6e8914ce))*

- update dependencies and improve version computation logic *(commit by **Stéphane ANDRE (E104915)** in [90a64883](https://github.com/sandre58/MyNet/commit/90a64883fe8c281e8510330683f51b968098b129))*

- refactor semver import and improve commit mapping logic *(commit by **Stéphane ANDRE (E104915)** in [1cee0f7b](https://github.com/sandre58/MyNet/commit/1cee0f7b9268cb5485c2c96113eeadad0a312cea))*

- update tool installation to simplify npm dependencies and enhance version computation *(commit by **Stéphane ANDRE (E104915)** in [ad788c69](https://github.com/sandre58/MyNet/commit/ad788c69f994896d2c3061aa7aa0068a34a13bc9))*

- update semver import to use default import syntax *(commit by **Stéphane ANDRE (E104915)** in [58a0668e](https://github.com/sandre58/MyNet/commit/58a0668eb210da9eb9d849ce02b31678bfb083b0))*

- refactor version computation to iterate over project directories *(commit by **Stéphane ANDRE (E104915)** in [ba225828](https://github.com/sandre58/MyNet/commit/ba225828188d4205200d506b8dfee2238efa08d2))*

- update tool installation step to include PATH for .NET tools *(commit by **Stéphane ANDRE (E104915)** in [8f58b5f4](https://github.com/sandre58/MyNet/commit/8f58b5f411f89415179a180c2f0b2acbc1b6c2c5))*

- update CI workflow to include version computation script and enhance build steps *(commit by **Stéphane ANDRE (E104915)** in [cad0ca52](https://github.com/sandre58/MyNet/commit/cad0ca52af82095da224cca1721a1b5aee5ee5ab))*

- update permissions to allow code checkout and push to GitHub Pages *(commit by **Stéphane ANDRE (E104915)** in [4890c1e1](https://github.com/sandre58/MyNet/commit/4890c1e190681fbe84db7e44663808d3ccd22de7))*

- remove GitHub Pages deployment workflow *(commit by **Stéphane ANDRE (E104915)** in [28819590](https://github.com/sandre58/MyNet/commit/2881959057288b44bc5a033237096c01a0be7543))*

- add NuGet package packing and upload steps to CI workflow *(commit by **Stéphane ANDRE (E104915)** in [2160c6bb](https://github.com/sandre58/MyNet/commit/2160c6bb029147bd19f415314a5b649c6f697dd8))*

- fix formatting in test command for better readability *(commit by **Stéphane ANDRE (E104915)** in [3f78d894](https://github.com/sandre58/MyNet/commit/3f78d8944a1797e98fcc3b7617a0fca489557ebd))*

- fix build and test commands in CI workflow *(commit by **Stéphane ANDRE (E104915)** in [4bebdc0b](https://github.com/sandre58/MyNet/commit/4bebdc0b4652e424a2966f2148b60b6fd17c80b1))*

- enhance testing workflow with coverage and report generation *(commit by **Stéphane ANDRE (E104915)** in [fd22a472](https://github.com/sandre58/MyNet/commit/fd22a472d7a2ff04f6abf4b8d3bac8809f0799ba))*

- Add verbosity flag to .NET workload restore step for detailed output *(commit by **Stéphane ANDRE (E104915)** in [2c0df057](https://github.com/sandre58/MyNet/commit/2c0df05747d4d9b426641af4b8a2ed2e53bd1618))*

- Add installation and restoration steps for wasm-tools and android workloads *(commit by **Stéphane ANDRE (E104915)** in [601ceb69](https://github.com/sandre58/MyNet/commit/601ceb6964be9b386d725c1c7b2beb86c497ce82))*

- Enable Windows targeting during project dependency restoration *(commit by **Stéphane ANDRE (E104915)** in [1cd6696e](https://github.com/sandre58/MyNet/commit/1cd6696ee3ef554487e553b4cb7ea3c33e4a99a5))*

- Enable Windows targeting in build steps for CI and release workflows; add Avalonia projects to release configuration *(commit by **Stéphane ANDRE (E104915)** in [b69b6028](https://github.com/sandre58/MyNet/commit/b69b60286bde24ff3c5f7ad2894bf0c76afbd117))*

- Rearrange steps for coverage upload and deployment in CI workflow *(commit by **Stéphane ANDRE (E104915)** in [a5332b85](https://github.com/sandre58/MyNet/commit/a5332b85c5ebbb12946bcaa24c16fdae86fd2850))*

- Update coverage report upload step to use upload-pages-artifact action *(commit by **Stéphane ANDRE (E104915)** in [494bb6d7](https://github.com/sandre58/MyNet/commit/494bb6d7d39335a6fe8b0298f55f34bf13ef5209))*

- Refactor CI workflow and update coverage report paths *(commit by **Stéphane ANDRE (E104915)** in [dd1128ef](https://github.com/sandre58/MyNet/commit/dd1128ef19217c2a70342788e01b324256414626))*

- Add token for Codecov upload step *(commit by **Stéphane ANDRE (E104915)** in [1283d50d](https://github.com/sandre58/MyNet/commit/1283d50dddf0835cf183ac5ea441c46eb377b804))*

- Add permissions for GitHub Pages deployment and OIDC authentication *(commit by **Stéphane ANDRE (E104915)** in [2b2f3361](https://github.com/sandre58/MyNet/commit/2b2f3361239298ed712bea9ca3b982303539db06))*

- Add deployment to GitHub Pages and upload coverage to Codecov *(commit by **Stéphane ANDRE (E104915)** in [b5f099f7](https://github.com/sandre58/MyNet/commit/b5f099f7e519d8185561d78a51b39f44282f8576))*

- Update release process and dependencies *(commit by **Stéphane ANDRE (E104915)** in [05c8d3d0](https://github.com/sandre58/MyNet/commit/05c8d3d065a6826c5aed80a37f89c5bd8d198653))*


### 📦 Build System

- Update analyzer and logging package versions *(commit by **Stéphane ANDRE (E104915)** in [399aea34](https://github.com/sandre58/MyNet/commit/399aea3464ae027616cbeb16d211a832b8791123))*


### 🔧 Chores

- Update project properties for improved build configuration *(commit by **Stéphane ANDRE (E104915)** in [199a9546](https://github.com/sandre58/MyNet/commit/199a95465b7d4d7b9e32933394a5bae0fe79bf74))*











---
