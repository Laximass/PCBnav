using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using PCB_Nav3.Properties;

namespace PCB_Nav3.ViewModels
{
    /// <summary>
    /// ViewModel for the Colours window. Provides bindable properties for selected colors,
    /// a command to save settings, and logic to restart the application.
    /// </summary>
    public class ColoursViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Color> AvailableColors { get; }

        private Color _selectedBackgroundColor;
        private Color _selectedTextColor;
        private Color _selectedHighlightColor;

        public Color SelectedBackgroundColor
        {
            get => _selectedBackgroundColor;
            set
            {
                _selectedBackgroundColor = value;
                OnPropertyChanged(nameof(SelectedBackgroundColor));
            }
        }

        public Color SelectedTextColor
        {
            get => _selectedTextColor;
            set
            {
                _selectedTextColor = value;
                OnPropertyChanged(nameof(SelectedTextColor));
            }
        }

        public Color SelectedHighlightColor
        {
            get => _selectedHighlightColor;
            set
            {
                _selectedHighlightColor = value;
                OnPropertyChanged(nameof(SelectedHighlightColor));
            }
        }

        public ICommand SaveCommand { get; }

        public event Action RequestClose;
        public event PropertyChangedEventHandler PropertyChanged;

        public ColoursViewModel()
        {
            AvailableColors = new ObservableCollection<Color>();
            foreach (var prop in typeof(Colors).GetProperties())
            {
                if (prop.GetValue(null) is Color color)
                    AvailableColors.Add(color);
            }

            SelectedBackgroundColor = (Color)ColorConverter.ConvertFromString(Settings.Default.BGC1);
            SelectedTextColor = (Color)ColorConverter.ConvertFromString(Settings.Default.TXTC2);
            SelectedHighlightColor = (Color)ColorConverter.ConvertFromString(Settings.Default.HLC1);

            SaveCommand = new RelayCommand(SaveSettings);
        }

        private void SaveSettings()
        {
            Settings.Default["BGC1"] = SelectedBackgroundColor.ToString();
            Settings.Default["TXTC1"] = SelectedTextColor.ToString();
            Settings.Default["HLC1"] = SelectedHighlightColor.ToString();
            Settings.Default.Save();

            string userName = Environment.UserName;
            string basePath = $"C:\\Users\\{userName}\\EdaPcb_Nav\\";
            string fileName = Path.Combine(basePath, "EDAPCBUSR_set.rsc");

            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);

                using FileStream fs = File.Create(fileName);
                void WriteLine(string line)
                {
                    byte[] bytes = new UTF8Encoding(true).GetBytes(line + Environment.NewLine);
                    fs.Write(bytes, 0, bytes.Length);
                }

                WriteLine($"BGCOLOUR:{Settings.Default.BGC1}");
                WriteLine($"HLCOLOUR:{Settings.Default.HLC1}");
                WriteLine($"TXTCOLOUR:{Settings.Default.TXTC1}");
                WriteLine($"DEFAULTPROJECT:{Settings.Default.defaultProject}");
                WriteLine($"LASTWIDTH:{Settings.Default.lastwidth}");
                WriteLine($"LASTHEIGHT:{Settings.Default.lastheight}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            if (MessageBox.Show("Please restart the navigator for the changes to take effect.", "Restart", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ProcessStartInfo info = new ProcessStartInfo
                {
                    Arguments = $"/C choice /C Y /N /D Y /T 1 & START \"\"\"{Assembly.GetEntryAssembly().Location}\"\"\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = "PCB_Nav3.exe"
                };
                Process.Start(info);
                Process.GetCurrentProcess().Kill();
            }

            RequestClose?.Invoke();
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
