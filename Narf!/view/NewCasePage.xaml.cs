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
    protected PlaybackPage _playbackPage;
    public PlaybackPage PlaybackPage {
      get {
        return _playbackPage;
      }
      protected set {
        _playbackPage = value;
      }
    }
    public NewCasePage(string videoPath) {
      InitializeComponent();
    }
  }
}
