using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace LinearProgramming
{
    public class BraceFoldingStrategy : AbstractFoldingStrategy
    {
        /// <summary>
        /// Creates a new BraceFoldingStrategy.
        /// </summary>
        public BraceFoldingStrategy()
        {
            OpeningBrace = "Begin";
            ClosingBrace = "End";
        }

        /// <summary>
        /// Gets/Sets the opening brace. The default value is '{'.
        /// </summary>
        public string OpeningBrace { get; set; }

        /// <summary>
        /// Gets/Sets the closing brace. The default value is '}'.
        /// </summary>
        public string ClosingBrace { get; set; }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
        {
            var newFoldings = new List<NewFolding>();

            var startOffsets = new Stack<int>();
            int lastNewLineOffset = 0;
            string openingBrace = OpeningBrace;
            string closingBrace = ClosingBrace;
            for (int i = 0; i < document.TextLength - 3; i++)
            {
                try
                {
                    char c = document.GetCharAt(i);
                    string cs = "";
                    if (i < document.TextLength - 5)
                    {
                        cs = document.Text.Substring(i, 5);
                    }
                    else if (i < document.TextLength - 3)
                    {
                        cs = document.Text.Substring(i, 3);
                    }

                    if (cs == openingBrace)
                    {
                        startOffsets.Push(i);
                    }
                    else
                    {
                        if (i < document.TextLength - 3)
                        {
                            cs = document.Text.Substring(i, 3);
                        }
                        if (cs == closingBrace && startOffsets.Count > 0)
                        {
                            int startOffset = startOffsets.Pop();
                            // don't fold if opening and closing brace are on the same line
                            if (startOffset < lastNewLineOffset)
                            {
                                newFoldings.Add(new NewFolding(startOffset, i + 1));
                            }
                        }
                    }

                    if (c == '\n' || c == '\r')
                    {
                        lastNewLineOffset = i + 1;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }
    }
}