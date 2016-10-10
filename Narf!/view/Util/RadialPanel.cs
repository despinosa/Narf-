using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Narf.View {
  public class RadialPanel : Panel {
    protected override Size MeasureOverride(Size availableSize) {
      foreach (UIElement elem in Children) {
        elem.Measure(new Size(double.PositiveInfinity,
                              double.PositiveInfinity));
      }
      return base.MeasureOverride(availableSize);
    }
    
    protected override Size ArrangeOverride(Size size) {
      if (Children.Count == 0) return size;

      // in rads
      double deltaTheta = (2.0 * Math.PI) / Children.Count;
      double theta = 3 * Math.PI / 2;
     
      // avoid completely vertical items
      if (Children.Count % 4 == 0) theta += deltaTheta / 2;

      // crude approximation
      double radiusX = size.Width / 2.4;
      double radiusY = size.Height / 2.4;

      foreach (UIElement elem in Children) {
        var farthest = new Point(elem.DesiredSize.Width,
                                 elem.DesiredSize.Height);

        var ideal = new Point(Math.Cos(-theta) * radiusX,
                              -Math.Sin(-theta) * radiusY);

        var actual = new Point(size.Width/2 + ideal.X - farthest.X/2,
                               size.Height/2 + ideal.Y - farthest.Y/2);

        double phi = 180 * ((theta / Math.PI) % 1) - 90; // in degs
        var rotate = new RotateTransform(phi, farthest.X/2, farthest.Y/2);

        elem.Arrange(new Rect(actual.X, actual.Y, farthest.X, farthest.Y));
        elem.RenderTransform = rotate;

        theta += deltaTheta;
      }

      return size;
    }
  }
}
