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

namespace LinearProgramming.Controls
{
    /// <summary>
    /// Interaction logic for TextEditorControl.xaml
    /// </summary>
    public partial class TextEditorControl
    {
        private readonly FoldingManager _foldingManager;
        private readonly AbstractFoldingStrategy _foldingStrategy;

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

            textEditor.Text = "# Welcome to Linear Programming Solver!" + Environment.NewLine;
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
        }

        public string FileName { get; set; }
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
    }
}