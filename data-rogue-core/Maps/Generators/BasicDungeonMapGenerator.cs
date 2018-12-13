using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntitySystem;

namespace data_rogue_core.Maps.Generators
{
    public class BasicDungeonMapGenerator : IMapGenerator
    {
        private IEntityEngineSystem Engine { get; }
        public IRandom Random;

        private int size = 100;
        private int numRooms = 10;
        private int roomSize = 10;

        private List<Room> Rooms = new List<Room>();

        private Entity wallCell;
        private Entity floorCell;
        private Entity spawnCell;

        public BasicDungeonMapGenerator(IEntityEngineSystem engine)
        {
            Engine = engine;
        }

        public Map Generate(string mapName, string seed)
        {
            Random = new RNG(seed);

            wallCell = Engine.GetEntityWithName("Cell:Wall");
            floorCell = Engine.GetEntityWithName("Cell:Empty");
            spawnCell = Engine.GetEntityWithName("Cell:SpawnPoint");

            var map = new Map(mapName, wallCell);

            Room previousRoom = null;

            for (int i = 0; i < numRooms; i++)
            {
                int leftX = Random.Next(0, size);
                int topY = Random.Next(0, size);

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

            var spawnAt = Random.PickOne(map.Cells.Where(m => m.Value == floorCell).ToList());

            map.SetCell(spawnAt.Key, spawnCell);

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
                random.Next(LeftX, RightX),
                random.Next(TopY, BottomY));
        }
    }
}
