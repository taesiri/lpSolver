using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using AvalonDock;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using Irony;
using Irony.Parsing;
using LinearProgramming.Controls;
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
        private ParseTree _parseTree;

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


        //Begin Parser Codes

        public void TryParse(string source)
        {

            //ClearParserOutput();
            if (_parser == null || !_parser.Language.CanParse()) return;
            _parseTree = null;
            GC.Collect(); //to avoid disruption of perf times with occasional collections
            //_parser.Context.TracingEnabled = chkParserTrace.Checked;

            try
            {
                _parseTree = _parser.Parse(source);
                if (_parseTree.HasErrors() == false)
                {
                    lpOutlineW.ClearWindow();
                    LPModel output = Modeler.ConvertParseTreeToModel(_parseTree);
                    lpOutlineW.SetLPName(output.Name);
                    List<LPConstraint> constraints = output.GetConstraint;
                    foreach (LPConstraint lpConstraint in constraints)
                    {
                        lpOutlineW.AddConstraint(lpConstraint);
                    }
                    List<string> variables = output.Variables;
                    foreach (string variable in variables)
                    {
                        lpOutlineW.AddVariable(variable);
                    }
                }
            }
            catch (Exception exp)
            {
                //gridCompileErrors.Rows.Add(null, ex.Message, null);
                //tabBottom.SelectedTab = pageParserOutput;
                MessageBox.Show(exp.Message, "Parser Error");
            }
            finally
            {
                _parseTree = _parser.Context.CurrentParseTree;
                ShowCompilerErrors();
                //if (chkParserTrace.Checked)
                //{
                //    ShowParseTrace();
                //}
                //ShowCompileStats();
                //ShowParseTree();
                //ShowAstTree();
                MessageBox.Show("Parsing Completed!");
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

        private void ShowCompilerErrors()
        {
            parserErrorW.ClearLogs();
            if (_parseTree == null || _parseTree.ParserMessages.Count == 0) return;
            foreach (LogMessage err in _parseTree.ParserMessages)
            {
                parserErrorW.LogError(new ErrLog(err.Message, String.Format("Line {0} at {1}", err.Location.Line,
                                                                            err.Location.Position),
                                                 err.ParserState.ToString()));
            }
        }

        private void MnuItemExitClick(object sender, RoutedEventArgs e)
        {
          
            Application.Current.Shutdown(0);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LPOutlineAncher.Show();
        }




       

    }
}