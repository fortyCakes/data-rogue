using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public abstract class BaseConsoleRenderer : IRenderer
    {
        public RLConsole Console { get; private set; }

        public BaseConsoleRenderer(RLConsole console)
        {
            this.Console = console;
        }
    }
}