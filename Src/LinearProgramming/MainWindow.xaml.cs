using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using AvalonDock.Layout;
using Irony.Parsing;
using LinearProgramming.Grammar;
using LinearProgramming.Model;
using LinearProgramming.Parser;
using Microsoft.Win32;

namespace LinearProgramming
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow Instance;
        private readonly Irony.Parsing.Parser _parser;
        private string _currentFileName;
        private int _documentCounter = 1;


        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            _parser = new Irony.Parsing.Parser(new LinearProgrammingGrammar());
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
                if (MessageBox.Show("?", "Solving the problem ?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    string strSource = editor.Lines.Aggregate("",
                                                              (current, line) =>
                                                              current +
                                                              (line.ToString(CultureInfo.InvariantCulture) +
                                                               Environment.NewLine));
                    TryParse(strSource);

                    //try
                    //{
                    //    IModelSolver solver = new MicrosoftSolverFoundation(LPModel.TryParse((List<string>) editorText));
                    //    solver.TrySolve();
                    //    string result = solver.GetResult();
                    //    MessageBox.Show(result, "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    //catch (Exception exp)
                    //{
                    //    MessageBox.Show("An Error Occurred in Solving or Parsing the Code!");
                    //}
                }
            }
        }


        //Begin Parser Codes

        public void TryParse(string source)
        {
            try
            {
                ParseTree parseTree = _parser.Parse(source);

                LPModel output = Modeler.ConvertParseTreeToModel(parseTree);

                output.ToString();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Parser Error");
            }
        }


        //End Parser Codes


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