using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public class ConsoleStaticTextRenderer : BaseConsoleRenderer, IStaticTextRenderer
    {
        public ConsoleStaticTextRenderer(RLConsole console) : base(console)
        {
        }

        public void Render(string text)
        {
            Console.Clear();

            //TODO wrap text;
            Console.Print(1, 1, text, RLColor.White);
        }
    }
}