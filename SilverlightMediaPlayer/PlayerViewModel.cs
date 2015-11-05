using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Web.Media.SmoothStreaming;

namespace SilverlightMediaPlayer
{
    public class PlayerViewModel : INotifyPropertyChanged
    {

        private SmoothStreamingMediaElement _element;

        private TimeSpan _position;

        private double bufferProgressValue;

        private string _positionText;

        private string _durationText;

        private ulong bitRatesCurrent;

        private IEnumerable<ulong> avlBitRates;

        public IEnumerable<ulong> AvlBitRates
        {
            get
            {
                return this.avlBitRates;
            }
            set
            {
                this.avlBitRates = value;
                this.RaisePropertyChanged("AvlBitRates");
            }
        }

        public ulong BitRatesCurrent
        {
            get
            {
                return this.bitRatesCurrent;
            }
            set
            {
                this.bitRatesCurrent = value;
                this.RaisePropertyChanged("BitRatesCurrent");
            }
        }

        public double BufferProgressValue
        {
            get
            {
                return this.bufferProgressValue;
            }
            set
            {
                this.bufferProgressValue = value;
                this.RaisePropertyChanged("BufferProgressValue");
            }
        }

        public string DurationText
        {
            get
            {
                return this._durationText;
            }
            set
            {
                this._durationText = value;
                this.RaisePropertyChanged("DurationText");
            }
        }

        public TimeSpan Position
        {
            get
            {
                return this._position;
            }
            set
            {
                this._position = value;
                if (this._position != value)
                {
                    this.RaisePropertyChanged("Position");
                }
                this.PositionChanged(this, EventArgs.Empty);
            }
        }

        public string PositionText
        {
            get
            {
                return this._positionText;
            }
            set
            {
                this._positionText = value;
                this.RaisePropertyChanged("PositionText");
            }
        }

        public PlayerViewModel(SmoothStreamingMediaElement element)
        {
            this._element = element;
            this.PositionChanged += new EventHandler((object s, EventArgs e) => this.UpdatePositionInfo());
            this.BufferChanged += new EventHandler((object s, EventArgs e) => this.UpdateBufferInfo());
        }

        private void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateBufferInfo()
        {
        }

        public void UpdateDurationInfo()
        {
            TimeSpan timeSpan = this._element.NaturalDuration.TimeSpan;
            this.DurationText = timeSpan.ToString("mm\\:ss");
        }

        public void UpdatePositionInfo()
        {
            this.PositionText = this.Position.ToString("mm\\:ss");
        }

        public event EventHandler BufferChanged;

        public event EventHandler PositionChanged;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
