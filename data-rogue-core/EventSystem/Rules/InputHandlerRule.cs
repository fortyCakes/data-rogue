using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntitySystem;
using data_rogue_core.Enums;
using RLNET;

namespace data_rogue_core.EventSystem.Rules
{
    public class InputHandlerRule : IEventRule
    {
        public List<EventType> EventTypes => new List<EventType>{EventType.Input};
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
                    Game.WorldState.PlayerControlSystem.HandleKeyPress(keyPress);
                    break;
            }

            return true;
        }
    }
}
