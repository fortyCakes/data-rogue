using System;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using data_rogue_core.Maps.Generators;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Maps.MapGenCommands;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public abstract class BaseBranchGenerator : IBranchGenerator
    {
        
        public abstract string GenerationType { get; }

        protected abstract List<Map> GenerateMaps(Branch branchDefinition, IEntityEngine engine, IPrototypeSystem prototypeSystem);

        protected abstract void CreateEntities(GeneratedBranch generatedBranch, Branch branch, IEntityEngine engine, IPositionSystem positionSystem, IPrototypeSystem prototypeSystem, string seed);

        protected IRandom Random { get; set; }

        public GeneratedBranch Generate(Branch branch, IEntityEngine engine, IPositionSystem positionSystem, IPrototypeSystem prototypeSystem, string seed)
        {
            Random = new RNG(seed);

            var generatedBranchMaps = GenerateMaps(branch, engine, prototypeSystem);

            var generatedBranch = new GeneratedBranch() { Maps = generatedBranchMaps };

            CreateEntities(generatedBranch, branch, engine, positionSystem, prototypeSystem, seed);
            ExecuteMapGenCommands(generatedBranch, branch, engine, prototypeSystem);

            Map previousMap = null;
            foreach (var map in generatedBranch.Maps)
            {
                if (previousMap != null)
                {
                    LinkStairs(previousMap, map, engine);
                }

                previousMap = map;
            }

            AddBranchPortals(generatedBranch, branch, engine, positionSystem, prototypeSystem);

            branch.Generated = true;

            return generatedBranch;
        }

        protected virtual void AddBranchPortals(GeneratedBranch generatedBranch, Branch branch, IEntityEngine engine, IPositionSystem positionSystem, IPrototypeSystem prototypeSystem)
        {
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

                thisPortal.BranchLink = prototypeSystem.Create(thatEnd.Branch).EntityId;

                var destinationBranch = prototypeSystem.Create(link.Value.Branch).Get<Branch>();

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

        protected virtual void LinkStairs(Map previousMap, Map map, IEntityEngine engine)
        {
            var stairsDown = GetStairs(previousMap, StairDirection.Down, engine);
            var stairsUp = GetStairs(map, StairDirection.Up, engine);

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

        protected void ExecuteMapGenCommands(GeneratedBranch generatedBranch, Branch branch, IEntityEngine engine, IPrototypeSystem prototypeSystem)
        {
            foreach (Map map in generatedBranch.Maps)
            {
                foreach (var command in map.MapGenCommands)
                {
                    var executor =  MapGenCommandExecutorFactory.GetExecutor(command.MapGenCommandType);

                    executor.Execute(map, engine, prototypeSystem, command, new Vector(0,0));
                }
            }
        }

        protected virtual List<KeyValuePair<MapCoordinate, Stairs>> GetStairs(Map map, StairDirection direction, IEntityEngine engine)
        {
            return engine.EntitiesWith<Stairs>()
                .Where(c => c.Get<Stairs>().Direction == direction)
                .Select(c => new KeyValuePair<MapCoordinate, Stairs>(c.Get<Position>().MapCoordinate, c.Get<Stairs>()))
                .ToList();
        }
    }
}