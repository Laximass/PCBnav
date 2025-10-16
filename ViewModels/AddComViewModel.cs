using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace PCB_Nav3.ViewModels
{
    public class AddComViewModel : INotifyPropertyChanged
    {
        private string _partNumber;
        private string _comment;
        private string _text3;
        private string _comfile;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action RequestClose;

        public string PartNumber
        {
            get => _partNumber;
            set
            {
                _partNumber = value;
                OnPropertyChanged(nameof(PartNumber));
            }
        }

        public string Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged(nameof(Comment));
            }
        }

        public ICommand SaveCommentCommand { get; }
        public ICommand CancelCommand { get; }

        public AddComViewModel(string selecTree, string job)
        {
            SaveCommentCommand = new RelayCommand(SaveComment);
            CancelCommand = new RelayCommand(() => RequestClose?.Invoke());

            Initialize(selecTree, job);
        }

        private void Initialize(string selecTree, string job)
        {
            PartNumber = selecTree;
            _text3 = job;

            if (_text3 != null)
            {
                if (job.Length < 17)
                {
                    string s1 = Regex.Replace(selecTree, "[^0-9-]", "");
                    s1 = s1.Length >= 4 ? s1.Remove(4) : s1;
                    PartNumber = s1;
                    _comfile = "\\comments.rsc";
                }
                else
                {
                    _comfile = "\\resource\\data\\comments.rsc";
                }
            }
            else
            {
                string path = selecTree.Length >= 4 ? selecTree.Remove(4) : selecTree;
                _text3 = "C:\\Projects\\" + path;
                _comfile = "\\comments.rsc";
            }

            string fullPath = _text3 + _comfile;
            if (File.Exists(fullPath))
            {
                Comment = File.ReadAllText(fullPath);
            }
        }

        private void SaveComment()
        {
            string fullPath = _text3 + _comfile;
            FileInfo fi = new FileInfo(fullPath);

            try
            {
                if (fi.Exists)
                {
                    fi.Delete();
                }

                using (FileStream fs = fi.Create())
                {
                    byte[] txt = new UTF8Encoding(true).GetBytes(Comment);
                    fs.Write(txt, 0, txt.Length);
                }

                using (StreamReader sr = File.OpenText(fullPath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            RequestClose?.Invoke();
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
