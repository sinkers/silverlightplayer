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
using System.Windows.Threading;
using System.ComponentModel;

namespace SilverlightMediaPlayer
{
    public class BitrateGraphControl : Control
    {

        private const string BitrateGraphCanvasName = "BitrateGraphCanvas";

        private const short knBitrateGraphItemHeight = 25;

        private const double knBitrateGraphLineMaxX = 500;

        private const double knBitrateGraphLineMinX = 60;

        private const double knBitrateGraphLineOffsetFromLabel = 10;

        private const double knBitrateLabelMinX = 5;

        private const double knBitrateLabelMinY = 5;

        private const double knGraphBottomMargin = 10;

        private const double knGraphRightMargin = 20;

        private readonly List<ulong> m_bitrateList = new List<ulong>();

        private BitrateGraphControl.BitrateGraph m_currentBitrateGraph = new BitrateGraphControl.BitrateGraph(new SolidColorBrush(Colors.Red));

        private BitrateGraphControl.BitrateGraph m_highestPlayableGraph = new BitrateGraphControl.BitrateGraph(new SolidColorBrush(Colors.Green));

        private readonly List<double> m_graphLineYValues = new List<double>();

        private readonly List<Line> m_lines = new List<Line>();

        private Canvas _bitrateGraphCanvas;

        private DispatcherTimer _updateTimer;

        public readonly static DependencyProperty AvailableBitratesProperty;

        public readonly static DependencyProperty CurrentBitrateProperty;

        public readonly static DependencyProperty MaximumPlayableBitrateProperty;

        public readonly static DependencyProperty ChartUpdateFrequencyProperty;

        private bool isLoaded;

        public IEnumerable<ulong> AvailableBitrates
        {
            get
            {
                return (IEnumerable<ulong>)base.GetValue(BitrateGraphControl.AvailableBitratesProperty);
            }
            set
            {
                base.SetValue(BitrateGraphControl.AvailableBitratesProperty, value);
            }
        }

        public TimeSpan ChartUpdateFrequency
        {
            get
            {
                return (TimeSpan)base.GetValue(BitrateGraphControl.ChartUpdateFrequencyProperty);
            }
            set
            {
                base.SetValue(BitrateGraphControl.ChartUpdateFrequencyProperty, value);
            }
        }

        public ulong CurrentBitrate
        {
            get
            {
                return (ulong)base.GetValue(BitrateGraphControl.CurrentBitrateProperty);
            }
            set
            {
                base.SetValue(BitrateGraphControl.CurrentBitrateProperty, value);
            }
        }

        public bool IsRecording
        {
            get
            {
                return this._updateTimer.IsEnabled;
            }
        }

        public ulong MaximumPlayableBitrate
        {
            get
            {
                return (ulong)base.GetValue(BitrateGraphControl.MaximumPlayableBitrateProperty);
            }
            set
            {
                base.SetValue(BitrateGraphControl.MaximumPlayableBitrateProperty, value);
            }
        }

        static BitrateGraphControl()
        {
            BitrateGraphControl.AvailableBitratesProperty = DependencyProperty.Register("AvailableBitrates", typeof(IEnumerable<ulong>), typeof(BitrateGraphControl), new PropertyMetadata(Enumerable.Empty<long>(), new PropertyChangedCallback(BitrateGraphControl.OnAvailableBitratesPropertyChanged)));
            BitrateGraphControl.CurrentBitrateProperty = DependencyProperty.Register("CurrentBitrate", typeof(ulong), typeof(BitrateGraphControl), new PropertyMetadata((object)((ulong)((long)0))));
            BitrateGraphControl.MaximumPlayableBitrateProperty = DependencyProperty.Register("MaximumPlayableBitrate", typeof(ulong), typeof(BitrateGraphControl), new PropertyMetadata((object)((ulong)((long)0))));
            BitrateGraphControl.ChartUpdateFrequencyProperty = DependencyProperty.Register("ChartUpdateFrequency", typeof(TimeSpan), typeof(BitrateGraphControl), new PropertyMetadata((object)TimeSpan.FromMilliseconds(70)));
        }

