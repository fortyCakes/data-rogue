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
                stringBuilder.Append("Name: ").AppendLine(systemContainer.PlayerSystem.Player.Get<Description>().Name);
                stringBuilder.AppendLine();
                stringBuilder.Append("Time: ").Append(systemContainer.TimeSystem.CurrentTime).AppendLine(" aut");

                var text = stringBuilder.ToString();

                return text.Replace("\r", "");
            }
        }

        public IStaticTextRenderer Renderer { get; private set; }

        private readonly ISystemContainer systemContainer;

        public DeathScreenActivity(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
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
