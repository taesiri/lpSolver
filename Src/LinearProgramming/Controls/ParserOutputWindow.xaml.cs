using System.Windows;

namespace LinearProgramming.Controls
{
    /// <summary>
    /// Interaction logic for ParserOutputWindow.xaml
    /// </summary>
    public partial class ParserOutputWindow : Window
    {
        public static ParserOutputWindow Instance;

        public ParserOutputWindow()
        {
            InitializeComponent();
            Instance = this;
        }

        public void AddErrorLog(ErrLog log)
        {
            parserOutputControl.LogError(log);
        }

        public void ClearLogs()
        {
            parserOutputControl.ClearLogs();
        }

        private void BtnClearOutputClicked(object sender, RoutedEventArgs e)
        {
            ClearLogs();
        }
    }
}