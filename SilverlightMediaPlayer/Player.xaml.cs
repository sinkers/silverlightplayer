using Microsoft.Web.Media.SmoothStreaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SilverlightMediaPlayer
{
    public partial class Player : UserControl
    {
        public Player()
        {
            this.InitializeComponent();

            PlayerViewModel viewMD = new PlayerViewModel(this.SmoothPlayer);
            this._viewModel = viewMD;
            base.DataContext = viewMD;
            this.SmoothPlayer.MediaOpened += new RoutedEventHandler((object s, RoutedEventArgs e) => this._viewModel.UpdateDurationInfo());
            this._viewModel.PositionChanged += new EventHandler((object s, EventArgs e) => this.UpdatePlayBar());
            this.SmoothPlayer.DownloadProgressChanged += new RoutedEventHandler((object s, RoutedEventArgs e) => this.UpdateBufferBar());
            HtmlPage.RegisterScriptableObject("Page", this);
            this.slider.BufferWidth = 0;
            Application.Current.Host.Content.FullScreenChanged += new EventHandler(this.Content_FullScreenChanged);
            this.slider.MouseDownEvent += new EventHandler(this.slider_MouseDownEvent);
            this.SmoothPlayer.MediaEnded += new RoutedEventHandler(this.SmoothPlayer_MediaEnded);
            this.SmoothPlayer.SizeChanged += new SizeChangedEventHandler(this.SmoothPlayer_SizeChanged);
            this.dispatcherTimer = new DispatcherTimer();
            this.dispatcherTimer.Tick += new EventHandler(this.dispatcherTimer_Tick);
            this.dispatcherTimer.Interval = new TimeSpan(0, 0, 2);

            // this.SmoothPlayer.SmoothStreamingSource = new Uri("http://wams.edgesuite.net/media/SintelTrailer_Smooth_from_WAME_720p_Main_Profile_CENC/CENC/sintel_trailer-720p.ism/manifest(format=mpd-time-csf)");

        }

        #region Properties
        private const double NaturalPlaySpeed = 1;

        private readonly PlayerViewModel _viewModel;

        private DispatcherTimer dispatcherTimer;

        private CustomLogData ologData;

        public string PassedInURL { get; set; }

        public bool IsinFullScreen
        {
            get;
            set;
        }

        public bool IsRewinding
        {
            get
            {
                double? playbackRate = this.SmoothPlayer.PlaybackRate;
                return ((double)playbackRate.GetValueOrDefault() >= 0 ? false : playbackRate.HasValue);
            }
        }

        public bool IsSlowMotion
        {
            get;
            set;
        }

        public IList<double> RewindPlaybackRates
        {
            get
            {
                IList<double> list = (
                    from i in this.SmoothPlayer.SupportedPlaybackRates
                    where i < 0
                    orderby i descending
                    select i).ToList<double>();
                return list;
            }
        }

        public IList<double> SlowMotionPlaybackRates
        {
            get
            {
                IList<double> list = (
                    from i in this.SmoothPlayer.SupportedPlaybackRates
                    where (i <= 0 ? false : i < 1)
                    orderby i
                    select i).ToList<double>();
                return list;
            }
        }

        public bool SupportsNaturalRewind
        {
            get
            {
                return this.RewindPlaybackRates.Any<double>();
            }
        }

        public bool SupportsSlowMotion
        {
            get
            {
                return this.SlowMotionPlaybackRates.Any<double>();
            }
        }
        #endregion

        #region Methods

        private void ConfigureToPlay()
        {
            switch (this.SmoothPlayer.CurrentState)
            {
                case SmoothStreamingMediaElementState.Playing:
                    {
                        this.SmoothPlayer.Pause();
                        this.playButton.Visibility = Visibility.Collapsed;
                        this.pauseButton.Visibility = Visibility.Visible;
                        break;
                    }
                case SmoothStreamingMediaElementState.Paused:
                    {
                        IList<double> supportedPlaybackRates = this.SmoothPlayer.SupportedPlaybackRates;
                        this.SmoothPlayer.SetPlaybackRate(supportedPlaybackRates[3]);

                        this.SmoothPlayer.Play();
                        this.playButton.Visibility = Visibility.Collapsed;
                        this.pauseButton.Visibility = Visibility.Visible;
                        break;
                    }
                case SmoothStreamingMediaElementState.Stopped:
                    {
                        this.SmoothPlayer.Play();
                        this.playButton.Visibility = Visibility.Collapsed;
                        this.pauseButton.Visibility = Visibility.Visible;
                        break;
                    }
            }
        }

        private void UpdateBufferBar()
        {
            this.slider.BufferWidth = this.SmoothPlayer.DownloadProgress * this.slider.ActualWidth;
        }

        private void UpdatePlayBar()
        {
            if (this.SmoothPlayer.NaturalDuration.TimeSpan != TimeSpan.Zero)
            {
                this.slider.Maximum = this.slider.ActualWidth;
                CustumSlider totalMilliseconds = this.slider;
                double num = this.SmoothPlayer.Position.TotalMilliseconds;
                TimeSpan timeSpan = this.SmoothPlayer.NaturalDuration.TimeSpan;
                totalMilliseconds.Value = num / timeSpan.TotalMilliseconds * this.slider.ActualWidth;
            }
        }

        private void Seek()
        {
            double value = this.slider.Value / (this.slider.Maximum - this.slider.Minimum) * 100;
            TimeSpan timeSpan = this.SmoothPlayer.NaturalDuration.TimeSpan;
            int totalSeconds = (int)(timeSpan.TotalSeconds * value / 100);
            TimeSpan timeSpan1 = new TimeSpan(0, 0, totalSeconds);
            this.SmoothPlayer.Position = timeSpan1;
            this.ologData = new CustomLogData()
            {
                Message = "Seek to new position.",
                Severity = SeverityEnum.Information,
                Source = "Player"
            };
            this.eventLogViewer.PrintLog(this.ologData);
        }
     

        [ScriptableMember]
        public void SetURL(string url)
        {
            if (url.Length > 0)
            {
                this.SmoothPlayer.SmoothStreamingSource = new Uri(url);
                PassedInURL = url;
                this.SmoothPlayer.AutoPlay = true;
                this.playButton.Visibility = Visibility.Collapsed;
                this.pauseButton.Visibility = Visibility.Visible;
            }
        }

        public void SetVideoBitrateRange(long minimumVideoBitrate, long maximumVideoBitrate, bool flushBuffer = true)
        {
            if (this.SmoothPlayer.ManifestInfo.Segments.First<SegmentInfo>() != null)
            {
                StreamInfo streamInfo = (
                    from i in this.SmoothPlayer.ManifestInfo.Segments.First<SegmentInfo>().SelectedStreams
                    where i.Type == MediaStreamType.Video
                    select i).Cast<StreamInfo>().FirstOrDefault<StreamInfo>();
                if (streamInfo != null)
                {
                    List<TrackInfo> list = (
                        from i in streamInfo.AvailableTracks
                        where (i.Bitrate < (ulong)minimumVideoBitrate ? false : i.Bitrate <= (ulong)maximumVideoBitrate)
                        select i).ToList<TrackInfo>();
                    if (list.Any<TrackInfo>())
                    {
                        streamInfo.SelectTracks(list, flushBuffer);
                    }
                }
            }
        }

        #endregion

        #region Player Control events

        private void Content_FullScreenChanged(object sender, EventArgs e)
        {
            if (!Application.Current.Host.Content.IsFullScreen)
            {
                this.IsinFullScreen = false;
                this.ControllerContainer.Visibility = Visibility.Visible;
            }
            else
            {
                this.IsinFullScreen = true;
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (this.IsinFullScreen)
            {
                this.ControllerContainer.Visibility = Visibility.Collapsed;
            }
            if (this.dispatcherTimer.IsEnabled)
            {
                this.dispatcherTimer.Stop();
            }
        }

        private void FastForwardElement_Click(object sender, RoutedEventArgs e)
        {
            if ((this.SmoothPlayer.CurrentState.Equals(SmoothStreamingMediaElementState.Paused) || this.SmoothPlayer.CurrentState.Equals(SmoothStreamingMediaElementState.Stopped) ? false : !this.SmoothPlayer.CurrentState.Equals(SmoothStreamingMediaElementState.Closed)))
            {
                IList<double> supportedPlaybackRates = this.SmoothPlayer.SupportedPlaybackRates;
                int num = supportedPlaybackRates.IndexOf(this.SmoothPlayer.PlaybackRate.Value);
                if (num < 4)
                {
                    this.SmoothPlayer.SetPlaybackRate(supportedPlaybackRates[4]);
                    this.ologData = new CustomLogData()
                    {
                        Message = "Fastforward enabled.",
                        Severity = SeverityEnum.Information,
                        Source = "Player"
                    };
                    this.eventLogViewer.PrintLog(this.ologData);
                }
                else if (num == 4)
                {
                    this.SmoothPlayer.SetPlaybackRate(supportedPlaybackRates[5]);
                    this.ologData = new CustomLogData()
                    {
                        Message = "Fastforward enabled.",
                        Severity = SeverityEnum.Information,
                        Source = "Player"
                    };
                    this.eventLogViewer.PrintLog(this.ologData);
                }
            }
        }

        private void FullScreenToggleElement_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsinFullScreen)
            {
                Application.Current.Host.Content.IsFullScreen = false;
                this.IsinFullScreen = false;
                this.ControllerContainer.Visibility = Visibility.Visible;
                this.ologData = new CustomLogData()
                {
                    Message = "Player changed to normal screen.",
                    Severity = SeverityEnum.Information,
                    Source = "Player"
                };
                this.eventLogViewer.PrintLog(this.ologData);
            }
            else
            {
                Application.Current.Host.Content.IsFullScreen = true;
                this.IsinFullScreen = true;
                this.ControllerContainer.Visibility = Visibility.Collapsed;
                this.ologData = new CustomLogData()
                {
                    Message = "Player changed to fullscreen.",
                    Severity = SeverityEnum.Information,
                    Source = "Player"
                };
                this.eventLogViewer.PrintLog(this.ologData);
            }
        }

        private void GraphToggleElement_Checked(object sender, RoutedEventArgs e)
        {
            if (this.bitRateGraph.Visibility != Visibility.Collapsed)
            {
                this.bitRateGraph.Visibility = Visibility.Collapsed;
                this.framerateControl.Visibility = Visibility.Collapsed;
                this.maxFramerateControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.bitRateGraph.Visibility = Visibility.Visible;
                this.framerateControl.Visibility = Visibility.Visible;
                this.maxFramerateControl.Visibility = Visibility.Visible;
                IEnumerable<ulong> nums = (
                    from i in this.SmoothPlayer.ManifestInfo.Segments.First<SegmentInfo>().AvailableStreams
                    where i.Type == MediaStreamType.Video
                    select i).SelectMany<StreamInfo, TrackInfo>((StreamInfo i) => i.AvailableTracks).Select<TrackInfo, ulong>((TrackInfo i) => i.Bitrate).Distinct<ulong>();
                this._viewModel.AvlBitRates = nums;
                this.bitRateGraph.MaximumPlayableBitrate = nums.Max<ulong>();
                this.bitRateGraph.IfNotNull<BitrateGraphControl>((BitrateGraphControl i) => i.StartRecording());
                this.framerateControl.IfNotNull<FramerateGraphControl>((FramerateGraphControl i) => i.StartRecording());
            }
        }

        private void GraphToggleElement_Unchecked(object sender, RoutedEventArgs e)
        {
            this.bitRateGraph.Visibility = Visibility.Collapsed;
            this.framerateControl.Visibility = Visibility.Collapsed;
            this.maxFramerateControl.Visibility = Visibility.Collapsed;
        }

        private void LogToggleElement_Checked(object sender, RoutedEventArgs e)
        {
            this.eventLogViewer.Visibility = Visibility.Visible;
        }

        private void LogToggleElement_Unchecked(object sender, RoutedEventArgs e)
        {
            this.eventLogViewer.Visibility = Visibility.Collapsed;
        }

        private void MaxBitrateLimiterElement_RecommendMaximumBitrate(object sender, CustomEventArgs<long> args)
        {
            if (args.Value > (long)0)
            {
                this.SetVideoBitrateRange((long)0, args.Value, false);
            }
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            this.playButton.Visibility = Visibility.Visible;
            this.pauseButton.Visibility = Visibility.Collapsed;
            this.SmoothPlayer.Pause();
            this.ologData = new CustomLogData()
            {
                Message = "Puased.",
                Severity = SeverityEnum.Information,
                Source = "Player"
            };
            this.eventLogViewer.PrintLog(this.ologData);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            this.ConfigureToPlay();
        }

        private void ReplayElement_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, 0);
            this.SmoothPlayer.Position = timeSpan;
            this.SmoothPlayer.SetPlaybackRate(1);
            this.SmoothPlayer.Play();
            this.ologData = new CustomLogData()
            {
                Message = "Replay enabled.",
                Severity = SeverityEnum.Information,
                Source = "Player"
            };
            this.eventLogViewer.PrintLog(this.ologData);
        }

        private void RewindElement_Click(object sender, RoutedEventArgs e)
        {
            if (!this.IsRewinding)
            {
                if (this.SupportsNaturalRewind)
                {
                    this.SmoothPlayer.SetPlaybackRate(this.RewindPlaybackRates.First<double>());
                    this.ologData = new CustomLogData()
                    {
                        Message = "Rewinding enabled.",
                        Severity = SeverityEnum.Information,
                        Source = "Player"
                    };
                    this.eventLogViewer.PrintLog(this.ologData);
                }
            }
        }

        private void slider_MouseDownEvent(object sender, EventArgs e)
        {
            this.Seek();
        }

        private void slider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void SlowMotionElement_Click(object sender, RoutedEventArgs e)
        {
            if (!(!this.SupportsSlowMotion ? true : this.IsSlowMotion))
            {
                this.SmoothPlayer.SetPlaybackRate(this.SlowMotionPlaybackRates.First<double>());
                this.IsSlowMotion = true;
                this.ologData = new CustomLogData()
                {
                    Message = "Slowmotion enabled.",
                    Severity = SeverityEnum.Information,
                    Source = "Player"
                };
                this.eventLogViewer.PrintLog(this.ologData);
            }
            else if (this.IsSlowMotion)
            {
                IList<double> supportedPlaybackRates = this.SmoothPlayer.SupportedPlaybackRates;
                this.SmoothPlayer.SetPlaybackRate(supportedPlaybackRates[3]);
                this.IsSlowMotion = false;
                this.ologData = new CustomLogData()
                {
                    Message = "Normal speed enabled.",
                    Severity = SeverityEnum.Information,
                    Source = "Player"
                };
                this.eventLogViewer.PrintLog(this.ologData);
            }
        }

        private void SourceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string content = ((sender as ComboBox).SelectedItem as ListBoxItem).Content as string;
            this.SmoothPlayer.BufferingTime = new TimeSpan(0, 0, 5);
            string str = content;
            if (str != null)
            {
                if (str == "Big Buck Bunny")
                {
                    this.SmoothPlayer.SmoothStreamingSource = new Uri("http://streams.smooth.vertigo.com/elephantsdream/Elephants_Dream_1024-h264-st-aac.ism/manifest");
                }
                else if (str == "Media Two")
                {
                    this.SmoothPlayer.SmoothStreamingSource = new Uri("http://wams.edgesuite.net/media/SintelTrailer_Smooth_from_WAME_720p_Main_Profile_CENC/CENC/sintel_trailer-720p.ism/manifest(format=mpd-time-csf)");
                }
                else if (str != "Media Three")
                {
                    if (str == "Media Four")
                    {
                    }
                }
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            this.SmoothPlayer.Stop();
        }

        #endregion

        #region Player Events

        private void SmoothPlayer_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            switch (this.SmoothPlayer.CurrentState)
            {
                case SmoothStreamingMediaElementState.Buffering:
                    {
                        this.ologData = new CustomLogData()
                        {
                            Message = "Buffering .",
                            Severity = SeverityEnum.Information,
                            Source = "Player"
                        };
                        this.eventLogViewer.PrintLog(this.ologData);
                        break;
                    }
                case SmoothStreamingMediaElementState.Paused:
                    {
                        break;
                    }
                case SmoothStreamingMediaElementState.Stopped:
                    {
                        break;
                    }
            }
        }

        private void SmoothPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            this.ologData = new CustomLogData()
            {
                Message = "Player loaded.",
                Severity = SeverityEnum.Information,
                Source = "Player"
            };
            this.eventLogViewer.PrintLog(this.ologData);
        }

        private void SmoothPlayer_ManifestReady(object sender, EventArgs e)
        {
            this.slider.Maximum = this.SmoothPlayer.EndPosition.TotalMilliseconds;
            this.ologData = new CustomLogData()
            {
                Message = "Manifest Ready.",
                Severity = SeverityEnum.Information,
                Source = "Player"
            };
            this.eventLogViewer.PrintLog(this.ologData);
        }

        private void SmoothPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.playButton.Visibility = Visibility.Visible;
            this.pauseButton.Visibility = Visibility.Collapsed;
            this.ologData = new CustomLogData()
            {
                Message = "Media ended.",
                Severity = SeverityEnum.Information,
                Source = "Player"
            };
            this.SmoothPlayer.SmoothStreamingSource = new Uri(PassedInURL);
            this.eventLogViewer.PrintLog(this.ologData);
        }

        private void SmoothPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            this.ologData = new CustomLogData()
            {
                Message = "Media failed.",
                Severity = SeverityEnum.Critical,
                Source = "Player"
            };
            this.eventLogViewer.PrintLog(this.ologData);
        }

        private void SmoothPlayer_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsinFullScreen)
            {
                this.ControllerContainer.Visibility = Visibility.Visible;
                this.dispatcherTimer.Start();
            }
        }

        private void SmoothPlayer_PlaybackTrackChanged(object sender, TrackChangedEventArgs e)
        {
            this.bitRateGraph.CurrentBitrate = e.NewTrack.Bitrate;
            this.framerateControl.CurrentFramerate = this.SmoothPlayer.RenderedFramesPerSecond;
            this.ologData = new CustomLogData()
            {
                Message = string.Concat("Playback bitrate changed to ", this.bitRateGraph.CurrentBitrate),
                Severity = SeverityEnum.Information,
                Source = "Player"
            };
            this.eventLogViewer.PrintLog(this.ologData);
        }

        private void SmoothPlayer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateBufferBar();
            this.ologData = new CustomLogData()
            {
                Message = "Player size changed.",
                Severity = SeverityEnum.Information,
                Source = "Player"
            };
            this.eventLogViewer.PrintLog(this.ologData);
        }

        private void SmoothPlayer_VideoHighestPlayableTrackChanged(object sender, TrackChangedEventArgs e)
        {
            this.ologData = new CustomLogData()
            {
                Message = "Video Highest PlayableTrackChanged.",
                Severity = SeverityEnum.Information,
                Source = "Player"
            };
            this.eventLogViewer.PrintLog(this.ologData);
        }

        #endregion

    }
}
