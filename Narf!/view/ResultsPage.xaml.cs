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
using Narf.Model;

namespace Narf.View {
  /// <summary>
  /// Lógica de interacción para ResultsPage.xaml
  /// </summary>
  public partial class ResultsPage : Page {
    private Case Case;

    public ResultsPage(Case @case) {
      Case = @case;
      InitializeComponent();
    }
  }
}
