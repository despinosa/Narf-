using Emgu.CV;
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
  public partial class NewCasePage : Page {
    public Capture[] Captures { get; protected set; }
    public Case Case { get; protected set; }
    public PlaybackPage PlaybackPage { get; protected set; }


    public NewCasePage(string[] videoPaths) {
      Captures = new Capture[videoPaths.Length];
      for (int i = 0; i < videoPaths.Length; i++) {
        Captures[i] = new Capture(videoPaths[i]);
      }
      InitializeComponent();
      mazeCombo.ItemsSource = Enum.GetValues(typeof(Maze));
    }

    private void newBttn_Click(object sender, RoutedEventArgs e) {
      Case = new Case() {
        Date = new DateTime(2016, 8, 30), Duration = (short)new TimeSpan(0, 5, 8).TotalSeconds,
        Substance = "SSRI", Dose = 9M, Subject = "Cerebro", Weight = 231.7M,
        Maze = (Maze) mazeCombo.SelectedItem
      };
      PlaybackPage = new PlaybackPage(Case, Captures);
      NavigationService.Navigate(PlaybackPage);
    }
  }
}
