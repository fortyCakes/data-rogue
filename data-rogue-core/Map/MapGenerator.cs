using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Display;
using data_rogue_core.Entities;
using data_rogue_core.Map.Vaults;
using RogueSharp;
using RogueSharp.DiceNotation;

namespace data_rogue_core.Map
{
    public struct MapGeneratorParameters
    {
        public int Width;
        public int Height;
        public int MaxRooms;
        public int RoomMaxSize;
        public int RoomMinSize;
        public int VaultChance;
    }

    public class MapGenerator
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _maxRooms;
        private readonly int _roomMaxSize;
        private readonly int _roomMinSize;
        private readonly int _vaultChance;

        private readonly DungeonMap _map;
        private IMonsterGenerator _monsterGenerator;
        private readonly IVaultGenerator _vaultGenerator;

        // Constructing a new MapGenerator requires the dimensions of the maps it will create
        // as well as the sizes and maximum number of rooms
        public MapGenerator(MapGeneratorParameters parameters, IMonsterGenerator monsterGenerator, IVaultGenerator vaultGenerator)
        {
            _width = parameters.Width;
            _height = parameters.Height;
            _maxRooms = parameters.MaxRooms;
            _roomMaxSize = parameters.RoomMaxSize;
            _roomMinSize = parameters.RoomMinSize;
            _vaultChance = parameters.VaultChance;
            _map = new DungeonMap();
            _monsterGenerator = monsterGenerator;
            _vaultGenerator = vaultGenerator;
        }

