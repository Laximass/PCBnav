using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PCB_Nav3.Properties;

namespace PCB_Nav3.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Visibility _adminButtonVisibility;
        private Brush _backgroundColor;
        private Brush _highlightColor;
        private Brush _textColor;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            ShowAdminCommand = new RelayCommand(ExecuteShowAdmin);
            CloseAdminCommand = new RelayCommand(ExecuteCloseAdmin);
            LoadUserSettings();
            ApplyColors();
            AdminButtonVisibility = Visibility.Collapsed;
        }

        public ICommand ShowAdminCommand { get; }
        public ICommand CloseAdminCommand { get; }

        public Visibility AdminButtonVisibility
        {
            get => _adminButtonVisibility;
            set
            {
                _adminButtonVisibility = value;
                OnPropertyChanged(nameof(AdminButtonVisibility));
            }
        }

        public Brush BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }

        public Brush HighlightColor
        {
            get => _highlightColor;
            set
            {
                _highlightColor = value;
                OnPropertyChanged(nameof(HighlightColor));
            }
        }

        public Brush TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                OnPropertyChanged(nameof(TextColor));
            }
        }

        private void LoadUserSettings()
        {
            // Load user settings from Settings.Default
            // This method replaces UserSettings() from code-behind
        }

        private void ApplyColors()
        {
            BackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.BGC1));
            HighlightColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.HLC1));
            TextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.TXTC2));
        }

        private void ExecuteShowAdmin(object obj)
        {
            AdminButtonVisibility = Visibility.Visible;
        }

        private void ExecuteCloseAdmin(object obj)
        {
            AdminButtonVisibility = Visibility.Collapsed;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}