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

        public bool Apply(EventType type, Entity sender, object eventData)
        {
            switch (Game.GameState)
            {
                case GameState.Menu:
                    Game.ActiveMenu.HandleKeyPress((RLKeyPress) eventData);
                    break;
                case GameState.StaticDisplay:
                    return false;
            }

            return true;
        }
    }
}
