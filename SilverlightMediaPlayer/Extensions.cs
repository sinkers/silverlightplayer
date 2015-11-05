using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightMediaPlayer
{
    public static class Extensions
    {
        public static void IfNotNull<TItem>(this TItem item, Action<TItem> action)
        where TItem : class
        {
            if (item != null)
            {
                action(item);
            }
        }
    }
}
