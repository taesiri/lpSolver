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
using LinearProgramming.Highlighter;
using Microsoft.Win32;

namespace LinearProgramming.Controls
{
    /// <summary>
    /// Interaction logic for TextEditorControl.xaml
    /// </summary>
    public partial class TextEditorControl
    {
        private readonly FoldingManager _foldingManager;
        private readonly AbstractFoldingStrategy _foldingStrategy;
        private bool _isTextChanged;

        public TextEditorControl(string name, int index)
        {
            InitializeComponent();

            FileName = name;
            FileIndex = index;

            var foldingUpdateTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(2)};
            foldingUpdateTimer.Tick += FoldingUpdateTimerTick;
            foldingUpdateTimer.Start();

            if (File.Exists(@"Highlighter\CustomHighlighting.xshd"))
            {
                var fileStream = new FileStream(@"Highlighter\CustomHighlighting.xshd", FileMode.Open, FileAccess.Read);
                using (var reader = new XmlTextReader(fileStream))
                {
                    textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
                fileStream.Close();
            }

            textEditor.TextArea.IndentationStrategy =
                new CSharpIndentationStrategy(textEditor.Options);
            _foldingStrategy = new BEFoldingStrategy();

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

            textEditor.Text = "# Welcome to lpSolver (Linear Programming Solver)!" + Environment.NewLine;
            textEditor.Text += "lpmodel ModelName" + FileIndex.ToString() + " \n{" + Environment.NewLine;
            textEditor.Text += string.Format("\t# TODO : Objectives {0}{0}", Environment.NewLine + "\t");

            textEditor.Text += "min 10*x + 20*y" + Environment.NewLine;
            textEditor.Text += "\tsubject to:" + Environment.NewLine;
            textEditor.Text += "\tx + y > 10;" + Environment.NewLine;
            textEditor.Text += "\t0<x;" + Environment.NewLine;
            textEditor.Text += "\t0<y;" + Environment.NewLine;
            textEditor.Text += Environment.NewLine + "\t";
            textEditor.Text += Environment.NewLine;
            textEditor.Text += "};" + Environment.NewLine;
            textEditor.Text += "# __EOF" + Environment.NewLine;

            textEditor.ShowLineNumbers = true;
            FileUrl = string.Empty;
        }

        public TextEditorControl(string name, int index, string code)
        {
            InitializeComponent();

            FileName = name;
            FileIndex = index;

            var foldingUpdateTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(2)};
            foldingUpdateTimer.Tick += FoldingUpdateTimerTick;
            foldingUpdateTimer.Start();

            if (File.Exists(@"Highlighter\CustomHighlighting.xshd"))
            {
                var fileStream = new FileStream(@"Highlighter\CustomHighlighting.xshd", FileMode.Open, FileAccess.Read);
                using (var reader = new XmlTextReader(fileStream))
                {
                    textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
                fileStream.Close();
            }

            textEditor.TextArea.IndentationStrategy =
                new CSharpIndentationStrategy(textEditor.Options);
            _foldingStrategy = new BEFoldingStrategy();

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

            textEditor.Text = code;

            textEditor.ShowLineNumbers = true;
            FileUrl = string.Empty;
        }

        public bool DocumentHasChanged { get; set; }

        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public int FileIndex { get; set; }

        public IEnumerable<String> Lines
        {
            get
            {
                //Collecting Lines
                List<string> code = textEditor.Document.Lines.Select(
                    line => textEditor.Text.Substring(line.Offset, line.Length))
                    .ToList();

                return code;
            }
        }

        public bool IsTextChanged
        {
            get
            {
                return _isTextChanged;
            }
        }

        private void FoldingUpdateTimerTick(object sender, EventArgs e)
        {
            if (_foldingStrategy != null)
            {
                _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);
            }
        }

        private void UserControlGotFocus1(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.dockManager.ActiveContent = Parent;
        }

        public void SelectTextAtPosition(int pos)
        {
            textEditor.Select(pos, 1);
        }

        public void DoClose()
        {
            //Check for Changes and Save Dialog

            if (DocumentHasChanged)
            {
                //Document needs to be SAVEd!
                MessageBoxResult result = MessageBox.Show("", "", MessageBoxButton.YesNoCancel);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        DoSave();
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                    case MessageBoxResult.No:
                        return;
                    default:
                        return;
                }
            }

            MessageBox.Show("");
        }

        public void DoSave()
        {
            string code = string.Empty;
            IEnumerable<string> lines = Lines;
            code = lines.Aggregate(code, (current, l) => current + (l + "\n"));

            if (FileUrl == string.Empty)
            {
                var dlg = new SaveFileDialog {Filter = "lsSolver Model File (*.lps)|*.lps"};
                if ((bool) dlg.ShowDialog())
                {
                    FileUrl = dlg.FileName;
                    Save(code);
                }
            }
            else
            {
                Save(code);
            }
        }

        private void Save(string codes)
        {
            try
            {
                var strWriter = new StreamWriter(FileUrl, false);
                strWriter.Write(codes);
                strWriter.Close();

                DocumentHasChanged = false;
            }
            catch (Exception exp)
            {
                //Sucks! this is not event standard way to Log error! Consider ReWriting the Log,ErrorLog Control | System
                MainWindow.Instance.statusBarControl.State = new LPSolverState("Could not Save file",
                                                                               EnumEditorStates.Error)
                                                                 {CurrentMessage = exp.Message};
            }
        }

        public void DoSaveAs()
        {
            string tmp = FileUrl;
            FileUrl = string.Empty;
            try
            {
                DoSave();
            }
            finally
            {
                FileUrl = tmp;
            }
        }

        private void TextEditorTextChanged(object sender, EventArgs e)
        {
            DocumentHasChanged = true;
            _isTextChanged = true;
        }

        public void TextParsed()
        {
            _isTextChanged = false;
        }
    }
}