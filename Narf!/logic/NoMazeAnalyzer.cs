﻿using Emgu.CV;
using Narf.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narf.Logic {
  class NoMazeAnalyzer : Analyzer {
    public NoMazeAnalyzer(Case @case, Capture[] sources, Entities session) :
            base(@case, sources, session) {
      throw new NotImplementedException();
    }
  }
}
