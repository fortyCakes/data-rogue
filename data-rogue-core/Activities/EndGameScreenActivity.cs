using data_rogue_core.Components;
using data_rogue_core.IOSystems;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using OpenTK.Input;
using System.Collections.Generic;
using System.Text;
using data_rogue_core.Maps;

namespace data_rogue_core.Activities
{
    public class EndGameScreenActivity : StaticTextActivity
    {
        private readonly ISystemContainer _systemContainer;

        public EndGameScreenActivity(ISystemContainer systemContainer, bool victory) : base(systemContainer.ActivitySystem, null, true)
        {
            _systemContainer = systemContainer;

            Text = GetEndGameScreenText(victory);
        }

        private string GetEndGameScreenText(bool victory)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(victory ? "You win!" : "You are dead.");
            stringBuilder.Append("Name: ").AppendLine(_systemContainer.PlayerSystem.Player.Get<Description>().Name);
            stringBuilder.AppendLine();
            stringBuilder.Append("Time: ").Append(_systemContainer.TimeSystem.CurrentTime).AppendLine(" aut");

            var text = stringBuilder.ToString();

            return text.Replace("\r", "");
        }
    }
}
