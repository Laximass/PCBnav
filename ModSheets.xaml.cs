using PCB_Nav3.Properties;
using PCB_Nav3.ViewModels;
using System;
using System.Windows;
using System.Windows.Media;

namespace PCB_Nav3
{
    public partial class ModSheets : Window
    {
        public ModSheets()
        {
            InitializeComponent();

            var viewModel = new ModSheetsViewModel();
            DataContext = viewModel;
        }

    }
}