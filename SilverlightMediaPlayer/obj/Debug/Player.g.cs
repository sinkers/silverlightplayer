﻿#pragma checksum "D:\Projects\NewSilverlightPlayer\SilverlightMediaPlayer\Player.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "656A0E4BE0DA13B0F955893C610ECC97"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Web.Media.SmoothStreaming;
using SilverlightMediaPlayer;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace SilverlightMediaPlayer {
    
    
    public partial class Player : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Web.Media.SmoothStreaming.SmoothStreamingMediaElement SmoothPlayer;
        
        internal SilverlightMediaPlayer.BitrateGraphControl bitRateGraph;
        
        internal SilverlightMediaPlayer.FramerateGraphControl framerateControl;
        
        internal SilverlightMediaPlayer.MaxBitrateLimiterControl maxFramerateControl;
        
        internal SilverlightMediaPlayer.EventsLog eventLogViewer;
        
        internal System.Windows.Controls.Grid ControllerContainer;
        
        internal System.Windows.Controls.Grid playercontrols;
        
        internal System.Windows.Controls.Button ReplayElement;
        
        internal System.Windows.Controls.Button RewindElement;
        
        internal System.Windows.Controls.Button playButton;
        
        internal System.Windows.Controls.Button pauseButton;
        
        internal System.Windows.Controls.Button FastForwardElement;
        
        internal System.Windows.Controls.Primitives.ToggleButton SlowMotionElement;
        
        internal System.Windows.Controls.Grid divider1;
        
        internal System.Windows.Controls.Grid timelinecontrols;
        
        internal SilverlightMediaPlayer.CustumSlider slider;
        
        internal System.Windows.Controls.Border TimeContainer;
        
        internal System.Windows.Controls.TextBlock CurrentPositionElement;
        
        internal System.Windows.Controls.TextBlock TimeSeparatorElement;
        
        internal System.Windows.Controls.TextBlock CurrentDurationElement;
        
        internal System.Windows.Controls.Slider volumeslider;
        
        internal System.Windows.Controls.Grid divider2;
        
        internal System.Windows.Controls.Grid functioncontrols;
        
        internal System.Windows.Controls.Primitives.ToggleButton GraphToggleElement;
        
        internal System.Windows.Controls.Primitives.ToggleButton LogToggleElement;
        
        internal System.Windows.Controls.Primitives.ToggleButton FullScreenToggleElement;
        
        internal System.Windows.Controls.Grid divider3;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/SilverlightMediaPlayer;component/Player.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.SmoothPlayer = ((Microsoft.Web.Media.SmoothStreaming.SmoothStreamingMediaElement)(this.FindName("SmoothPlayer")));
            this.bitRateGraph = ((SilverlightMediaPlayer.BitrateGraphControl)(this.FindName("bitRateGraph")));
            this.framerateControl = ((SilverlightMediaPlayer.FramerateGraphControl)(this.FindName("framerateControl")));
            this.maxFramerateControl = ((SilverlightMediaPlayer.MaxBitrateLimiterControl)(this.FindName("maxFramerateControl")));
            this.eventLogViewer = ((SilverlightMediaPlayer.EventsLog)(this.FindName("eventLogViewer")));
            this.ControllerContainer = ((System.Windows.Controls.Grid)(this.FindName("ControllerContainer")));
            this.playercontrols = ((System.Windows.Controls.Grid)(this.FindName("playercontrols")));
            this.ReplayElement = ((System.Windows.Controls.Button)(this.FindName("ReplayElement")));
            this.RewindElement = ((System.Windows.Controls.Button)(this.FindName("RewindElement")));
            this.playButton = ((System.Windows.Controls.Button)(this.FindName("playButton")));
            this.pauseButton = ((System.Windows.Controls.Button)(this.FindName("pauseButton")));
            this.FastForwardElement = ((System.Windows.Controls.Button)(this.FindName("FastForwardElement")));
            this.SlowMotionElement = ((System.Windows.Controls.Primitives.ToggleButton)(this.FindName("SlowMotionElement")));
            this.divider1 = ((System.Windows.Controls.Grid)(this.FindName("divider1")));
            this.timelinecontrols = ((System.Windows.Controls.Grid)(this.FindName("timelinecontrols")));
            this.slider = ((SilverlightMediaPlayer.CustumSlider)(this.FindName("slider")));
            this.TimeContainer = ((System.Windows.Controls.Border)(this.FindName("TimeContainer")));
            this.CurrentPositionElement = ((System.Windows.Controls.TextBlock)(this.FindName("CurrentPositionElement")));
            this.TimeSeparatorElement = ((System.Windows.Controls.TextBlock)(this.FindName("TimeSeparatorElement")));
            this.CurrentDurationElement = ((System.Windows.Controls.TextBlock)(this.FindName("CurrentDurationElement")));
            this.volumeslider = ((System.Windows.Controls.Slider)(this.FindName("volumeslider")));
            this.divider2 = ((System.Windows.Controls.Grid)(this.FindName("divider2")));
            this.functioncontrols = ((System.Windows.Controls.Grid)(this.FindName("functioncontrols")));
            this.GraphToggleElement = ((System.Windows.Controls.Primitives.ToggleButton)(this.FindName("GraphToggleElement")));
            this.LogToggleElement = ((System.Windows.Controls.Primitives.ToggleButton)(this.FindName("LogToggleElement")));
            this.FullScreenToggleElement = ((System.Windows.Controls.Primitives.ToggleButton)(this.FindName("FullScreenToggleElement")));
            this.divider3 = ((System.Windows.Controls.Grid)(this.FindName("divider3")));
        }
    }
}

