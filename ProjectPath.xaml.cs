
using System.Windows;
using System.Windows.Media;
using PCB_Nav3.Properties;

namespace PCB_Nav3
{
    /// <summary>
    /// Interaction logic for ProjectPath.xaml
    /// </summary>
    public partial class ProjectPath : Window
    {
        public ProjectPath()
        {
            InitializeComponent();

            var viewModel = new ProjectPathViewModel();
            viewModel.RequestClose += () => this.Hide();
            DataContext = viewModel;
        }

    }
}
