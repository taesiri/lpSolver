using Irony.Parsing;

namespace LinearProgramming.Grammar
{
    [Language("LinearProgramming", "0.3", "LP Data Format")]
    public class LinearGrammar : Irony.Parsing.Grammar
    {
        public LinearGrammar()
        {
            var identifier = new IdentifierTerminal("appIdentifier");
            var variable = new IdentifierTerminal("variable");

            var number = new NumberLiteral("number");

            var lpProgram = new NonTerminal("lpApp");
            var lpAppName = new NonTerminal("lpAppName");
            var lpModel = new NonTerminal("lpModel");

            var lpGoal = new NonTerminal("lpGoal");
            var lpPolynomial = new NonTerminal("lpPolynomial");
            var lpConstraints = new NonTerminal("lpConstraints");
            var lpConstraint = new NonTerminal("lpConstraints");
            var lpOperator = new NonTerminal("lpOperator", "lp Operation symbol");

            var lpMonomial = new NonTerminal("Monomial");

            lpProgram.Rule = lpAppName + "{" + lpModel + "}" + ";";

            lpAppName.Rule = ToTerm("lpmodel") + identifier;

            lpModel.Rule = lpGoal + ToTerm("subject to:") + lpConstraints;

            lpGoal.Rule = ToTerm("max") + lpPolynomial | ToTerm("min") + lpPolynomial;

            lpConstraints.Rule = MakePlusRule(lpConstraints, null, lpConstraint);
            lpConstraint.Rule = lpPolynomial + lpOperator + number + ";" | number + lpOperator + lpPolynomial + ";";

            lpOperator.Rule = ToTerm("<") | "==" | ">" | "<=" | ">=";

            lpPolynomial.Rule = MakePlusRule(lpPolynomial, ToTerm("+"), lpMonomial);

            lpMonomial.Rule = variable | number + ToTerm("*") + variable;

            Root = lpProgram;
        }
    }
}