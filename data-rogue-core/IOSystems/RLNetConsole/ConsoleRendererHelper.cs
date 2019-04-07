using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public static class ConsoleRendererHelper
    {
        public static void PrintBar(RLConsole console, int x, int y, int length, string name, Counter counter, RLColor color)
        {
            if (counter.Max == 0)
            {
                console.Print(x, y, name + ": 0/0", RLColor.White);
                return;
            }

            var counterStart = name.Length + 2;
            var counterText = counter.ToString();

            for (int i = 0; i < length; i++)
            {
                char glyph = ' ';

                if (i < name.Length)
                {
                    glyph = name[i];
                }
                else if (i == name.Length)
                {
                    glyph = ':';
                }
                else if (i >= counterStart && i < counterStart + counterText.Length)
                {
                    glyph = counterText[i - counterStart];
                }

                if ((decimal)i / length < (decimal)counter.Current / counter.Max)
                {
                    console.Set(x + i, y, RLColor.Black, color, glyph);
                }
                else
                {
                    console.Set(x + i, y, color, RLColor.Black, glyph);
                }


            }
        }

        internal static void PrintStat(RLConsole statsConsole, int x, int y, string statName, string value, RLColor color)
        {
            statsConsole.Print(x, y, $"{statName}: {value}", color);
        }

        internal static void PrintStat(RLConsole statsConsole, int x, int y, string statName, int value, RLColor color)
        {
            PrintStat(statsConsole, x, y, statName, value.ToString(), color);
        }
    }
}
