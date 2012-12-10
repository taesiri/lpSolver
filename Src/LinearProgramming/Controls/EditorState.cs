namespace LinearProgramming.Controls
{
    public class EditorState
    {
        public EditorState()
        {
            CurrentState = EnumEditorStates.Normal;
            CurrentMessage = string.Empty;
        }

        public EditorState(string message, EnumEditorStates state)
        {
            CurrentState = state;
            CurrentMessage = message;
        }

        public EnumEditorStates CurrentState { get; set; }
        public string CurrentMessage { get; set; }
    }
}