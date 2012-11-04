using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using LinearProgramming.Parser;
using LinearProgramming.Solver;
using Microsoft.Win32;

namespace LinearProgramming
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly FoldingManager _foldingManager;
        private readonly AbstractFoldingStrategy _foldingStrategy;
        private string _currentFileName;

        public MainWindow()
        {
            InitializeComponent();

            var foldingUpdateTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(2)};
            foldingUpdateTimer.Tick += FoldingUpdateTimerTick;
            foldingUpdateTimer.Start();

            var fileStream = new FileStream(@"Highlighter\CustomHighlighting.xshd", FileMode.Open, FileAccess.Read);
            using (var reader = new XmlTextReader(fileStream))
            {
                textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
            fileStream.Close();

            textEditor.TextArea.IndentationStrategy =
                new CSharpIndentationStrategy(textEditor.Options);
            _foldingStrategy = new BraceFoldingStrategy();

            if (_foldingStrategy != null)
            {
                if (_foldingManager == null)
                    _foldingManager = FoldingManager.Install(textEditor.TextArea);
                _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);
            }
            else
            {
                if (_foldingManager != null)
                {
                    FoldingManager.Uninstall(_foldingManager);
                    _foldingManager = null;
                }
            }

            textEditor.Text = "# Welcome to LP Solver using Microsoft Solver Foundation" + Environment.NewLine;
            textEditor.Text += "Begin" + Environment.NewLine;
            textEditor.Text += string.Format("\t# TODO : Objectives {0}{0}", Environment.NewLine + "\t");
            textEditor.Text += Environment.NewLine;
            textEditor.Text += "End" + Environment.NewLine;
            textEditor.Text += "# __EOF" + Environment.NewLine;

            textEditor.ShowLineNumbers = true;
        }


        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog {CheckFileExists = true};
            if ((bool) dlg.ShowDialog())
            {
                _currentFileName = dlg.FileName;
                textEditor.Load(_currentFileName);
            }
        }

        private void SaveFileClick(object sender, EventArgs e)
        {
            if (_currentFileName == null)
            {
                var dlg = new SaveFileDialog {DefaultExt = ".cslp"};
                if ((bool) dlg.ShowDialog())
                {
                    _currentFileName = dlg.FileName;
                }
                else
                {
                    return;
                }
            }
            textEditor.Save(_currentFileName);
        }

        private void FoldingUpdateTimerTick(object sender, EventArgs e)
        {
            if (_foldingStrategy != null)
            {
                _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);
            }
        }

        private void BtnSolveClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> programCode =
                    textEditor.Document.Lines.Select(line => textEditor.Text.Substring(line.Offset, line.Length)).ToList
                        ();
                //Cleaning the Code
                programCode = ParserHelper.ClearUpTheCode(programCode);
                var solver = new SolverEngine(LPModel.TryParse(programCode));
                string result = solver.SolveIt();
                MessageBox.Show(result, "Result", MessageBoxButton.OK, MessageBoxImage.Information);

                //solver.Destry();
            }
            catch (Exception exp)
            {
                MessageBox.Show("An Error Occurred in Solving or Parsing the Code!");
            }
        }
    }
}