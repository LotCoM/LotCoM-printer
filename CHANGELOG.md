
# `v0.0.1` 
### Initial UI Design and Project Rename
- Main Page UI Design in XAML.
- MVVM implementations.
- Production Datasource Model implementations.
- Rename (LCMS -> LotCoM).

# `v0.0.11`
### Final UI Design and Functionality
- Parts List population for any selected Process.
- Model # implication when a Part is selected.
- UI Entry controls reset their values and states when the Process is changed.

# `v0.0.2`
### Entered Data Validation
- Implement `PrintValidator` class that captures and ensures that all required entries are made for any Process.

# `v0.1.0`
### Label Generation
- Implement connection between `InterfaceCaptureValidator` and digital Label generation.
- Implement digital Label generation classes.

## `v0.1.1`
### Print Spooling
- Implement connection between Label generation and printing.
- Implement Print Spooling classes.

## `v0.2.0`
- Adopt [SemVer](https://semver.org/) (Semantic Versioning 2.0.0) versioning conventions.
- Implement integration with Database for external Process Part Number control.

## `v0.2.1`
- Build environment setup (manifests) for Windows MSIX Packaging.

## `v0.3.0`
- Operator identification (initial) input in UI.
- Refactor `ProcessRequirements` datasource to leverage more code reuse and enforce universal basket data fields.
- Resolve issue with error messaging on UI validation failure ([#16](https://github.com/LotCoM/LotCoM-printer/issues/16)).
- Resolve issue with improper JBK # formatting ([#17](https://github.com/LotCoM/LotCoM-printer/issues/17)).
- Resolve a fatal formatting error in the Deburr JBK # field ([#18](https://github.com/LotCoM/LotCoM-printer/issues/18)).
- Resolve omission of Process Title in Label text ([#19](https://github.com/LotCoM/LotCoM-printer/issues/19)).
- Resolve issue with required data fields not hiding/showing on Process ([#20](https://github.com/LotCoM/LotCoM-printer/issues/20)).