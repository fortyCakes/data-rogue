using System.Linq;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public class ConsoleStaticTextRenderer : BaseConsoleRenderer, IStaticTextRenderer
    {
        public ConsoleStaticTextRenderer(RLConsole console) : base(console)
        {
        }

        public void Render(string text, bool renderEntireSpace)
        {
            if (renderEntireSpace)
            {
                Console.Clear();
            }

            var width = Console.Width;
            var height = Console.Height;

            var textLength = text.Length;
            
            int lineLength;

            if (textLength < 80)
            {
                lineLength = textLength;
            }
            else
            {
                lineLength = 80;
            }

            var textLines = WordWrapper.WordWrap(text, lineLength);

            int numLines = textLines.Count(c => c == '\n');

            int offsetX = lineLength / 2;
            int offsetY = numLines / 2;

            int leftX = width / 2 - offsetX - 1;
            int topY = height / 2 - offsetY - 1;

            //TODO wrap text;
            Console.Print(leftX, topY, new string('+', lineLength + 4), RLColor.White);
            for (int i = 1; i < numLines + 3; i++)
            {
                Console.Print(leftX, topY + i, $"+{new string(' ', lineLength + 2)}+", RLColor.White);
            }

            Console.Print(leftX, topY + numLines + 3, new string('+', lineLength + 4), RLColor.White);

            int lineNum = 0;
            foreach (string line in textLines.Split('\n'))
            {
                Console.Print(leftX + 2, topY + 2 + lineNum, line, RLColor.White);
                lineNum++;
            }

        }
    }
}