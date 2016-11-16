﻿using Emgu.CV;
using Microsoft.Win32;
using Narf.Logic;
using Narf.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Narf.View {
  /// <summary>
  /// Lógica de interacción para PlaybackPage.xaml
  /// </summary>
  partial class PlaybackPage : Page {
    public Analyzer Analyzer { get; }
    CancellationToken Token { get; set; } = CancellationToken.None;
    Case Case { get; set; }
    DispatcherTimer RefreshTimer { get; }
    Entities Session { get; }
    IEnumerable<Image> Displays { get; set; }
    OverlayPanel OverlayPanel { get; }
    ResultsPage ResultsPage { get; }

    public PlaybackPage(Entities session, Case @case,
                        IEnumerable<Capture> captures) {
      Session = session;
      Case = @case;
      Analyzer = Analyzer.ForCase(@case, captures); // puede lanzar excepción
      RefreshTimer = new DispatcherTimer(DispatcherPriority.Render);
      OverlayPanel = new OverlayPanel(Session, this);
      ResultsPage = new ResultsPage(Case);
      InitializeComponent();
      RefreshTimer.Start();
    }

    void Refresh(object sender, EventArgs args) {
      var timer = (DispatcherTimer)sender;
      var newFrames = Analyzer.NextFrames();
      if (newFrames.All(f => f == null)) {
        timer.Stop();
      } else {
        foreach (int angle in Enum.GetValues(typeof(CaptureAngle))) {
          Displays.ElementAt(angle).Source = newFrames.ElementAt(angle);
        }
      }
    }

    void CleanupHandler(object sender, EventArgs args) {
      Analyzer.Dispose();
      NavigationService.Navigate(ResultsPage);
    }

    void Grid_Initialized(object sender, EventArgs e) {
      OverlayPanel.Visibility = Visibility.Collapsed;
      Grid.SetRowSpan(OverlayPanel, 2);
      Grid.SetColumnSpan(OverlayPanel, 2);
      Panel.SetZIndex(OverlayPanel, 1000);
      _mainPanel.Children.Add(OverlayPanel);
      Displays = new Image[3] { mainDisplay, leftDisplay, rightDisplay };
      RefreshTimer.Interval = TimeSpan.FromSeconds(Analyzer.DeltaT);
      RefreshTimer.Tick += Refresh;
    }

    void Panel_MouseEnter(object sender, MouseEventArgs args) {
      _hiddenPanel.Visibility = Visibility.Visible;
    }

    void Panel_MouseLeave(object sender, MouseEventArgs args) {
      _hiddenPanel.Visibility = Visibility.Collapsed;
    }

    void Pause_Click(object sender, RoutedEventArgs args) {
      MouseEnter -= Panel_MouseEnter;
      MouseLeave -= Panel_MouseLeave;
      _hiddenPanel.Visibility = Visibility.Collapsed;
      OverlayPanel.Visibility = Visibility.Visible;
      RefreshTimer.Stop();
    }

    public void Play_Click(object sender, RoutedEventArgs args) {
      MouseEnter += Panel_MouseEnter;
      MouseLeave += Panel_MouseLeave;
      OverlayPanel.Visibility = Visibility.Collapsed;
      RefreshTimer.Start();
    }

    public void Next_Click(object sender, RoutedEventArgs args) {
      var nextFrames = Analyzer.NextFrames();
      if (nextFrames.All(f => f == null)) return;
      foreach (int angle in Enum.GetValues(typeof(CaptureAngle))) {
        Displays.ElementAt(angle).Source = nextFrames.ElementAt(angle);
      }
    }

    public void Prev_Click(object sender, RoutedEventArgs args) {
      var prevFrames = Analyzer.PrevFrames();
      if (prevFrames.All(f => f == null)) return;
      foreach (int angle in Enum.GetValues(typeof(CaptureAngle))) {
        Displays.ElementAt(angle).Source = prevFrames.ElementAt(angle);
      }
    }

    public void Behaviour_Click(object sender, RoutedEventArgs args) {
      var behaviour = (Behaviour)sender;
      Analyzer.BehaviourTriggered(behaviour);
      Session.SaveChangesAsync();
      MouseEnter += Panel_MouseEnter;
      MouseLeave += Panel_MouseLeave;
      OverlayPanel.Visibility = Visibility.Collapsed;
      RefreshTimer.Start();
    }
  }
}
