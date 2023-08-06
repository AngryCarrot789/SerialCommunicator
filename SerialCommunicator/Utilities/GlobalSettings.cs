using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SerialCommunicator.Utilities
{
    public static class GlobalSettings
    {
        public const string DEFAULT_SERIAL_END = "\n";

        public const int DEFAULT_GRAPH_SPEED_MS = 50;

        public const int SETTINGMENU_PANEL_WIDTH = 204;
        public const double SETTINGMENU_ANIMATION_TIME = 0.2;

        public static SolidColorBrush NORM_BRUSH = new SolidColorBrush(Colors.Transparent) { Opacity = 1 };
        public static SolidColorBrush ALERT_BRUSH = new SolidColorBrush(Colors.Orange) { Opacity = 0.1 };
        public static SolidColorBrush ERR_BRUSH = new SolidColorBrush(Colors.Red) { Opacity = 0.1 };
    }
}
