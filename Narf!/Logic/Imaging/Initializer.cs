using Emgu.CV;
using Narf.Model;
using Narf.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Narf.Logic.Imaging {
  /// <summary>
  /// Clase de apoyo en la inicialización del análisis de video.
  /// Se alimenta del primer flujo de imágenes, para definir un
  /// background promedio a restar a todos los demás cuadros y
  /// detecta bordes en el laberinto. Trabaja sólo con la vista
  /// aérea.
  /// </summary>
  class Initializer {
    Analyzer Main { get; }
    CyclicBuffer<Mat> FrameBuffer { get; }
    Maze Maze { get; }

    Initializer(Analyzer main, CyclicBuffer<Mat> frameBuffer) {
      Main = main;
      FrameBuffer = frameBuffer;
    }

    public void Detect(object obj) {
      throw new NotImplementedException();
    }
  }
}