        public BitrateGraphControl()
        {
            base.DefaultStyleKey = typeof(BitrateGraphControl);
            this._updateTimer = new DispatcherTimer();
            this._updateTimer.Tick += new EventHandler(this.UpdateTimer_Tick);
        }

        private static int CompareDescending(ulong x, ulong y)
        {
            return y.CompareTo(x);
        }

        private double ComputeBitrateGraphValue(ulong bitrate)
        {
            double item;
            int num = 0;
            while (true)
            {
                if (!(num >= this.m_bitrateList.Count ? false : num < this.m_graphLineYValues.Count))
                {
                    item = 0;
                    break;
                }
                else if (bitrate != this.m_bitrateList[num])
                {
                    num++;
                }
                else
                {
                    item = this.m_graphLineYValues[num];
                    break;
                }
            }
            return item;
        }

        private Line CreateGraphLine(double X1, double Y1, double X2, double Y2, SolidColorBrush color)
        {
            Line line = new Line();
            line = new Line()
            {
                X1 = X1,
                Y1 = Y1,
                X2 = X2,
                Y2 = Y2,
                Stroke = color,
                StrokeThickness = 2.5,
                Tag = "GraphLine"
            };
            this.m_lines.Add(line);
            return line;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(520, 10 + (double)(this.m_bitrateList.Count * 25));
            return size;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._bitrateGraphCanvas = base.GetTemplateChild("BitrateGraphCanvas") as Canvas;
            this.isLoaded = true;
            if (this.AvailableBitrates.Any<ulong>())
            {
                this.OnAvailableBitratesChanged();
            }
        }

        private void OnAvailableBitratesChanged()
        {
            if (this.isLoaded)
            {
                ulong[] array = (
                    from i in this.AvailableBitrates
                    select i).ToArray<ulong>();
                this.UpdateBitrateList(array);
            }
        }

