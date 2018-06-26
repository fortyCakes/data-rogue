using RLNET;

namespace data_rogue_core.Entities
{
    public interface IRLConsoleWriter
    {
        void Set(int x, int y, RLColor foreground, RLColor background, char symbol);

        void SetBackColor(int x, int y, RLColor color);
    }

    class RLConsoleWriter : IRLConsoleWriter
    {
        private readonly RLConsole _console;

        public RLConsoleWriter(RLConsole console)
        {
            _console = console;
        }

        public void Set(int x, int y, RLColor foreground, RLColor background, char symbol)
        {
            _console.Set(x,y,foreground, background, symbol);
        }

        public void SetBackColor(int x, int y, RLColor color)
        {
            _console.SetBackColor(x, y, color);
        }
    }
}