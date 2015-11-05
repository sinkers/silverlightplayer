using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightMediaPlayer
{
    public class MaxBitrateLimiterControl:Control
    {
        private const string LimitMaxBitrateSliderName = "LimitMaxBitrateSlider";

        private const string NowDownloadingSliderName = "NowDownloadingSlider";

        private const string NowDownloadingBitrateLabelName = "NowDownloadingBitrateLabel";

        private const string MaxBitrateLimiterLabelName = "MaxBitrateLimiterLabel";

        private Slider _limitMaxBitrateSlider;

        private Slider _nowDownloadingSlider;

        private TextBlock _nowDownloadingBitrateLabel;

        private TextBlock _maxBitrateLimiterLabel;

        public readonly static DependencyProperty AvailableBitratesProperty;

        public readonly static DependencyProperty DownloadBitrateProperty;

        public IEnumerable<ulong> AvailableBitrates
        {
            get
            {
                return (IEnumerable<ulong>)base.GetValue(MaxBitrateLimiterControl.AvailableBitratesProperty);
            }
            set
            {
                base.SetValue(MaxBitrateLimiterControl.AvailableBitratesProperty, value);
            }
        }

        public long DownloadBitrate
        {
            get
            {
                return (long)base.GetValue(MaxBitrateLimiterControl.DownloadBitrateProperty);
            }
            set
            {
                base.SetValue(MaxBitrateLimiterControl.DownloadBitrateProperty, value);
            }
        }

        static MaxBitrateLimiterControl()
        {
            MaxBitrateLimiterControl.AvailableBitratesProperty = DependencyProperty.Register("AvailableBitrates", typeof(IEnumerable<ulong>), typeof(MaxBitrateLimiterControl), new PropertyMetadata(Enumerable.Empty<ulong>(), new PropertyChangedCallback(MaxBitrateLimiterControl.OnAvailableBitratesPropertyChanged)));
            MaxBitrateLimiterControl.DownloadBitrateProperty = DependencyProperty.Register("DownloadBitrate", typeof(long), typeof(MaxBitrateLimiterControl), new PropertyMetadata((object)((long)0), new PropertyChangedCallback(MaxBitrateLimiterControl.OnDownloadBitratePropertyChanged)));
        }

        public MaxBitrateLimiterControl()
        {
            base.DefaultStyleKey = typeof(MaxBitrateLimiterControl);
        }

        private void LimitMaxBitrateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this._maxBitrateLimiterLabel != null)
            {
                ulong num = (ulong)Math.Round(e.NewValue);
                this._maxBitrateLimiterLabel.Text = num.ToString();
            }
            this.RecommendMaximumBitrate.IfNotNull<EventHandler<CustomEventArgs<long>>>((EventHandler<CustomEventArgs<long>> i) => i(this, new CustomEventArgs<long>((long)(e.NewValue * 1000))));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._nowDownloadingBitrateLabel = base.GetTemplateChild("NowDownloadingBitrateLabel") as TextBlock;
            this._nowDownloadingSlider = base.GetTemplateChild("NowDownloadingSlider") as Slider;
            this._maxBitrateLimiterLabel = base.GetTemplateChild("MaxBitrateLimiterLabel") as TextBlock;
            this._limitMaxBitrateSlider = base.GetTemplateChild("LimitMaxBitrateSlider") as Slider;
            this.OnAvailableBitratesChanged();
            this._limitMaxBitrateSlider.IfNotNull<Slider>((Slider i) => i.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.LimitMaxBitrateSlider_ValueChanged));
        }

        private void OnAvailableBitratesChanged()
        {
            if ((this.AvailableBitrates == null ? false : this.AvailableBitrates.Count<ulong>() > 0))
            {
                ulong num = (this.AvailableBitrates.Any<ulong>() ? this.AvailableBitrates.Min<ulong>() : (ulong)((long)0));
                ulong num1 = (this.AvailableBitrates.Any<ulong>() ? this.AvailableBitrates.Max<ulong>() : (ulong)((long)0));
                if (this._nowDownloadingSlider != null)
                {
                    this._nowDownloadingSlider.Minimum = 0;
                    this._nowDownloadingSlider.Maximum = (double)((float)num1) / 1000;
                    this._nowDownloadingSlider.Value = 0;
                }
                if (this._limitMaxBitrateSlider != null)
                {
                    this._limitMaxBitrateSlider.Minimum = Math.Ceiling((double)((float)num) / 1000);
                    this._limitMaxBitrateSlider.Maximum = Math.Ceiling((double)((float)num1) / 1000);
                    this._limitMaxBitrateSlider.Value = this._limitMaxBitrateSlider.Maximum;
                    this._maxBitrateLimiterLabel.Text = this._limitMaxBitrateSlider.Maximum.ToString();
                }
            }
        }

        private static void OnAvailableBitratesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MaxBitrateLimiterControl).IfNotNull<MaxBitrateLimiterControl>((MaxBitrateLimiterControl i) => i.OnAvailableBitratesChanged());
        }

        private void OnDownloadBitrateChanged()
        {
            long downloadBitrate = this.DownloadBitrate / (long)1000;
            this._nowDownloadingSlider.IfNotNull<Slider>((Slider i) => i.Value = (double)downloadBitrate);
            this._nowDownloadingBitrateLabel.IfNotNull<TextBlock>((TextBlock i) => i.Text = downloadBitrate.ToString());
        }

        private static void OnDownloadBitratePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MaxBitrateLimiterControl).IfNotNull<MaxBitrateLimiterControl>((MaxBitrateLimiterControl i) => i.OnDownloadBitrateChanged());
        }

        public event EventHandler<CustomEventArgs<long>> RecommendMaximumBitrate;
    }
}
