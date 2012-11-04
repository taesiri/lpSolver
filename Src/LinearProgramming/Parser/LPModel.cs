using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ICSharpCode.AvalonEdit.Utils;
using Microsoft.SolverFoundation.Services;

namespace LinearProgramming.Parser
{
    public class LPModel
    {
        private readonly List<string> _constraint;
        private readonly List<string> _variables;

        public LPModel(string objective, GoalKind kind, IEnumerable<string> vars, IEnumerable<string> constraint)
        {
            _variables = new List<string>(vars);
            _constraint = new List<string>(constraint);
            GoalKind = kind;
            Objective = objective;
        }

        public LPModel(LPModel cData)
        {
            _variables = new List<string>(cData.GetVariables);
            _constraint = new List<string>(cData.GetConstraint);
            GoalKind = cData.GoalKind;
            Objective = cData.Objective;
        }

        public GoalKind GoalKind { get; set; }
        public string Objective { get; set; }


        public List<string> GetVariables
        {
            get { return _variables; }
        }

        public List<string> GetConstraint
        {
            get { return _constraint; }
        }


        public static LPModel TryParse(List<string> linesToParse)
        {
            try
            {
                var cleanedLines = ParserHelper.ClearUpTheCode(linesToParse);

                var deleteList = cleanedLines.Where(thisLine => thisLine.Contains("Begin") || thisLine.Contains("End")).ToList();
                //Removing all Begin and End!!!!! Parsing by Hand!
                foreach (var item in deleteList)
                {
                    cleanedLines.Remove(item);
                }

                GoalKind gKind;
                if (cleanedLines[0].Substring(0, 3) == "max")
                {
                    gKind  = GoalKind.Maximize;
                }
                else if (cleanedLines[0].Substring(0, 3) == "min")
                {
                    gKind = GoalKind.Minimize;
                }
                else
                {
                    throw new InvalidDataException("Couldn't Parse the Objective!");
                }
                var caseObjective = cleanedLines[0].Remove(0, 4);
                var variables = ParserHelper.ExtractVariables(cleanedLines[0]);

                cleanedLines.RemoveAt(0);

                if (cleanedLines[0] != "subject to:")
                    throw new InvalidDataException("Couldn't Parse the Code!");

                cleanedLines.RemoveAt(0);

                var result = new LPModel(caseObjective, gKind, variables, cleanedLines);
                return result;
            }
            catch (Exception exp)
            {
                Debug.Write("Couldn't Parse the Code!");
                return null;
            }
        }
    }
}