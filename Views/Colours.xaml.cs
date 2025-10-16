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

namespace PCB_Nav3
{
    /// <summary>
    /// Interaction logic for Colours.xaml
    /// </summary>
    public partial class Colours : Window
    {
        //private string text3;

        //public Colours()
        //{
        //    InitializeComponent();
        //    SetColours();
        //    BGcmbColors.ItemsSource = typeof(Colors).GetProperties();
        //    TXTcmbColors.ItemsSource = typeof(Colors).GetProperties();
        //    HLcmbColors.ItemsSource = typeof(Colors).GetProperties();

        //}

        // this colects the colours set in the applciaiton settings and sets them.
        private void SetColours()
        {
            string BackgroundColour = Settings.Default.BGC1;
            string TextColour = Settings.Default.TXTC2;

            ColGrid.Resources["BGColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundColour));
            BGClbl.Resources["TextColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));
            HLTXTlbl.Resources["TextColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));
            HLCOLlbl.Resources["TextColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));
            
        }


        private void SaveCBtn_Click(object sender, RoutedEventArgs e)
        {
            // this section takes the entered colour values and sets them to the correct setting
            // this also saves them to the user setting files or creates one if it dosent exist.

            if (BGcmbColors.SelectedItem != null)
            {
                string color = BGcmbColors.SelectedItem.ToString();
                color = color.Replace("System.Windows.Media.Color ", "");

                if (color != "")
                {
                    Settings.Default["BGC1"] = color;
                    Settings.Default.Save();
                }
            }

            if (TXTcmbColors.SelectedItem != null)
            {
                string color2 = TXTcmbColors.SelectedItem.ToString();
                color2 = color2.Replace("System.Windows.Media.Color ", "");
                if (color2 != "")
                {
                    Settings.Default["TXTC1"] = color2;
                    Settings.Default.Save();
                }
            }

            if (HLcmbColors.SelectedItem != null)
            {
                string color3 = HLcmbColors.SelectedItem.ToString();
                color3 = color3.Replace("System.Windows.Media.Color ", "");
                if (color3 != "")
                {
                    Settings.Default["HLC1"] = color3;
                    Settings.Default.Save();
                }
            }
            // herer we write the new colours to user settings files.
            string userName = Environment.UserName;
            
            text3 = "C:\\Users\\" + userName + "\\EdaPcb_Nav\\";
            string comfile = "EDAPCBUSR_set.rsc";

            bool exists2 = System.IO.File.Exists(text3 + comfile);
            string fileName = text3 + comfile;
            FileInfo fi = new FileInfo(fileName);
            try
            {
                // Check if file already exists. If yes, delete it.     
                if (fi.Exists)
                {
                    fi.Delete();
                }

                // Create a new file     
                using (FileStream fs = fi.Create())
                {
                    Byte[] txt = new UTF8Encoding(true).GetBytes("BGCOLOUR:" + Settings.Default.BGC1 + Environment.NewLine);
                    fs.Write(txt, 0, txt.Length);
                    Byte[] txt2 = new UTF8Encoding(true).GetBytes("HLCOLOUR:" + Settings.Default.HLC1 + Environment.NewLine);
                    fs.Write(txt2, 0, txt2.Length);
                    Byte[] txt3 = new UTF8Encoding(true).GetBytes("TXTCOLOUR:" + Settings.Default.TXTC1 + Environment.NewLine);
                    fs.Write(txt3, 0, txt3.Length);
                    Byte[] txt4 = new UTF8Encoding(true).GetBytes("DEFAULTPROJECT:" + Settings.Default.defaultProject + Environment.NewLine);
                    fs.Write(txt4, 0, txt4.Length);
                    Byte[] txt5 = new UTF8Encoding(true).GetBytes("LASTWIDTH:" + Settings.Default.lastwidth + Environment.NewLine);
                    fs.Write(txt5, 0, txt5.Length);
                    Byte[] txt6 = new UTF8Encoding(true).GetBytes("LASTHEIGHT:" + Settings.Default.lastheight + Environment.NewLine);
                    fs.Write(txt6, 0, txt6.Length);

                }

                // Write file contents on console.     
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
            //***************************************************

            // the sections restarts the ap[plication applying the new colourse selected.
            if (MessageBox.Show("Please restart the navigator for the changes to take effect.", "Restart", MessageBoxButton.YesNo,MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                //no stuff
            }
            else
            {
                //yes stuff
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = "/C choice /C Y /N /D Y /T 1 & START \"\" \"" + Assembly.GetEntryAssembly().Location + "\"";
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.FileName = "PCB_Nav3.exe";
                Process.Start(Info);
                Process.GetCurrentProcess().Kill();
            }

            
        }
    }

}
