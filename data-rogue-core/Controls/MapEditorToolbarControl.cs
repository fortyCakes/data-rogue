using data_rogue_core.Components;
using data_rogue_core.Controls.MapEditorTools;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.IOSystems;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;

namespace data_rogue_core.Controls
{
    public class MapEditorToolbarControl : BaseControl
    {
        public override bool CanHandleMouse => true;

        public override ActionEventData HandleMouse(MouseData mouse, IDataRogueControlRenderer renderer, ISystemContainer systemContainer)
        {
            if (mouse.IsLeftClick)
            {
                var tool = renderer.EntityFromMouseData(this, systemContainer, mouse);
                if (tool != null)
                {
                    return new ActionEventData
                    {
                        Action = ActionType.ChangeMapEditorTool,
                        IsAction = false,
                        Parameters = tool.DescriptionName
                    };
                }
            }

            return null;
        }

        public static IEnumerable<IMapEditorTool> GetToolbarControls() => new List<IMapEditorTool>
        {
            new PenTool()
        };
        
    }

}
