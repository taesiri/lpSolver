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
using LinearProgramming.Parser;

namespace LinearProgramming
{
    /// <summary>
    /// Interaction logic for TextEditorControl.xaml
    /// </summary>
    public partial class TextEditorControl
    {
        private readonly FoldingManager _foldingManager;
        private readonly AbstractFoldingStrategy _foldingStrategy;
        public string FileName { get; set; }

        public TextEditorControl()
        {
            InitializeComponent();

            var foldingUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
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

            textEditor.Text = "# Welcome to Linear Programming Solver!" + Environment.NewLine;
            textEditor.Text += "Begin LP Problem Name" + Environment.NewLine;
            textEditor.Text += string.Format("\t# TODO : Objectives {0}{0}", Environment.NewLine + "\t");
            textEditor.Text += Environment.NewLine;
            textEditor.Text += "End" + Environment.NewLine;
            textEditor.Text += "# __EOF" + Environment.NewLine;

            textEditor.ShowLineNumbers = true;
        }

        public IEnumerable<String> Lines
        {
            get
            {
                //Collecting Lines
                var pureCode = textEditor.Document.Lines.Select(
                    line => textEditor.Text.Substring(line.Offset, line.Length))
                    .ToList();

                //Cleaning the Code
                pureCode = ParserHelper.ClearUpTheCode(pureCode);

                return pureCode;
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
            MainWindow.Instance.UpdateDocumentOutline();
        }
    }
}