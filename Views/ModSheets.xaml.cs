using PCB_Nav3.Properties;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace PCB_Nav3
{
    /// <summary>
    /// Interaction logic for ModSheets.xaml
    /// </summary>
    public partial class ModSheets : Window
    {

        public ModSheets()
        {
            InitializeComponent();
            SetColours();
            PopulateList();


        }



        public class MenuItem
        {
            //sets the class for the tree view (Menu)
            public MenuItem()
            {
                this.Items = new ObservableCollection<MenuItem>();
            }

            public string Title { get; set; }
            public string Tag { get; set; }
            public ObservableCollection<MenuItem> Items { get; set; }
        }


        string ModTree;
        private object pathS;

        private void SetColours()
        {
            // this section will take the saved or default colours from the settings file and set them as default brush colours
            String BackgroundColour = Settings.Default.BGC1;
            String HiglightColour = Settings.Default.HLC1;
            String TextColour = Settings.Default.TXTC1;
            Mod_Grid.Resources["BGColour"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundColour));
            ModView.Resources[SystemColors.HighlightBrushKey] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(HiglightColour));
            ModView.Resources[SystemColors.InactiveSelectionHighlightBrushKey] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(HiglightColour));

            ModView.Resources[SystemColors.HighlightTextBrushKey] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));
            ModView.Resources[SystemColors.InactiveSelectionHighlightTextBrushKey] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextColour));
        }

        private void Edit_Mod_btn_Click(object sender, RoutedEventArgs e)
        {
            string File2Opn = pathS.ToString();
            if (File2Opn != "")
            {
                Process.Start("explorer", File2Opn);
            }

        }

        private void ModView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Edit_Mod_btn_Click(sender,e);
        }

        private void SaveAll_Mod_btn_Click(object sender, RoutedEventArgs e)
        {
            


        }

        private void Create_Mod_btn_Click(object sender, RoutedEventArgs e)
        {
            string File2Opn = "Q:\\CR8000_Design_Data\\Drawing_Notes\\Mod_sheet\\PCB_mod_sheet-v1.doc";
            Process.Start("explorer", File2Opn);

        }

        public void ClickCap(object sender, RoutedEventArgs args)

        {
            var vm = ((sender as FrameworkElement).DataContext);

            ModTree = ((PCB_Nav3.ModSheets.MenuItem)vm).Title;
            pathS = ((PCB_Nav3.ModSheets.MenuItem)vm).Tag;
        }

        public void PopulateList()
        {

            //sets root folder title to treeview
            MenuItem Modtitle = new MenuItem() { Title = "           Publish Folder ", Tag = "" };
            ModView.Items.Add(Modtitle);

            string ModSht = "*_mod.doc";
            string ModPath1 = Settings.Default.job + "\\publish";
            string text = Directory.GetFiles(ModPath1, ModSht, SearchOption.AllDirectories).FirstOrDefault();

            bool exists = System.IO.Directory.Exists(ModPath1);
            if (!exists)
            {
                MenuItem Note = new MenuItem() { Title = "Publish Folder NOT Found!", Tag = "" };
                ModView.Items.Add(Note);
            }
            else
            {

                DirectoryInfo objDirectoryInfo = new DirectoryInfo(ModPath1);
                FileInfo[] ModFiles = objDirectoryInfo.GetFiles(searchPattern: ModSht);
                //loops through each folder in projects and addes it to the tree under the main title
                if (ModFiles.Length > 0)
                {
                    foreach (var file in ModFiles)
                    {

                        MenuItem root = new MenuItem() { Title = file.Name, Tag = file.FullName };

                        //adds tree root item to the tree view window
                        ModView.Items.Add(root);

                    }
                }
                else
                {
                    MenuItem NoNote = new MenuItem() { Title = "No Modsheets Found!", Tag = "" };
                    ModView.Items.Add(NoNote);
                }

            }
            MenuItem Spacer = new MenuItem() { Title = "", Tag = "" };
            ModView.Items.Add(Spacer);

            //sets root folder title to treeview
            MenuItem Modtitle2 = new MenuItem() { Title = " DocsPCB\\Modsheets Folder ", Tag = "" };
            ModView.Items.Add(Modtitle2);

            string ModSht2 = "*_mod.doc";
            string ModPath2 = Settings.Default.job + "\\docsPcb\\modSheets";
            string text2 = Directory.GetFiles(ModPath1, ModSht, SearchOption.AllDirectories).FirstOrDefault();

            bool exists2 = System.IO.Directory.Exists(ModPath2);
            if (!exists2)
            {
                MenuItem Note = new MenuItem() { Title = "DocsPCB\\Modsheets NOT Found!", Tag = "" };
                ModView.Items.Add(Note);
            }
            else
            {

                DirectoryInfo objDirectoryInfo2 = new DirectoryInfo(ModPath2);
                FileInfo[] ModFiles2 = objDirectoryInfo2.GetFiles(searchPattern: ModSht2);
                //loops through each folder in projects and addes it to the tree under the main title
                if (ModFiles2.Length > 0)
                {
                    foreach (var file in ModFiles2)
                    {

                        MenuItem root = new MenuItem() { Title = file.Name, Tag = file.FullName };

                        //adds tree root item to the tree view window
                        ModView.Items.Add(root);
                    }
                }
                else
                {
                    MenuItem NoNote = new MenuItem() { Title = "No Modsheets Found!", Tag = "" };
                    ModView.Items.Add(NoNote);
                }

            }
        }

        private void Refresh_Mod_btn_Click(object sender, RoutedEventArgs e)
        {
            ModView.Items.Clear();
            PopulateList();
        }

       

        


        //**************************************
    }
}
