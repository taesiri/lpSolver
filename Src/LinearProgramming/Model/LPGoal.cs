namespace LinearProgramming.Model
{
    public class LPGoal
    {
        public LPGoal()
        {
            Goal = new LPPolynomial();
            GoalType = LPGoalType.Maximize;
        }

        public LPPolynomial Goal { get; set; }
        public LPGoalType GoalType { set; get; }
    }
}