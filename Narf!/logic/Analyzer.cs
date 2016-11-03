using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.VideoSurveillance;
using Narf.Util;
using Narf.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Narf.Logic {
  public enum SourceAngle { Aerial, Closed, Open }

  class Analyzer : IDisposable {
    public static Analyzer ForCase(Case @case, Capture[] sources) {
      switch (@case.Maze) {
        default:
          return new Analyzer(@case, sources);
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
    static readonly int BUFFER_SIZE = 30;
    public double DeltaT { get; }
    public double TimePlaying { get; protected set; }
    public Capture[] Sources { get; }
    protected Case Case { get; }
    protected CyclicBuffer<ImageSource>[] FrameBuffers { get; }
    protected ImagePreprocessor ImagePreprocessor { get; }
    protected ObjectTracker ObjectTracker { get; }
    protected Thread Worker { get; }

    public Analyzer(Case @case, Capture[] sources) {
      Case = @case;
      Sources = sources;
      FrameBuffers = new CyclicBuffer<ImageSource>[Sources.Length];
      DeltaT = 1 / (from s in Sources select
                    s.GetCaptureProperty(CapProp.Fps)).Average();
      FrameBuffers = (from s in Sources select new
                      CyclicBuffer<ImageSource>(BUFFER_SIZE)).ToArray();
      TimePlaying = 0;
      Worker = new Thread(AnalyzeAndBuffer);
      Worker.Start();
    }

    protected virtual void AnalyzeFrames(IEnumerable<Mat> frames,
                                         double time) {
    }
    
    protected virtual void AnalyzeAndBuffer() {
      double headStart = 0;
      Mat[] newFrames;
      do {
        headStart += DeltaT;
        newFrames = (from s in Sources select s.QuerySmallFrame()).ToArray();
        foreach (int angle in Enum.GetValues(typeof(SourceAngle))) {
          var converted = BitmapSourceConvert.ToBitmapSource(newFrames[angle]);
          converted.Freeze();
          FrameBuffers[angle].Write(converted);
        }
        AnalyzeFrames(newFrames, TimePlaying + headStart);
      } while (newFrames.All(f => f != null));
      foreach (var buffer in FrameBuffers) buffer.Finished = true;
    }

    public IEnumerable<ImageSource> NextFrames() {
      TimePlaying += (from b in FrameBuffers where b.HasFront
                      select DeltaT).FirstOrDefault();
      return (from b in FrameBuffers select b.Read()).ToArray();
    }

    public IEnumerable<ImageSource> PrevFrames() {
      TimePlaying -= (from b in FrameBuffers where b.HasBack
                      select DeltaT).FirstOrDefault();
      return (from b in FrameBuffers select b.ReadBack()).ToArray();
    }

    public void BehaviourTriggered(Behaviour behaviour) {
      var @event = new BehaviourEvent() {
        Case = Case, Behaviour = behaviour, Time = (short)TimePlaying
      };
      // enviar info del frame a KB de ML
      Case.BehaviourEvents.Add(@event);
    }

    public void Dispose() {
      foreach (var s in Sources) s.Dispose();
      Worker.Join();
    }
  }
}
