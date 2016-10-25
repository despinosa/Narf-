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
        .OrderBy(b => (from e in Session.BehaviourEvents where
                       e.Behaviour.Id == b.Id select e).Count());
      var onOuter = behaviours.Count() / (int) (1 + Math.Sqrt(5)) * 2;
      foreach (var behaviour in behaviours.Take(onOuter)) {
        var button = new Button() { Content = behaviour.Name };
        button.Click += (sender, args) =>
            Main.Behaviour_Click(behaviour, args);
        outerRing.Children.Add(button);
      }
      foreach (var behaviour in
               behaviours.Skip(onOuter).Take(behaviours.Count() - onOuter)) {
        var button = new Button() { Content = behaviour.Name };
        button.Click += (sender, args) =>
            Main.Behaviour_Click(behaviour, args);
        innerRing.Children.Add(button);
      }
    }

    void Play_Click(object sender, RoutedEventArgs args) {
      Main.Play_Click(sender, args);
    }

    void Prev_Click(object sender, RoutedEventArgs args) {
      Main.Prev_Click(sender, args);
    }

    void Next_Click(object sender, RoutedEventArgs args) {
      Main.Next_Click(sender, args);
    }
  }
}
