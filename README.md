# PCB_Nav3 MVVM Refactor

This project is a refactored version of the original PCB Navigator application using the MVVM architecture.

## Structure

- **Views/**: Contains all XAML and code-behind files.
- **ViewModels/**: Contains ViewModel classes for each window.
- **Models/**: Contains data models used across the application.
- **Properties/**: Contains application settings.

## Setup

1. Open the project in Visual Studio.
2. Ensure all namespaces and references are correct.
3. Build and run the application.

## Notes

- All logic has been moved from code-behind to ViewModels.
- Views are now bound to ViewModels using data binding.
- Models encapsulate data structures for jobs, settings, and ModSheets.

