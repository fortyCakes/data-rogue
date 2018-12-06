using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntitySystem;
using data_rogue_core.Enums;
using data_rogue_core.Systems;
using RLNET;

namespace data_rogue_core.EventSystem.Rules
{
    public class InputHandlerRule : IEventRule
    {
        private IPlayerControlSystem PlayerControlSystem;

        public InputHandlerRule(IPlayerControlSystem playerControlSystem)
        {
            PlayerControlSystem = playerControlSystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Input };
        public int RuleOrder => 0;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            RLKeyPress keyPress = (RLKeyPress)eventData;

            switch (Game.GameState)
            {
                case GameState.Menu:
                    Game.ActiveMenu.HandleKeyPress(keyPress);
                    break;
                case GameState.StaticDisplay:
                    return false;
                case GameState.Playing:
                    PlayerControlSystem.HandleKeyPress(keyPress);
                    break;
            }

            return true;
        }
    }
}
