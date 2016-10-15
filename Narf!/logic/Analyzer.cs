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
          return (Analyzer)new NoMazeAnalyzer(case_, sources);
        case Maze.Plus:
        default:
          throw new NotImplementedException("No Analyzer for " +
                                            case_.Maze.ToString());
      }
    }
    public Capture[] Sources { get; protected set; }
    public Case Case { get; protected set; }
    protected TimeSpan Ellapsed { get; set; }
    protected Mat[] CurrentFrames { get; set; }

    public abstract Mat NextFrameFor(SourceAngle angle);
    public void MarkBehaviour(Behaviour behaviour) {
    }
  }
}