        private static void OnAvailableBitratesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BitrateGraphControl).IfNotNull<BitrateGraphControl>((BitrateGraphControl i) => i.OnAvailableBitratesChanged());
        }

        private void Repaint(ulong bitrate, ulong highestPlayableBitrate)
        {
            if (bitrate != (long)0)
            {
                this.RepaintGraph(this.m_currentBitrateGraph, bitrate);
                this.RepaintGraph(this.m_highestPlayableGraph, highestPlayableBitrate);
                for (int i = this.m_lines.Count - 1; i >= 0; i--)
                {
                    if (this.m_lines[i].Visibility != Visibility.Collapsed)
                    {
                        this._bitrateGraphCanvas.Children.Remove(this.m_lines[i]);
                        this._bitrateGraphCanvas.Children.Add(this.m_lines[i]);
                    }
                    else
                    {
                        Line item = this.m_lines[i];
                        this.m_lines.RemoveAt(i);
                    }
                }
            }
        }

        private void RepaintGraph(BitrateGraphControl.BitrateGraph graph, ulong bitrate)
        {
            if (graph.CurrentLine != null)
            {
                if (graph.LastBitrate != bitrate)
                {
                    Line line = this.CreateGraphLine(graph.CurrentLine.X2, this.ComputeBitrateGraphValue(bitrate), graph.CurrentLine.X2, this.ComputeBitrateGraphValue(bitrate), graph.Color);
                    this._bitrateGraphCanvas.Children.Add(line);
                    Line line1 = this.CreateGraphLine(graph.CurrentLine.X2, graph.CurrentLine.Y1, graph.CurrentLine.X2, line.Y1, graph.Color);
                    this._bitrateGraphCanvas.Children.Add(line1);
                    graph.CurrentLine = line;
                }
                else if (graph.CurrentLine.X2 >= 500)
                {
                    foreach (Line mLine in this.m_lines)
                    {
                        if ((mLine == null ? false : mLine.Visibility == Visibility.Visible))
                        {
                            if (mLine.X1 > 60)
                            {
                                Line x1 = mLine;
                                x1.X1 = x1.X1 - 1;
                            }
                            if (mLine.X2 <= 60)
                            {
                                mLine.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                Line x2 = mLine;
                                x2.X2 = x2.X2 - 1;
                            }
                        }
                    }
                    Line currentLine = graph.CurrentLine;
                    currentLine.X2 = currentLine.X2 + 1;
                }
                else
                {
                    Line currentLine1 = graph.CurrentLine;
                    currentLine1.X2 = currentLine1.X2 + 1;
                }
                graph.LastBitrate = bitrate;
            }
            else
            {
                graph.LastBitrate = bitrate;
                graph.CurrentLine = this.CreateGraphLine(60, this.ComputeBitrateGraphValue(bitrate), 60, this.ComputeBitrateGraphValue(bitrate), graph.Color);
                this._bitrateGraphCanvas.Children.Add(graph.CurrentLine);
            }
        }

        public void Reset()
        {
            this.m_lines.Clear();
            this._bitrateGraphCanvas.IfNotNull<Canvas>((Canvas i) => i.Children.Clear());
            this.m_currentBitrateGraph = new BitrateGraphControl.BitrateGraph(new SolidColorBrush(Colors.Red));
            this.m_highestPlayableGraph = new BitrateGraphControl.BitrateGraph(new SolidColorBrush(Colors.Green));
        }

        public void StartRecording()
        {
            this._updateTimer.Interval = this.ChartUpdateFrequency;
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this._updateTimer.Start();
            }
        }

        public void StopRecording()
        {
            this._updateTimer.Stop();
        }

        private void UpdateBitrateList(IEnumerable<ulong> bitrates)
        {
            double num;
            this.m_bitrateList.Clear();
            foreach (ulong bitrate in bitrates)
            {
                this.m_bitrateList.Add(bitrate);
            }
            this.m_bitrateList.Sort(new Comparison<ulong>(BitrateGraphControl.CompareDescending));
            double num1 = 5;
            foreach (int mBitrateList in this.m_bitrateList)
            {
                TextBlock textBlock = new TextBlock()
                {
                    Foreground = new SolidColorBrush(Colors.White)
                };
                if ((double)((double)mBitrateList / 1000) <= 1000)
                {
                    num = (double)((double)mBitrateList / 1000);
                    textBlock.Text = string.Concat(num.ToString(), "K");
                }
                else
                {
                    num = (double)((double)mBitrateList / 1000) / 1000;
                    textBlock.Text = string.Concat(num.ToString(), "M");
                }
                textBlock.SetValue(Canvas.LeftProperty, 5.0);
                textBlock.SetValue(Canvas.TopProperty, num1);
                textBlock.Tag = mBitrateList.ToString();
                this._bitrateGraphCanvas.Children.Add(textBlock);
                Line line = new Line()
                {
                    X1 = 60,
                    X2 = 500,
                    Y1 = num1 + 10,
                    Y2 = num1 + 10,
                    Tag = "Gray",
                    Stroke = new SolidColorBrush(Colors.LightGray),
                    StrokeThickness = 1.5
                };
                this.m_graphLineYValues.Add(num1 + 10);
                this._bitrateGraphCanvas.Children.Add(line);
                num1 = num1 + 25;
            }
            base.InvalidateMeasure();
        }

        private void UpdateGraphValue(ulong bitrate, ulong highestPlayableBitrate)
        {
            this.Repaint(bitrate, highestPlayableBitrate);
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            this.UpdateGraphValue(this.CurrentBitrate, this.MaximumPlayableBitrate);
        }

        private class BitrateGraph
        {
            public Line CurrentLine;

            public ulong LastBitrate;

            public SolidColorBrush Color;

            public BitrateGraph(SolidColorBrush color)
            {
                this.Color = color;
            }
        }
    }
}
