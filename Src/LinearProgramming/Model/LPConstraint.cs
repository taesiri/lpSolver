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

        public LPPolynomial RightHand { get; set; }
        public LPPolynomial LeftHand { get; set; }
        public LPOperatorType OperatorType { get; set; }


        public List<string> ConstraintVariables
        {
            get { return RightHand.GetVariables.Union(LeftHand.GetVariables).ToList(); }
        }
    }
}