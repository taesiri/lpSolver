namespace LinearProgramming.Model
{
    public class LPSolution
    {
        public LPSolution()
        {
        }

        public LPSolution(string resultStr)
        {
            ResultSummary = resultStr;
        }

        public string ResultSummary { get; set; }
    }
}