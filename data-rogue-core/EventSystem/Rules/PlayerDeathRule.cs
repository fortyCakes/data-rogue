using System.Drawing;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    class PlayerDeathRule : IEventRule
    {
        public PlayerDeathRule(IEntityEngine engine, IMessageSystem messageSystem)
        {
            EntityEngine = engine;
            MessageSystem = messageSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Death };
        public int RuleOrder => 0;

        private IEntityEngine EntityEngine { get; }
        public IMessageSystem MessageSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (IsPlayer(sender))
            {
                Game.ActivityStack.Push(MainMenu.GetMainMenu());
                Game.ActivityStack.Push(new DeathScreenActivity(Game.RendererFactory));
            }

            return true;
        }

        private static bool IsPlayer(IEntity sender)
        {
            return sender == Game.WorldState.Player;
        }
    }
}
