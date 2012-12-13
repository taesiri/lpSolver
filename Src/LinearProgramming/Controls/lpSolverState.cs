namespace LinearProgramming.Controls
{
    public class LPSolverState
    {
        public LPSolverState()
        {
            CurrentState = EnumEditorStates.Normal;
            CurrentMessage = string.Empty;
        }

        public LPSolverState(string message, EnumEditorStates state)
        {
            CurrentState = state;
            CurrentMessage = message;
        }

        public EnumEditorStates CurrentState { get; set; }
        public string CurrentMessage { get; set; }
        public string FullMessage { get; set; }
    }
}