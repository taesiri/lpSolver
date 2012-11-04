using System;
using System.Collections.Generic;
using LinearProgramming.Parser;
using Microsoft.SolverFoundation.Services;

namespace LinearProgramming.Solver
{
    public class SolverEngine
    {
        private readonly LPModel _model;

        public SolverEngine(LPModel model)
        {
            _model = new LPModel(model);
        }

        public string SolveIt()
        {
            try
            {
                SolverContext context = SolverContext.GetContext();
                Model model = context.CreateModel();

                var vDecisions = new List<Decision>();
                List<string> vars = _model.GetVariables;
                foreach (string variable in vars)
                {
                    var d = new Decision(Domain.RealNonnegative, variable);
                    vDecisions.Add(d);
                }
                foreach (Decision decision in vDecisions)
                {
                    model.AddDecision(decision);
                }

                List<string> consts = _model.GetConstraint;
                int counter = 0;
                foreach (string line in consts)
                {
                    counter++;
                    if (!String.IsNullOrEmpty(line))
                    {
                        model.AddConstraint("Constraint" + counter, line);
                    }
                }

                //Add Goal
                model.AddGoal("Answer", _model.GoalKind, _model.Objective);


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
                return result;
            }
            catch (Exception exp)
            {
                return "Solver - An Error has been occurred during solving model!";
            }
        }

        internal void Destry()
        {
            throw new NotImplementedException();
        }
    }
}