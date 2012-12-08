using System.Collections.ObjectModel;
using System.Windows.Controls;
using LinearProgramming.Model;

namespace LinearProgramming.Controls
{
    /// <summary>
    /// Interaction logic for LPOutline.xaml
    /// </summary>
    public partial class LPOutline : UserControl
    {
        public ObservableCollection<ConstraintList> Constraints = new ObservableCollection<ConstraintList>();
        public ObservableCollection<VariableList> Variables = new ObservableCollection<VariableList>();

        public LPOutline()
        {
            InitializeComponent();
        }

        public ObservableCollection<VariableList> GetVariables
        {
            get { return Variables; }
        }

        public ObservableCollection<ConstraintList> GetConstraints
        {
            get { return Constraints; }
        }

        public void SetLPName(string lpName)
        {
            TVIName.Header = lpName;
        }

        public void AddVariable(string variable)
        {
            Variables.Add(new VariableList(variable));
        }

        public void AddConstraint(LPConstraint constraint)
        {
            Constraints.Add(new ConstraintList(constraint));
        }
        public void ClearWindow()
        {
            Constraints.Clear();
            Variables.Clear();
        }
    }

    public class ConstraintList
    {
        private readonly string _str = "";
        public ConstraintList(LPConstraint data)
        {
            InsideConstraint = data;
            _str = data.ToString();
        }

        public string GetConstraintString
        {
            get { return _str; }
        }

        public LPConstraint InsideConstraint { get; set; }
    }

    public class VariableList
    {
        public VariableList(string data)
        {
            VariableName = data;
        }

        public string GetVarName
        {
            get { return VariableName; }
        }

        public string VariableName { get; set; }
    }
}