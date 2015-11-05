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
    public class FramerateGraphControl : Control
    {

        private const string FrameRateLabelName = "FrameRateLabel";

        private const string FrameRateGraphCanvasName = "FrameRateGraphCanvas";

        private const int knDesiredControlHeight = 100;

        private const int knDesiredControlWidth = 200;

        private const int knGraphLineMaxX = 190;

        private const int knGraphLineMaxY = 15;

        private const int knGraphLineMinX = 30;

        private const int knGraphLineMinY = 85;

        private const double knMaxFps = 60;

        private object _threadSync = new object();

        private DispatcherTimer _updateTimer;

        private TextBlock _frameRateLabel;

        private Canvas _frameRateGraphCanvas;

        private static SolidColorBrush kbrGraphLineBrush;

        private static double knGraphLineThickness;

        private static Size kszDesiredSize;

        private Line m_currentLine;

        private double m_lastFps = -1;

        public readonly static DependencyProperty ChartUpdateFrequencyProperty;

        public readonly static DependencyProperty CurrentFramerateProperty;

        public TimeSpan ChartUpdateFrequency
        {
            get
            {
                return (TimeSpan)base.GetValue(FramerateGraphControl.ChartUpdateFrequencyProperty);
            }
            set
            {
                base.SetValue(FramerateGraphControl.ChartUpdateFrequencyProperty, value);
            }
        }

        public double CurrentFramerate
        {
            get
            {
                return (double)base.GetValue(FramerateGraphControl.CurrentFramerateProperty);
            }
            set
            {
                base.SetValue(FramerateGraphControl.CurrentFramerateProperty, value);
            }
        }

        public bool IsRecording
        {
            get
            {
                return this._updateTimer.IsEnabled;
            }
        }

        static FramerateGraphControl()
        {
            FramerateGraphControl.kbrGraphLineBrush = new SolidColorBrush(Colors.Green);
            FramerateGraphControl.knGraphLineThickness = 2.5;
            FramerateGraphControl.kszDesiredSize = new Size(200, 100);
            FramerateGraphControl.ChartUpdateFrequencyProperty = DependencyProperty.Register("ChartUpdateFrequency", typeof(TimeSpan), typeof(FramerateGraphControl), new PropertyMetadata((object)TimeSpan.FromMilliseconds(70)));
            FramerateGraphControl.CurrentFramerateProperty = DependencyProperty.Register("CurrentFramerate", typeof(double), typeof(FramerateGraphControl),null);
        }

        public FramerateGraphControl()
        {
            base.DefaultStyleKey = typeof(FramerateGraphControl);
            this._updateTimer = new DispatcherTimer();
            this._updateTimer.Tick += new EventHandler(this.UpdateTimer_Tick);
        }

        private double ComputeFpsGraphValue(double fps)
        {
            if (fps > 60)
            {
                fps = 60;
            }
            if (fps < 0)
            {
                fps = 0;
            }
            return 85 - fps / 60 * 70;
        }

        private Line CreateGraphLine()
        {
            Line line = new Line()
            {
                Stroke = FramerateGraphControl.kbrGraphLineBrush,
                StrokeThickness = FramerateGraphControl.knGraphLineThickness
            };
            return line;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return FramerateGraphControl.kszDesiredSize;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._frameRateLabel = base.GetTemplateChild("FrameRateLabel") as TextBlock;
            this._frameRateGraphCanvas = base.GetTemplateChild("FrameRateGraphCanvas") as Canvas;
        }

        private void Repaint(double fps)
        {
            Line item;
            double num;
            fps = Math.Round(fps);
            this._frameRateLabel.Text = fps.ToString();
            if (this.m_currentLine != null)
            {
                if (this.m_lastFps != fps)
                {
                    Line line = this.CreateGraphLine();
                    double x2 = this.m_currentLine.X2;
                    num = x2;
                    line.X2 = x2;
                    line.X1 = num;
                    double num1 = this.ComputeFpsGraphValue(fps);
                    num = num1;
                    line.Y2 = num1;
                    line.Y1 = num;
                    this._frameRateGraphCanvas.Children.Add(line);
                    Line y1 = this.CreateGraphLine();
                    double x21 = this.m_currentLine.X2;
                    num = x21;
                    y1.X2 = x21;
                    y1.X1 = num;
                    y1.Y1 = this.m_currentLine.Y1;
                    y1.Y2 = line.Y1;
                    this._frameRateGraphCanvas.Children.Add(y1);
                    this.m_currentLine = line;
                }
                else if (this.m_currentLine.X2 >= 190)
                {
                    foreach (FrameworkElement child in this._frameRateGraphCanvas.Children)
                    {
                        item = child as Line;
                        if ((item == null ? false : item.Visibility == Visibility.Visible))
                        {
                            if (item.X1 > 30)
                            {
                                Line x1 = item;
                                x1.X1 = x1.X1 - 1;
                            }
                            if (item.X2 <= 30)
                            {
                                item.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                Line line1 = item;
                                line1.X2 = line1.X2 - 1;
                            }
                        }
                    }
                    Line mCurrentLine = this.m_currentLine;
                    mCurrentLine.X2 = mCurrentLine.X2 + 1;
                }
                else
                {
                    Line mCurrentLine1 = this.m_currentLine;
                    mCurrentLine1.X2 = mCurrentLine1.X2 + 1;
                }
                for (int i = 0; i < this._frameRateGraphCanvas.Children.Count; i++)
                {
                    item = this._frameRateGraphCanvas.Children[i] as Line;
                    if ((item == null ? false : item.Visibility == Visibility.Collapsed))
                    {
                        this._frameRateGraphCanvas.Children.Remove(item);
                    }
                }
                this.m_lastFps = fps;
            }
            else
            {
                this.m_lastFps = fps;
                this.m_currentLine = this.CreateGraphLine();
                Line mCurrentLine2 = this.m_currentLine;
                double num2 = 30;
                num = num2;
                this.m_currentLine.X2 = num2;
                mCurrentLine2.X1 = num;
                Line line2 = this.m_currentLine;
                Line mCurrentLine3 = this.m_currentLine;
                double num3 = this.ComputeFpsGraphValue(fps);
                num = num3;
                mCurrentLine3.Y2 = num3;
                line2.Y1 = num;
                this._frameRateGraphCanvas.Children.Add(this.m_currentLine);
            }
        }

        public void Reset()
        {
            lock (this._threadSync)
            {
                this.m_currentLine = null;
                if (this._frameRateGraphCanvas != null)
                {
                    List<UIElement> list = (
                        from i in this._frameRateGraphCanvas.Children
                        where i is Line
                        select i).ToList<UIElement>();
                    list.ForEach((UIElement i) => this._frameRateGraphCanvas.Children.Remove(i));
                }
            }
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

        private void UpdateGraphValue(double fps)
        {
            this.Repaint(fps);
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            lock (this._threadSync)
            {
                this.UpdateGraphValue(this.CurrentFramerate);
            }
        }
    }
}
