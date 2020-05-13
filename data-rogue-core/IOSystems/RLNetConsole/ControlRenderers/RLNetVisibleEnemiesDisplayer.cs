using System;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using RLNET;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Controls;

namespace data_rogue_core.IOSystems
{

    public class RLNetVisibleEnemiesDisplayer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(VisibleEnemiesControl);
        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;
            var enemiesInFov = GetEnemiesInFov(systemContainer, playerFov);
            var line = 0;
            foreach (var enemy in enemiesInFov.OrderBy(e => e.EntityId))
            {
                PrintEntityDetails(display, enemy, console, line);
                line += 2;

                if (control.Position.Top + line >= control.Position.Bottom) break;
            }
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;
            var enemiesInFov = GetEnemiesInFov(systemContainer, playerFov);

            return new Size(display.Position.Width, enemiesInFov.Count * 2);
        }

        private static IEntity GetEnemy(MapCoordinate mapCoordinate, ISystemContainer systemContainer)
        {
            return systemContainer.PositionSystem.EntitiesAt(mapCoordinate).FirstOrDefault(e => IsEnemy(systemContainer, e));
        }

        private static bool IsEnemy(ISystemContainer systemContainer, IEntity e)
        {
            return !systemContainer.PlayerSystem.IsPlayer(e) && e.Has<Health>() && e.Has<Appearance>();
        }

        private static List<IEntity> GetEnemiesInFov(ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var enemiesInFov = new List<IEntity>();

            foreach (var mapCoordinate in playerFov)
            {
                var enemy = GetEnemy(mapCoordinate, systemContainer);
                if (enemy != null)
                {
                    enemiesInFov.Add(enemy);
                }
            }

            return enemiesInFov;
        }
    }
}
