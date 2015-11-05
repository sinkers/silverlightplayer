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
    public class CustomEventArgs<T> : EventArgs
    {
        private readonly T m_value;

        public T Value
        {
            get
            {
                return this.m_value;
            }
        }

        public CustomEventArgs(T value)
        {
            this.m_value = value;
        }
    }
}
