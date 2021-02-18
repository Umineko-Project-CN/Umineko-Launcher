using System;
using System.Windows;
using System.Windows.Controls;

namespace UminekoLauncher.Dialogs
{
    /// <summary>
    /// ConfigPopup.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigPopup : UserControl
    {
        public ConfigPopup()
        {
            InitializeComponent();
            Visibility = Visibility.Collapsed;
        }
        public void LoadConfig(string path)
        {
            GameConfig.LoadConfig(path);
            // 我太菜了，不会写 MVVM，有大神来帮忙改改嘛

            #region 分辨率
            cmbDisplayResolution.SelectedIndex = Convert.ToInt32(GameConfig.DisplayResolution);
            #endregion

            #region 显示模式
            cmbDisplayMode.SelectedIndex = Convert.ToInt32(GameConfig.DisplayMode);
            #endregion

            #region 缩放全屏
            cmbScale.SelectedIndex = Convert.ToInt32(!GameConfig.IsScaleEnabled);
            #endregion

            #region 片头曲版本
            cmbLegacyOp.SelectedIndex = Convert.ToInt32(GameConfig.IsLegacyOpEnabled);
            #endregion
        }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {

            #region 分辨率
            GameConfig.DisplayResolution = (DisplayResolution)cmbDisplayResolution.SelectedIndex;
            #endregion

            #region 显示模式
            GameConfig.DisplayMode = (DisplayMode)cmbDisplayMode.SelectedIndex;
            #endregion

            #region 缩放全屏
            GameConfig.IsScaleEnabled = !Convert.ToBoolean(cmbScale.SelectedIndex);
            #endregion

            #region 片头曲版本
            GameConfig.IsLegacyOpEnabled = Convert.ToBoolean(cmbLegacyOp.SelectedIndex);
            #endregion

            Visibility = Visibility.Collapsed;
        }
    }
}
