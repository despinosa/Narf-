using Emgu.CV;
using Emgu.CV.CvEnum;
using Narf.Logic.Util;
using Narf.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narf.Logic {
  public enum SourceAngle { Aerial, Closed, Open }
  class Analyzer {
    public static Analyzer ForCase(Case case_, Capture[] sources) {
      switch (case_.Maze) {
        default:
          return new Analyzer(case_, sources);
        /* TO DO: hacer abstracta e implementar estas subclases
        case Maze.None:
          return new NoMazeAnalyzer(case_, sources);
        case Maze.Plus:
          return new XMazeAnalyzer(case_, sources);
        default:
          throw new NotImplementedException("No Analyzer for " +
                                            case_.Maze.ToString()); */
      }
    }
    static readonly int BUFFER_SIZE = 100;
    public float SecondsEllapsed { get; protected set; }
    public Capture[] Sources { get; }
    protected Case Case { get; }
    protected CyclicBuffer<Mat>[] FrameBuffers { get; }
    public Analyzer(Case case_, Capture[] sources) {
      Case = case_;
      Sources = sources;
      FrameBuffers = new CyclicBuffer<Mat>[Sources.Length];
      for (int i = 0; i < Sources.Length; i++) {
        FrameBuffers[i] = new CyclicBuffer<Mat>(BUFFER_SIZE);
      }
      SecondsEllapsed = 0;
    }

    public Mat NextFrameFor(SourceAngle angle) {
      if (FrameBuffers[(int)angle].HasForward()) {
        return FrameBuffers[(int)angle].ForwardRead();
      }
      var newFrame = Sources[(int)angle].QuerySmallFrame();
      FrameBuffers[(int)angle].Write(newFrame);
      return newFrame;
    }

    public void BehaviourTriggered(Behaviour behaviour) {
      var event_ = new BehaviourEvent() {
        Case = Case, Behaviour = behaviour, Time = (short)SecondsEllapsed
      };
      Case.BehaviourEvents.Add(event_);
    }
  }
}
