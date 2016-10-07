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

    // Measure each children and give as much room as they want 
    protected override Size MeasureOverride(Size availableSize) {
      foreach (UIElement elem in Children) {

        // Give Infinite size as the avaiable size for all the children
        elem.Measure(new Size(double.PositiveInfinity,
                              double.PositiveInfinity));
      }
      return base.MeasureOverride(availableSize);
    }

    /* Arrange all children based on the geometric equations for the
     * circle.
     */
    protected override Size ArrangeOverride(Size size) {
      if (Children.Count == 0) return size;
      double angle = 0;

      // Divide circumference by number of elements
      double incrementalAngularSpace = (2.0 * Math.PI) / Children.Count;

      /* An approximate radus based on the avialable size, obviusly a
       * better approach is needed here.
       */
      double radiusX = size.Width / 2.4;
      double radiusY = size.Height / 2.4;
      foreach (UIElement elem in Children) {
        RotateTransform rotation = new RotateTransform((Math.PI/2 + angle) *
                                                       180/Math.PI);
        elem.RenderTransform = rotation;

        // Calculate the point on the circle for the element
        Point point = new Point(Math.Cos(angle) * radiusX,
                                -Math.Sin(angle) * radiusY);

        /* Offsetting the point to the Avalable rectangular area which
         * is FinalSize.
         */
        Point realPoint = new Point(size.Width/2 + point.X -
                                      elem.DesiredSize.Width/2,
                                    size.Height/2 + point.Y -
                                      elem.DesiredSize.Height/2);

        /* Call Arrange method on the child element by giving the
         * calculated point as the placementPoint.
         */
        elem.Arrange(new Rect(realPoint.X, realPoint.Y, elem.DesiredSize.Width,
                              elem.DesiredSize.Height));

        // Calculate the new angle for the next element
        angle += incrementalAngularSpace;
      }

      return size;
    }
  }
}
