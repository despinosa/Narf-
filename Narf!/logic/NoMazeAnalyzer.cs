using Emgu.CV;
using Narf.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narf.Logic {
  class NoMazeAnalyzer : Analyzer {
    public NoMazeAnalyzer(Case case_, Capture[] sources) {
      Case = case_;
      Sources = sources;
      CurrentFrames = new Mat[Enum.GetValues(typeof(SourceAngle)).Length];
    }

    public override Mat NextFrameFor(SourceAngle angle) {
      CurrentFrames[(int)angle]?.Dispose();
      CurrentFrames[(int)angle] = Sources[(int)angle].QueryFrame();
      return CurrentFrames[(int)angle];
    }
  }
}
