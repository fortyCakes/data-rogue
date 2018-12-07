using data_rogue_core.Activities;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.Menus;
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

            IActivity currentActivity = Game.ActivityStack.Peek();
            switch (currentActivity.Type)
            {
                case ActivityType.Menu:
                    (currentActivity.Data as Menu)?.HandleKeyPress(keyPress);
                    break;
                case ActivityType.StaticDisplay:
                    Game.ActivityStack.Pop();
                    return false;
                case ActivityType.Gameplay:
                    PlayerControlSystem.HandleKeyPress(keyPress);
                    break;
            }

            return true;
        }
    }
}
