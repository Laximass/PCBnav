using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Compression;
using System.IO;
using System.Windows;
using System.Windows.Shapes;
using System.Reflection;
using PCB_Nav3.Properties;
using System.Windows.Controls;
using System.Text;
using System.Windows.Media;
using System.Windows.Markup;

namespace PCB_Nav3
{
    /// <summary>
    /// Interaction logic for CreateNewProj.xaml
    /// </summary>
    public partial class CreateNewProj : Window
    {
        //public CreateNewProj()
        //{
        //    InitializeComponent();
        //    SetColours();
        //}

        private void SetColours()
        {
            // takes the colourse from the applications settings and applys them.
            string BackgroundColour = Settings.Default.BGC1;
            string TextColour = Settings.Default.TXTC2;
            CreateGrid.Resources["BGColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundColour));
            ProjNumlbl.Resources["TextColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));
            PartNumlbl.Resources["TextColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));
            Commentlbl.Resources["TextColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));
            ProjectNumtb.Resources["TextColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));
            PartNumtb.Resources["TextColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));
            Commenttb.Resources["TextColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));


        }

        private void CreateNewOKbtn_Click(object sender, RoutedEventArgs e)
        {
            // this takes the entered vales fromt he user form and creates a new job and adds it to the projhects folder.
            //this code was copied from the EDA navigator source code to make sure the correct process is followed when 
            string Comment = Commenttb.Text;
            string path = ProjectNumtb.Text;
            string text = PartNumtb.Text;
            string path2 = System.IO.Path.Combine(Settings.Default.defaultProject, path);
            string text2 = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            string text3 = System.IO.Path.Combine(path2, text);
            ZipFile.ExtractToDirectory(GetZipTemplate(), text2);
            CopyFolder(System.IO.Path.Combine(text2, "A-0000-0000-01-A\\"), text3);
            DirectoryInfo directoryInfo = new DirectoryInfo(text2);
            FileInfo[] files = directoryInfo.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                files[i].Delete();
            }
            DirectoryInfo[] directories = directoryInfo.GetDirectories();
            for (int i = 0; i < directories.Length; i++)
            {
                directories[i].Delete(recursive: true);
            }
            string text4 = Settings.Default.defaultMasterProject;
            string text5 = text3 + text4;
            string text6 = text3 + "\\" + text + ".cir";
            string text7 = "*A-0000-0000-01-A";
            IEnumerable<string> enumerable = Directory.EnumerateFiles(text3, text7 + "*.*", SearchOption.AllDirectories);
            clearPlaceHolder(text3);
            Getcomment(text3);
            foreach (string item in enumerable)
            {
                string directoryName = System.IO.Path.GetDirectoryName(item);
                string fileName = System.IO.Path.GetFileName(item);
                string text8 = System.IO.Path.Combine(directoryName, fileName.Replace("A-0000-0000-01-A", text));
                if (!File.Exists(text8))
                {
                    File.Move(item, text8);
                }
                if (File.Exists(text8))
                {
                    File.Delete(item);
                }
            }
            if (Directory.Exists(text6))
            {
                Directory.Delete(text5, recursive: true);
            }
            if (!Directory.Exists(text6))
            {
                              
                Directory.Move(text5, text6);
                MessageBox.Show("Project created successfully");
                CreateNewCANbtn_Click(sender, e);
            }
            else
            {
                MessageBox.Show("A project already exists with this project name");
            }
            string fileName2 = text3 + "\\" + text + ".sdm";
            string text9 = text + ".cir";
            string newText = "TOPCIRCUIT:" + text9;
            if (ExtractLine(fileName2, 2).Contains("A-0000-0000-01-A.cir"))
            {
                lineChanger(newText, fileName2, 2);
            }



        }

        private static void lineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] array = File.ReadAllLines(fileName);
            array[line_to_edit - 1] = newText;
            File.WriteAllLines(fileName, array);
        }

        private static string ExtractLine(string fileName, int line)
        {
            return File.ReadAllLines(fileName)[line - 1];
        }

        public static void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string obj in files)
            {
                string fileName = System.IO.Path.GetFileName(obj);
                string destFileName = System.IO.Path.Combine(destFolder, fileName);
                File.Copy(obj, destFileName, overwrite: true);
            }
            files = Directory.GetDirectories(sourceFolder);
            foreach (string obj2 in files)
            {
                string fileName2 = System.IO.Path.GetFileName(obj2);
                string destFolder2 = System.IO.Path.Combine(destFolder, fileName2);
                CopyFolder(obj2, destFolder2);
            }
        }

        private void clearPlaceHolder(string folderName)
        {
            foreach (string item in Directory.EnumerateFiles(folderName, "_Placeholder.txt", SearchOption.AllDirectories))
            {
                File.Delete(item);
            }
        }

        private string GetZipTemplate()
        {
            
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName("\\\\renishaw.com\\global\\GB\\PLC\\GE\\Data\\Electronics\\Documents\\EQs_from_PCB_supplier\\EQ System Progs\\PCB Naigator\\"), "A-0000-0000-01-A.zip");
                       
        }

        private void Getcomment(string text3)
        {
            string comfile = "\\resource\\data\\comments.rsc";
            bool exists2 = System.IO.File.Exists(text3 + comfile);
            if(!exists2)
            {
                string fileName = text3 + comfile;
                FileInfo fi = new FileInfo(fileName);

                // Create a new file     
                using (FileStream fs = fi.Create())
                {
                    Byte[] txt = new UTF8Encoding(true).GetBytes(Commenttb.Text);
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
                        Byte[] txt = new UTF8Encoding(true).GetBytes(Commenttb.Text);
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
        }

        private void CreateNewCANbtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
