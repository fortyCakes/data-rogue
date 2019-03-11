using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
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
        private IPlayerControlSystem _playerControlSystem;
        private ITargetingSystem _targetingSystem;
        private IActivitySystem _activitySystem;

        public InputHandlerRule(ISystemContainer systemContainer)
        {
            _playerControlSystem = systemContainer.PlayerControlSystem;
            _targetingSystem = systemContainer.TargetingSystem;
            _activitySystem = systemContainer.ActivitySystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Input };
        public int RuleOrder => 0;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var inputEventData = (InputEventData)eventData;

            RLKeyPress keyPress = inputEventData.Keyboard;
            RLMouse mouse = inputEventData.Mouse;

            IActivity currentActivity = _activitySystem.Peek();
            switch (currentActivity.Type)
            {
                case ActivityType.Menu:
                    (currentActivity.Data as Menu)?.HandleKeyPress(keyPress);
                    break;
                case ActivityType.StaticDisplay:
                    _activitySystem.Pop();
                    return false;
                case ActivityType.Gameplay:
                    _playerControlSystem.HandleKeyPress(keyPress);
                    _playerControlSystem.HandleMouseInput(mouse);
                    break;
                case ActivityType.Form:
                    (currentActivity.Data as Form)?.HandleKeyPress(keyPress);
                    break;
                case ActivityType.Targeting:
                    _targetingSystem.HandleMouseInput(mouse);
                    break;
            }

            return true;
        }
    }
}
