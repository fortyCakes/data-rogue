using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public static class ConsoleRendererHelper
    {
        public static void DisplayEntitySummary(RLConsole console, int x, int y, IEntity hoveredEntity)
        {
            var appearance = hoveredEntity.Get<Appearance>();

            var description = hoveredEntity.TryGet<Description>();

            console.Print(x + 1, y + 0, appearance.Glyph.ToString(), appearance.Color.ToRLColor());

            var name = description?.Name ?? "";

            console.Print(x + 3, y + 0, name, RLColor.White);

            var fighter = hoveredEntity.TryGet<Fighter>();

            if (fighter != null)
            {
                PrintBar(console, x + 1, y + 3, console.Width - 2, "hp", fighter.Health, RLColor.Red);
                PrintBar(console, x + 1, y + 5, console.Width - 2, "aura", fighter.Aura, RLColor.Yellow);
                PrintBar(console, x + 1, y + 7, console.Width - 2, "tilt", fighter.Tilt, RLColor.Magenta);
            }
        }

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
    }
}
