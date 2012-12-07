using System;
using System.Collections.Generic;
using Irony.Parsing;
using LinearProgramming.Model;

namespace LinearProgramming.Parser
{
    public static class Modeler
    {
        public static LPModel ConvertParseTreeToModel(ParseTree tree)
        {
            string modelName = "";
            var lpGoal = new LPGoal();
            var lpConstraints = new List<LPConstraint>();

            if (tree.Root.ToString() == "lpApp")
            {
                foreach (ParseTreeNode node in tree.Root.ChildNodes)
                {
                    switch (node.ToString())
                    {
                        case "lpAppName":
                            modelName = ParseModelName(node);
                            break;
                        case "lpModel":
                            foreach (ParseTreeNode subNodes in node.ChildNodes)
                            {
                                switch (subNodes.ToString())
                                {
                                    case "lpGoal":
                                        lpGoal = ParseGoal(subNodes);
                                        break;
                                    case "lpConstraints":
                                        lpConstraints = ParseConstraints(subNodes);
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                throw new Exception("The Root element is not Correct");
            }

            return new LPModel(modelName, lpGoal, lpConstraints);
        }

        private static string ParseModelName(ParseTreeNode lpAppNameNode)
        {
            return lpAppNameNode.ChildNodes[1].ToString();
        }

        private static LPGoal ParseGoal(ParseTreeNode lpGoalNode)
        {
            var rGoal = new LPGoal();
            if (lpGoalNode.ChildNodes[0].ToString() == "min (Keyword)")
                rGoal.GoalType = LPGoalType.Minimize;
            else if (lpGoalNode.ChildNodes[0].ToString() == "max (Keyword)")
                rGoal.GoalType = LPGoalType.Maximize;

            rGoal.Goal = ParsePolynomial(lpGoalNode.ChildNodes[1]);

            return rGoal;
        }

        private static LPPolynomial ParsePolynomial(ParseTreeNode lpPolynomialNode)
        {
            var lpPoly = new LPPolynomial();


            switch (lpPolynomialNode.ChildNodes[0].ToString())
            {
                case "Monomial":
                    lpPoly = ParseMonomial(lpPolynomialNode.ChildNodes[0]);
                    break;
                case "lpNumber":
                    lpPoly = new LPPolynomial {Constant = ParseLPNumber(lpPolynomialNode.ChildNodes[0])};
                    break;
            }

            if (lpPolynomialNode.ChildNodes.Count == 3)
            {
                LPPolynomial oPoly = ParsePolynomial(lpPolynomialNode.ChildNodes[2]);
                lpPoly = LPPolynomial.JoinPolys(oPoly, lpPoly);
            }
            return lpPoly;
        }


        private static LPPolynomial ParseMonomial(ParseTreeNode node)
        {
            var lpPoly = new LPPolynomial();
            switch (node.ChildNodes.Count)
            {
                case 1:
                    lpPoly.AddNewVariable(node.ChildNodes[0].ToString(), 1f);
                    break;
                case 3:
                    double coefficient = 0;
                    switch (node.ChildNodes[0].ToString())
                    {
                        case "lpNumber":
                            coefficient = ParseLPNumber(node.ChildNodes[0]);
                            break;
                        case "number":
                            coefficient = ParseNumber(node.ChildNodes[0]);
                            break;
                        default:
                            throw new Exception("Invalid Model - Modeler Error At Monomial");
                    }

                    lpPoly.AddNewVariable(node.ChildNodes[2].ToString().Replace("(variable)", "").Replace(" ", ""),
                                          coefficient);
                    break;
            }

            return lpPoly;
        }

        private static double ParseLPNumber(ParseTreeNode node)
        {
            switch (node.ChildNodes.Count)
            {
                case 1:
                    return ParseNumber(node.ChildNodes[0]);
                case 2:
                    double number = ParseNumber(node.ChildNodes[1]);
                    if (node.ChildNodes[0].ChildNodes[0].ToString() == "- (Key symbol)")
                        number *= -1;
                    else if (node.ChildNodes[0].ChildNodes[0].ToString() != "+ (Key symbol)")
                        throw new Exception("Invalid Model - Modeler Error At LPNumber");

                    return number;

                default:
                    throw new Exception("Invalid Model - Modeler Error At LPNumber");
            }
        }

        private static double ParseNumber(ParseTreeNode node)
        {
            return Convert.ToDouble(node.ToString().Replace("(number)", ""));
        }


        private static List<LPConstraint> ParseConstraints(ParseTreeNode node)
        {
            var rList = new List<LPConstraint>();
            foreach (ParseTreeNode item in node.ChildNodes)
            {
                if (item.ToString() == "lpConstraints")
                    rList.Add(ParseConstraint(item));
            }
            return rList;
        }

        private static LPConstraint ParseConstraint(ParseTreeNode node)
        {
            if (node.ChildNodes.Count != 4)
                throw new Exception("Invalid Model - Modeler Error At ParseConstraint");

            LPPolynomial lPoly = ParsePolynomial(node.ChildNodes[0]);
            LPOperatorType optr = ParseOperator(node.ChildNodes[1]);
            LPPolynomial rPoly = ParsePolynomial(node.ChildNodes[2]);

            return new LPConstraint(lPoly, rPoly, optr);
        }

        private static LPOperatorType ParseOperator(ParseTreeNode node)
        {
            string str = node.ChildNodes[0].ToString().Replace("(Key symbol)", "").Replace(" ", "");

            switch (str)
            {
                case "==":
                    return LPOperatorType.Equals;
                case "<":
                    return LPOperatorType.LessThan;
                case "=<":
                    return LPOperatorType.LessOrEqualsTo;
                case ">":
                    return LPOperatorType.MoreThan;
                case ">=":
                    return LPOperatorType.MoreOrEqualsTo;

                default:
                    throw new Exception("Invalid Model - Modeler Error At ParseOperator");
            }
        }
    }
}