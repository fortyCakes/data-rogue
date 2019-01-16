using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public class ConsoleFormRenderer : BaseConsoleRenderer, IFormRenderer
    {
        public ConsoleFormRenderer(RLConsole console) : base(console)
        {
        }
        
        public void Render(Form form)
        {
            throw new System.NotImplementedException();
        }
    }
}