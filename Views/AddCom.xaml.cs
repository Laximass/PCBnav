using System;
using System.IO;
using System.Windows;
using PCB_Nav3.Properties;
using System.Text;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace PCB_Nav3
{
    /// <summary>
    /// Interaction logic for CreateNewProj.xaml
    /// </summary>
    public partial class AddCom : Window
    {
        string text3;
        string comfile;

        public AddCom(string SelecTree, string job)
        {
            InitializeComponent();
            SetColours();
            PartNumComtb.Text = SelecTree;
            text3 = job;
            if (text3 != null)
            {
                if (job.Length < 17)
                {
                    string s = SelecTree.ToString();
                    string s1 = Regex.Replace(s, "[^0-9-]", "");
                    s1 = s1.Remove(4);
                    PartNumComtb.Text = s1;
                    comfile = "\\comments.rsc";
                }
                else
                {
                    comfile = "\\resource\\data\\comments.rsc";
                }
                
            }
            else
            {
                string path = SelecTree.Remove(4);
                text3 = "C:\\Projects\\" + path;
                comfile = "\\comments.rsc"; 

            }
            
            bool exists2 = System.IO.File.Exists(text3 + comfile);
            if (exists2)
            {
                CommentComtb.Text = System.IO.File.ReadAllText(text3 + comfile);
            }

        }

        private void SetColours()
        {
            string BackgroundColour = Settings.Default.BGC1;
            String TextColour = Settings.Default.TXTC2;

            AddComGrid.Resources["BGColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundColour));
            PartNumlbl.Resources["TextColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));
            Commentlbl.Resources["TextColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));
            PartNumComtb.Resources["TextColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));
            CommentComtb.Resources["TextColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));
        }

        private void CreateNewComCANbtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }


        private void CreateNewComOKbtn_Click(object sender, RoutedEventArgs e)
        {
            
            string Comment = CommentComtb.Text;

            //string comfile = "\\resource\\data\\comments.rsc";
            bool exists2 = System.IO.File.Exists(text3 + comfile);
            if (!exists2)
            {
                string fileName = text3 + comfile;
                FileInfo fi = new FileInfo(fileName);

                // Create a new file     
                using (FileStream fs = fi.Create())
                {
                    Byte[] txt = new UTF8Encoding(true).GetBytes(CommentComtb.Text);
                    fs.Write(txt, 0, txt.Length);

                }

                // Write file contents on console.     
                using StreamReader sr = File.OpenText(fileName);
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                }
            }
            else
            {
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
                        Byte[] txt = new UTF8Encoding(true).GetBytes(CommentComtb.Text);
                        fs.Write(txt, 0, txt.Length);

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
            Hide();
        }
    }
    
}
