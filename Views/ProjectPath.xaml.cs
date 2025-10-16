using System;
using System.IO;
using System.Windows;
using PCB_Nav3.Properties;
using System.Text;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace PCB_Nav3
{
    /// <summary>
    /// Interaction logic for CreateNewProj.xaml
    /// </summary>
    public partial class ProjectPath : Window
    {
        public ProjectPath()
        {
            InitializeComponent();
            SetColours();
        }

        private void SetColours()
        {
            // takes the colourse from the applications settings and applys them.
            string BackgroundColour = Settings.Default.BGC1;
            string TextColour = Settings.Default.TXTC2;
            CreateGrid.Resources["BGColour"] = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(BackgroundColour));
            ProjPathlbl.Resources["TextColour"] = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(TextColour));
            ProjectPathtb.Resources["TextColour"] = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(TextColour));
          
        }

        private void SetProjOKbtn_Click(object sender, RoutedEventArgs e)
        {
            string path = ProjectPathtb.Text;
            if(path != "")
            {
                
                Settings.Default["defaultProject"] = path;
                //Settings.Default["CustomPathProj"] = path;
                Settings.Default.Save();
                Savesetting();
                Hide();
            }
            
        }

        private void Savesetting()
        {
            string userName = Environment.UserName;

            string text3 = "C:\\Users\\" + userName + "\\EdaPcb_Nav\\";
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
        }

        private void SetProjCANbtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
