using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public static class MapRendererHelper
    {
        public static void DrawCell(RLConsole mapConsole, int x, int y, IPositionSystem positionSystem, Map currentMap, int lookupX, int lookupY, List<MapCoordinate> playerFov, CellTargeting cellTargeting = CellTargeting.None)
        {
            MapCoordinate coordinate = new MapCoordinate(currentMap.MapKey, lookupX, lookupY);
            var backColor = RLColor.Black;

            var isInFov = playerFov.Contains(coordinate);

            Appearance appearance = GetAppearanceAt(positionSystem, currentMap, coordinate, ref backColor, isInFov);

            var foreColor = isInFov ? appearance.Color.ToRLColor() : RLColor.Gray;

            backColor = ApplyTargetingColor(cellTargeting, backColor, isInFov);

            mapConsole.Set(x, y, foreColor, backColor, appearance.Glyph);
        }

        private static RLColor ApplyTargetingColor(CellTargeting cellTargeting, RLColor backColor, bool isInFov)
        {
            if (isInFov)
            {
                if (cellTargeting.HasFlag(CellTargeting.CurrentTarget))
                {
                    return RLColor.Red;
                }

                if (cellTargeting.HasFlag(CellTargeting.Targetable))
                {
                    return RLColor.LightRed;
                }
            }

            return backColor;
        }

        public static Appearance GetAppearanceAt(IPositionSystem positionSystem, Map currentMap, MapCoordinate coordinate, ref RLColor backColor, bool isInFov)
        {
            Appearance appearance = null;


            var entity = positionSystem
                .EntitiesAt(coordinate)
                .OrderByDescending(a => a.Get<Appearance>().ZOrder)
                .FirstOrDefault(e => isInFov || IsRemembered(currentMap, coordinate, e));

            if (entity != null)
            {
                appearance = entity.Get<Appearance>();

                if (entity.Has<Fighter>())
                {
                    var fighter = entity.Get<Fighter>();

                    if (fighter.BrokenTicks > 0)
                    {
                        backColor = RLColor.Red;
                    }
                    else
                    {
                        backColor = ColorExtensions.Gradient(fighter.Tilt.Max, Color.Black, Color.Purple, fighter.Tilt.Current);
                    }
                }
            }
            else
            {
                appearance = new Appearance()
                {
                    Color = Color.Black,
                    Glyph = ' ',
                    ZOrder = 0
                };

                backColor = RLColor.Black;
            }

            return appearance;
        }

        private static bool IsRemembered(Map currentMap, MapCoordinate coordinate, IEntity e)
        {
            return currentMap.SeenCoordinates.Contains(coordinate) && e.Has<Memorable>();
        }
    }
}
