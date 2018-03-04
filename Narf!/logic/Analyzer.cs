using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;
using Narf.Util;
using Narf.Logic.Imaging;
using Narf.Model;
using Narf.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Narf.Logic {
  public enum CaptureAngle { Aerial, Closed, Open }

  public delegate void NewFrameLoaded(object sender, EventArgs args);

  public class Analyzer : IDisposable {
    // TO DO: revisar diseño debajo, tal vez podemos hacerla concreta
    public static Analyzer ForCase(Case @case, IEnumerable<Capture> captures,
                                   Entities session) {
      switch (@case.Maze) {
        default:
          return new Analyzer(@case, captures, session);
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

    public double DeltaT { get; }
    public double TimePlaying { get; protected set; }
    public IEnumerable<Capture> Captures { get; }
    protected Case Case { get; }
    protected IEnumerable<CyclicBuffer<ImageSource>> FrameBuffers { get; }
    protected Thread Worker { get; }
    Initializer Initializer;
    SubjectTracker Tracker;
    BehaviourMatcher Matcher;

    public Analyzer(Case @case, IEnumerable<Capture> sources,
                    Entities session) {
      Case = @case;
      Captures = sources;
      DeltaT = 1 / (from s in Captures select
                    s.GetCaptureProperty(CapProp.Fps)).Average();
      FrameBuffers = (
        from s in Captures select new
        CyclicBuffer<ImageSource>(Settings.Default.VideoBufferSize)
      ).ToArray();
      TimePlaying = 0;
      Worker = new Thread(AnalyzeAndBuffer);
      Worker.Start();
    }
    
    protected virtual void AnalyzeAndBuffer() {
      int count = 0;
      bool isInitializing = true;
      var imager = new Thread(Initializer.Detect);
      var matcher = new Thread(Matcher.Match);
      Mat[] asMats;
      do {
        count += 1;
        asMats = (from s in Captures select s.QuerySmallFrame()).ToArray();
        if (count % 3 == 0) {
          if (imager.IsAlive) imager.Join();
          if (isInitializing) imager = new Thread(Initializer.Detect);
          else imager = new Thread(Tracker.Track);
          // necesitamos pasar mensaje entre estos (Tracker.background)
          imager.Start(asMats[(int)CaptureAngle.Aerial]);
        }
        if (count % 5 == 0 && !isInitializing) {
          if (matcher.IsAlive) matcher.Join();
          matcher = new Thread(Initializer.Detect);
          matcher.Start(asMats);
        }
# if DEBUG
        var asImages = (from f in asMats select
                        f.ToImage<Hsv, byte>()).ToArray();
# endif
        foreach (int angle in Enum.GetValues(typeof(CaptureAngle))) {
          var asSource = BitmapSourceConvert.ToBitmapSource(asMats[angle]);
          asSource.Freeze();
          try {
            FrameBuffers.ElementAt(angle).Write(asSource);
          } catch (ThreadInterruptedException interruption) {
            return;
          }
        }
      } while (asMats.All(f => f != null));
      foreach (var buffer in FrameBuffers) buffer.Finished = true;
    }

    public IEnumerable<ImageSource> GetNextFrames() {
      TimePlaying += (from b in FrameBuffers where b.HasFront
                      select DeltaT).FirstOrDefault();
      return (from b in FrameBuffers select b.Read()).ToArray();
    }

    public IEnumerable<ImageSource> GetPrevFrames() {
      TimePlaying -= (from b in FrameBuffers where b.HasBack
                      select DeltaT).FirstOrDefault();
      return (from b in FrameBuffers select b.ReadBack()).ToArray();
    }

    public void TriggerBehaviour(Behaviour behaviour) {
      var @event = new BehaviourEvent() {
        Case = Case, Behaviour = behaviour, Time = (short)TimePlaying
      };
      // enviar info del frame a KB de ML
      Case.BehaviourEvents.Add(@event);
    }

    public void Dispose() {
      foreach (var source in Captures) source.Dispose();
      foreach (var buffer in FrameBuffers) buffer.Dispose();
      if (!Worker.Join(500)) { // probar harrrto!
        Worker.Interrupt();
        Worker.Abort();
      }
    }
  }
}
