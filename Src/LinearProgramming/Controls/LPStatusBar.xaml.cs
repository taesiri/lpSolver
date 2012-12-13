using System.Windows.Input;
using System.Windows.Media;

namespace LinearProgramming.Controls
{
    /// <summary>
    /// Interaction logic for LPStatusBar.xaml
    /// </summary>
    public partial class LPStatusBar
    {
        private LPSolverState _state;

        public LPStatusBar()
        {
            InitializeComponent();
            State = new LPSolverState();
        }

        public LPSolverState State
        {
            get { return _state; }
            set
            {
                _state = value;
                if (value.CurrentState == EnumEditorStates.Error)
                {
                    lblState.Content = string.Format("Error: {0}", _state.CurrentMessage);
                    Background = Brushes.OrangeRed;
                }
                else if (value.CurrentState == EnumEditorStates.Normal)
                {
                    lblState.Content = string.Format("{0}", _state.CurrentMessage);
                    Background = Brushes.DodgerBlue;
                }
                else if (value.CurrentState == EnumEditorStates.Warning)
                {
                    lblState.Content = string.Format("{0}", _state.CurrentMessage);
                    Background = Brushes.GreenYellow;
                }
            }
        }

        private void StatusBarDoubleClick(object sender, MouseButtonEventArgs e)
        {
            switch (_state.CurrentState)
            {
                case EnumEditorStates.Normal:

                    break;
                case EnumEditorStates.Error:
                    MainWindow.Instance.ShowErrorListControl();
                    break;
                case EnumEditorStates.Warning:
                    break;
            }
        }
    }
}