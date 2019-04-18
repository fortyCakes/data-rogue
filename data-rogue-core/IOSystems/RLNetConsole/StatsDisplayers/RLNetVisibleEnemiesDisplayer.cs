﻿using System;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
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

            foreach (var enemy in enemiesInFov.OrderBy(e => e.EntityId))
            {
                PrintEntityDetails(display, enemy, console);
            }
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var enemiesInFov = GetEnemiesInFov(systemContainer, playerFov);

            return new Size(console.Width, enemiesInFov.Count * 3);
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
