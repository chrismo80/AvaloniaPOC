# Avalonia POC

This solution is a proof of concept to evaluate Avalonia as UI framework for cross-platform desktop applications.

The solution focuses on the following aspects:
- how to add and use dependency injection
- how to add and share multi language resource files across assemblies
- how to use data templates to visualize multiple instances of the same object type
- how to use and share assets across assemblies
- how to create custom windows for on screen keyboards for touch screens

The solution consists of the following modules:
- **CompanyName.Core**: service interfaces, service base implementations, base classes, core extensions (e.g. logging, messaging)
- **CompanyName.UI**: Avalonia base components and implementations that are reusable for all project applications (e.g. splash screen, on-screen keyboard, dialogs, etc.)
- **ProjectExampleHMI**: Example desktop application (UI playground)
- **UnitTests**: simple show-case for service unit tests