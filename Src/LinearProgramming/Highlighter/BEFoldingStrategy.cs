using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace LinearProgramming.Highlighter
{
    public class BEFoldingStrategy : AbstractFoldingStrategy
    {
        /// <summary>
        /// Creates a new BEFoldingStrategy.
        /// </summary>
        public BEFoldingStrategy()
        {
            OpeningBrace = '{';
            ClosingBrace = '}';
        }

        /// <summary>
        /// Gets/Sets the opening brace. The default value is '{'.
        /// </summary>
        public char OpeningBrace { get; set; }

        /// <summary>
        /// Gets/Sets the closing brace. The default value is '}'.
        /// </summary>
        public char ClosingBrace { get; set; }

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
            char openingBrace = OpeningBrace;
            char closingBrace = ClosingBrace;
            for (int i = 0; i < document.TextLength - 1; i++)
            {
                try
                {
                    char c = document.GetCharAt(i);

                    if (c == openingBrace)
                    {
                        startOffsets.Push(i);
                    }
                    else if( c== closingBrace)
                    {
                        if (startOffsets.Count > 0)
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