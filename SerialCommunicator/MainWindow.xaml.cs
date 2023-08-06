using SerialCommunicator.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TheRThemes;

namespace SerialCommunicator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool SettingsMenuShowing;
        private void AutoShowMenu(object sender, RoutedEventArgs e)
        {
            if (!SettingsMenuShowing)
            {
                AnimateSettingsMenu(0, 
                    GlobalSettings.SETTINGMENU_PANEL_WIDTH, 
                    TimeSpan.FromSeconds(GlobalSettings.SETTINGMENU_ANIMATION_TIME));
                SettingsMenuShowing = true;
            }
            else
            {
                AnimateSettingsMenu(
                    GlobalSettings.SETTINGMENU_PANEL_WIDTH, 0, 
                    TimeSpan.FromSeconds(GlobalSettings.SETTINGMENU_ANIMATION_TIME));
                SettingsMenuShowing = false;
            }
        }

        public void AnimateSettingsMenu(double from, double to, TimeSpan time)
        {
            DoubleAnimation da = new DoubleAnimation(from, to, time);
            da.AccelerationRatio = 0;
            da.DecelerationRatio = 1;

            SettingsMenu.BeginAnimation(WidthProperty, da);
        }

        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            switch (int.Parse(((MenuItem)sender).Uid))
            {
                case 0: ThemesController.SetTheme(ThemesController.ThemeTypes.Light); break;
                case 1: ThemesController.SetTheme(ThemesController.ThemeTypes.ColourfulLight); break;
                case 2: ThemesController.SetTheme(ThemesController.ThemeTypes.Dark); break;
                case 3: ThemesController.SetTheme(ThemesController.ThemeTypes.ColourfulDark); break;
            }
            e.Handled = true;
        }
    }
}
