using Emgu.CV;
using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narf.Logic {
  public abstract class Analyzer {
    Capture capture;
    Mat currentFrame;
    public Mat QueryFrame() {
      currentFrame = capture.QueryFrame();
      return currentFrame;
    }
    public Double GetCaptureProperty(CapProp prop) {
      return capture.GetCaptureProperty(prop);
    }
  }
}
