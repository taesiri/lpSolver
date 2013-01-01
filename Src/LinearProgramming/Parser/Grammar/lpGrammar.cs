using Irony.Parsing;

namespace LinearProgramming.Grammar
{
    [Language("LinearProgramming", "0.1", "LP Grammar Data Format; Work in Progress")]
    public class LinearProgrammingGrammar : Irony.Parsing.Grammar
    {
        public LinearProgrammingGrammar()
        {
            var comment = new CommentTerminal("comment", "#", "\n", "\r");
            NonGrammarTerminals.Add(comment);

            var identifier = new IdentifierTerminal("appIdentifier");
            var variable = new IdentifierTerminal("variable");

            var number = new NumberLiteral("number", NumberOptions.AllowSign);

            var lpProgram = new NonTerminal("lpApp");
            var lpAppName = new NonTerminal("lpAppName");
            var lpModel = new NonTerminal("lpModel");

            var lpGoal = new NonTerminal("lpGoal");
            var lpPolynomial = new NonTerminal("lpPolynomial");
            var lpConstraints = new NonTerminal("lpConstraints");
            var lpConstraint = new NonTerminal("lpConstraints");
            var lpOperator = new NonTerminal("lpOperator", "lp Operation symbol");
            var lpBinOp = new NonTerminal("lpBinOp", "lp Binary Operation symbol");
            var lpMonomial = new NonTerminal("Monomial");
            var lpNumber = new NonTerminal("lpNumber");

            lpProgram.Rule = lpAppName + "{" + lpModel + "}" + ";";

            lpAppName.Rule = ToTerm("lpmodel") + identifier;

            lpModel.Rule = lpGoal + ToTerm("subject to") +":" + lpConstraints;

            lpGoal.Rule = ToTerm("max") + lpPolynomial | ToTerm("min") + lpPolynomial;

            lpConstraints.Rule = MakePlusRule(lpConstraints, null, lpConstraint);

            //ReduceHere();
            lpConstraint.Rule = lpPolynomial + lpOperator + lpPolynomial + ";";

            
            lpOperator.Rule = ToTerm("<") | "==" | ">" | "<=" | ">=";

           
            lpPolynomial.Rule = lpMonomial | lpMonomial + lpBinOp + lpPolynomial  | lpNumber |
                                lpNumber + lpBinOp + lpPolynomial;

            lpMonomial.Rule = variable | lpNumber + ToTerm("*") + variable;


            lpBinOp.Rule = ToTerm("+") | "-";
            lpNumber.Rule = number | lpBinOp + number;
            
            
            MarkReservedWords("max", "min", "lpmodel", "subject to");


           // ReduceIf("");
            Root = lpProgram;
        }
    }
}