using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows;
using System.Windows.Input;
using PCB_Nav3.Properties;

namespace PCB_Nav3.ViewModels
{
    public class CreateNewProjViewModel : INotifyPropertyChanged
    {
        private string _projectCode;
        private string _partNumber;
        private string _jobTitle;

        public string ProjectCode
        {
            get => _projectCode;
            set
            {
                _projectCode = value;
                OnPropertyChanged(nameof(ProjectCode));
            }
        }

        public string PartNumber
        {
            get => _partNumber;
            set
            {
                _partNumber = value;
                OnPropertyChanged(nameof(PartNumber));
            }
        }

        public string JobTitle
        {
            get => _jobTitle;
            set
            {
                _jobTitle = value;
                OnPropertyChanged(nameof(JobTitle));
            }
        }

        public ICommand CreateProjectCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action RequestClose;
        public event PropertyChangedEventHandler PropertyChanged;

        public CreateNewProjViewModel()
        {
            CreateProjectCommand = new RelayCommand(CreateProject);
            CancelCommand = new RelayCommand(() => RequestClose?.Invoke());
        }

        private void CreateProject()
        {
            string comment = JobTitle;
            string path = ProjectCode;
            string part = PartNumber;
            string basePath = Path.Combine(Settings.Default.defaultProject, path);
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            string fullPath = Path.Combine(basePath, part);

            ZipFile.ExtractToDirectory(GetZipTemplate(), tempPath);
            CopyFolder(Path.Combine(tempPath, "A-0000-0000-01-A\\"), fullPath);

            DirectoryInfo tempDir = new DirectoryInfo(tempPath);
            foreach (var file in tempDir.GetFiles()) file.Delete();
            foreach (var dir in tempDir.GetDirectories()) dir.Delete(true);

            string masterProject = Settings.Default.defaultMasterProject;
            string masterPath = fullPath + masterProject;
            string cirPath = fullPath + "\\" + part + ".cir";
            string pattern = "*A-0000-0000-01-A";
            var files = Directory.EnumerateFiles(fullPath, pattern + "*.*", SearchOption.AllDirectories);

            clearPlaceHolder(fullPath);
            GetComment(fullPath, comment);

            foreach (var file in files)
            {
                string dir = Path.GetDirectoryName(file);
                string name = Path.GetFileName(file);
                string newName = Path.Combine(dir, name.Replace("A-0000-0000-01-A", part));
                if (!File.Exists(newName)) File.Move(file, newName);
                if (File.Exists(newName)) File.Delete(file);
            }

            if (Directory.Exists(cirPath))
            {
                Directory.Delete(masterPath, true);
            }

            if (!Directory.Exists(cirPath))
            {
                Directory.Move(masterPath, cirPath);
                MessageBox.Show("Project created successfully");
                RequestClose?.Invoke();
            }
            else
            {
                MessageBox.Show("A project already exists with this project name");
            }

            string sdmFile = fullPath + "\\" + part + ".sdm";
            string cirName = part + ".cir";
            string newText = "TOPCIRCUIT:" + cirName;

            if (ExtractLine(sdmFile, 2).Contains("A-0000-0000-01-A.cir"))
            {
                lineChanger(newText, sdmFile, 2);
            }
        }

        private void lineChanger(string newText, string fileName, int lineToEdit)
        {
            string[] lines = File.ReadAllLines(fileName);
            lines[lineToEdit - 1] = newText;
            File.WriteAllLines(fileName, lines);
        }

        private string ExtractLine(string fileName, int line)
        {
            return File.ReadAllLines(fileName)[line - 1];
        }

        private void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            foreach (var file in Directory.GetFiles(sourceFolder))
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest, true);
            }

            foreach (var dir in Directory.GetDirectories(sourceFolder))
            {
                string name = Path.GetFileName(dir);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(dir, dest);
            }
        }

        private void clearPlaceHolder(string folderName)
        {
            foreach (var file in Directory.EnumerateFiles(folderName, "_Placeholder.txt", SearchOption.AllDirectories))
            {
                File.Delete(file);
            }
        }

        private string GetZipTemplate()
        {
            return Path.Combine(Path.GetDirectoryName("\\renishaw.com\\global\\GB\\PLC\\GE\\Data\\Electronics\\Documents\\EQs_from_PCB_supplier\\EQ System Progs\\PCB Naigator\\"), "A-0000-0000-01-A.zip");
        }

        private void GetComment(string path, string comment)
        {
            string comfile = "\\resource\\data\\comments.rsc";
            string fullPath = path + comfile;
            FileInfo fi = new FileInfo(fullPath);

            try
            {
                if (fi.Exists) fi.Delete();
                using FileStream fs = fi.Create();
                byte[] txt = new UTF8Encoding(true).GetBytes(comment);
                fs.Write(txt, 0, txt.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
