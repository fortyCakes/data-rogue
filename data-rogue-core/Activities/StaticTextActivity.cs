using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public class StaticTextActivity : IActivity
    {
        public ActivityType Type => ActivityType.StaticDisplay;
        public object Data => Text;
        public bool RendersEntireSpace => false;

        public string Text { get; set; }
        public IStaticTextRenderer Renderer { get; private set; }

        public StaticTextActivity(string staticText)
        {
            Text = staticText;
        }

        public void Render(ISystemContainer systemContainer)
        {
            Renderer.Render(Text, RendersEntireSpace);
        }

        public void Initialise(IRenderer renderer)
        {
            Renderer = (IStaticTextRenderer)renderer;
        }
    }
}
