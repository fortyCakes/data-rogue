using data_rogue_core.Menus;

namespace data_rogue_core.Renderers
{
    public interface IMenuRenderer : IRenderer
    {
        void Render(Menu menu);
    }
}