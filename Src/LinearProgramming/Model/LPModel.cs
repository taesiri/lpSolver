using System.Collections.Generic;
using System.Linq;

namespace LinearProgramming.Model
{
    public class LPModel
    {
        private readonly List<LPConstraint> _constraint;


        public LPModel(LPModel otherModel)
        {
            _constraint = new List<LPConstraint>(otherModel.GetConstraint);

            GoalKind = otherModel.GoalKind;
            Objective = otherModel.Objective;
            Name = otherModel.Name;

        }

        public LPModel()
        {
            _constraint = new List<LPConstraint>();

            GoalKind = null;
            Objective = new LPPolynomial();
            Name = "NewModel";
        }

        public LPPolynomial Objective { get; set; }

        public List<string> Variables
        {
            get { return Objective.GetVariables; }
        }

        public LPGoalType? GoalKind { get; set; }

        public string Name { get; set; }

        public List<LPConstraint> GetConstraint
        {
            get { return _constraint; }
        }

        public List<string> GetConstraintStringList
        {
            get { return _constraint.Select(constraint => constraint.ToString()).ToList(); }
        }
    }
}