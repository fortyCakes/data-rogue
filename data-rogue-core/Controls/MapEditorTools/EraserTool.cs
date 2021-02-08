﻿using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System.Collections.Generic;

namespace data_rogue_core.Controls.MapEditorTools
{

    public class EraserTool : IMapEditorTool
    {
        public IEntity Entity => new Entity(0, "EraserTool",
            new IEntityComponent[] {
                new Description {Name = "Eraser Tool", Detail = "A tool for erasing tiles." },
                new Appearance { Glyph = 'E' },
                new SpriteAppearance { Bottom = "eraser_tool" }
            });

        public bool RequiresClick => false;

        public void Apply(IMap map, MapCoordinate mapCoordinate, IEntity currentCell, IEntity alternateCell)
        {
            map.RemoveCell(mapCoordinate);
        }

        public IEnumerable<MapCoordinate> GetTargetedCoordinates(IMap map, MapCoordinate mapCoordinate)
        {
            return new List<MapCoordinate> { mapCoordinate };
        }
    }
}