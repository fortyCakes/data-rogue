using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System;
using System.Collections.Generic;

namespace data_rogue_core.Controls.MapEditorTools
{
    public class FillTool : IMapEditorTool
    {
        public IEntity Entity => new Entity(0, "FillTool",
            new IEntityComponent[] {
                new Description {Name = "Fill Tool", Detail = "A tool that paints all touching cells of the same type." },
                new Appearance { Glyph = 'B' },
                new SpriteAppearance { Bottom = "fill_tool" }
            });

        public bool RequiresClick => true;

        public void Apply(IMap map, MapCoordinate mapCoordinate, IEntity currentCell, IEntity alternateCell, ISystemContainer systemContainer)
        {
            foreach(var coordinate in GetTargetedCoordinates(map, mapCoordinate))
            {
                map.SetCell(coordinate, currentCell);
            }
        }

        public IEnumerable<MapCoordinate> GetTargetedCoordinates(IMap map, MapCoordinate mapCoordinate)
        {
            var cell = map.CellAt(mapCoordinate);

            Func<MapCoordinate, bool> CanFillInto = (coordinate) => { return map.CellAt(coordinate) == cell; };
            
            return FloodFillHelper.FloodFill(mapCoordinate, CanFillInto);
        }

        public virtual IEnumerable<MapCoordinate> GetInternalCoordinates(IMap map, MapCoordinate secondCoordinate) => new List<MapCoordinate>();
    }
}