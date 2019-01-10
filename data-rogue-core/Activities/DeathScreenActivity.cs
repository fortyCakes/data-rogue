using data_rogue_core.Components;
using data_rogue_core.Renderers;
using System.Text;

namespace data_rogue_core.Activities
{
    public class DeathScreenActivity : IActivity
    {
        public ActivityType Type => ActivityType.StaticDisplay;
        public object Data => Text;
        public bool RendersEntireSpace => true;

        public string Text {
            get
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine("You are dead.");
                stringBuilder.AppendLine($"Name: {Game.WorldState.Player.Get<Description>().Name}");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"Time: {Game.TimeSystem.CurrentTime} aut");

                return stringBuilder.ToString();
            }
        }
        public IStaticTextRenderer Renderer { get; }

        public DeathScreenActivity(IRendererFactory rendererFactory)
        {
            Renderer = (IStaticTextRenderer)rendererFactory.GetRendererFor(Type);
        }
        
        public void Render()
        {
            Renderer.Render(Text, RendersEntireSpace);
        }
    }
}
