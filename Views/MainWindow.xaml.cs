using PCB_Nav3.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using File = System.IO.File;
using SearchOption = System.IO.SearchOption;

namespace PCB_Nav3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public string SelecTree;
        //public string text3;
        //public string path1;
        //public double CurWidth { get; set; }
        //public double NewWidth { get; set; }
        //public ObservableCollection<string> ProjFold = new ObservableCollection<string>();
       //public ObservableCollection<string> Project = new ObservableCollection<string>();
        //private List<string> expandedNodes = new List<string>();
        //private List<string> lastselcted = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            //UserSettings();
            //SetColours();
            //Buttonhide();
            //SetWaH();

            //btnshow.Click += Btnshow_Click;
            //btnclose.Click += Btnclose_Click;
            
            
            
            Settings.Default.assyname = null;
            Settings.Default.job = null;

            System.Diagnostics.Debug.WriteLine("assyname set to null");
            System.Diagnostics.Debug.WriteLine("job set to null");
            System.Diagnostics.Debug.WriteLine(Settings.Default.defaultProject);

            Versionlbl.Content = "V 1.0.3.4"; // keep inline with publish number or update check will fail!

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(1, 0, 0);
            dispatcherTimer.Start();
        }

        private void SetWaH()
        {
            if(Settings.Default.lastwidth != this.Width)
            {
                this.Width = Settings.Default.lastwidth;
            }
            
            if(Settings.Default.lastheight != this.Height)
            {
                this.Height = Settings.Default.lastheight;
            }
            
        }

        private void Buttonhide()
        {
            string textAdmin = "\\\\renishaw.com\\global\\GB\\PLC\\GE\\Data\\Electronics\\Documents\\EQs_from_PCB_supplier\\EQ System Progs\\PCB Naigator\\Application Files\\AdminIDs.txt";
            bool exists2 = System.IO.File.Exists("\\\\renishaw.com\\global\\GB\\PLC\\GE\\Data\\Electronics\\Documents\\EQs_from_PCB_supplier\\EQ System Progs\\PCB Naigator\\Application Files\\AdminIDs.txt");
            string userName = Environment.UserName.ToUpper();

            if (exists2)
            {
                System.IO.File.ReadLines(textAdmin);
                using StreamReader file = new StreamReader(textAdmin);
                int counter = 0;
                string ln;

                ln = file.ReadLine();

                while (ln != null)
                {
                    string users = ln;
                    if (users.Equals(userName))
                    {
                        btnshow.Visibility = Visibility.Visible;
                        return;
                    }
                    else
                    {
                            btnshow.Visibility = Visibility.Hidden;
                    }
                        ln = file.ReadLine();
                }
                file.Close();
                System.Diagnostics.Debug.WriteLine("File has " + counter + " lines.");
            }
            //string userName = Environment.UserName.ToUpper();
            //string[] users = new string[]{"AL138484","AT142586","BF134799","MP144169","HL152803","KT110723","MC136218","NL142363","RL131139","SJ141655"};
            //if (users.Contains(userName))
            //{
            //    btnshow.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    btnshow.Visibility = Visibility.Hidden;
            //}
        }
       
        private void Btnclose_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["CloseMenu"] as Storyboard;
            btnshow.Visibility = Visibility.Visible;
            sb.Begin(PopinMenu);
        }

        private void Btnshow_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["OpenMenu"] as Storyboard;
            btnshow.Visibility = Visibility.Hidden;
            sb.Begin(PopinMenu);
        }

        private void SetColours()
        {
            // this section will take the saved or default colours from the settings file and set them as default brush colours
            String BackgroundColour = Settings.Default.BGC1;
            String HiglightColour = Settings.Default.HLC1;
            String TextColour = Settings.Default.TXTC1;

            MainGrid.Resources["BGColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundColour));

            TRView.Resources[SystemColors.HighlightBrushKey] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(HiglightColour));
            TRView.Resources[SystemColors.InactiveSelectionHighlightBrushKey] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(HiglightColour));

            TRView.Resources[SystemColors.HighlightTextBrushKey] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));
            TRView.Resources[SystemColors.InactiveSelectionHighlightTextBrushKey] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Settings.Default.lastwidth = Width;
            Settings.Default.lastheight = Height;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Savesetting();
        }

        //*********************** new get populate projects   ***********************

        private void ClosetreeBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeViewItem item in TRView.Items)
            {
                item.IsExpanded = false;
            }
            TRView.Items.Refresh();
        }

        private void TRView_Expanded(object sender, RoutedEventArgs e)
        {
            var item = ((System.Windows.Controls.HeaderedItemsControl)e.OriginalSource).Header;
            
            string header = item.ToString();
            if (header != "Projects")
            {
                if (expandedNodes.Contains(header) == false)
                {
                    expandedNodes.Add(header);
                }
            }
           
            
        }
        
        private void TRView_Collapsed(object sender, RoutedEventArgs e)
        {
            this.SizeToContent = SizeToContent.Manual;
            var item = ((System.Windows.Controls.HeaderedItemsControl)e.OriginalSource).Header;
            
            string header = item.ToString();
            if (expandedNodes.Contains(header))
            {
                expandedNodes.Remove(header);
            }
            
            
        }

        private void TreeView_Loaded(object sender, RoutedEventArgs e)
        {
            // ... Create a TreeViewItem.
            TreeViewItem item = new TreeViewItem() { Header = "Projects", Tag = "" };
            TRView.Items.Add(item);

            bool exists = System.IO.Directory.Exists(Settings.Default.defaultProject);
            if (!exists)
            {
                //if it dosent exist it then asks for the file path to your projects.
                if (MessageBox.Show("Projects Folder Not found, would you like to point to it", "Project folder", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    //no stuff
                    return;
                }
                else
                { //yes stuff
                    Window window = new ProjectPath();
                    window.ShowDialog();
                }
            }
            else
            {

                //add project folder as headders
                DirectoryInfo objDirectoryInfo = new DirectoryInfo(Settings.Default.defaultProject);
                DirectoryInfo[] ProjFolders = objDirectoryInfo.GetDirectories();

                foreach (var direc in ProjFolders)
                {
                    //create new subdir holding name and path
                    TreeViewItem item2 = new TreeViewItem();
                    string comfile3 = "\\comments.rsc";
                    bool exists3 = System.IO.File.Exists(direc + comfile3);
                    if (!exists3)
                    {
                        item2.Header = direc.Name;
                        item2.Tag = direc.FullName;
                    }
                    else
                    {
                        string Comment = System.IO.File.ReadAllText(direc + comfile3);
                        if (Comment.Length > 0)
                        {
                            
                            item2.Header = direc.Name + "    (" + Comment + ")";
                            item2.Tag = direc.FullName;
                        }
                        else
                        {
                            item2.Header = direc.Name;
                            item2.Tag = direc.FullName;
                        }
                            
                    }
                    DirectoryInfo objDirectoryInfo2 = new DirectoryInfo(direc.FullName);
                    DirectoryInfo[] ProjFoldersSub = objDirectoryInfo2.GetDirectories();

                    foreach (var direc2 in ProjFoldersSub)
                    {
                        bool Subdir1 = System.IO.Directory.Exists(direc2.FullName + "\\publish");
                        if (!Subdir1)
                        {
                            TreeViewItem item3 = new TreeViewItem() { Header = direc2.Name, Tag = direc2.FullName };

                            DirectoryInfo objDirectoryInfo3 = new DirectoryInfo(direc2.FullName);
                            DirectoryInfo[] ProjFoldersSub2 = objDirectoryInfo3.GetDirectories();
                            foreach (var direc3 in ProjFoldersSub2)
                            {
                                //adds each sub directory with comments if availbe 
                                string comfile = "\\resource\\data\\comments.rsc";
                                bool exists2 = System.IO.File.Exists(direc3 + comfile);
                                if (!exists2)
                                {
                                    TreeViewItem childItem2 = new TreeViewItem() { Header = direc3.Name, Tag = direc3.FullName };
                                    item3.Items.Add(childItem2);
                                    if (expandedNodes.Contains(item3.Header))
                                    {
                                        item3.IsExpanded = true;
                                    }
                                    if (lastselcted.Contains(childItem2.Tag))
                                    {
                                        childItem2.IsSelected = true;
                                    }
                                }
                                else
                                {
                                    string Comment = System.IO.File.ReadAllText(direc3 + comfile);
                                    if (Comment.Length > 0)
                                    {
                                        TreeViewItem childItem2 = new TreeViewItem() { Header = direc3.Name + "    (" + Comment + ")", Tag = direc3.FullName };
                                        item3.Items.Add(childItem2);
                                        if (expandedNodes.Contains(item3.Header))
                                        {
                                            item3.IsExpanded = true;
                                        }
                                        if (lastselcted.Contains(childItem2.Tag))
                                        {
                                            childItem2.IsSelected = true;
                                        }
                                    }
                                    else
                                    {

                                        TreeViewItem childItem2 = new TreeViewItem() { Header = direc3.Name, Tag = direc3.FullName };
                                        item3.Items.Add(childItem2);
                                        if (expandedNodes.Contains(item3.Header))
                                        {
                                            item3.IsExpanded = true;
                                        }
                                        if (lastselcted.Contains(childItem2.Tag))
                                        {
                                            childItem2.IsSelected = true;
                                        }
                                    }
                                }
                            }
                            item2.Items.Add(item3);
                        }
                        else
                        {
                            //adds each sub directory with comments if availbe 
                            string comfile = "\\resource\\data\\comments.rsc";
                            bool exists2 = System.IO.File.Exists(direc2 + comfile);
                            if (!exists2)
                            {
                                TreeViewItem childItem1 = new TreeViewItem() { Header = direc2.Name, Tag = direc2.FullName };
                                item2.Items.Add(childItem1);
                                if (expandedNodes.Contains(item2.Header))
                                {
                                    item2.IsExpanded = true;
                                }
                                if (lastselcted.Contains(childItem1.Tag))
                                {
                                    childItem1.IsSelected = true;
                                }
                            }
                            else
                            {
                                string Comment = System.IO.File.ReadAllText(direc2 + comfile);
                                if (Comment.Length > 0)
                                {
                                    TreeViewItem childItem1 = new TreeViewItem() { Header = direc2.Name + "    (" + Comment + ")", Tag = direc2.FullName };
                                    item2.Items.Add(childItem1);
                                    if (expandedNodes.Contains(item2.Header))
                                    {
                                        item2.IsExpanded = true;
                                    }
                                    if (lastselcted.Contains(childItem1.Tag))
                                    {
                                        childItem1.IsSelected = true;
                                    }
                                }
                                else
                                {

                                    TreeViewItem childItem1 = new TreeViewItem() { Header = direc2.Name, Tag = direc2.FullName };
                                    item2.Items.Add(childItem1);
                                    if (expandedNodes.Contains(item2.Header))
                                    {
                                        item2.IsExpanded = true;
                                    }
                                    if (lastselcted.Contains(childItem1.Tag))
                                    {
                                        childItem1.IsSelected = true;
                                    }
                                }
                            }
                        }

                    }
                    TRView.Items.Add(item2);
                }
            }

        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tree = sender as TreeView;

            // ... Determine type of SelectedItem.
            if (tree.SelectedItem is TreeViewItem)
            {
                var item = tree.SelectedItem as TreeViewItem;
                //this.Title = "Selected header: " + item.Tag.ToString();
                
                if (item != null)
                {
                    // Do something.

                    SelecTree = item.Header.ToString();
                    if (SelecTree.Equals("Projects"))
                    {
                        return;
                    }
                    int tooLongInt = 16;
                    if (SelecTree.Length <5)
                    {
                        lastselcted.Clear();
                        Settings.Default.assyname = null;
                        Settings.Default.job = null;

                        System.Diagnostics.Debug.WriteLine("assyname set to null");
                        System.Diagnostics.Debug.WriteLine("job set to null");
                        return;
                    }
                    if (SelecTree.Length > tooLongInt)
                    {
                        if (!SelecTree.Contains("_uploaded"))
                        {
                            DirectoryInfo objDirectoryInfo = new DirectoryInfo(item.Tag.ToString());

                            string comfile = "\\resource\\data\\comments.rsc";
                            bool exists2 = System.IO.File.Exists(item.Tag + comfile);

                            if (exists2)
                            {
                                string Comment = System.IO.File.ReadAllText(item.Tag + comfile);
                                if (Comment.Length > 0)
                                {
                                    SelecTree = SelecTree.Replace(Comment, "");

                                    SelecTree = SelecTree.Replace(" ", "");
                                    
                                    SelecTree = SelecTree.Replace("()", "");
                                }
                                
                            }
                            
                           
                        }

                    }
                    DirectoryInfo objDirectoryInfo3 = new DirectoryInfo(item.Tag.ToString());

                    bool exists3 = System.IO.Directory.Exists(objDirectoryInfo3.FullName);
                    if (exists3)
                    {
                        //if match is found then sets the values for assyname and job.
                        string obj = SelecTree;
                        string fileName = obj;
                        string jobPath = item.Tag.ToString();
                        lastselcted.Clear();
                        lastselcted.Add(jobPath);
                        Settings.Default.assyname = fileName;
                        Settings.Default.job = jobPath;

                        System.Diagnostics.Debug.WriteLine(" default assembly - " + fileName);
                        System.Diagnostics.Debug.WriteLine(" default job - " + jobPath);
                        System.Diagnostics.Debug.WriteLine(" Selectree - " + SelecTree);
                    }
                }
                else
                {
                    SelecTree = item.Header.ToString();
                    lastselcted.Clear();
                    Settings.Default.assyname = null;
                    Settings.Default.job = null;

                    System.Diagnostics.Debug.WriteLine("assyname set to null");
                    System.Diagnostics.Debug.WriteLine("job set to null");
                }

            }

        }

        private void rePopulateProjectsList(object sender, RoutedEventArgs e)
        {
            
            TRView.Items.Clear();            
            TreeView_Loaded(sender,e);
            TRView.Items.Refresh();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // code goes here
            chkupdate();
        }

        private void chkupdate()
        {
            DirectoryInfo objDirectoryInfo = new DirectoryInfo("\\\\renishaw.com\\global\\GB\\PLC\\GE\\Data\\Electronics\\Documents\\EQs_from_PCB_supplier\\EQ System Progs\\PCB Naigator\\Application Files\\");
            DirectoryInfo[] ProjFolders = objDirectoryInfo.GetDirectories();

            foreach (var folder in ProjFolders)
            {
                string v = folder.Name;
                v = v.Substring(8, 8);
                v = v.Replace("_", "");

                string v2 = this.Versionlbl.Content.ToString();
                v2 = v2.Substring(2);
                v2 = v2.Replace(".", "");
                int Update = Int16.Parse(v);
                int Current = Int16.Parse(v2);

                if (Update > Current)
                {
                    string Updatemsg = File.ReadAllText("\\\\renishaw.com\\global\\GB\\PLC\\GE\\Data\\Electronics\\Documents\\EQs_from_PCB_supplier\\EQ System Progs\\PCB Naigator\\Application Files\\PopUpdateTxt.txt");
                   
                    MessageBox.Show(Application.Current.MainWindow, Updatemsg, "Update!");
                    
                }
            }
        }

        //*********************************************************************************************************

        private static void LineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] array = File.ReadAllLines(fileName);
            array[line_to_edit - 1] = newText;
            File.WriteAllLines(fileName, array);
        }

        private static string ExtractLine(string fileName, int line)
        {
            return File.ReadAllLines(fileName)[line - 1];
        }

        void UserSettings()
        {
            string userName = Environment.UserName;
            string text3 = "C:\\Users\\" + userName + "\\EdaPcb_Nav\\";
            string comfile = "EDAPCBUSR_set.rsc";

            bool exists2 = System.IO.File.Exists(text3 + comfile);

            if (exists2)
            {
                System.IO.File.ReadLines(text3 + comfile);
                using StreamReader file = new StreamReader(text3 + comfile);
                int counter = 0;
                string ln;

                ln = file.ReadLine();

                while (ln != null)
                {
                    System.Diagnostics.Debug.WriteLine(ln);
                    if (counter == 0)
                    {
                        // this section reads the saved user setting (BG color, text colour) and applies it to the application so
                        // the user dose not need to reset thier colour scheem each time the application updates.

                        string line1 = ln.Replace("BGCOLOUR:", "");
                        Settings.Default["BGC1"] = line1;
                    }
                    else if (counter == 1)
                    {
                        string line2 = ln.Replace("HLCOLOUR:", "");
                        Settings.Default["HLC1"] = line2;
                    }
                    else if (counter == 2)
                    {
                        string line3 = ln.Replace("TXTCOLOUR:", "");
                        Settings.Default["TXTC1"] = line3;
                    }
                    else if (counter == 3)
                    {
                        string line4 = ln.Replace("DEFAULTPROJECT:", ""); 
                        Settings.Default["defaultProject"] = line4;
                    }
                    else if (counter == 4)
                    {
                       
                        string numbers = ln.Replace("LASTWIDTH:", "");
                        double line5 = Convert.ToDouble(numbers);
                        Settings.Default["lastwidth"] = line5;
                        
                    }
                    else if (counter == 5)
                    {
                       
                        string numbers2 = ln.Replace("LASTHEIGHT:", "");
                        double line6 = Convert.ToDouble(numbers2);
                        Settings.Default["lastheight"] = line6;
                        
                    }
                    counter++;
                    ln = file.ReadLine();
                }
                file.Close();
                System.Diagnostics.Debug.WriteLine("File has " + counter + " lines.");
            }
            else
            {
                System.IO.Directory.CreateDirectory(text3);
                string fileName = text3 + comfile;
                FileInfo fi = new FileInfo(fileName);

                // Create a new file on target C:\ 
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
                    Byte[] txt6 = new UTF8Encoding(true).GetBytes("LASTHEIGHT:" + Settings.Default.lastheight);
                    fs.Write(txt6, 0, txt6.Length);
                }

                // Write file contents on target console.     
                using StreamReader sr = File.OpenText(fileName);
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                }
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

        private new void MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // this will capture a righ click event and open the right clcik menu
            ContextMenu cm = FindResource("RCMenuItem") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;
        }

        private void Colours_Click(object sender, RoutedEventArgs e)
        {
            // this will open the colors page for setting custom colours to the GUI
            Window window = new Colours();
            window.ShowDialog();
        }

        private void AddJobTitle_Click(object sender, RoutedEventArgs e)
        {
               
            //this will run the excicutible to add a comment/job title, then reset the treeview and lists to stay in sync
            Window window = new AddCom(SelecTree, Settings.Default.job);
            window.ShowDialog();

            Settings.Default.assyname = null;
            Settings.Default.job = null;

            System.Diagnostics.Debug.WriteLine("assyname set to null");
            System.Diagnostics.Debug.WriteLine("job set to null");

            TRView.Items.Clear();
            rePopulateProjectsList(sender, e);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            //this will reset all paths held in the background and then re-populate the treeview with fresh data. this prevent indexing errors.
            ///<summary></summary>

            Settings.Default.assyname = null;
            Settings.Default.job = null;

            System.Diagnostics.Debug.WriteLine("assyname set to null");
            System.Diagnostics.Debug.WriteLine("job set to null");


            rePopulateProjectsList(sender, e);

        }



        //the below buttons open up DF, DG, DFM, etc.

        private void DGBtn_Click(object sender, RoutedEventArgs e)
        {
            // this will open up the schematic if present in the job folder
            if (Settings.Default.job != null)
            {
                if (Settings.Default.job.Contains("_uploaded"))
                {
                    MessageBox.Show(Application.Current.MainWindow, "Error, this has been uploaded, please remove _uploaded from title and try again!");
                }
                else
                {
                    string searchPattern = Settings.Default.assyname + ".sdm";
                    string searchdir = Settings.Default.job + "\\" + searchPattern;
                    bool exist = File.Exists(searchdir);
                    if (exist)
                    {


                        string text = Directory.GetFiles(Settings.Default.job, "*.sdm", SearchOption.AllDirectories).FirstOrDefault();

                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            var process = Process.Start(new ProcessStartInfo(text) { UseShellExecute = true });
                            process.WaitForInputIdle();
                        }

                    }
                    else
                    {
                        MessageBox.Show(Application.Current.MainWindow, "Error, this is not a DG job");
                    }

                }
            }
            
        }

        private void DFBtn_Click(object sender, RoutedEventArgs e)
        {
            //this searches for the DF PCB file that match the assyname and if it exist it will open it.

            //******* im am here making it open a altium job ********** //
            if (Settings.Default.job != null)
            {
                if (Settings.Default.job.Contains("_uploaded"))
                {
                    MessageBox.Show(Application.Current.MainWindow, "Error, this has been uploaded, please remove _uploaded from title and try again!");
                }
                else
                {
                    string searchPattern = Settings.Default.assyname + ".dsgn";
                    string searchdir = Settings.Default.job + "\\df\\";
                    bool exist = Directory.Exists(searchdir);
                    if (exist)
                    {

                        string text = Directory.GetFiles(searchdir, searchPattern, SearchOption.AllDirectories).FirstOrDefault();

                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            var process = Process.Start(new ProcessStartInfo(text) { UseShellExecute = true });
                            process.WaitForInputIdle();
                        }

                    }
                    else
                    {
                        MessageBox.Show(Application.Current.MainWindow, "Error, this is not a DF job or PCB file does not exist");
                    }
                }
            }
        }

        private void DFMBtn_Click(object sender, RoutedEventArgs e)
        {
            //this searches for the DF Panel file that match the assyname and if it exist it will open it.
            if (Settings.Default.job != null)
            {

                if (Settings.Default.job.Contains("_uploaded"))
                {
                    MessageBox.Show(Application.Current.MainWindow, "Error, this has been uploaded, please remove _uploaded from title and try again!");
                }
                else
                {
                    string searchPattern = Settings.Default.assyname + ".mdgn";
                    string searchdir = Settings.Default.job + "\\df\\";
                    bool exist = Directory.Exists(searchdir);
                    if (exist)
                    {

                        string text = Directory.GetFiles(searchdir, searchPattern, SearchOption.AllDirectories).FirstOrDefault();

                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            var process = Process.Start(new ProcessStartInfo(text) { UseShellExecute = true });
                            process.WaitForInputIdle();
                        }

                    }
                    else
                    {
                        MessageBox.Show(Application.Current.MainWindow, "Error, this is not a DF job or PCB file does not exist");
                    }
                }
            }
        }

        private void CreateNewProjbtn_Click(object sender, RoutedEventArgs e)
        {
            // this will run the ceate new job excicutible, and then refresh the treeview and background lists so everything stays in sync.

            Window window = new CreateNewProj();
            window.ShowDialog();

            Settings.Default.assyname = null;
            Settings.Default.job = null;

            System.Diagnostics.Debug.WriteLine("assyname set to null");
            System.Diagnostics.Debug.WriteLine("job set to null");

            TRView.Items.Clear();
            rePopulateProjectsList(sender, e);
            //Expandnode(sender, e);

        }

        private void RenameProjbtn_Click(object sender, RoutedEventArgs e)
        {
            // this will run the Rename project excicutible, and then refresh the treeview and background lists so everything stays in sync.
            if (Settings.Default.job != null)
            {
                string executable = Settings.Default.renameProj;
                LaunchProgram(executable, Settings.Default.job, Settings.Default.assyname);

                Settings.Default.assyname = null;
                Settings.Default.job = null;

                System.Diagnostics.Debug.WriteLine("assyname set to null");
                System.Diagnostics.Debug.WriteLine("job set to null");

                TRView.Items.Clear();
                rePopulateProjectsList(sender, e);
                //Expandnode(sender, e);
            }


        }

        private void LaunchProgram(string executable, string job, string assyname)
        {
            //this sets the correct path for runnning excicutibles, and awits for the excicutible to finish before alowing the user to interact with the navigator.

            var process = Process.Start(executable, "-sfile \"" + job + "\" -file \"" + assyname + "\"");
            process.WaitForExit();
        }

        private void btnPublishOP_Click(object sender, EventArgs e)
        {
            // this will run publish outputs excicutible
            if (Settings.Default.job != null)
            {
                if (Settings.Default.job.Contains("_uploaded"))
                {
                    MessageBox.Show(Application.Current.MainWindow, "Error, this has been uploaded, please remove _uploaded from title and try again!");
                }
                else
                {

                    string executable = Settings.Default.pubOP;
                    LaunchProgram(executable, Settings.Default.job, Settings.Default.assyname);

                }
            }
             
            
        }

        private void btnPublishProtoOP_Click(object sender, RoutedEventArgs e)
        {
            // this will run publish prototype outputs excicutible
            if (Settings.Default.job != null)
            {
                if (Settings.Default.job.Contains("_uploaded"))
                {
                    MessageBox.Show(Application.Current.MainWindow, "Error, this has been uploaded, please remove _uploaded from title and try again!");
                }
                else
                {
                    string executable = Settings.Default.publishProtoOP;
                    LaunchProgram(executable, Settings.Default.job, Settings.Default.assyname);
                }
            }
        }

        private void ChkOPbtn_Click(object sender, RoutedEventArgs e)
        {
            // this will run Check outputs excicutible
            if (Settings.Default.job != null)
            {
                if (Settings.Default.job.Contains("_uploaded"))
                {
                    MessageBox.Show(Application.Current.MainWindow, "Error, this has been uploaded, please remove _uploaded from title and try again!");
                }
                else
                {
                    string executable = Settings.Default.checkOPDG;
                    LaunchProgram(executable, Settings.Default.job, Settings.Default.assyname);
                }
            }
            
            
        }

        private void UTTCBtn_Click(object sender, RoutedEventArgs e)
        {
            // this will run Teamcentre upload excicutible then reset the tree view
            if (Settings.Default.job != null)
            {
                if (Settings.Default.job.Contains("_uploaded"))
                {
                    MessageBox.Show(Application.Current.MainWindow, "Error, this has been uploaded, please remove _uploaded from title and try again!");
                }
                else
                {
                    string executable = Settings.Default.tcUpload;
                    LaunchProgram(executable, Settings.Default.job, Settings.Default.assyname);
                    
                    Settings.Default.assyname = null;
                    Settings.Default.job = null;

                    System.Diagnostics.Debug.WriteLine("assyname set to null");
                    System.Diagnostics.Debug.WriteLine("job set to null");

                    TRView.Items.Clear();
                    rePopulateProjectsList(sender, e);
                    //Expandnode(sender, e);
                }
            }
            
        }

        private void SD_DG_Btn_Click(object sender, RoutedEventArgs e)
        {
            // this will run SD to DG convertion excicutible
            if (Settings.Default.job != null)
            {
                string executable = Settings.Default.SDToDG;
                LaunchProgram(executable, Settings.Default.job, Settings.Default.assyname);
            }
        }

        private void SD_DG_Com_Btn_Click(object sender, RoutedEventArgs e)
        {
            // this will run SD to DG compare excicutible
            if (Settings.Default.job != null)
            {
                string executable = Settings.Default.compSDDG;
                LaunchProgram(executable, Settings.Default.job, Settings.Default.assyname);
            }
        }

        private void BD_DF_Btn_Click(object sender, RoutedEventArgs e)
        {
            // this will run BD to DF convertion excicutible
            if (Settings.Default.job != null)
            {
                string executable = Settings.Default.BDtoDFCon;
                LaunchProgram(executable, Settings.Default.job, Settings.Default.assyname);
            }
        }

        private void EDPMFBtn_Click(object sender, RoutedEventArgs e)
        {
            // this will run Edit peramiter files  excicutible
            if (Settings.Default.job != null)
            {
                if (Settings.Default.job.Contains("_uploaded"))
                {
                    MessageBox.Show(Application.Current.MainWindow, "Error, this has been uploaded, please remove _uploaded from title and try again!");
                }
                else
                {
                    string executable = Settings.Default.editPDF;
                    LaunchProgram(executable, Settings.Default.job, Settings.Default.assyname);
                }
            }
        }
        //testing out updated parameter files - change button path back before saving 
        private void EDPMFBtn_Click2(object sender, RoutedEventArgs e)
        {
            string userName = Environment.UserName;

            string text3 = "C:\\Users\\" + userName + "\\EdaPcb_Nav\\";
            string comfile = "renDFEditPdfParams_Ash.bat";

            bool exists2 = System.IO.File.Exists(text3 + comfile);
            string fileName = text3 + comfile;
            // this will run Edit peramiter files  excicutible
            if (Settings.Default.job != null)
            {
                if (Settings.Default.job.Contains("_uploaded"))
                {
                    MessageBox.Show(Application.Current.MainWindow, "Error, this has been uploaded, please remove _uploaded from title and try again!");
                }
                else
                {
                    string executable = fileName;
                    LaunchProgram(executable, Settings.Default.job, Settings.Default.assyname);
                }
            }
        }

        private void HelpNote_Btn_Click(object sender, RoutedEventArgs e)
        {
            //string HelpNotes = "https://renishawplc.sharepoint.com/:w:/r/sites/EDAPCBDesign/PCB%20Design%20Docs/DF_ADVANCED_HELP_NOTES.docx?d=wcff13fd207b04698889eaf4484b88747&csf=1&web=1&e=FzSM7h";
            //Process.Start("MicrosoftEdge.exe", HelpNotes);
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "https://renishawplc.sharepoint.com/:w:/r/sites/EDAPCBDesign/PCB%20Design%20Docs/DF_ADVANCED_HELP_NOTES.docx?d=wcff13fd207b04698889eaf4484b88747&csf=1&web=1&e=FzSM7h";
                p.StartInfo.UseShellExecute = true;
                p.Start();
            }
        }

        private void ProPart_Btn_Click(object sender, RoutedEventArgs e)
        {
            string ProPart = "\\\\gbvuap0275\\Data\\dump-from-CDB\\ProPart_Finder_Trial.xlsb";
            Process.Start("explorer", ProPart);
        }


        // Folder buttons

        private void JobFolbtn_Click(object sender, RoutedEventArgs e)
        {
            // this will open the selected job folder in explorer
            if (Settings.Default.job != null)
            {
                Process.Start("explorer", Settings.Default.job);
            }
        }

        private void ScheFolbtn_Click(object sender, RoutedEventArgs e)
        {
            // this will open the selected jobs schematic folder in explorer
            if (Settings.Default.job != null)
            {
                string Folder2Opn = Settings.Default.job + "\\" + Settings.Default.assyname + ".cir";
                Process.Start("explorer", Folder2Opn);
            }
        }

        private void DFfolbtn_Click(object sender, RoutedEventArgs e)
        {
            // this will open the selected jobs DF folder in explorer
            if (Settings.Default.job != null)
            {
                string Folder2Opn = Settings.Default.job + "\\df";
                Process.Start("explorer", Folder2Opn);
            }
        }

        private void DocPCBbtn_Click(object sender, RoutedEventArgs e)
        {
            // this will open the selected jobs PCB_document folder in explorer
            if (Settings.Default.job != null)
            {
                string Folder2Opn = Settings.Default.job + "\\docsPcb";
                Process.Start("explorer", Folder2Opn);
            }
        }

        private void GerFolbtn_Click(object sender, RoutedEventArgs e)
        {
            // this will open the selected jobs folder where the gerbers are stored in explorer
            if (Settings.Default.job != null)
            {
                string Folder2Opn = Settings.Default.job + "\\df\\manufacturingDf\\gerbers";
                Process.Start("explorer", Folder2Opn);
            }
        }

        private void PubFolbtn_Click(object sender, RoutedEventArgs e)
        {
            // this will open the selected jobs publish folder in explorer
            if (Settings.Default.job != null)
            {
                string Folder2Opn = Settings.Default.job + "\\publish";
                Process.Start("explorer", Folder2Opn);
            }
        }


        // Functions
        private void RemovUPloadbtn_Click(object sender, RoutedEventArgs e)
        {

            if (Settings.Default.job != null)
            {
                if (Settings.Default.job.Contains("_uploaded"))
                {
                    string dest = Settings.Default.job;
                    dest = dest.Replace("_uploaded", "");
                    if (!string.IsNullOrWhiteSpace(dest))
                    {
                        try
                        {
                            Directory.Move(Settings.Default.job, dest);
                        }
                        catch
                        {
                            MessageBox.Show("Unable to rename, please check for any open files or folders linked to this partnumber!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Unable to rename");
                    }


                    Settings.Default.assyname = null;
                    Settings.Default.job = null;

                    System.Diagnostics.Debug.WriteLine("assyname set to null");
                    System.Diagnostics.Debug.WriteLine("job set to null");

                    TRView.Items.Clear();
                    rePopulateProjectsList(sender, e);
                    //Expandnode(sender, e);

                }
            }
        }

        private void SetProj_Click(object sender, RoutedEventArgs e)
        {
            Window window = new ProjectPath();
            window.ShowDialog();

            TRView.Items.Clear();
            rePopulateProjectsList(sender, e);
        }

        private void RotaBtn_Click(object sender, RoutedEventArgs e)
        {
            
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "https://renishawplc.sharepoint.com/:x:/r/sites/EDAPCBDesign/_layouts/15/Doc.aspx?sourcedoc=%7BDC807F5B-DD0A-4371-81B6-4FC28CDACC11%7D&file=Checking%20Rota.xlsx&action=default&mobileredirect=true";
                p.StartInfo.UseShellExecute = true;
                p.Start();
            }
        }

        private void NotesBtn_Click(object sender, RoutedEventArgs e)
        {
            string Folder2Opn = "Q:\\CR8000_Design_Data\\Drawing_Notes\\Drawing_Titles_and_Notes";
            Process.Start("explorer", Folder2Opn);

        }

        private void ModShBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.job != null)
            {
                Window window = new ModSheets();
                window.ShowDialog();
            }


        }

        private void Updatelog_Click(object sender, RoutedEventArgs e)
        {
            Window window = new UpdatelogWin();
            window.ShowDialog();
        }

        private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
        {
            string HelpNotes = "\\\\renishaw.com\\global\\GB\\PLC\\GE\\Data\\Electronics\\Documents\\EQs_from_PCB_supplier\\EQ System Progs\\PCB Naigator\\Application Files\\EDA PCB Nav Doc.docx";
            Process.Start("explorer",HelpNotes);

        }

        private void MDDocBtnoc_Click(object sender, RoutedEventArgs e)
        {
            //string MDDocNotes = "https://renishawplc.sharepoint.com/:w:/r/sites/EDAPCBDesign/PCB%20Design%20Docs/Creating%20an%20M-Drawing%20-%20Mar2022.docx?d=w1c8650c126994ab0b683ff71c1414a83&csf=1&web=1&e=Ykt4ds";
           // Process.Start("MicrosoftEdge.exe", MDDocNotes);
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "https://renishawplc.sharepoint.com/:w:/r/sites/EDAPCBDesign/PCB%20Design%20Docs/Creating%20an%20M-Drawing%20-%20Mar2022.docx?d=w1c8650c126994ab0b683ff71c1414a83&csf=1&web=1&e=Ykt4ds";
                p.StartInfo.UseShellExecute = true;
                p.Start();
            }
        }

        
    }
}

