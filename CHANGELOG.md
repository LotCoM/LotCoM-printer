
# `v0.0.1` 
### Initial UI Design and Project Rename
- Main Page UI Design in XAML.
- MVVM implementations.
- Production Datasource Model implementations.
- Rename (LCMS -> LotCoM).

## `v0.0.11`
#### Final UI Design and Functionality
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
#### Print Spooling
- Implement connection between Label generation and printing.
- Implement Print Spooling classes.

# `v0.2.0`
### External Part Control
- Adopt [SemVer](https://semver.org/) (Semantic Versioning 2.0.0) versioning conventions.
- Implement integration with Database for external Process Part Number control.

## `v0.2.1`
#### Build Setup
- Build environment setup (manifests) for Windows MSIX Packaging.

# `v0.3.0`
### Operator ID Input
- Operator identification (initial) input in UI.
- Refactor `ProcessRequirements` datasource to leverage more code reuse and enforce universal basket data fields.
- Resolve issue with error messaging on UI validation failure ([#16](https://github.com/LotCoM/LotCoM-printer/issues/16)).
- Resolve issue with improper JBK # formatting ([#17](https://github.com/LotCoM/LotCoM-printer/issues/17)).
- Resolve a fatal formatting error in the Deburr JBK # field ([#18](https://github.com/LotCoM/LotCoM-printer/issues/18)).
- Resolve omission of Process Title in Label text ([#19](https://github.com/LotCoM/LotCoM-printer/issues/19)).
- Resolve issue with required data fields not hiding/showing on Process ([#20](https://github.com/LotCoM/LotCoM-printer/issues/20)).

# `v0.4.0`
### JBK Queue System
- Global JBK # queueing and assignment system integration.
- General performance and clarity refactoring.
- Model Number UI control changed from a configured Picker to an implied Entry.

## `v0.4.1`
#### Label Serialization Build-out
- Implement `LotQueue`, a copy of `JBKQueue` to complete the Label serialization system.
- Implement actual consumption of serial numbers.

## `v0.4.2 (dead branch)`
#### Improve Serialization Integrity
- Adress a gap in the serialization system, outlined in [#25](https://github.com/LotCoM/LotCoM-printer/issues/25).
- Approach 1; **not selected for implementation**.

## `v0.4.21`
#### Resolve Serial Number System timing gap
Approach 2:
- Consume the serial number at print execution:
  - Waits until the print job is started to read the queued serial number.
  - Places the serial number in the UI to be validated.
    - Consumes it if the job is successful; does not consume if not.
- Pros:
  - Eliminates* the problem and leaves no gaps in the FIFO ordering system.
    - There is a very, very small gap between queue read time and consumption time.
      - This could still duplicate serial numbers.
      - Future efficiency improvements should be made to limit the time between.
- Cons:
  - Operators cannot see the serial number while printing the Label.

## `0.4.22`
#### Build Environment Work
- Drop `v` prefixing convention for version numbers (not compliant with SemVer).
- Update manifests to include new certificate information.

## `0.4.23`
#### CI/CD Pipeline - Phase 1 (CI)

## `0.4.24`
#### CI/CD Pipeline - Phase 2 (CD)

# `0.5`
### Partial Basket and Consolidation System

## `0.5.0`
#### Partial Label Implementation
- Implement `PartialLabel.cs`, an extension of the `Label.cs` class with special formatting.

## `0.5.1`
#### Queue Consumption and Reserving (Partial Label)
- Refactor Serial # assignment and queueing to eliminate the gap outlined in [#53](https://github.com/LotCoM/LotCoM-printer/issues/53).
- Resolve [#58](https://github.com/LotCoM/LotCoM-printer/issues/58).
- Implement standalone `Serialization` namespace, classes, and system.
  - `CachedSerialNumber.cs`: represents a simple `Part Number`: `Serial Number` pair with basic methods.
  - `SerialCache.cs`: controls interaction with the Serial Caching file system and contents.
  - `Serializer.cs`: assigns Serial Numbers to new Labels (both `Full` and `Partial`).
### `0.5.1.1`
- Refactor CI/CD pipeline to: 
  - Activate on pushes to `stable` instead of `main`;
  - Ensure that the `.NET runtime` is packaged with the application.
- Add application version to window title bar.