using System;
using System.Text;

namespace data_rogue_core.Utils
{
    public class WordWrapper
    {
        protected readonly static string _newline = "\n";

        public static string WordWrap(string the_string, int width)
        {
            int pos, next;
            StringBuilder sb = new StringBuilder();

            // Lucidity check
            if (width < 1)
                return the_string;

            // Parse each line of text
            for (pos = 0; pos < the_string.Length; pos = next)
            {
                // Find end of line
                int eol = the_string.IndexOf(_newline, pos);

                if (eol == -1)
                    next = eol = the_string.Length;
                else
                    next = eol + _newline.Length;

                // Copy this line of text, breaking into smaller lines as needed
                if (eol > pos)
                {
                    do
                    {
                        int len = eol - pos;

                        if (len > width)
                            len = BreakLine(the_string, pos, width);

                        sb.Append(the_string, pos, len);
                        sb.Append(_newline);

                        // Trim whitespace following break
                        pos += len;

                        while (pos < eol && Char.IsWhiteSpace(the_string[pos]))
                            pos++;

                    } while (eol > pos);
                }
                else sb.Append(_newline); // Empty line
            }

            return sb.ToString();
        }

        /// <summary>
        /// Locates position to break the given line so as to avoid
        /// breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        public static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            int i = max - 1;
            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
                i--;
            if (i < 0)
                return max; // No whitespace found; break at maximum length
            // Find start of whitespace
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
                i--;
            // Return length of text before whitespace
            return i + 1;
        }
    }
}
