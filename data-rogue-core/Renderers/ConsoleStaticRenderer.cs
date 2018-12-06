using RLNET;

namespace data_rogue_core
{
    public class ConsoleStaticRenderer
    {
        public static void Render(RLConsole console, string text)
        {
            console.Clear();

            //TODO wrap text;
            console.Print(1, 1, text, RLColor.White);
        }
    }
}