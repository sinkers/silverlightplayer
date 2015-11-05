using System;
using System.Linq;
using System.Text;
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
    public class EventsLog:Control
    {
        private Canvas _eventLogCanvas;

        private StringBuilder sb = new StringBuilder();

        public EventsLog()
        {
            base.DefaultStyleKey = typeof(EventsLog);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._eventLogCanvas = base.GetTemplateChild("EventLogContainer") as Canvas;
            this.PrintLog(null);
        }

        public void PrintLog(CustomLogData logData)
        {
            UIElementCollection children;
            string text;
            if (this._eventLogCanvas != null & logData != null)
            {
                this.sb = new StringBuilder();
            }
            if (logData != null)
            {
                this.sb.Append(Environment.NewLine);
                this.sb.Append(string.Concat("Message: ", logData.Message, ". "));
                this.sb.Append(string.Concat("Severity: ", logData.Severity.ToString(), ". "));
                this.sb.Append(string.Concat("Source: ", logData.Source, ". "));
                if (this._eventLogCanvas != null)
                {
                    children = this._eventLogCanvas.Children;
                    text = (children.First<UIElement>() as TextBlock).Text;
                    (children.First<UIElement>() as TextBlock).Text = string.Concat((children.First<UIElement>() as TextBlock).Text, this.sb.ToString());
                }
            }
            else if (this._eventLogCanvas != null)
            {
                children = this._eventLogCanvas.Children;
                text = (children.First<UIElement>() as TextBlock).Text;
                (children.First<UIElement>() as TextBlock).Text = string.Concat((children.First<UIElement>() as TextBlock).Text, this.sb.ToString());
            }
        }
    }
}
