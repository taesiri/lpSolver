using System;
using System.Collections.Generic;
using LinearProgramming.Model;
using Microsoft.SolverFoundation.Services;

namespace LinearProgramming.Solver
{
    public class MicrosoftSolverFoundation : IModelSolver
    {
        private readonly LPModel _dataModel;
        private bool _isSolved;
        private string _result;

        public MicrosoftSolverFoundation(LPModel model)
        {
            _dataModel = new LPModel(model);
        }

        #region IModelSolver Members

        public void TrySolve()
        {
            try
            {
                SolverContext context = SolverContext.GetContext();
                context.ClearModel();
                Microsoft.SolverFoundation.Services.Model model = context.CreateModel();

                var vDecisions = new List<Decision>();
                List<string> vars = _dataModel.GetVariables;
                foreach (string variable in vars)
                {
                    var d = new Decision(Domain.RealNonnegative, variable);
                    vDecisions.Add(d);
                }
                foreach (Decision decision in vDecisions)
                {
                    model.AddDecision(decision);
                }

                List<string> consts = _dataModel.GetConstraint;
                int counter = 0;
                foreach (string line in consts)
                {
                    counter++;
                    if (!String.IsNullOrEmpty(line))
                    {
                        model.AddConstraint("Constraint" + counter, line);
                    }
                }


                GoalKind gKind;
                Enum.TryParse(_dataModel.GoalKind.ToString(), true, out gKind);

                //Add Goal
                model.AddGoal("Answer", gKind, _dataModel.Objective);

                Solution solution = context.Solve(new SimplexDirective());
                Report report = solution.GetReport();
                string result = "";

                foreach (Decision dec in vDecisions)
                {
                    result +=
                        String.Format("Variable : {0} - Optimal Value : {1}", dec.Name, dec) +
                        Environment.NewLine;
                }

                result += Environment.NewLine + report;
                context.ClearModel();
                _result = result;
                _isSolved = true;
            }
            catch (Exception exp)
            {
                throw new Exception("Solver - An Error has been occurred during solving model!");
            }
        }

        public string GetResult()
        {
            if (_isSolved)
                return _result;
            throw new Exception("The Model is not Solved Yet!");
        }

        public SolvedData GetSolvedData()
        {
            if (_isSolved)
                return new SolvedData();
            throw new Exception("The Model is not Solved Yet!");
        }

        #endregion
    }
}