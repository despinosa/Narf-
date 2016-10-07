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

      CollectionView view = (CollectionView)CollectionViewSource.
          GetDefaultView(casesList.ItemsSource);
      PropertyGroupDescription description =
          new PropertyGroupDescription("Maze");
      view.GroupDescriptions.Add(description);
    }

    private void testInit() {
      Mat image = new Mat(270, 465, DepthType.Cv8U, 3);
      image.SetTo(new Bgr(111, 111, 111).MCvScalar);
      CvInvoke.PutText(image, "Preview", new System.Drawing.Point(10, 50),
                       FontFace.HersheyPlain, 3.0,
                       new Bgr(255.0, 0.0, 0.0).MCvScalar);
      previewDisplay.Image = image;

      Cases = new List<Case>();
      Cases.Add(new Case() {
        Date = new DateTime(2016, 10, 12), Duration = new TimeSpan(0, 4, 46),
        Substance = "SSRI", Dose = 5M, Subject = "Pinky", Weight = 228.7M,
        Maze = Maze.None
      });
      Cases.Add(new Case() {
        Date = new DateTime(2016, 7, 17), Duration = new TimeSpan(0, 4, 53),
        Substance = "SNDRA", Dose = 12M, Subject = "Pinky", Weight = 246.9M,
        Maze = Maze.Cross
      });
      Cases.Add(new Case() {
        Date = new DateTime(2016, 10, 3), Duration = new TimeSpan(0, 5, 12),
        Substance = "THC", Dose = 10M, Subject = "Cerebro", Weight = 242.4M,
        Maze = Maze.Cross
      });
      Cases.Add(new Case() {
        Date = new DateTime(2016, 8, 30), Duration = new TimeSpan(0, 5, 8),
        Substance = "SSRI", Dose = 9M, Subject = "Cerebro", Weight = 231.7M,
        Maze = Maze.None
      });
      Cases.Add(new Case() {
        Date = new DateTime(2016, 9, 22), Duration = new TimeSpan(0, 5, 3),
        Substance = "SNDRA", Dose = 7M, Subject = "Pinky", Weight = 266.4M,
        Maze = Maze.Cross
      });
      casesList.ItemsSource = Cases;
    }

    private void newCaseBttn_Click(object sender, RoutedEventArgs args) {
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

    private void delCaseBttn_Click(object sender, RoutedEventArgs e) {
      MessageBoxResult? result = MessageBox.Show("¿Eliminar caso de estudio?",
                                                 "Eliminar caso",
                                                 MessageBoxButton.YesNo,
                                                 MessageBoxImage.Exclamation);
      if (result == MessageBoxResult.Yes) {
      }
    }
  }
}
