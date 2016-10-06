using Emgu.CV;
using Emgu.CV.CvEnum;
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
    private string videoPath;
    private Analyzer analyzer;
    private TimeSpan refreshInterval;
    private CancellationToken token = CancellationToken.None;

    public PlaybackPage(string videoPath, MazeType mazeType) {
      this.videoPath = videoPath;
      if (mazeType == MazeType.Cross) {
        analyzer = new XMazeAnalyzer(videoPath);
      } else if (mazeType == MazeType.None) {
        analyzer = new NoMazeAnalyzer(videoPath);
      }
      InitializeComponent();
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
      Mat image = analyzer.QueryFrame();
      if (image == null) token = new CancellationToken();
      else videoDisplay.Image = image;
    }

    private void videoDisplay_LoadCompleted(object sender,
                                            AsyncCompletedEventArgs e) {
      try {
        double fps = analyzer.GetCaptureProperty(CapProp.Fps);
        refreshInterval = TimeSpan.FromSeconds(1 / fps);
        PeriodicRefresh();
      } catch (NullReferenceException ex) {
        MessageBox.Show(ex.Message);
        return;
      }
    }
  }
}
