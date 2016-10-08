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

      double deltaTheta = (2.0 * Math.PI) / Children.Count;
      double theta = 3 * Math.PI / 2;
     
      if (Children.Count % 4 == 0) theta -= deltaTheta / 2;
      else theta -= deltaTheta; // avoid completely vertical items
      
      double radiusX = size.Width / 2.4; // crude approximation
      double radiusY = size.Height / 2.4;
      foreach (UIElement elem in Children) {
        Point center = new Point(elem.DesiredSize.Width / 2,
                                 elem.DesiredSize.Height / 2);

        Point ideal = new Point(Math.Cos(-theta) * radiusX,
                                -Math.Sin(-theta) * radiusY);

        Point actual = new Point(size.Width/2 + ideal.X - center.X,
                                 size.Height/2 + ideal.Y - center.Y);

        double phi = 180 * ((theta / Math.PI) % 1) - 90;
        RotateTransform rotation = new RotateTransform(phi, center.X,
                                                       center.Y);

        elem.Arrange(new Rect(actual.X, actual.Y, elem.DesiredSize.Width,
                              elem.DesiredSize.Height));
        elem.RenderTransform = rotation;

        theta += deltaTheta;
      }

      return size;
    }
  }
}
