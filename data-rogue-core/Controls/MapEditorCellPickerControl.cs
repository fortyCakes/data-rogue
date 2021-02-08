using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Controls
{
    public class MapEditorCellPickerControl : BaseControl
    {
        public override bool CanHandleMouse => true;
        public override ActionEventData HandleMouse(MouseData mouse, IDataRogueControlRenderer renderer, ISystemContainer systemContainer)
        {
            if (mouse.IsLeftClick)
            {
                var mapEditor = systemContainer.ActivitySystem.MapEditorActivity;

                var command = renderer.StringFromMouseData(this, systemContainer, mouse);

                if (command == "Primary Cell")
                {
                    mapEditor.ShowChangePrimaryCellDialogue();
                }

                if (command == "Secondary Cell")
                {
                    mapEditor.ShowChangeSecondaryCellDialogue();
                }

                if (command == "Default Cell")
                {
                    mapEditor.ShowChangeDefaultCellDialogue();
                }
            }

            return null;
        }
    }

}
