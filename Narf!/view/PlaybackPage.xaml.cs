using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using Emgu.CV.WPF;
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
    int angleFinshedFlag = 0;
    internal Analyzer Analyzer { get; }
    CancellationToken Token { get; set; } = CancellationToken.None;
    DispatcherTimer FinishedTimer { get; }
    DispatcherTimer[] RefreshTimers { get; }
    Entities Session { get; }
    Image[] Displays { get; }
    OverlayPanel OverlayPanel { get; }

    public PlaybackPage(Entities session, Case case_, Capture[] captures) {
      Session = session;
      Analyzer = Analyzer.ForCase(case_, captures); // puede lanzar excepción
      RefreshTimers = new DispatcherTimer[3];
      FinishedTimer = new DispatcherTimer();
      Displays = new Image[3];
      OverlayPanel = new OverlayPanel(Session, this);
      InitializeComponent();
      OverlayPanel.Visibility = Visibility.Collapsed;
      Grid.SetRowSpan(OverlayPanel, 2);
      Grid.SetColumnSpan(OverlayPanel, 2);
      Panel.SetZIndex(OverlayPanel, 1000);
      _mainPanel.Children.Add(OverlayPanel);
      Displays[(int)SourceAngle.Aerial] = mainDisplay;
      Displays[(int)SourceAngle.Closed] = leftDisplay;
      Displays[(int)SourceAngle.Open] = rightDisplay;
      foreach (DispatcherTimer timer in RefreshTimers) timer.IsEnabled = true;
      FinishedTimer.IsEnabled = true;
    }

    void Refresh(object sender, EventArgs args) {
      var timer = (DispatcherTimer)sender;
      var frame = Analyzer.NextFrameFor((SourceAngle)timer.Tag);
      if (frame == null) {
        timer.IsEnabled = false;
        angleFinshedFlag += 1 << (int)timer.Tag;
      } else {
        Displays[(int)timer.Tag].Source = BitmapSourceConvert.
          ToBitmapSource(frame);
      }
    }

    void Grid_Initialized(object sender, EventArgs e) {
      var maxInterval = double.MaxValue;
      foreach (SourceAngle angle in Enum.GetValues(typeof(SourceAngle))) {
        if (Analyzer.Sources[(int)angle] != null) {
          double interval = 1 / Analyzer.Sources[(int)angle].
            GetCaptureProperty(CapProp.Fps);
          maxInterval = Math.Max(maxInterval, interval);
          RefreshTimers[(int)angle] = new DispatcherTimer() {
            Interval = TimeSpan.FromSeconds(interval), Tag = angle
          };
          RefreshTimers[(int)angle].Tick += Refresh;
        }
      }
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
      foreach (DispatcherTimer timer in RefreshTimers) timer.IsEnabled = false;
    }

    public void Play_Click(object sender, RoutedEventArgs args) {
      MouseEnter += Panel_MouseEnter;
      MouseLeave += Panel_MouseLeave;
      OverlayPanel.Visibility = Visibility.Collapsed;
      foreach (DispatcherTimer timer in RefreshTimers) timer.IsEnabled = true;
    }

    public void Next_Click(object sender, RoutedEventArgs args) {
      for (int i = 0; i < Displays.Length; i++) {
        var frame = Analyzer.NextFrameFor((SourceAngle)i);
        if (frame != null) {
          Displays[i].Source = BitmapSourceConvert.ToBitmapSource(frame);
        }
      }
    }


    public void Prev_Click(object sender, RoutedEventArgs args) {
      for (int i = 0; i < Displays.Length; i++) {
        var frame = Analyzer.PrevFrameFor((SourceAngle)i);
        if (frame != null) {
          Displays[i].Source = BitmapSourceConvert.ToBitmapSource(frame);
        }
      }
    }

    public void Behaviour_Click(object sender, RoutedEventArgs args) {
      var behaviour = (Behaviour)sender;
      Analyzer.BehaviourTriggered(behaviour);
      Session.SaveChangesAsync();
      MouseEnter += Panel_MouseEnter;
      MouseLeave += Panel_MouseLeave;
      OverlayPanel.Visibility = Visibility.Collapsed;
      foreach (DispatcherTimer timer in RefreshTimers) timer.IsEnabled = true;
    }
  }
}
