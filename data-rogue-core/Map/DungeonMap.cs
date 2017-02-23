using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Display;
using data_rogue_core.Entities;
using data_rogue_core.Interfaces;
using RLNET;
using RogueSharp;

namespace data_rogue_core.Map
{
    public class DungeonMap : RogueSharp.Map
    {
        // ... start of new code
        public List<IRoom> Rooms;
        public List<Door> Doors;
        private readonly List<Monster> _monsters;
        private char[,] _symbols;
        private RLColor[,] _colors;

        private int ViewpointX { get { return Game.Player.X; } }
        private int ViewpointY { get { return Game.Player.Y; } }

        public DungeonMap()
        {
            // Initialize the list of rooms when we create a new DungeonMap
            Rooms = new List<IRoom>();
            Doors = new List<Door>();
            _monsters = new List<Monster>();
        }

        public IEnumerable<IDrawable> GetDrawables()
        {
            IEnumerable<IDrawable> drawables = Doors.OfType<IDrawable>().Union(_monsters);

            return drawables;
        }
        
        public void AddPlayer(Player player)
        {
            Game.Player = player;
            SetIsWalkable(player.X, player.Y, false);
            UpdatePlayerFieldOfView();
            Game.SchedulingSystem.Add(player);
        }

        // This method will be called any time we move the player to update field-of-view
        public void UpdatePlayerFieldOfView()
        {
            Player player = Game.Player;
            bool transparent = GetIsTransparent(player.X, player.Y);
            SetIsTransparent(player.X, player.Y, true);

            // Compute the field-of-view based on the player's location and awareness
            ComputeFov(player.X, player.Y, player.Awareness, true);
            // Mark all cells in field-of-view as having been explored
            foreach (Cell cell in GetAllCells())
            {
                if (IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }

            SetIsTransparent(player.X, player.Y, transparent);
        }

        

        // Returns true when able to place the Actor on the cell or false otherwise
        public bool SetActorPosition(Actor actor, int x, int y)
        {
            if (GetCell(x, y).IsWalkable)
            {
                if (actor is Player)
                {
                    UpdatePlayerFieldOfView();
                }

                SetIsWalkable(actor.X, actor.Y, true);
                
                actor.X = x;
                actor.Y = y;

                SetIsWalkable(actor.X, actor.Y, false);

                OpenDoor(actor, x, y);

                if (actor is Player)
                {
                    UpdatePlayerFieldOfView();
                }
                return true;
            }
            return false;
        }

        private void SetIsTransparent(int x, int y, bool isTransparent)
        {
            Cell cell = GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, isTransparent, cell.IsWalkable, cell.IsExplored);
        }

        private bool GetIsTransparent(int x, int y)
        {
            Cell cell = GetCell(x, y);
            return cell.IsTransparent;
        }

        // A helper method for setting the IsWalkable property on a Cell
        public void SetIsWalkable(int x, int y, bool isWalkable)
        {
            Cell cell = GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }

        public void AddMonster(Monster monster)
        {
            _monsters.Add(monster);
            // After adding the monster to the Map make sure to make the cell not walkable
            SetIsWalkable(monster.X, monster.Y, false);
            Game.SchedulingSystem.Add(monster);
        }

        // Look for a random location in the room that is walkable.
        public Point GetRandomWalkableLocationInRoom(IRoom room)
        {
            if (DoesRoomHaveWalkableSpace(room))
            {
                for (int i = 0; i < 100; i++)
                {
                    int x = Game.Random.Next(1, room.Width - 2) + room.X;
                    int y = Game.Random.Next(1, room.Height - 2) + room.Y;
                    if (IsWalkable(x, y))
                    {
                        return new Point(x, y);
                    }
                }
            }

            // If we didn't find a walkable location in the room return null
            return null;
        }

