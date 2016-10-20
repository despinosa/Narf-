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
    int AngleRefreshFlag { get; set; } = 0;
    CancellationToken Token { get; set; } = CancellationToken.None;
    DispatcherTimer[] RefreshTimers { get; set; }
    internal Analyzer Analyzer { get; }
    Image[] Displays { get; }
    OverlayPanel OverlayPanel { get; set; }

    public PlaybackPage(Case case_, Capture[] captures) {
      Analyzer = Analyzer.ForCase(case_, captures);
      RefreshTimers  = new DispatcherTimer[3];
      Displays = new Image[3];
      InitializeComponent();
      Displays[(int)SourceAngle.Aerial] = mainDisplay;
      Displays[(int)SourceAngle.Closed] = leftDisplay;
      Displays[(int)SourceAngle.Open] = rightDisplay;
      foreach (DispatcherTimer timer in RefreshTimers) timer.IsEnabled = true;
    }

    /*
    protected async Task PeriodicRefresh(SourceAngle angle) {
      while (!Token.IsCancellationRequested) {
        AngleRefreshFlag += 1 >> (int)angle;
        ((Action)Refresh).Invoke();
        if (RefreshIntervals[(int)angle] > TimeSpan.Zero) {
          await Task.Delay(RefreshIntervals[(int)angle], Token);
        }
      }
    } */

    void Refresh(dynamic sender, EventArgs args) {
      var frame = Analyzer.NextFrameFor(sender.Tag);
      if (frame == null) {
        sender.IsEnabled = false;
      } else {
        Displays[(int)sender.Tag].Source = BitmapSourceConvert.
          ToBitmapSource(frame);
      }
    }

    void Grid_Initialized(object sender, EventArgs e) {
      foreach (SourceAngle angle in Enum.GetValues(typeof(SourceAngle))) {
        if (Analyzer.Sources[(int)angle] != null) {
          double fps = Analyzer.Sources[(int)angle].
            GetCaptureProperty(CapProp.Fps);
          RefreshTimers[(int)angle] = new DispatcherTimer() {
            Interval = TimeSpan.FromSeconds(1 / fps), Tag = angle
          };
          RefreshTimers[(int)angle].Tick += Refresh;
        }
      }
    }
  }
}
