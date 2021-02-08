using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Controls
{
    public class MapEditorControl: MapControl
    {
        public override ActionEventData HandleMouse(MouseData mouse, IDataRogueControlRenderer renderer, ISystemContainer systemContainer)
        {
            var mapEditor = systemContainer.ActivitySystem.MapEditorActivity;

            if ((mouse.LeftButtonDown && !mapEditor.CurrentTool.RequiresClick) || mouse.IsLeftClick)
            {
                MapCoordinate mapCoordinate = systemContainer.RendererSystem.Renderer.GetMapEditorMapCoordinateFromMousePosition(systemContainer.RendererSystem.CameraPosition, mouse.X, mouse.Y);
                mapEditor.ApplyTool(mapCoordinate, mapEditor.PrimaryCell);
            }

            if ((mouse.RightButtonDown && !mapEditor.CurrentTool.RequiresClick) || mouse.IsRightClick)
            {
                MapCoordinate mapCoordinate = systemContainer.RendererSystem.Renderer.GetMapEditorMapCoordinateFromMousePosition(systemContainer.RendererSystem.CameraPosition, mouse.X, mouse.Y);
                mapEditor.ApplyTool(mapCoordinate, mapEditor.SecondaryCell);
            }

            return null;
        }
    }
}
