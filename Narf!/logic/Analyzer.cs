using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;
using Narf.Util;
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

  public class Analyzer : IDisposable {
    public static Analyzer ForCase(Case @case, IEnumerable<Capture> captures) {
      switch (@case.Maze) {
        default:
          return new Analyzer(@case, captures);
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
    protected CvBlob Blob { get; set; }
    protected CvTracks Tracks { get; }
    protected IEnumerable<CyclicBuffer<ImageSource>> FrameBuffers { get; }
    protected Thread Worker { get; }

    public Analyzer(Case @case, IEnumerable<Capture> sources) {
      Case = @case;
      Captures = sources;
      FrameBuffers = new CyclicBuffer<ImageSource>[Captures.Count()];
      DeltaT = 1 / (from s in Captures select
                    s.GetCaptureProperty(CapProp.Fps)).Average();
      FrameBuffers = (
        from s in Captures select new
        CyclicBuffer<ImageSource>(Settings.Default.VideoBufferSize)
      ).ToArray();
      TimePlaying = 0;
      Tracks = new CvTracks();
      Worker = new Thread(AnalyzeAndBuffer);
      Worker.Start();
    }

    protected virtual void AnalyzeFrames(IEnumerable<Image<Hsv, byte>> frames,
                                         double time) {
    }
    
    protected virtual void AnalyzeAndBuffer() {
      double headStart = 0;
      Mat[] newFrames;
      do {
        headStart += DeltaT;
        newFrames = (from s in Captures select s.QuerySmallFrame()).ToArray();
        var asImages = (from f in newFrames select
                        f.ToImage<Hsv, byte>()).ToArray();
#if DEBUG
        AnalyzeFrames(asImages, TimePlaying + headStart);
#endif
        foreach (int angle in Enum.GetValues(typeof(CaptureAngle))) {
          var asSource = BitmapSourceConvert.ToBitmapSource(newFrames[angle]);
          asSource.Freeze();
          FrameBuffers.ElementAt(angle).Write(asSource);
        }
#if DEBUG
#else
        AnalyzeFrames(asImages, TimePlaying + headStart);
#endif
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
      foreach (var source in Captures) source.Dispose();
      foreach (var buffer in FrameBuffers) buffer.Dispose();
      if (!Worker.Join(500)) Worker.Abort(); // probar harrrto!
    }
  }
}
