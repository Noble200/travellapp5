using Avalonia.Controls;
using Allva.Desktop.ViewModels;

namespace Allva.Desktop.Views
{
    public partial class UpdateNotificationView : Window
    {
        public UpdateNotificationView()
        {
            InitializeComponent();
        }

        public UpdateNotificationView(UpdateNotificationViewModel viewModel) : this()
        {
            DataContext = viewModel;
            viewModel.CerrarDialogo = () => Close();
        }
    }
}