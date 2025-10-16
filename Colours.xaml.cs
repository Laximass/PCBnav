using PCB_Nav3.Properties;
using System;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Text;
using System.Xml.Linq;
using PCB_Nav3.ViewModels;

namespace PCB_Nav3
{
    /// <summary>
    /// Interaction logic for Colours.xaml
    /// </summary>
    public partial class Colours : Window
    {
        private string text3;

        public Colours()
        {
            InitializeComponent();
            DataContext = new ColoursViewModel();
        }
    }

}

        // this colects the colours set in the applciaiton settings and sets them.


