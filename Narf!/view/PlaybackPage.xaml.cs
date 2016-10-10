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

namespace Narf.View {
  /// <summary>
  /// Lógica de interacción para PlaybackPage.xaml
  /// </summary>
  public partial class PlaybackPage : Page {
    public Analyzer Analyzer { get; }
    private int AngleRefreshFlag { get; set; } = 0;
    private CancellationToken Token { get; set; } = CancellationToken.None;
    public Image[] Displays { get; }
    public OverlayPanel OverlayPanel { get; protected set; }
    public TimeSpan[] RefreshIntervals { get; }

    public PlaybackPage(Case case_, Capture[] captures) {
      Analyzer = Analyzer.ForCase(case_, captures);
      RefreshIntervals = new TimeSpan[3];
      Displays = new Image[3];
      InitializeComponent();
      Displays[(int)SourceAngle.Aerial] = mainDisplay;
      if (captures.Length > 1) Displays[(int)SourceAngle.Closed] = leftDisplay;
      if (captures.Length > 2) Displays[(int)SourceAngle.Open] = rightDisplay;
    }

    protected async Task PeriodicRefresh(SourceAngle angle) {
      while (!Token.IsCancellationRequested) {
        AngleRefreshFlag += 1 >> (int)angle;
        ((Action)Refresh).Invoke();
        if (RefreshIntervals[(int)angle] > TimeSpan.Zero) {
          await Task.Delay(RefreshIntervals[(int)angle], Token);
        }
      }
    }

    protected void Refresh() {
      for (int angle = 0; AngleRefreshFlag != 0; angle++) {
        int angleChooser = (1 << angle) & AngleRefreshFlag;
        if (angleChooser != 0) {
          var frame =  Analyzer.Sources[angle].QueryFrame();
          if (frame == null) {
            Token = new CancellationToken();
          } else {
            Displays[angle].Source = BitmapSourceConvert.ToBitmapSource(frame);
          }
          AngleRefreshFlag -= angleChooser;
        }
      }
    }

    private void Grid_Initialized(object sender, EventArgs e) {
      foreach (SourceAngle angle in Enum.GetValues(typeof(SourceAngle))) {
        if (Analyzer.Sources[(int)angle] != null) {
          double fps = Analyzer.Sources[(int)angle].
            GetCaptureProperty(CapProp.Fps);
          RefreshIntervals[(int)angle] = TimeSpan.FromSeconds(1 / fps);
          PeriodicRefresh(angle);
        }
      }
    }
  }
}
