using LinearProgramming.Model;

namespace LinearProgramming.Controls
{
    /// <summary>
    /// Interaction logic for OutlineWindow.xaml
    /// </summary>
    public partial class OutlineWindow
    {
        public OutlineWindow()
        {
            InitializeComponent();
        }

        public void SetLpName(string name)
        {
            lpOutlineControl.SetLPName(name);
        }

        public void AddVariable(string variable)
        {
            lpOutlineControl.AddVariable(variable);
        }

        public void AddConstraint(LPConstraint constraint)
        {
            lpOutlineControl.AddConstraint(constraint);
        }

        public void ClearWindow()
        {
            lpOutlineControl.Variables.Clear();
            lpOutlineControl.Constraints.Clear();
        }
    }
}