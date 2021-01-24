using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_one.EventSystem.Utils;

namespace data_rogue_core.EventSystem.Rules
{

    public class MorgueAction : ApplyActionRule
    {
        private ISystemContainer _systemContainer;

        public MorgueAction(ISystemContainer systemContainer) : base(systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public override ActionType actionType => ActionType.Morgue;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            _systemContainer.SaveSystem.SaveMorgueFile(MorgueHelper.GenerateMorgueText(_systemContainer));
            
            return false;
        }
    }
}
