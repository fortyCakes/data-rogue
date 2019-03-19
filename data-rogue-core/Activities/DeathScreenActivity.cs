using data_rogue_core.Components;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;
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
                stringBuilder.AppendLine($"Name: {systemContainer.PlayerSystem.Player.Get<Description>().Name}");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"Time: {systemContainer.TimeSystem.CurrentTime} aut");

                var text = stringBuilder.ToString();

                return text.Replace("\r", "");
            }
        }
        public IStaticTextRenderer Renderer { get; }

        private readonly ISystemContainer systemContainer;

        public DeathScreenActivity(IRendererFactory rendererFactory, ISystemContainer systemContainer)
        {
            Renderer = (IStaticTextRenderer)rendererFactory.GetRendererFor(Type);
            this.systemContainer = systemContainer;
        }
        
        public void Render(ISystemContainer systemContainer)
        {
            Renderer.Render(Text, RendersEntireSpace);
        }
    }
}
