using data_rogue_core.Renderers;

namespace data_rogue_core.Activities
{
    public class LoadingScreenActivity : IActivity
    {
        public ActivityType Type => ActivityType.StaticDisplay;
        public object Data => Text;
        public bool RendersEntireSpace => true;

        public string Text { get; set; }
        public IStaticTextRenderer Renderer { get; }

        public LoadingScreenActivity(string staticText, IRendererFactory rendererFactory)
        {
            Text = staticText;
            Renderer = (IStaticTextRenderer)rendererFactory.GetRendererFor(Type);
        }
        
        public void Render()
        {
            Renderer.Render(Text, RendersEntireSpace);
        }
    }
}
