using data_rogue_core.Activities;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Forms;
using data_rogue_core.Menus;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using RLNET;

namespace data_rogue_core.EventSystem.Rules
{
    public class InputHandlerRule : IEventRule
    {
        private IPlayerControlSystem PlayerControlSystem;
        private ITargetingSystem TargetingSystem;

        public InputHandlerRule(ISystemContainer systemContainer)
        {
            PlayerControlSystem = systemContainer.PlayerControlSystem;
            TargetingSystem = systemContainer.TargetingSystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Input };
        public int RuleOrder => 0;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var inputEventData = (InputEventData)eventData;

            RLKeyPress keyPress = inputEventData.Keyboard;
            RLMouse mouse = inputEventData.Mouse;

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
                case ActivityType.Form:
                    (currentActivity.Data as Form)?.HandleKeyPress(keyPress);
                    break;
                case ActivityType.Targeting:
                    TargetingSystem.HandleMouseInput(mouse);
                    break;
            }

            return true;
        }
    }
}
