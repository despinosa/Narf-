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
  /// Lógica de interacción para OverlayPanel.xaml
  /// </summary>
  partial class OverlayPanel : Grid {
    Entities Session { get; set; }
    int CaseId { get; set; }
    PlaybackPage Main { get; }
    public OverlayPanel(Entities session, PlaybackPage main) {
      Session = session;
      Main = main;
      InitializeComponent();
      SetupButtons();
    }
    void SetupButtons() {
      var behaviours = new List<Behaviour>(Session.Behaviours)
        .OrderByDescending(b => (from e in Session.BehaviourEvents where
                                 e.Behaviour == b select true).Count());
      var onInner = behaviours.Count() / (int) (1 + Math.Sqrt(5)) * 2;
      foreach (var behaviour in behaviours.Take(onInner)) {
        var button = new Button() { Content = behaviour.Name };
        button.Click += (sender, args) =>
            Main.Analyzer.BehaviourTriggered(behaviour);
        innerRing.Children.Add(button);
      }
      foreach (var behaviour in behaviours.Take(behaviours.Count()-onInner)) {
        var button = new Button() { Content = behaviour.Name };
        button.Click += (sender, args) =>
            Main.Analyzer.BehaviourTriggered(behaviour);
        innerRing.Children.Add(button);
      }
    }
  }
}
