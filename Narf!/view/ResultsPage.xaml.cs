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
using TimelineEx;
using TimelineLibrary;

namespace Narf.View {
  /// <summary>
  /// Lógica de interacción para ResultsPage.xaml
  /// </summary>
  public partial class ResultsPage : Page {
    Case Case;
    List<TimelineEvent> Events;

    public ResultsPage(Case @case) {
      Case = @case;
      Events = (from e in Case.BehaviourEvents
                select new TimelineEvent() {
                  EventColor = "Green",
                  Description = "evento conductual",
                  Title = e.Behaviour.Name,
                  StartDate = (new DateTime()).AddSeconds(e.Time)
                }).ToList();
      Events.AddRange(from t in Case.Transitions
                      select new TimelineEvent() {
                        EventColor = "Purple",
                        Description = "transición",
                        Title = Enum.GetName(typeof(Zone), t.From) + " a " +
                          Enum.GetName(typeof(Zone), t.To),
                        StartDate = (new DateTime()).AddSeconds(t.Time)
                      });
      InitializeComponent();
    }

    private void _timeline_TimelineReady(object sender, EventArgs args) {
      _timeline.MaxDateTime = (new DateTime()).AddSeconds(Case.Duration);
      _timeline.ResetEvents(Events);
    }
  }
}
