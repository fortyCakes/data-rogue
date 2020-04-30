using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public static class MapRendererHelper
    {
        public static void DrawCell(RLConsole mapConsole, int x, int y, IPositionSystem positionSystem, IMap currentMap, int lookupX, int lookupY, List<MapCoordinate> playerFov, TargetingStatus cellTargeting = TargetingStatus.NotTargeted)
        {
            MapCoordinate coordinate = new MapCoordinate(currentMap.MapKey, lookupX, lookupY);
            var backColor = RLColor.Black;

            var isInFov = playerFov.Contains(coordinate);

            Appearance appearance = GetAppearanceAt(positionSystem, currentMap, coordinate, ref backColor, isInFov);

            var foreColor = isInFov ? appearance.Color.ToRLColor() : RLColor.Gray;

            backColor = ApplyTargetingColor(cellTargeting, backColor, isInFov);

            mapConsole.Set(x, y, foreColor, backColor, appearance.Glyph);
        }

        private static RLColor ApplyTargetingColor(TargetingStatus cellTargeting, RLColor backColor, bool isInFov)
        {
            if (isInFov)
            {
                if (cellTargeting.HasFlag(TargetingStatus.Targeted))
                {
                    return RLColor.Red;
                }

                if (cellTargeting.HasFlag(TargetingStatus.Targetable))
                {
                    return RLColor.LightRed;
                }
            }

            return backColor;
        }

        public static Appearance GetAppearanceAt(IPositionSystem positionSystem, IMap currentMap, MapCoordinate coordinate, ref RLColor backColor, bool isInFov)
        {
            Appearance appearance = null;


            var entity = positionSystem
                .EntitiesAt(coordinate)
                .OrderByDescending(a => a.Get<Appearance>().ZOrder)
                .FirstOrDefault(e => isInFov || IsRemembered(currentMap, coordinate, e));

            if (entity != null)
            {
                appearance = entity.Get<Appearance>();
                
                backColor = RLColor.Black;
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

        private static bool IsRemembered(IMap currentMap, MapCoordinate coordinate, IEntity e)
        {
            return currentMap.SeenCoordinates.Contains(coordinate) && e.Has<Memorable>();
        }
    }
}
