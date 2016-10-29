using Emgu.CV;
using Emgu.CV.CvEnum;
using Narf.Logic.Util;
using Narf.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    protected double DeltaT { get; }
    public double TimePlaying { get; protected set; }
    public Capture[] Sources { get; }
    protected Case Case { get; }
    protected DisposablesCyclicBuffer<Mat>[] FrameBuffers { get; }
    protected Thread Worker { get; }

    public Analyzer(Case case_, Capture[] sources) {
      Case = case_;
      Sources = sources;
      FrameBuffers = new DisposablesCyclicBuffer<Mat>[Sources.Length];
      for (int i = 0; i < Sources.Length; i++) {
        DeltaT += Sources[i].GetCaptureProperty(CapProp.Fps);
        FrameBuffers[i] = new DisposablesCyclicBuffer<Mat>(BUFFER_SIZE);
      }
      DeltaT = 1 / DeltaT;
      TimePlaying = 0f;
      Worker = new Thread(AnalyzeAndBuffer);
      Worker.Start();
    }
    
    protected virtual void AnalyzeAndBuffer() {
      var newFrames = Enumerable.Select(Sources, (s => s.QuerySmallFrame()));
      while (newFrames.All(f => f != null)) {
        // procesar frame ahora!
        foreach (var source in Enum.GetValues(typeof(SourceAngle))) {
          FrameBuffers[(int)source].Write(newFrames.ElementAt((int)source));
        }
        newFrames = Enumerable.Select(Sources, (s => s.QuerySmallFrame()));
      }
    }

    public Mat NextFrameFor(SourceAngle angle) {
      if (FrameBuffers[(int)angle].HasForward()) TimePlaying += DeltaT;
      return FrameBuffers[(int)angle].ForwardRead();
    }

    public Mat PrevFrameFor(SourceAngle angle) {
      if (FrameBuffers[(int)angle].HasBackward()) TimePlaying -= DeltaT;
      return FrameBuffers[(int)angle].BackwardRead();
    }

    public void BehaviourTriggered(Behaviour behaviour) {
      var event_ = new BehaviourEvent() {
        Case = Case, Behaviour = behaviour, Time = (short)TimePlaying
      };
      Case.BehaviourEvents.Add(event_);
    }
  }
}
