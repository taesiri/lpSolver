using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using AvalonDock.Layout;
using Irony;
using Irony.Parsing;
using LinearProgramming.Controls;
using LinearProgramming.Grammar;
using LinearProgramming.LPReport;
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
        private readonly DispatcherTimer _parserTimer;
        private int _documentCounter = 1;


        private TextEditorControl _lastActiveDocument;
        private ParseTree _parseTree;


        public MainWindow()
        {
            InitializeComponent();

            Instance = this;

            _parser = new Irony.Parsing.Parser(new LinearProgrammingGrammar());


            _parserTimer = new DispatcherTimer {Interval = new TimeSpan(5*1000)};
            _parserTimer.Tick += ParserTimerTick;
        }

        private void ParserTimerTick(object sender, EventArgs e)
        {
            ParseCurrentTab();
        }

        private void NewFileClick(object sender, RoutedEventArgs e)
        {
            CreateNewFile();
        }

        private void CreateNewFile()
        {
            var editorDocument = new LayoutDocument {Title = "Linear Program Problem " + _documentCounter};
            editorDocument.Content = new TextEditorControl(editorDocument.Title, _documentCounter);
            editorDocument.Closing += EditorDocumentClosing;
            CenterDockPane.Children.Add(editorDocument);
            _documentCounter++;

            _parserTimer.Start();
        }

        private void EditorDocumentClosing(object sender, CancelEventArgs e)
        {
            var textEditor = ((LayoutDocument) sender).Content as TextEditorControl;
            if (textEditor != null)
            {
                if (Equals(_lastActiveDocument, textEditor))
                {
                    _lastActiveDocument = null;

                    lpOutlineControl.ClearWindow();
                    parserErrorControl.ClearLogs();
                }
            }
        }


        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog {CheckFileExists = true, Filter = "{LP Solver Files (*.lps)|*.lps"};
            if ((bool) dlg.ShowDialog())
            {
                //_currentFileName = dlg.FileName;
                ////textEditor.Load(_currentFileName);
                //_parserTimer.Start();
            }
        }

        private void SaveFileClick(object sender, EventArgs e)
        {
            if (_lastActiveDocument != null)
            {
                _lastActiveDocument.DoSave();
            }
        }

        private void SaveAsFileClick(object sneder, EventArgs e)
        {
            if (_lastActiveDocument != null)
            {
                _lastActiveDocument.DoSaveAs();
            }
        }

        private void CloseCurrentDocument(object sender, RoutedEventArgs e)
        {
            if (_lastActiveDocument != null)
            {
                // TODO : Close Current Active Document
                _lastActiveDocument.DoClose();
            }
        }

        private void BtnSolveClicked(object sender, RoutedEventArgs e)
        {
            ParseCurrentTab();


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

            if (MessageBox.Show("Do you want to Generate Report", "Report ?", MessageBoxButton.YesNo) ==
                MessageBoxResult.Yes)
            {
                var reportWindow = new LPReportWindow();
                reportWindow.Show();
            }
        }


        //Begin Parser Codes

        public void ParseCurrentTab()
        {
            var editor = dockManager.ActiveContent as TextEditorControl;
            if (editor != null)
            {
                string code = string.Empty;

                IEnumerable<string> lines = editor.Lines;

                code = lines.Aggregate(code, (current, l) => current + (l + "\n"));

                TryParse(code);
            }
        }

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
                    UpdateOutlineWindow();
                    statusBarControl.State = new LPSolverState("Done.", EnumEditorStates.Normal);
                }
                //else
                //{
                //    throw new Exception("Could not Parse the Model. Please Check Error List");
                //}
            }
            catch (Exception exp)
            {
                statusBarControl.State = new LPSolverState("Parser Error - " + exp.Message, EnumEditorStates.Error);
            }
            finally
            {
                _parseTree = _parser.Context.CurrentParseTree;
                ShowCompilerErrors();
            }
        }

        //End Parser Codes

        private void UpdateOutlineWindow()
        {
            lpOutlineControl.ClearWindow();
            LPModel output = Modeler.ConvertParseTreeToModel(_parseTree);
            lpOutlineControl.SetLPName(String.Format("{0} : {1}", "Model Name", output.Name));
            lpOutlineControl.SetObjective(String.Format("{0}", output.Objective));
            List<LPConstraint> constraints = output.GetConstraint;
            foreach (LPConstraint lpConstraint in constraints)
            {
                lpOutlineControl.AddConstraint(lpConstraint);
            }
            List<string> variables = output.Variables;
            foreach (string variable in variables)
            {
                lpOutlineControl.AddVariable(variable);
            }
        }

        private void ShowCompilerErrors()
        {
            parserErrorControl.ClearLogs();
            if (_parseTree == null || _parseTree.ParserMessages.Count == 0) return;
            foreach (LogMessage err in _parseTree.ParserMessages)
            {
                parserErrorControl.LogError(new ErrLog(err.Message, String.Format("Line {0} at {1}", err.Location.Line,
                                                                                  err.Location.Position),
                                                       err.ParserState.ToString(), err));
            }
            statusBarControl.State = new LPSolverState("Could not Parse the Model. Please Check Error List",
                                                       EnumEditorStates.Error);
        }


        public void SelectTextBoxAtPosition(int pos)
        {
            if (_lastActiveDocument != null)
            {
                _lastActiveDocument.SelectTextAtPosition(pos);
            }
        }

        private void MnuItemExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        public void ShowErrorListControl()
        {
            ParserErrorPane.Show();
            ParserErrorPane.IsActive = true;
        }

        public void ShowOutlineControl()
        {
            OutlinePane.Show();
            OutlinePane.IsActive = true;
        }

        private void MnuErrorListClick(object sender, RoutedEventArgs e)
        {
            ShowErrorListControl();
        }

        private void MnuOutlineClick(object sender, RoutedEventArgs e)
        {
            ShowOutlineControl();
        }

        private void DockManagerActiveContentChanged(object sender, EventArgs e)
        {
            var d = dockManager.ActiveContent as TextEditorControl;
            if (d != null)
                _lastActiveDocument = d;
        }


        private void BtnToggleAutoParserClick(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton) != null && (sender as ToggleButton).IsChecked == true)
            {
                _parserTimer.IsEnabled = true;
            }
            else if ((sender as ToggleButton) != null && (sender as ToggleButton).IsChecked == false)
            {
                _parserTimer.IsEnabled = false;
            }
        }
    }
}