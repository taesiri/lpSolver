namespace LinearProgramming.Controls
{
    public class ErrLog
    {
        public ErrLog(string msg, string location, string state)
        {
            ErrorMessage = msg;
            ParserState = state;
            ErrorLocation = location;
        }

        public string ErrorMessage { get; set; }
        public string ErrorLocation { get; set; }
        public string ParserState { get; set; }
    }
}