using data_rogue_core.Forms;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public interface IFormRenderer : IRenderer
    {
        void Render(Form form);
    }
}