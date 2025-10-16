using PCB_Nav3.Properties;
using System.Windows;
using System.Windows.Media;
using File = System.IO.File;

namespace PCB_Nav3
{
    /// <summary>
    /// Interaction logic for UpdatelogWin.xaml
    /// </summary>
    public partial class UpdatelogWin : Window
    {
        public UpdatelogWin()
        {
            InitializeComponent();
            SetColours();

            Updaettb.Text = File.ReadAllText("\\\\renishaw.com\\global\\GB\\PLC\\GE\\Data\\Electronics\\Documents\\EQs_from_PCB_supplier\\EQ System Progs\\PCB Naigator\\Application Files\\UpdateTxt.txt");

        }

        private void SetColours()
        {
            // takes the colourse from the applications settings and applys them.
            string BackgroundColour = Settings.Default.BGC1;
            string TextColour = Settings.Default.TXTC2;
            updategrid.Resources["BGColour"] = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(BackgroundColour));
            Updaettb.Resources["TextColour"] = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(TextColour));
            Updateloglbl.Resources["TextColour"] = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(TextColour));

        }

    }
}
