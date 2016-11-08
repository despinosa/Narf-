using Emgu.CV;
using Emgu.CV.CvEnum;
using Narf.Logic;
using Narf.Model;
using Narf.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
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
  /// Lógica de interacción para NewCasePage.xaml
  /// </summary>
  partial class NewCasePage : Page {
    static int Modulo(int x, int m) {
      int r = x % m;
      return r < 0 ? r + m : r;
    }
    
    int PreviewIndex { get; set; }
    Capture[] Unordered { get; set; }
    Capture[] Ordered { get; set; }
    Case Case { get; set; }
    Entities Session { get; }
    ImageSource[] Previews { get; }
    PlaybackPage PlaybackPage { get; set; }


    public NewCasePage(string[] videoPaths, Entities session) {
      Unordered = (from p in videoPaths select new Capture(p)).ToArray();
      Ordered = new Capture[Unordered.Count()];
      Previews = (
        from c in Unordered select
        BitmapSourceConvert.ToBitmapSource(c.QuerySmallFrame())
      ).ToArray();
      Session = session;
      InitializeComponent();
      _currentPreview.Source = Previews[PreviewIndex];
      mazeCombo.ItemsSource = Enum.GetValues(typeof(Maze));
    }

    bool Validate() {
      var valid = _date.SelectedDate != null && _time.Value != null &&
        mazeCombo.SelectedItem != null && _substance.Text != "" &&
        _subject.Text != "" && _weight.Value != null &&
        Ordered.All(o => o != null);
      if (!valid) {
        MessageBox.Show("Debe llenar todos los datos.",
                        "Datos incompletos", MessageBoxButton.OK,
                        MessageBoxImage.Error);
      }
      return valid;
    }

    void _new_Click(object sender, RoutedEventArgs args) {
      if (!Validate()) return;
      var duration = (from c in Unordered select
                      c.GetCaptureProperty(CapProp.FrameCount) /
                      c.GetCaptureProperty(CapProp.Fps)).Min();
      var date = (DateTime)_date.SelectedDate +
        ((DateTime)_time.Value).TimeOfDay;
      var videoHash = 0;
      foreach (int angle in Enum.GetValues(typeof(CaptureAngle))) {
        videoHash ^= Unordered.ElementAt(angle).GetHashCode();
      }
      Case = new Case() {
        Maze = (Maze)mazeCombo.SelectedItem, Substance = _substance.Text,
        Dose = _dose.Value, Date = date, Duration = (short)duration,
        Subject = _subject.Text, Weight = (decimal)_weight.Value,
        VideoHash = videoHash, Notes = _notes.Text,
      };
      try {
        Session.Cases.Add(Case);
        PlaybackPage = new PlaybackPage(Session, Case, Ordered);
        Session.SaveChangesAsync();
      } catch (Exception exc) when (exc is DbEntityValidationException ||
                                    exc is ArgumentException) {
        var result = MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK,
                                     MessageBoxImage.Error);
        if (result == MessageBoxResult.OK) {
          NavigationService.GoBack();
          return;
        }
      }
      NavigationService.Navigate(PlaybackPage);
    }

    private void _previous_Click(object sender, RoutedEventArgs e) {
      PreviewIndex = Modulo(PreviewIndex - 1, Previews.Count());
      _currentPreview.Source = Previews.ElementAt(PreviewIndex);
    }

    private void _next_Click(object sender, RoutedEventArgs e) {
      PreviewIndex = Modulo(PreviewIndex + 1, Previews.Count());
      _currentPreview.Source = Previews.ElementAt(PreviewIndex);
    }

    private void _markAngle_Checked(object sender, RoutedEventArgs e) {
      if (sender == _markAerial) {
        Ordered[(int)CaptureAngle.Aerial] = Unordered[PreviewIndex];
      } else if (sender == _markClosed) {
        Ordered[(int)CaptureAngle.Closed] = Unordered[PreviewIndex];
      } else if (sender == _markOpen) {
        Ordered[(int)CaptureAngle.Open] = Unordered[PreviewIndex];
      }
    }

    private void _markAngle_Unchecked(object sender, RoutedEventArgs e) {
      if (sender == _markAerial) {
        Ordered[(int)CaptureAngle.Aerial] = null;
      } else if (sender == _markClosed) {
        Ordered[(int)CaptureAngle.Closed] = null;
      } else if (sender == _markOpen) {
        Ordered[(int)CaptureAngle.Open] = null;
      }
    }
  }
}
