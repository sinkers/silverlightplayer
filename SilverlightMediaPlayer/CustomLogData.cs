﻿using System;
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
    public class CustomLogData
    {
        public string Message
        {
            get;
            set;
        }

        public SeverityEnum Severity
        {
            get;
            set;
        }

        public string Source
        {
            get;
            set;
        }

        public CustomLogData()
        {
        }
    }
}