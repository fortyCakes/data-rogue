using System;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;
using data_rogue_core.Maps.Generators;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Systems;

namespace data_rogue_core
{
    public abstract class BaseBranchGenerator : IBranchGenerator
    {
        public abstract string GenerationType { get; }

        protected abstract List<Map> GenerateMaps(Branch branchDefinition, IEntityEngineSystem engine);

        protected abstract void AddEntities(GeneratedBranch branch, Branch branchDefinition, IEntityEngineSystem engine, IPositionSystem position);

        

        protected IRandom Random { get; set; }

        public GeneratedBranch Generate(Branch branchDefinition, IEntityEngineSystem engine, IPositionSystem positionSystem, string seed)
        {
            Random = new RNG(seed);

            var generatedBranchMaps = GenerateMaps(branchDefinition, engine);

            var generatedBranch = new GeneratedBranch() { Maps = generatedBranchMaps };

            AddEntities(generatedBranch, branchDefinition, engine, positionSystem);

            Map previousMap = null;
            foreach (var map in generatedBranch.Maps)
            {
                if (previousMap != null)
                {
                    LinkStairs(previousMap, map);
                }

                previousMap = map;
            }

            AddBranchPortals(generatedBranch, branchDefinition, engine, positionSystem);

            branchDefinition.Generated = true;

            return generatedBranch;
        }

        protected virtual void AddBranchPortals(GeneratedBranch branch, Branch branchDefinition, IEntityEngineSystem engine, IPositionSystem positionSystem)
        {
            var links = engine.GetAll<BranchLink>();

            var relevantLinks = new Dictionary<BranchLinkEnd, BranchLinkEnd>();
            foreach (var link in links)
            {
                if (link.From.Branch == branchDefinition.BranchName)
                {
                    relevantLinks.Add(link.From, link.To);
                }

                if (link.To.Branch == branchDefinition.BranchName)
                {
                    relevantLinks.Add(link.To, link.From);
                }
            }

            foreach (var link in relevantLinks)
            {
                var thisEnd = link.Key;
                var thatEnd = link.Value;

                var level = Random.Between(thisEnd.LevelFrom, thisEnd.LevelTo);

                var mapKey = new MapKey($"{branchDefinition.BranchName}:{level}");

                var potentialPortals = engine.EntitiesWith<Portal>()
                    .Where(p => p.Get<Position>().MapCoordinate.Key == mapKey)
                    .Where(p => string.IsNullOrEmpty(p.Get<Portal>().BranchLink))
                    .ToList();

                if (!potentialPortals.Any())
                {
                    throw new Exception("No available portals to place branch link");
                }

                var portalEntity = Random.PickOne(potentialPortals);
                var thisPortal = portalEntity.Get<Portal>();

                thisPortal.BranchLink = thatEnd.Branch;

                var destinationBranch = engine.GetEntityWithName(link.Value.Branch).Get<Branch>();

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


        protected virtual void LinkStairs(Map previousMap, Map map)
        {
            var stairsDown = GetStairs(previousMap, StairDirection.Down);
            var stairsUp = GetStairs(map, StairDirection.Up);

            if (stairsUp.Count != stairsDown.Count)
            {
                throw new ApplicationException("Stair counts do not match between floors");
            }

            while (stairsDown.Count > 0)
            {
                var downStairs = Random.PickOne(stairsDown);
                var upStairs = Random.PickOne(stairsUp);

                downStairs.Value.Get<Stairs>().Destination = upStairs.Key;
                upStairs.Value.Get<Stairs>().Destination = downStairs.Key;

                stairsDown.Remove(downStairs);
                stairsUp.Remove(upStairs);
            }
        }

        protected virtual List<KeyValuePair<MapCoordinate, IEntity>> GetStairs(Map map, StairDirection direction)
        {
            return map.Cells.Where(c => c.Value.Has<Stairs>() && c.Value.Get<Stairs>().Direction == direction).ToList();
        }
    }
}