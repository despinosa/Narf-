using Emgu.CV;
using Emgu.CV.CvEnum;
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

namespace Narf_.view {
  /// <summary>
  /// Lógica de interacción para PlaybackPage.xaml
  /// </summary>
  public partial class PlaybackPage : Page {
    private string videoPath = "/resources/sample.mp4";
    private Capture capture;
    private TimeSpan refreshInterval;
    private CancellationToken token = CancellationToken.None;

    public PlaybackPage() {
      InitializeComponent();
      capture = new Capture(videoPath);
    }

    private async Task PeriodicRefresh() {
      while (!token.IsCancellationRequested) {
        ((Action) Refresh).Invoke();
        if (refreshInterval > TimeSpan.Zero) {
          await Task.Delay(refreshInterval, token);
        }
      }
    }

    private void Refresh() {
      Mat image = capture.QueryFrame();
      if (image == null) token = new CancellationToken();
      else videoDisplay.Image = image;
    }

    private void videoDisplay_LoadCompleted(object sender,
                                            AsyncCompletedEventArgs e) {
      if (capture == null) {
        try {
          capture = new Capture(videoPath);
        } catch (NullReferenceException ex) {
          MessageBox.Show(ex.Message);
          return;
        }
      }
      Double fps = capture.GetCaptureProperty(CapProp.Fps);
      refreshInterval = TimeSpan.FromSeconds(1 / fps);
      PeriodicRefresh();
    }
  }
}
