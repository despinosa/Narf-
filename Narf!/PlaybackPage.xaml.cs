using System;
using System.Collections.Generic;
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

using Emgu.CV;

namespace Narf_ {
  /// <summary>
  /// Lógica de interacción para PlaybackPage.xaml
  /// </summary>
  public partial class PlaybackPage : Page {
    private Capture capture;

    public PlaybackPage() {
      InitializeComponent();
    }

    private async Task RunPeriodicAsync(Action onTick, TimeSpan interval,
                                        CancellationToken token) {
      while (!token.IsCancellationRequested) {
        onTick?.Invoke();
        if (interval > TimeSpan.Zero) await Task.Delay(interval, token);
      }
    }

    private void Refresh() {
      Mat image = capture.QueryFrame();
      videoDisplay.Image = image;
    }

    public void videoDisplay_Loaded(Object sender, EventArgs args) {
      if (capture == null) {
        try {
          capture = new Capture("sample.mp4");
        } catch (NullReferenceException ex) {
          MessageBox.Show(ex.Message);
          return;
        }
      }
      Double fps = capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);
      TimeSpan interval = TimeSpan.FromMilliseconds(1000 / fps);
      RunPeriodicAsync(Refresh, interval, CancellationToken.None);
    }
  }
}
