using System.Drawing;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class ShowToastWhenEnteringBranchRule : IEventRule
    {
        private readonly ISystemContainer _systemContainer;

        public ShowToastWhenEnteringBranchRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.UsePortal };
        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.AfterSuccess;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var portal = eventData as Portal;

            var branchEntity = _systemContainer.PrototypeSystem.Get(portal.BranchLink);
            var branch = branchEntity.Get<Branch>();

            var branchName = branch.BranchName;
            if (branchName.StartsWith("Branch:"))
            {
                branchName = branchName.Remove(0, 7);
            }

            var toast = new ToastActivity(_systemContainer.ActivitySystem, branchName, Color.White);

            _systemContainer.ActivitySystem.Push(toast);

            return true;
        }
    }
}