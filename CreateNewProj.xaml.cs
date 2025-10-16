using PCB_Nav3.Properties;
using PCB_Nav3.ViewModels;
using System.Windows;
using System.Windows.Media;

namespace PCB_Nav3
{
    /// <summary>
    /// Interaction logic for CreateNewProj.xaml
    /// </summary>
    public partial class CreateNewProj : Window
    {
        public CreateNewProj()
        {
            InitializeComponent();

            var viewModel = new CreateNewProjViewModel();
            viewModel.RequestClose += () => this.Hide();
            DataContext = viewModel;
        }

    }
}