        // Generate a new Map that places rooms randomly
        public DungeonMap CreateMap()
        {
            // Set the properties of all cells to false
            _map.Initialize(_width, _height, '#', Colors.Wall);

            // Try to place as many rooms as the specified maxRooms
            for (int r = 0; r < _maxRooms; r++)
            {
                IRoom newRoom;
                if (Game.Random.Next(100) < _vaultChance)
                {
                    newRoom = CreateVaultRoom();
                }
                else
                {
                    newRoom = CreateRectangleRoom();
                }
                if (newRoom != null) _map.Rooms.Add(newRoom);
            }
            // Iterate through each room that we wanted placed 
            // call CreateRoom to make it
            foreach (IRoom room in _map.Rooms)
            {
                room.CreateRoom(_map);
            }

            // Iterate through each room that was generated
            // Don't do anything with the first room, so start at r = 1 instead of r = 0
            for (int r = 1; r < _map.Rooms.Count; r++)
            {
                // For all remaing rooms get the center of the room and the previous room
                int previousRoomCenterX = _map.Rooms[r - 1].Center.X;
                int previousRoomCenterY = _map.Rooms[r - 1].Center.Y;
                int currentRoomCenterX = _map.Rooms[r].Center.X;
                int currentRoomCenterY = _map.Rooms[r].Center.Y;

                // Give a 50/50 chance of which 'L' shaped connecting hallway to tunnel out
                if (Game.Random.Next(1, 2) == 1)
                {
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, previousRoomCenterY);
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, currentRoomCenterX);
                }
                else
                {
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, previousRoomCenterX);
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, currentRoomCenterY);
                }
            }

            foreach (IRoom room in _map.Rooms)
            {
                CreateDoors(room);
            }

            PlacePlayer();
            PlaceMonsters();

            return _map;
        }

        private IRoom CreateVaultRoom()
        {
            var vault = _vaultGenerator.GetVault();
            int roomWidth = vault.Width;
            int roomHeight = vault.Height;
            int roomXPosition = Game.Random.Next(0, _width - roomWidth - 1);
            int roomYPosition = Game.Random.Next(0, _height - roomHeight - 1);

            var newRoom = new VaultRoom(vault, roomXPosition, roomYPosition);

            bool newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));
            if (!newRoomIntersects)
            {
                return newRoom;
            }
            else
            {
                return null;
            }
        }

        private IRoom CreateRectangleRoom()
        {
            // Determine the size and position of the room randomly
            int roomWidth = Game.Random.Next(_roomMinSize, _roomMaxSize);
            int roomHeight = Game.Random.Next(_roomMinSize, _roomMaxSize);
            int roomXPosition = Game.Random.Next(0, _width - roomWidth - 1);
            int roomYPosition = Game.Random.Next(0, _height - roomHeight - 1);

            var newRoom = new RectangleRoom(roomXPosition, roomYPosition,
                roomWidth, roomHeight);

            bool newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));

            if (!newRoomIntersects)
            {
                return newRoom;
            }
            else
            {
                return null;
            }
        }

        // Given a rectangular area on the Map
        // set the cell properties for that area to true
        

        // Carve a tunnel out of the Map parallel to the x-axis
        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                if (!IsInRoom(x,yPosition))
                    _map.SetCellProperties(x, yPosition, true, true, '.', Colors.Floor);
            }
        }

        // Carve a tunnel out of the Map parallel to the y-axis
        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                if (!IsInRoom(xPosition,y))
                    _map.SetCellProperties(xPosition, y, true, true, '.', Colors.Floor);
            }
        }

        private bool IsInRoom(int x, int y)
        {
            return _map.Rooms.Any(r => r.Contains(x, y));
        }

        private void PlacePlayer()
        {
            Player player = Game.Player;
            if (player == null)
            {
                player = new Player();
            }

            player.X = _map.Rooms[0].Center.X;
            player.Y = _map.Rooms[0].Center.Y;

            _map.AddPlayer(player);
        }

        private void PlaceMonsters()
        {
            foreach (var room in _map.Rooms)
            {
                // Each room has a 60% chance of having monsters
                if (Dice.Roll("1D10") < 7)
                {
                    // Generate between 1 and 4 monsters
                    var numberOfMonsters = Dice.Roll("1D4");
                    for (int i = 0; i < numberOfMonsters; i++)
                    {
                        // Find a random walkable location in the room to place the monster
                        Point randomRoomLocation = _map.GetRandomWalkableLocationInRoom(room);
                        // It's possible that the room doesn't have space to place a monster
                        // In that case skip creating the monster
                        if (randomRoomLocation != null)
                        {
                            // Temporarily hard code this monster to be created at level 1
                            var monster = _monsterGenerator.GetNewMonster();
                            monster.X = randomRoomLocation.X;
                            monster.Y = randomRoomLocation.Y;
                            _map.AddMonster(monster);
                        }
                    }
                }
            }
        }

        private void CreateDoors(IRoom room)
        {
            // The boundaries of the room
            int xMin = room.Left-1;
            int xMax = room.Right;
            int yMin = room.Top-1;
            int yMax = room.Bottom;


            // Put the rooms border cells into a list
            List<Cell> borderCells = new List<Cell>();
            if (yMin > 0 && xMin>0 && xMax < _map.Width)              borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMin, xMax, yMin));
            if (xMin > 0 && yMin > 0 && yMax < _map.Height)           borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMin, xMin, yMax));
            if (xMax < _map.Width && yMax < _map.Height && xMin > 0)  borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMax, xMax, yMax));
            if (yMax < _map.Height && xMax < _map.Width && yMin > 0)  borderCells.AddRange(_map.GetCellsAlongLine(xMax, yMin, xMax, yMax));

            // Go through each of the rooms border cells and look for locations to place doors.
            foreach (Cell cell in borderCells)
            {
                if (IsPotentialDoor(cell))
                {
                    // A door must block field-of-view when it is closed.
                    _map.SetCellProperties(cell.X, cell.Y, false, true);
                    _map.Doors.Add(new Door
                    {
                        X = cell.X,
                        Y = cell.Y,
                        IsOpen = false
                    });
                }
            }
        }

        // Checks to see if a cell is a good candidate for placement of a door
        private bool IsPotentialDoor(Cell cell)
        {
            // If the cell is not walkable
            // then it is a wall and not a good place for a door
            if (!cell.IsWalkable)
            {
                return false;
            }

            // Store references to all of the neighboring cells 
            Cell right = _map.GetCell(cell.X + 1, cell.Y);
            Cell left = _map.GetCell(cell.X - 1, cell.Y);
            Cell top = _map.GetCell(cell.X, cell.Y - 1);
            Cell bottom = _map.GetCell(cell.X, cell.Y + 1);

            if (right == null || left == null || top == null || bottom == null) return false;

            // Make sure there is not already a door here
            if (_map.GetDoor(cell.X, cell.Y) != null ||
                _map.GetDoor(right.X, right.Y) != null ||
                _map.GetDoor(left.X, left.Y) != null ||
                _map.GetDoor(top.X, top.Y) != null ||
                _map.GetDoor(bottom.X, bottom.Y) != null)
            {
                return false;
            }

            // This is a good place for a door on the left or right side of the room
            if (right.IsWalkable && left.IsWalkable && !top.IsWalkable && !bottom.IsWalkable)
            {
                return true;
            }

            // This is a good place for a door on the top or bottom of the room
            if (!right.IsWalkable && !left.IsWalkable && top.IsWalkable && bottom.IsWalkable)
            {
                return true;
            }
            return false;
        }
    }
}