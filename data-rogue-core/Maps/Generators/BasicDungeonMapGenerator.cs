using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using System;

namespace data_rogue_core.Maps.Generators
{
    public class BasicDungeonMapGenerator : IMapGenerator
    {
        public string FloorCell { get; }
        public string WallCell { get; }
        private IEntityEngine Engine { get; }
        public IPrototypeSystem PrototypeSystem { get; }

        public IRandom Random;

        private int size = 50;
        private int numRooms = 10;
        private int roomSize = 10;

        private IEntity wallCell;
        private IEntity floorCell;

        public BasicDungeonMapGenerator(ISystemContainer systemContainer, string floorCell, string wallCell)
        {
            FloorCell = floorCell;
            WallCell = wallCell;
            Engine = systemContainer.EntityEngine;
            PrototypeSystem = systemContainer.PrototypeSystem;
        }

        public IMap Generate(string mapName, IRandom random, IProgress<string> progress)
        {
            Random = random;

            wallCell = PrototypeSystem.Get(WallCell);
            floorCell = PrototypeSystem.Get(FloorCell);

            var map = new Map(mapName, wallCell);

            Room previousRoom = null;

            for (int i = 0; i < numRooms; i++)
            {
                int leftX = Random.Between(0, size);
                int topY = Random.Between(0, size);

                var room = new Room
                {
                    LeftX = leftX,
                    RightX = leftX + roomSize - 1,
                    TopY = topY,
                    BottomY = topY + roomSize - 1
                };

                map.SetCellsInRange(
                    room.LeftX, room.RightX,
                    room.TopY, room.BottomY,
                    floorCell);

                if (previousRoom != null)
                {
                    DigTunnel(map, room, previousRoom);
                }

                previousRoom = room;
            }

            return map;
        }

        private void DigTunnel(Map map, Room room, Room previousRoom)
        {
            var point1 = room.GetPointInRoom(Random);
            var point2 = previousRoom.GetPointInRoom(Random);

            map.SetCellsInRange(point1.X, point2.X, point1.Y, point1.Y, floorCell);
            map.SetCellsInRange(point2.X, point2.X, point1.Y, point2.Y, floorCell);
        }
    }

    internal class Room
    {
        public int LeftX { get; set; }
        public int TopY { get; set; }
        public int RightX { get; set; }
        public int BottomY { get; set; }

        public Vector GetPointInRoom(IRandom random)
        {
            return new Vector(
                random.Between(LeftX, RightX),
                random.Between(TopY, BottomY));
        }
    }
}
