using Emgu.CV;
using Emgu.CV.CvEnum;
using Narf.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narf.Logic {
  public enum SourceAngle { Aerial, Closed, Open }
  abstract class Analyzer {
    public static Analyzer ForCase(Case case_, Capture[] sources) {
      switch (case_.Maze) {
        case Maze.None:
          return new NoMazeAnalyzer(case_, sources);
        case Maze.Plus: // TO DO
        default:
          throw new NotImplementedException("No Analyzer for " +
                                            case_.Maze.ToString());
      }
    }
    public Capture[] Sources { get; protected set; }
    public Case Case { get; protected set; }
    public TimeSpan Ellapsed { get; set; }
    public Mat[] CurrentFrames { get; set; }

    public abstract Mat NextFrameFor(SourceAngle angle);
    public void BehaviourTriggered(Behaviour behaviour) {
      var event_ = new BehaviourEvent() {
        Case = Case, Behaviour = behaviour, Time = (short)Ellapsed.TotalSeconds
      };
      Case.BehaviourEvents.Add(event_);
    }
  }
}
