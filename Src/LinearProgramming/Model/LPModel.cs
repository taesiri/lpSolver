using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LinearProgramming.Parser;

namespace LinearProgramming.Model
{
    public class LPModel
    {
        private readonly List<string> _constraint;
        private readonly List<string> _variables;

        public LPModel(string name,string objective, LPGoalType kind, IEnumerable<string> vars, IEnumerable<string> constraint)
        {
            _variables = new List<string>(vars);
            _constraint = new List<string>(constraint);
            GoalKind = kind;
            Objective = objective;
            Name = name;
        }

        public LPModel(LPModel cData)
        {
            _variables = new List<string>(cData.GetVariables);
            _constraint = new List<string>(cData.GetConstraint);
            GoalKind = cData.GoalKind;
            Objective = cData.Objective;
            Name = cData.Name;
        }

        public LPGoalType GoalKind { get; set; }
        public string Objective { get; set; }
        public string Name { get; set; }

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
                List<string> cleanedLines = ParserHelper.ClearUpTheCode(linesToParse);

                List<string> deleteList =
                    cleanedLines.Where(thisLine => thisLine.Contains("Begin") || thisLine.Contains("End")).ToList();
                //Removing all Begin and End!!!!! Parsing by Hand!
                foreach (string item in deleteList)
                {
                    cleanedLines.Remove(item);
                }

                LPGoalType gKind;
                if (cleanedLines[0].Substring(0, 3) == "max")
                {
                    gKind = LPGoalType.Maximize;
                }
                else if (cleanedLines[0].Substring(0, 3) == "min")
                {
                    gKind = LPGoalType.Minimize;
                }
                else
                {
                    throw new InvalidDataException("Couldn't Parse the Objective!");
                }
                string caseObjective = cleanedLines[0].Remove(0, 4);
                List<string> variables = ParserHelper.ExtractVariables(cleanedLines[0]);

                cleanedLines.RemoveAt(0);

                if (cleanedLines[0] != "subject to:")
                    throw new InvalidDataException("Couldn't Parse the Code!");

                cleanedLines.RemoveAt(0);

                var result = new LPModel("noName",caseObjective, gKind, variables, cleanedLines);
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