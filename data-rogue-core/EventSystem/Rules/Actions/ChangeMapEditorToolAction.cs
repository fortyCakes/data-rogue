using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class ChangeMapEditorToolAction : ApplyActionRule
    {
        public ChangeMapEditorToolAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.ChangeMapEditorTool;
        public override ActivityType activityType => ActivityType.MapEditor;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var mapEditorActivity = _systemContainer.ActivitySystem.MapEditorActivity;

            mapEditorActivity.SetTool(eventData.Parameters);
            _systemContainer.MessageSystem.Write($"Change tool: {eventData.Parameters}");

            return true;
        }
    }
}