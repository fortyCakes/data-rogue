using data_rogue_core.Menus;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public class MenuActivity : IActivity
    {
        public ActivityType Type => ActivityType.Menu;
        public object Data => Menu;
        public bool RendersEntireSpace => true;
        public Menu Menu { get; }

        public MenuActivity(Menu menu)
        {
            Menu = menu;
            Menu.Activity = this;
        }

        public IMenuRenderer Renderer { get; set; }

        public void Render(ISystemContainer systemContainer)
        {
            Renderer.Render(Menu);
        }

        public void Initialise(IRenderer renderer)
        {
            Renderer = (IMenuRenderer)renderer;
        }
    }
}
