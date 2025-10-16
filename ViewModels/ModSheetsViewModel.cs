using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using PCB_Nav3.Properties;

namespace PCB_Nav3.ViewModels
{
    public class MenuItem
    {
        public MenuItem()
        {
            Items = new ObservableCollection<MenuItem>();
        }

        public string Title { get; set; }
        public string Tag { get; set; }
        public ObservableCollection<MenuItem> Items { get; set; }
    }

    public class ModSheetsViewModel
    {
        public ObservableCollection<MenuItem> ModSheetItems { get; set; } = new();
        public MenuItem SelectedItem { get; set; }

        public ICommand EditCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand RefreshCommand { get; }

        public ModSheetsViewModel()
        {
            EditCommand = new RelayCommand(OpenSelectedModSheet);
            CreateCommand = new RelayCommand(CreateNewModSheet);
            RefreshCommand = new RelayCommand(PopulateModSheets);

            PopulateModSheets();
        }

        private void PopulateModSheets()
        {
            ModSheetItems.Clear();

            AddModSheetsFromFolder(Settings.Default.job + "\\publish", "Publish Folder");
            ModSheetItems.Add(new MenuItem { Title = "", Tag = "" });
            AddModSheetsFromFolder(Settings.Default.job + "\\docsPcb\\modSheets", "DocsPCB\\Modsheets Folder");
        }

        private void AddModSheetsFromFolder(string folderPath, string sectionTitle)
        {
            ModSheetItems.Add(new MenuItem { Title = $"           {sectionTitle}", Tag = "" });

            if (!Directory.Exists(folderPath))
            {
                ModSheetItems.Add(new MenuItem { Title = $"{sectionTitle} NOT Found!", Tag = "" });
                return;
            }

            var files = new DirectoryInfo(folderPath).GetFiles("*_mod.doc");
            if (files.Length == 0)
            {
                ModSheetItems.Add(new MenuItem { Title = "No Modsheets Found!", Tag = "" });
                return;
            }

            foreach (var file in files)
            {
                ModSheetItems.Add(new MenuItem { Title = file.Name, Tag = file.FullName });
            }
        }

        private void OpenSelectedModSheet()
        {
            if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Tag))
            {
                Process.Start("explorer", SelectedItem.Tag);
            }
        }

        private void CreateNewModSheet()
        {
            string templatePath = "Q:\\CR8000_Design_Data\\Drawing_Notes\\Mod_sheet\\PCB_mod_sheet-v1.doc";
            Process.Start("explorer", templatePath);
        }
    }
}
