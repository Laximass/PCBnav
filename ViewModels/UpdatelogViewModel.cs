using System;
using System.ComponentModel;
using System.IO;

namespace PCB_Nav3.ViewModels
{
    /// <summary>
    /// ViewModel for displaying the update log text.
    /// </summary>
    public class UpdatelogViewModel : INotifyPropertyChanged
    {
        private string _updateText;

        /// <summary>
        /// Text content of the update log.
        /// </summary>
        public string UpdateText
        {
            get => _updateText;
            set
            {
                _updateText = value;
                OnPropertyChanged(nameof(UpdateText));
            }
        }

        /// <summary>
        /// Constructor loads the update log from a fixed file path.
        /// </summary>
        public UpdatelogViewModel()
        {
            LoadUpdateLog();
        }

        /// <summary>
        /// Loads the update log text from the predefined file path.
        /// </summary>
        private void LoadUpdateLog()
        {
            try
            {
                if (File.Exists(@"\\\\renishaw.com\\\\global\\\\GB\\\\PLC\\\\GE\\\\Data\\\\Electronics\\\\Documents\\\\EQs_from_PCB_supplier\\\\EQ System Progs\\\\PCB Naigator\\\\Application Files\\\\UpdateTxt.txt"))
                {
                    UpdateText = File.ReadAllText(@"\\\\renishaw.com\\\\global\\\\GB\\\\PLC\\\\GE\\\\Data\\\\Electronics\\\\Documents\\\\EQs_from_PCB_supplier\\\\EQ System Progs\\\\PCB Naigator\\\\Application Files\\\\UpdateTxt.txt");
                }
                else
                {
                    UpdateText = "Update log file not found.";
                }
            }
            catch (Exception ex)
            {
                UpdateText = $"Error loading update log: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
