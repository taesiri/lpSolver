using System.Collections.ObjectModel;

namespace LinearProgramming.Controls
{
    /// <summary>
    /// Interaction logic for ParserOutput.xaml
    /// </summary>
    public partial class ParserOutput
    {
        public ObservableCollection<ErrLog> ECollections = new ObservableCollection<ErrLog>();

        public ParserOutput()
        {
            InitializeComponent();
        }

        public ObservableCollection<ErrLog> GetList
        {
            get { return ECollections; }
        }

        public void LogError(ErrLog log)
        {
            ECollections.Add(log);
        }
        public void ClearLogs()
        {
            ECollections.Clear();
        }
    }
}