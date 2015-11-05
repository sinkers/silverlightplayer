using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightMediaPlayer
{
    public class ControlHelper
    {

        public static void CheckToggleButton(ToggleButton control, bool check)
        {
            if (control != null)
            {
                control.IsChecked = new bool?(check);
            }
        }

        public static void EnableControl(Control control, bool enabled)
        {
            if (control != null)
            {
                control.IsEnabled = enabled;
            }
        }

        public static void RaiseEvent(RoutedEventHandler handler, object sender)
        {
            if (handler != null)
            {
                handler(sender, new RoutedEventArgs());
            }
        }

        public static void RaiseEvent(RoutedEventHandler handler, object sender, RoutedEventArgs args)
        {
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        public static void RaiseEvent(DependencyPropertyChangedEventHandler handler, object sender, DependencyPropertyChangedEventArgs args)
        {
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        public static void SetSliderValue(RangeBase control, double value)
        {
            if (control != null)
            {
                control.Value = value;
            }
        }

        public static void SetTextBlockText(TextBlock control, string text)
        {
            if (control != null)
            {
                control.Text = text;
            }
        }

        public static void SetTextBlockTime(TextBlock control, TimeSpan time)
        {
            if (control != null)
            {
                CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                object[] hours = new object[] { time.Hours, time.Minutes, time.Seconds };
                control.Text = string.Format(invariantCulture, "{0:00}:{1:00}:{2:00}", hours);
            }
        }
    }
}
