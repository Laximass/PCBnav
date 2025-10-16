using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PCB_Nav3.ViewModels;

namespace PCB_Nav3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var mainWindow = new MainWindow();
			mainWindow.DataContext = new MainViewModel(); // Inject ViewModel
			mainWindow.Show();
		}
        
        
    }
}
