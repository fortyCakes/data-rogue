using data_rogue_core.Components;
using data_rogue_core.IOSystems;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using OpenTK.Input;
using System.Text;

namespace data_rogue_core.Activities
{
    public class EndGameScreenActivity : IActivity
    {
        public ActivityType Type => ActivityType.StaticDisplay;
        public object Data => Text;
        public bool RendersEntireSpace => true;

        public bool Running => true;
        public string Text { get; set; }

        private string GetEndGameScreenText(bool victory)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(victory? "You win!" : "You are dead.");
            stringBuilder.Append("Name: ").AppendLine(_systemContainer.PlayerSystem.Player.Get<Description>().Name);
            stringBuilder.AppendLine();
            stringBuilder.Append("Time: ").Append(_systemContainer.TimeSystem.CurrentTime).AppendLine(" aut");

            var text = stringBuilder.ToString();

            return text.Replace("\r", "");
        }

        public IStaticTextRenderer Renderer { get; private set; }

        private readonly ISystemContainer _systemContainer;

        public EndGameScreenActivity(ISystemContainer systemContainer, bool victory)
        {
            this._systemContainer = systemContainer;

            Text = GetEndGameScreenText(victory);
        }

        public void Render(ISystemContainer systemContainer)
        {
            Renderer.Render(Text, RendersEntireSpace);
        }

        public void Initialise(IRenderer renderer)
        {
            Renderer = (IStaticTextRenderer)renderer;
        }

        public void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            if (keyboard != null && keyboard.Key != Key.Unknown)
            {
                Close();
            }
        }

        public void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            if (mouse.IsLeftClick)
            {
                Close();
            }
        }

        public void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            // None
        }

        private void Close()
        {
            _systemContainer.ActivitySystem.RemoveActivity(this);
        }
    }
}
