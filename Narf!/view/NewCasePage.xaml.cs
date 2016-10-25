using Emgu.CV;
using Emgu.CV.CvEnum;
using Narf.Logic;
using Narf.Model;
using System;
using System.Collections.Generic;
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
    Capture[] Captures { get; set; }
    Case Case { get; set; }
    PlaybackPage PlaybackPage { get; set; }
    Entities Session { get; }


    public NewCasePage(string[] videoPaths, Entities session) {
      Captures = new Capture[videoPaths.Length];
      for (int i = 0; i < videoPaths.Length; i++) {
        Captures[i] = new Capture(videoPaths[i]);
      }
      Session = session;
      InitializeComponent();
      mazeCombo.ItemsSource = Enum.GetValues(typeof(Maze));
    }

    bool _validate() {
      var valid = date.SelectedDate != null && time.Value != null &&
        mazeCombo.SelectedItem != null && substance.Text != "" &&
        subject.Text != "" && weight.Value != null;
      if (!valid) {
        MessageBox.Show("Debe llenar todos los datos.",
                        "Datos incompletos", MessageBoxButton.OK,
                        MessageBoxImage.Error);
      }
      return valid;
    }

    void new_Click(object sender, RoutedEventArgs args) {
      if (!_validate()) return;
      var duration = (from c in Captures select
                      c.GetCaptureProperty(CapProp.FrameCount) /
                      c.GetCaptureProperty(CapProp.Fps)).Min();
      var _date = (DateTime)date.SelectedDate + ((DateTime)time.Value).TimeOfDay;
      var videoHash = 0;
      foreach (SourceAngle angle in Enum.GetValues(typeof(SourceAngle))) {
        videoHash ^= Captures[(int)angle].GetHashCode();
      }
      Case = new Case() {
        Date = _date, Duration = (short)duration,
        Substance = substance.Text, Dose = dose.Value, Subject = subject.Text,
        Weight = (decimal)weight.Value, Maze = (Maze)mazeCombo.SelectedItem,
        VideoHash = videoHash, Notes = notes.Text,
        Preview = Captures[(int)SourceAngle.Aerial].QuerySmallFrame().GetData()
      };
      Session.Cases.Add(Case);
      try {
        PlaybackPage = new PlaybackPage(Session, Case, Captures);
      } catch (ArgumentException exc) {
        var result = MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK,
                                     MessageBoxImage.Error);
        if (result == MessageBoxResult.OK) {
          NavigationService.GoBack();
          return;
        }
      }
      Session.SaveChangesAsync();
      NavigationService.Navigate(PlaybackPage);
    }
  }
}
