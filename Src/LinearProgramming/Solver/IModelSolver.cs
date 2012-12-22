namespace LinearProgramming.Solver
{
    public interface IModelSolver
    {
        void TrySolve();
        string GetResult();
        SolvedData GetSolvedData();
    }
}