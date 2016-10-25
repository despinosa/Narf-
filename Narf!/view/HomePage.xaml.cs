using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.WPF;
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
  partial class HomePage : Page {
    static readonly short MAX_CAPTURES = 3;
    ICollection<Case> Cases { get; set; }
    NewCasePage NewCasePage { get; set; }
    Entities Session { get; }

    public HomePage() {
      InitializeComponent();

      Session = new Entities();
      RefreshCases();
    }

    void RefreshCases() {
      Cases = new HashSet<Case>(Session.Cases);
      casesList.ItemsSource = Cases;
      var view = (CollectionView)CollectionViewSource.
          GetDefaultView(casesList.ItemsSource);
      var description = new PropertyGroupDescription("Maze");
      view.GroupDescriptions.Add(description);
      casesList.ItemsSource = Cases;
    }

    void newCaseBttn_Click(object sender, RoutedEventArgs args) {
      var fileDialog = new OpenFileDialog();
      fileDialog.Multiselect = true;
      fileDialog.DefaultExt = ".avi";
      fileDialog.Filter = "Archivos AVI (*.avi)|*.avi|" + 
        "Archivos MP4 (*.mp4)|*.mp4";
      bool? result = fileDialog.ShowDialog();
      if (result == true) {
        if (fileDialog.FileNames.Length > MAX_CAPTURES) {
          MessageBox.Show("Sólo se pueden procesar hasta " + MAX_CAPTURES +
                          "archivos", "Demasiados archivos fuente",
                          MessageBoxButton.OK, MessageBoxImage.Error);
        }
        Session.SaveChanges();
        NewCasePage = new NewCasePage(fileDialog.FileNames, Session);
        NavigationService.Navigate(NewCasePage);
      }
    }

    void delCaseBttn_Click(object sender, RoutedEventArgs args) {
      var result = MessageBox.Show("¿Eliminar caso de estudio?",
                                   "Eliminar caso", MessageBoxButton.YesNo,
                                   MessageBoxImage.Exclamation);
      if (result == MessageBoxResult.Yes) {
        foreach (var selected in casesList.SelectedItems) {
          Session.Cases.Remove((Case)selected);
        }
        Session.SaveChanges();
        RefreshCases();
      }
    }

    void casesList_SelectionChanged(object sender, RoutedEventArgs args) {
      var selected = (Case)casesList.SelectedItem;
      // TO DO
    }
  }
}
