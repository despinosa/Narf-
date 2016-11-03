//----------------------------------------------------------------------------
//  Copyright (C) 2004-2016 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using Emgu.CV;
using System.Drawing;

namespace Narf.Util {
  public static class BitmapSourceConvert {
      /// <summary>
      /// Delete a GDI object
      /// </summary>
      /// <param name="o">The poniter to the GDI object to be deleted</param>
      /// <returns></returns>
      [DllImport("gdi32")]
      private static extern int DeleteObject(IntPtr o);

      /// <summary>
      /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
      /// </summary>
      /// <param name="image">The Emgu CV Image</param>
      /// <returns>The equivalent BitmapSource</returns>
      public static BitmapSource ToBitmapSource(IImage image) {
         using (Bitmap source = image.Bitmap) {
            IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

            BitmapSource bs = Imaging.CreateBitmapSourceFromHBitmap(
                ptr,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );

            DeleteObject(ptr); //release the HBitmap
            return bs;
         }
      }
   }
}