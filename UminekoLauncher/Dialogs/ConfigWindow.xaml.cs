using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UminekoLauncher.Dialogs
{
    /// <summary>
    /// ConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();

            // 我太菜了，不会写 MVVM，有大神来帮忙改改嘛

            #region 分辨率
            cmbDisplayResolution.SelectedIndex = Convert.ToInt32(GameConfig.DisplayResolution);
            #endregion

            #region 显示模式
            switch (GameConfig.DisplayMode)
            {
                case DisplayMode.Auto:
                    radAuto.IsChecked = true;
                    break;
                case DisplayMode.Window:
                    radWindow.IsChecked = true;
                    break;
                case DisplayMode.FullScreen:
                    radFullscreen.IsChecked = true;
                    break;
                default:
                    break;
            }
            #endregion

            #region 缩放全屏
            if (GameConfig.IsScaleEnabled)
            {
                radScaleOn.IsChecked = true;
            }
            else
            {
                radScaleOff.IsChecked = true;
            }
            #endregion

        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {

            #region 分辨率
            switch (cmbDisplayResolution.SelectedIndex)
            {
                case 0:
                    GameConfig.DisplayResolution = DisplayResolution.x1280;
                    break;
                case 1:
                    GameConfig.DisplayResolution = DisplayResolution.x1366;
                    break;
                case 2:
                    GameConfig.DisplayResolution = DisplayResolution.x1440;
                    break;
                case 3:
                    GameConfig.DisplayResolution = DisplayResolution.x1600;
                    break;
                case 4:
                    GameConfig.DisplayResolution = DisplayResolution.x1920;
                    break;
                case 5:
                    GameConfig.DisplayResolution = DisplayResolution.x2560;
                    break;
                default:
                    break;
            }
            #endregion

            #region 显示模式
            if (radAuto.IsChecked == true)
            {
                GameConfig.DisplayMode = DisplayMode.Auto;
            }
            else if (radWindow.IsChecked == true)
            {
                GameConfig.DisplayMode = DisplayMode.Window;
            }
            else if (radFullscreen.IsChecked == true)
            {
                GameConfig.DisplayMode = DisplayMode.FullScreen;
            }
            #endregion

            #region 缩放全屏
            GameConfig.IsScaleEnabled = (bool)radScaleOn.IsChecked; // 这个本来可以直接绑定一下，暂时还是就这样吧
            #endregion

            DialogResult = true;
        }
    }
}
