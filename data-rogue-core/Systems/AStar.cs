using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Maps;

namespace data_rogue_core.Systems
{
    public static class AStar
    {
        private class AStarLocation
        {
            public AStarLocation Parent;

            public int X;
            public int Y;
            public int DistanceFromStart;
            
            public int EstimatedFromEnd;
            public int LocationScore => DistanceFromStart + EstimatedFromEnd;

            public void SetDistanceTo(AStarLocation destination)
            {
                EstimatedFromEnd = Math.Abs(destination.X - X) + Math.Abs(destination.Y - Y);
            }
        }

        public static IEnumerable<MapCoordinate> Path(Map map, MapCoordinate origin, MapCoordinate destination)
        {
            AStarLocation current = null;

            var start = new AStarLocation() {X = origin.X, Y = origin.Y};
            var target = new AStarLocation() {X = destination.X, Y = destination.Y};
            var openList = new List<AStarLocation>();
            var closedList = new List<AStarLocation>();
            int distanceFromStart = 0;

            openList.Add(start);

            while (openList.Count > 0)
            {
                current = openList.OrderBy(l => l.LocationScore).First();

                closedList.Add(current);
                openList.Remove(current);

                if (IsSameLocation(current, target))
                {
                    break;
                }

                var adjacentSquares = GetPassableAdjacentSquares(current, map);
                distanceFromStart = current.DistanceFromStart + 1;

                foreach (var adjacentSquare in adjacentSquares)
                {
                    if (closedList.Any(l => IsSameLocation(adjacentSquare, l)))
                    {
                        continue;
                    }

                    if (openList.Any(l => IsSameLocation(adjacentSquare, l)))
                    {
                        if (distanceFromStart + adjacentSquare.EstimatedFromEnd < adjacentSquare.LocationScore)
                        {
                            adjacentSquare.DistanceFromStart = distanceFromStart;
                            adjacentSquare.Parent = current;
                        }
                    }
                    else
                    {
                        adjacentSquare.DistanceFromStart = distanceFromStart;
                        adjacentSquare.SetDistanceTo(target);
                        adjacentSquare.Parent = current;

                        // and add it to the open list
                        openList.Insert(0, adjacentSquare);
                    }
                }
            }

            if (!IsSameLocation(current, target)) return null;

            var path = GetPath(current);

            return path.Select(l => new MapCoordinate(map.MapKey, l.X, l.Y));
        }

        private static List<AStarLocation> GetPath(AStarLocation current)
        {
            var path = new List<AStarLocation>();

            do
            {
                path.Add(current);
                current = current.Parent;
            } while (current.Parent != null);

            path.Reverse();

            return path;
        }
        

        private static IEnumerable<AStarLocation> GetPassableAdjacentSquares(AStarLocation current, Map map)
        {
            var x = current.X;
            var y = current.Y;

            var proposedLocations = new List<AStarLocation>()
            {
                new AStarLocation { X = x, Y = y - 1 },
                new AStarLocation { X = x, Y = y + 1 },
                new AStarLocation { X = x - 1, Y = y },
                new AStarLocation { X = x + 1, Y = y },
                new AStarLocation { X = x + 1, Y = y - 1 },
                new AStarLocation { X = x - 1, Y = y + 1 },
                new AStarLocation { X = x - 1, Y = y - 1 },
                new AStarLocation { X = x + 1, Y = y + 1 },
            };

            return proposedLocations.Where(loc => IsPassable(map, loc));
        }

        private static bool IsPassable(Map map, AStarLocation location)
        {
            var coordinate = new MapCoordinate(map.MapKey, location.X, location.Y);

            return map.CellAt(coordinate).Get<Physical>().Passable;
        }

        private static bool IsSameLocation(AStarLocation current, AStarLocation target)
        {
            return current.X == target.X && current.Y == target.Y;
        }
    }
}