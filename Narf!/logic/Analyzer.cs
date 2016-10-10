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
  public abstract class Analyzer {
    public static Analyzer ForCase(Case case_, Capture[] sources) {
      if (case_ is Case) { // replace with subclasses
        return (Analyzer)new NoMazeAnalyzer(case_, sources);
      } else {
        throw new NotImplementedException("No Analyzer for " +
                                          case_.GetType().ToString());
      }
    }
    public Capture[] Sources { get; protected set; } = { null, null, null };
    public Case Case { get; set; }
    public Mat CurrentFrame { get; protected set; }
  }
}
