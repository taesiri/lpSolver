using System.Collections.Generic;
using System.Linq;

namespace LinearProgramming.Model
{
    public class LPConstraint
    {
        public LPConstraint()
        {
            RightHand = new LPPolynomial();
            LeftHand = new LPPolynomial();

            OperatorType = LPOperatorType.Equals;
        }

        public LPConstraint(LPPolynomial leftPolynomial, LPPolynomial rightPolynomial, LPOperatorType operatorType)
        {
            RightHand = new LPPolynomial(rightPolynomial);
            LeftHand = new LPPolynomial(leftPolynomial);

            OperatorType = operatorType;
        }


        public LPPolynomial RightHand { get; set; }
        public LPPolynomial LeftHand { get; set; }
        public LPOperatorType OperatorType { get; set; }


        public List<string> ConstraintVariables
        {
            get { return RightHand.GetVariables.Union(LeftHand.GetVariables).ToList(); }
        }

        public override string ToString()
        {
            return LeftHand + "  " + OperatorType.ToString() + "  " + RightHand;
        }
    }
}