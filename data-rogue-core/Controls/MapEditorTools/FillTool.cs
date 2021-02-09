﻿using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

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
            
            return FloodFill(mapCoordinate, CanFillInto);
        }

        public static IEnumerable<MapCoordinate> FloodFill(MapCoordinate mapCoordinate, Func<MapCoordinate, bool> canFillInto)
        {
            var targetedCoordinates = new List<MapCoordinate>();

            var coordinatesToCheck = new Queue<MapCoordinate>();
            coordinatesToCheck.Enqueue(mapCoordinate);

            var alreadyChecked = new List<MapCoordinate> { };

            while (coordinatesToCheck.Any())
            {
                var coordinate = coordinatesToCheck.Dequeue();

                if (canFillInto(coordinate))
                {
                    targetedCoordinates.Add(coordinate);

                    foreach (var adjacentVector in Vector.GetAdjacentCellVectors())
                    {
                        var newCoordinate = coordinate + adjacentVector;
                        if (!alreadyChecked.Contains(newCoordinate) && !coordinatesToCheck.Contains(newCoordinate))
                        {
                            coordinatesToCheck.Enqueue(newCoordinate);
                        }
                    }
                }

                alreadyChecked.Add(coordinate);

                var vector = mapCoordinate - coordinate;

                if (vector.Length > 1000 || alreadyChecked.Count > 1000)
                {
                    // The flood fill has almost certainly escaped.
                    return new List<MapCoordinate>();
                }
            }

            return targetedCoordinates;
        }

        public virtual IEnumerable<MapCoordinate> GetInternalCoordinates(IMap map, MapCoordinate secondCoordinate) => new List<MapCoordinate>();
    }
}