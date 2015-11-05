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
    public class CustumVolumSlider:Control
    {
        public readonly static DependencyProperty ValueProperty;

        public double Value
        {
            get
            {
                return (double)base.GetValue(CustumVolumSlider.ValueProperty);
            }
            set
            {
                base.SetValue(CustumVolumSlider.ValueProperty, value);
            }
        }

        static CustumVolumSlider()
        {
            CustumVolumSlider.ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(CustumSlider), new PropertyMetadata(null));
        }

        public CustumVolumSlider()
        {
            base.DefaultStyleKey = typeof(CustumVolumSlider);
        }
    }
}
