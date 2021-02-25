using System.Windows;

namespace UminekoLauncher.Dialogs
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : Window
    {
        public MessageWindow(string message, Window owner = null, bool isOkCancel = false)
        {
            InitializeComponent();
            if (owner != null)
            {
                Owner = owner;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            if (isOkCancel)
            {
                btnCancel.Visibility = Visibility.Visible;
            }
            textMessage.Text = message;
        }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
