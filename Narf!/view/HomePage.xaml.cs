using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Narf_.view {
  /// <summary>
  /// Lógica de interacción para HomePage.xaml
  /// </summary>
  public partial class HomePage : Page {
    public HomePage() {
      using(Capture capture = new Capture("/resources/sample.mp4")) {
        previewDisplay.Image = capture?.QuerySmallFrame();
      }
      InitializeComponent();
    }

    private void previewDisplay_LoadCompleted(object sender,
                                              AsyncCompletedEventArgs e) {
    }
  }
}
