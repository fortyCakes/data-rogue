using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace data_rogue_core.IOSystems
{

    public class RLNetVisibleEnemiesDisplayer : RLNetStatsRendererHelper
    {
        public override string DisplayType => "VisibleEnemies";

        protected override void DisplayInternal(RLConsole console, StatsDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int line)
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

            foreach (var enemy in enemiesInFov.OrderBy(e => e.EntityId))
            {
                PrintEntityDetails(display, enemy, console, ref line);
            }
        }

        private static IEntity GetEnemy(MapCoordinate mapCoordinate, ISystemContainer systemContainer)
        {
            return systemContainer.PositionSystem.EntitiesAt(mapCoordinate).FirstOrDefault(e => IsEnemy(systemContainer, e));
        }

        private static bool IsEnemy(ISystemContainer systemContainer, IEntity e)
        {
            return !systemContainer.PlayerSystem.IsPlayer(e) && e.Has<Health>() && e.Has<Appearance>();
        }
    }
}
