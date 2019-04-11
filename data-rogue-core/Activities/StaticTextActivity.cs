using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public class StaticTextActivity : IActivity, IStaticTextActivity
    {
        public ActivityType Type => ActivityType.StaticDisplay;
        public object Data => Text;
        public bool RendersEntireSpace => false;

        public string Text { get; set; }
        public IStaticTextRenderer Renderer { get; private set; }
        public bool CloseOnKeyPress { get; }

        public StaticTextActivity(string staticText, bool closeOnKeyPress = true)
        {
            Text = staticText;
            CloseOnKeyPress = closeOnKeyPress;
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
