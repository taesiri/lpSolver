using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using AvalonDock.Layout;
using LinearProgramming.Model;
using LinearProgramming.Solver;
using Microsoft.Win32;

namespace LinearProgramming
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow Instance;
        private string _currentFileName;
        private int _documentCounter = 1;


        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
        }

        private void NewFileClick(object sender, RoutedEventArgs e)
        {
            CreateNewFile();
        }

        private void CreateNewFile()
        {
            var editorDocument = new LayoutDocument {Title = "Linear Program Problem " + _documentCounter.ToString()};
            editorDocument.Content = new TextEditorControl {FileName = editorDocument.Title};
            CenterDockPane.Children.Add(editorDocument);
            _documentCounter++;
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog {CheckFileExists = true, Filter = "{LP Solver Files (*.lps)|*.lps"};
            if ((bool) dlg.ShowDialog())
            {
                _currentFileName = dlg.FileName;
                //textEditor.Load(_currentFileName);
            }
        }

        private void SaveFileClick(object sender, EventArgs e)
        {
            if (_currentFileName == null)
            {
                var dlg = new SaveFileDialog {DefaultExt = ".lps"};
                if ((bool) dlg.ShowDialog())
                {
                    _currentFileName = dlg.FileName;
                }
                else
                {
                    return;
                }
            }
            //textEditor.Save(_currentFileName);
        }


        private void BtnSolveClicked(object sender, RoutedEventArgs e)
        {
            var editor = dockManager.ActiveContent as TextEditorControl;
            if (editor != null)
            {
                IEnumerable<string> editorText = editor.Lines;

                if (MessageBox.Show("?", "Solving the problem ?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        IModelSolver solver = new MicrosoftSolverFoundation(LPModel.TryParse((List<string>) editorText));
                        solver.TrySolve();
                        string result = solver.GetResult();
                        MessageBox.Show(result, "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show("An Error Occurred in Solving or Parsing the Code!");
                    }
                }
            }
        }

        private void LayoutDocumentClosing1(object sender, CancelEventArgs e)
        {
            // Here!
            e.Cancel = true;
        }

        public void UpdateDocumentOutline()
        {
            ////CenterDockPane.Children.Add("");\

            ////var ld = new LayoutDocument();
            ////ld.Title = "Title here!";
            ////ld.Content = new TextEditorControl();
            ////CenterDockPane.Children.Add(ld);
            ////CenterDockPane.Children.Add(new LayoutDocument()
            ////{
            ////    Title = "Classes",

            ////});

            ////foreach (var variable in docS)
            ////{

            ////}

            ////var editor = dockManager.ActiveContent as TextEditorControl;
            ////if (editor != null)
            ////{
            ////    //docOutline.DoUpdate(editor.Lines);
            ////}
        }
    }
}