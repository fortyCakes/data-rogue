using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Renderers;
using RLNET;

namespace data_rogue_core.Activities
{
    public class StaticTextActivity : IActivity
    {
        public ActivityType Type => ActivityType.StaticDisplay;
        public object Data => Text;
        public bool RendersEntireSpace => false;

        public string Text { get; set; }
        public IStaticTextRenderer Renderer { get; }

        public StaticTextActivity(string staticText, IRendererFactory rendererFactory)
        {
            Text = staticText;
            Renderer = (IStaticTextRenderer)rendererFactory.GetRendererFor(Type);
        }
        
        public void Render()
        {
            Renderer.Render(Text);
        }
    }
}
