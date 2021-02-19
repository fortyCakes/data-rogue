using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Maps.MapGenCommands;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTMapEditorRenderer : BLTMapRenderer
    {
        public override Type DisplayType => typeof(MapEditorControl);

        protected override List<IEntity> GetEntitiesAt(ISystemContainer systemContainer, IMap map, MapCoordinate mapCoordinate)
        {
            var commands = map.MapGenCommands.Where(c => c.Vector == mapCoordinate.ToVector()).ToList();

            var entities = GetEntitiesForMapGenCommands(systemContainer, commands);

            entities.AddRange(systemContainer.PositionSystem.EntitiesAt(mapCoordinate));

            return entities;
        }

        private List<IEntity> GetEntitiesForMapGenCommands(ISystemContainer systemContainer, List<MapGenCommand> commands)
        {
            var entityCommandEntities = commands
                .Where(c => c.MapGenCommandType == MapGenCommandType.Entity)
                .Select(c => EntityCommandExecutor.GetEntityName(c, out _))
                .Select(entityName => systemContainer.PrototypeSystem.GetPrototype(entityName))
                .ToList();

            var otherCommandEntities = commands
                .Where(c => c.MapGenCommandType != MapGenCommandType.Entity)
                .Select(c => c.MapGenCommandType)
                .Select(c => systemContainer.EntityEngine.AllEntities.Single(e => e.Has<MapGenerationCommand>() && e.Get<MapGenerationCommand>().Command == c))
                .ToList();

            entityCommandEntities.AddRange(otherCommandEntities);

            return entityCommandEntities;
        }
    }
}