using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Input;
using PCB_Nav3.Properties;

namespace PCB_Nav3.ViewModels
{
    /// <summary>
    /// ViewModel for ProjectPath window. Handles setting and saving the default project folder path.
    /// </summary>
    public class ProjectPathViewModel : INotifyPropertyChanged
    {
        private string _projectFolderPath;

        /// <summary>
        /// The folder path to be set as the default project location.
        /// </summary>
        public string ProjectFolderPath
        {
            get => _projectFolderPath;
            set
            {
                _projectFolderPath = value;
                OnPropertyChanged(nameof(ProjectFolderPath));
            }
        }

        /// <summary>
        /// Command to save the folder path and write settings to .rsc file.
        /// </summary>
        public ICommand SetPathCommand { get; }

        /// <summary>
        /// Command to cancel and close the window.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Event to request the window to close.
        /// </summary>
        public event Action RequestClose;

        public event PropertyChangedEventHandler PropertyChanged;

        public ProjectPathViewModel()
        {
            SetPathCommand = new RelayCommand(SavePath);
            CancelCommand = new RelayCommand(() => RequestClose?.Invoke());
        }

        private void SavePath()
        {
            if (!string.IsNullOrWhiteSpace(ProjectFolderPath))
            {
                Settings.Default["defaultProject"] = ProjectFolderPath;
                Settings.Default.Save();
                SaveSettingsToFile();
                RequestClose?.Invoke();
            }
        }

        private void SaveSettingsToFile()
        {
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

                WriteLine("BGCOLOUR:" + Settings.Default.BGC1);
                WriteLine("HLCOLOUR:" + Settings.Default.HLC1);
                WriteLine("TXTCOLOUR:" + Settings.Default.TXTC1);
                WriteLine("DEFAULTPROJECT:" + Settings.Default.defaultProject);
                WriteLine("LASTWIDTH:" + Settings.Default.lastwidth);
                WriteLine("LASTHEIGHT:" + Settings.Default.lastheight);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
