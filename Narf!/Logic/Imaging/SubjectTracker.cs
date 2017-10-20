using Emgu.CV;
using Emgu.CV.Cvb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narf.Logic.Imaging {
  /// <summary>
  /// Clase de apoyo en el grueso del análisis de video. Conociendo
  /// el background, lo resta a cada cuadro dado y devuelve las
  /// coordenadas del sujeto. Trabaja solo con la vista aérea.
  /// </summary>
  class SubjectTracker {
    Mat Background { get; }
    CvBlob Subject { get; set; }
    CvTracks Tracks { get; set; }

    public void Track(object obj) {
      throw new NotImplementedException();
    }
  }
}
