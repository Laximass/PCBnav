using PCB_Nav3.Properties;
using System.Windows;
using System.Windows.Media;

namespace PCB_Nav3
{
    /// <summary>
    /// Interaction logic for UpdatelogWin.xaml
    /// </summary>
    public partial class UpdatelogWin : Window
    {
        public UpdatelogWin()
        {
            InitializeComponent();

            DataContext = new ViewModels.UpdatelogViewModel();
        }

    }
}
