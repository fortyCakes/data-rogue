using System;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Maps.MapGenCommands;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core
{
    public abstract class BaseBranchGenerator : IBranchGenerator
    {        
        public abstract string GenerationType { get; }

        protected abstract List<Map> GenerateMaps(ISystemContainer systemContainer, Branch branchDefinition);

        protected IRandom Random { get; set; }

        public GeneratedBranch Generate(ISystemContainer systemContainer, IEntity branchEntity)
        {
            var branch = branchEntity.Get<Branch>();

            Random = new RNG(systemContainer.Seed + branch.BranchName);

            var generatedBranchMaps = GenerateMaps(systemContainer, branch);

            var generatedBranch = new GeneratedBranch() { Maps = generatedBranchMaps };

            CreateEntities(systemContainer, generatedBranch, branchEntity);
            ExecuteMapGenCommands(systemContainer, generatedBranch, branch);

            Map previousMap = null;
            foreach (var map in generatedBranch.Maps.OrderBy(m => m.MapKey.Key))
            {
                if (previousMap != null)
                {
                    LinkStairs(systemContainer, previousMap, map);
                }

                previousMap = map;
            }

            AddBranchPortals(systemContainer, generatedBranch, branch);

            branch.Generated = true;

            return generatedBranch;
        }

        protected virtual void CreateEntities(ISystemContainer systemContainer, GeneratedBranch generatedBranch, IEntity branchEntity)
        {
            var entityGenerationSteps = branchEntity.Components.OfType<EntityGenerationStrategy>();

            foreach(var step in entityGenerationSteps)
            {
                var generator = EntityGeneratorFactory.GetGenerator(step.EntityGenerationType);

                generator.Generate(systemContainer, generatedBranch, branchEntity, step, Random);
            }
        }

        protected virtual void AddBranchPortals(ISystemContainer systemContainer, GeneratedBranch generatedBranch, Branch branch)
        {
            var engine = systemContainer.EntityEngine;
            var prototypeSystem = systemContainer.PrototypeSystem;
            var positionSystem = systemContainer.PositionSystem;

            var links = engine.GetAll<BranchLink>();

            var relevantLinks = new Dictionary<BranchLinkEnd, BranchLinkEnd>();
            foreach (var link in links)
            {
                if (link.From.Branch == branch.BranchName)
                {
                    relevantLinks.Add(link.From, link.To);
                }

                if (link.To.Branch == branch.BranchName)
                {
                    relevantLinks.Add(link.To, link.From);
                }
            }

            foreach (var link in relevantLinks)
            {
                var thisEnd = link.Key;
                var thatEnd = link.Value;

                var level = Random.Between(thisEnd.LevelFrom, thisEnd.LevelTo);

                var mapKey = new MapKey(branch.LevelName(level));

                var potentialPortals = engine.EntitiesWith<Portal>()
                    .Where(p => p.Get<Position>().MapCoordinate.Key == mapKey)
                    .Where(p => !p.Get<Portal>().BranchLink.HasValue)
                    .ToList();

                if (!potentialPortals.Any())
                {
                    throw new Exception("No available portals to place branch link");
                }

                var portalEntity = Random.PickOne(potentialPortals);
                var thisPortal = portalEntity.Get<Portal>();

                thisPortal.BranchLink = prototypeSystem.Get(thatEnd.Branch).EntityId;

                var destinationBranch = prototypeSystem.Get(link.Value.Branch).Get<Branch>();

                thisEnd.Location = portalEntity.Get<Position>().MapCoordinate;

                if (thatEnd.Location != null)
                {
                    var thatPortal = positionSystem.EntitiesAt(thatEnd.Location)
                        .Single(e => e.Has<Portal>())
                        .Get<Portal>();

                    thisPortal.Destination = thatEnd.Location;
                    thatPortal.Destination = thisEnd.Location;
                }
            }
        }

        protected virtual void LinkStairs(ISystemContainer systemContainer, Map previousMap, Map map)
        {
            var stairsDown = GetStairs(previousMap, StairDirection.Down, systemContainer.EntityEngine);
            var stairsUp = GetStairs(map, StairDirection.Up, systemContainer.EntityEngine);

            if (stairsUp.Count != stairsDown.Count)
            {
                throw new ApplicationException("Stair counts do not match between floors");
            }

            while (stairsDown.Count > 0)
            {
                var downStairs = Random.PickOne(stairsDown);
                var upStairs = Random.PickOne(stairsUp);

                downStairs.Value.Destination = upStairs.Key;
                upStairs.Value.Destination = downStairs.Key;

                stairsDown.Remove(downStairs);
                stairsUp.Remove(upStairs);
            }
        }

        protected void ExecuteMapGenCommands(ISystemContainer systemContainer, GeneratedBranch generatedBranch, Branch branch)
        {
            foreach (Map map in generatedBranch.Maps)
            {
                foreach (var command in map.MapGenCommands)
                {
                    var executor =  MapGenCommandExecutorFactory.GetExecutor(command.MapGenCommandType);

                    executor.Execute(systemContainer, map, command, new Vector(0,0));
                }
            }
        }

        protected virtual List<KeyValuePair<MapCoordinate, Stairs>> GetStairs(Map map, StairDirection direction, IEntityEngine engine)
        {
            return engine.EntitiesWith<Stairs>()
                .Where(e => e.Get<Position>().MapCoordinate.Key == map.MapKey)
                .Where(c => c.Get<Stairs>().Direction == direction)
                .Select(c => new KeyValuePair<MapCoordinate, Stairs>(c.Get<Position>().MapCoordinate, c.Get<Stairs>()))
                .ToList();
        }



        protected void PlaceStairs(ISystemContainer systemContainer, GeneratedBranch generatedBranch)
        {
            Map previousMap = null;
            foreach (Map map in generatedBranch.Maps.OrderBy(m => m.MapKey.Key))
            {
                if (previousMap != null)
                {
                    PlaceStairSet(systemContainer, previousMap, map);
                }

                previousMap = map;
            }
        }

        protected void PlaceStairSet(ISystemContainer systemContainer, Map previousMap, Map map)
        {

            var downStairsCoordinate = previousMap.GetEmptyPosition(systemContainer.PrototypeSystem, systemContainer.PositionSystem, Random);
            var upStairsCoordinate = map.GetEmptyPosition(systemContainer.PrototypeSystem, systemContainer.PositionSystem, Random);

            var downStairs = systemContainer.PrototypeSystem.CreateAt("Props:StairsDown", downStairsCoordinate);
            var upStairs = systemContainer.PrototypeSystem.CreateAt("Props:StairsUp", upStairsCoordinate);
        }

        protected void PlaceDefaultEntrancePortal(ISystemContainer systemContainer, GeneratedBranch generatedBranch, Branch branch)
        {
            var firstLayer = generatedBranch.Maps.First();

            MapCoordinate emptyPosition = firstLayer.GetEmptyPosition(systemContainer.PrototypeSystem, systemContainer.PositionSystem, Random);

            systemContainer.PrototypeSystem.CreateAt("Props:Portal", emptyPosition);
        }

        
    }
}