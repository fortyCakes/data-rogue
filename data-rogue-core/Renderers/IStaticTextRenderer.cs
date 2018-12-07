using data_rogue_core.Renderers.ConsoleRenderers;
using RLNET;

namespace data_rogue_core.Renderers
{
    public interface IStaticTextRenderer : IRenderer
    {
        void Render(string text);
    }
}