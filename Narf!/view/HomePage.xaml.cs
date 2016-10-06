using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.Win32;
using Narf.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Narf.View {
  /// <summary>
  /// Lógica de interacción para HomePage.xaml
  /// </summary>
  public partial class HomePage : Page {
    protected NewCasePage _newCasePage;
    List<Case> Cases { get; set; }
    public NewCasePage NewCasePage {
      get {
        return _newCasePage;
      }
      protected set {
        _newCasePage = value;
      }
    }
    public HomePage() {
      InitializeComponent();
      testInit();
    }

    private void testInit() {
      Mat image = new Mat(100, 400, DepthType.Cv8U, 3);
      image.SetTo(new Bgr(255, 255, 255).MCvScalar);
      CvInvoke.PutText(image, "Hello, world", new System.Drawing.Point(10, 50),
                       FontFace.HersheyPlain, 3.0,
                       new Bgr(255.0, 0.0, 0.0).MCvScalar);
      previewDisplay.Image = image;

      Cases = new List<Case>();
      Cases.Add(new Case() {
        Date = new DateTime(2016, 10, 12),
        Substance = "SSRI", Rat = "Pinky",
        MazeType = MazeType.None
      });
      Cases.Add(new Case() {
        Date = new DateTime(2016, 10, 3),
        Substance = "THC", Rat = "Cerebro",
        MazeType = MazeType.Cross
      });
      Cases.Add(new Case() {
        Date = new DateTime(2016, 9, 22),
        Substance = "SNDRA", Rat = "Pinky",
        MazeType = MazeType.Cross
      });
      casesList.ItemsSource = Cases;

      CollectionView view = (CollectionView) CollectionViewSource.
          GetDefaultView(casesList.ItemsSource);
      PropertyGroupDescription description =
          new PropertyGroupDescription("MazeType");
      view.GroupDescriptions.Add(description);
    }

    private void newBttn_Click(object sender, RoutedEventArgs args) {
      OpenFileDialog fileDialog = new OpenFileDialog();
      fileDialog.Multiselect = false;
      fileDialog.DefaultExt = ".avi";
      fileDialog.Filter = "Archivos AVI (*.avi)|*.avi | " + 
        "Archivos MP4 (*.mp4)|*.mp4";
      bool? result = fileDialog.ShowDialog();
      if(result == true) {
        NewCasePage = new NewCasePage(fileDialog.FileName);
        NavigationService.Navigate(NewCasePage);
      }
    }
  }
}
