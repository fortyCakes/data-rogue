﻿using data_rogue_core.Components;
using data_rogue_core.Systems.Interfaces;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace data_rogue_core.Activities
{
    public class EndGameScreenActivity : StaticTextActivity
    {
        private readonly ISystemContainer _systemContainer;

        public override bool RendersEntireSpace => true;

        public EndGameScreenActivity(Rectangle position, Padding padding, ISystemContainer systemContainer, bool victory) : base(position, padding, systemContainer.ActivitySystem, null, true)
        {
            _systemContainer = systemContainer;

            Text = GetEndGameScreenText(victory);
        }

        private string GetEndGameScreenText(bool victory)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine();
            stringBuilder.AppendLine(victory ? " You win!" : " You are dead.");
            stringBuilder.Append(" Name: ").AppendLine(_systemContainer.PlayerSystem.Player.Get<Description>().Name);
            stringBuilder.AppendLine();
            stringBuilder.Append(" Time: ").Append(_systemContainer.TimeSystem.CurrentTime).AppendLine(" aut");

            var text = stringBuilder.ToString();

            return text.Replace("\r", "");
        }
    }
}