        // Iterate through each Cell in the room and return true if any are walkable
        public bool DoesRoomHaveWalkableSpace(IRoom room)
        {
            for (int x = 1; x <= room.Width - 2; x++)
            {
                for (int y = 1; y <= room.Height - 2; y++)
                {
                    if (IsWalkable(x + room.X, y + room.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void RemoveMonster(Monster monster)
        {
            _monsters.Remove(monster);
            // After removing the monster from the Map, make sure the cell is walkable again
            SetIsWalkable(monster.X, monster.Y, true);
            Game.SchedulingSystem.Remove(monster);
        }

        public Monster GetMonsterAt(int x, int y)
        {
            return _monsters.FirstOrDefault(m => m.X == x && m.Y == y);
        }

        // Return the door at the x,y position or null if one is not found.
        public Door GetDoor(int x, int y)
        {
            return Doors.SingleOrDefault(d => d.X == x && d.Y == y);
        }

        // The actor opens the door located at the x,y position
        private void OpenDoor(Actor actor, int x, int y)
        {
            Door door = GetDoor(x, y);
            if (door != null && !door.IsOpen)
            {
                door.IsOpen = true;
                var cell = GetCell(x, y);
                // Once the door is opened it should be marked as transparent and no longer block field-of-view
                SetCellProperties(x, y, true, cell.IsWalkable, cell.IsExplored);

                Game.MessageLog.Add($"{actor.Name} opened a door");
            }
        }

        public void SetCellProperties(int x, int y, bool isTransparent, bool isWalkable, bool isExplored, char symbol,
            RLColor color)
        {
            SetCellProperties(x,y,isTransparent, isWalkable, isExplored);
            _symbols[x, y] = symbol;
            _colors[x, y] = color;
        }

        public void SetCellProperties(int x, int y, bool isTransparent, bool isWalkable,  char symbol,
            RLColor color)
        {
            SetCellProperties(x, y, isTransparent, isWalkable);
            _symbols[x, y] = symbol;
            _colors[x, y] = color;
        }

        public void Clear(bool isTransparent, bool isWalkable, char symbol, RLColor color)
        {
            foreach (DungeonCell cell in GetAllCells())
            {
                SetCellProperties(cell.X, cell.Y, isTransparent, isWalkable, symbol, color);
            }
        }

        public new DungeonMap Clone()
        {
            var map = new DungeonMap();
            map.Initialize(Width, Height);
            foreach (DungeonCell cell in GetAllCells())
            {
                map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, cell.IsExplored, cell.Symbol, cell.Color);
            }
            return map;
        }

        internal void Initialize(int width, int height, char symbol, RLColor color)
        {
            Initialize(width, height);
            _symbols =new char[width,height];
            _colors = new RLColor[width, height];
            Clear(false, false, symbol, color);
        }

        internal new void Initialize(int width, int height)
        {
            char symbol = '#';
            RLColor color = Colors.Wall;

            base.Initialize(width, height);
            _symbols = new char[width, height];
            _colors = new RLColor[width, height];
            Clear(false, false, symbol, color);
        }

        public void Copy(DungeonMap sourceMap)
        {
            Copy(sourceMap, 0, 0);
        }

        public void Copy(DungeonMap sourceMap, int left, int top)
        {
            if (sourceMap == null)
            {
                throw new ArgumentNullException("sourceMap", "Source map cannot be null");
            }

            if (sourceMap.Width + left > Width)
            {
                throw new ArgumentException("Source map 'width' + 'left' cannot be larger than the destination map width");
            }
            if (sourceMap.Height + top > Height)
            {
                throw new ArgumentException("Source map 'height' + 'top' cannot be larger than the destination map height");
            }
            foreach (DungeonCell cell in sourceMap.GetAllCells())
            {
                SetCellProperties(cell.X + left, cell.Y + top, cell.IsTransparent, cell.IsWalkable, cell.IsExplored, cell.Symbol, cell.Color);
            }
        }

        public new IEnumerable<DungeonCell> GetAllCells()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return GetCell(x, y);
                }
            }
        }

        public new DungeonCell GetCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return null;

            var cell = (this as RogueSharp.Map).GetCell(x, y);
            return new DungeonCell(x,y, _symbols[x,y], _colors[x,y], cell.IsTransparent, cell.IsWalkable, cell.IsInFov, cell.IsExplored);
        }

        public void RevealAll()
        {
            foreach (DungeonCell cell in GetAllCells())
            {
                SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
            }
            UpdatePlayerFieldOfView();
        }
    }
}
