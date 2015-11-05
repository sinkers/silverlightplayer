
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SilverlightMediaPlayer
{
    public class BoolToVisibilityConverter:IValueConverter
    {
        public BoolToVisibilityConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null || !(value is bool) || !(bool)value ? Visibility.Collapsed : Visibility.Visible);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null || !(value is Visibility) ? false : (Visibility)value == Visibility.Visible);
        }
    }
}
