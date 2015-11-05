using System;
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
    [TemplatePart(Name = "rctBuffer", Type = typeof(Rectangle))]
    public class CustumSlider:Slider
    {
        private Rectangle rect;

        public readonly static DependencyProperty BufferWidthProperty;

        public double BufferWidth
        {
            get
            {
                return (double)base.GetValue(CustumSlider.BufferWidthProperty);
            }
            set
            {
                base.SetValue(CustumSlider.BufferWidthProperty, value);
            }
        }

        static CustumSlider()
        {
            CustumSlider.BufferWidthProperty = DependencyProperty.Register("BufferWidth", typeof(double), typeof(CustumSlider), new PropertyMetadata(null));
        }

        public CustumSlider()
        {
            base.DefaultStyleKey = typeof(CustumSlider);
        }

        private void b_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            double horizontalChange = e.HorizontalChange;
            double actualWidth = horizontalChange / this.rect.ActualWidth;
            this.MouseDownEvent(sender, new MyEVent()
            {
                percentage = horizontalChange
            });
        }

        public override void OnApplyTemplate()
        {
            Thumb templateChild = base.GetTemplateChild("HorizontalThumb") as Thumb;
            this.rect = base.GetTemplateChild("rctActualBar") as Rectangle;
            templateChild.DragCompleted += new DragCompletedEventHandler(this.b_DragCompleted);
            base.OnApplyTemplate();
        }

        private void rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        public event EventHandler MouseDownEvent;
    }
}
