using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Menus;
using data_rogue_core.Renderers;
using RLNET;

namespace data_rogue_core.Activities
{
    public class MenuActivity : IActivity
    {
        public ActivityType Type => ActivityType.Menu;
        public object Data => Menu;
        public Menu Menu { get; }

        public MenuActivity(Menu menu, IRendererFactory rendererFactory)
        {
            Menu = menu;
            Renderer = (IMenuRenderer)rendererFactory.GetRendererFor(Type);
        }

        public IMenuRenderer Renderer { get; set; }

        public void Render()
        {
            Renderer.Render(Menu);
        }


    }
}
