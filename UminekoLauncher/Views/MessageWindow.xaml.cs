using System.Windows;

namespace UminekoLauncher.Views
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : AnimatedWindow
    {
        public MessageWindow(string message, bool isOkCancel = false)
        {
            InitializeComponent();
            Window mainWindow = Application.Current.MainWindow;
            if (mainWindow.IsActive)
            {
                Owner = mainWindow;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            if (isOkCancel)
            {
                btnCancel.Visibility = Visibility.Visible;
            }
            textMessage.Text = message;
        }

        /// <summary>
        /// 显示信息窗口。
        /// </summary>
        /// <param name="message">要显示的信息。</param>
        /// <param name="isOkCancel">为 <see cref="bool">true</see> 时，显示取消按钮。</param>
        /// <returns>返回用户选择结果。</returns>
        public static bool? Show(string message, bool isOkCancel = false)
            => new MessageWindow(message, isOkCancel).ShowDialog();

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}