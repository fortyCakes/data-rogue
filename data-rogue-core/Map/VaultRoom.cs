using System;
using RogueSharp;

namespace data_rogue_core.Map
{
    class VaultRoom : IRoom
    {
        private DungeonMap _map;

        public VaultRoom(DungeonMap map, int roomXPosition, int roomYPosition)
        {
            _map = map;

            BoundingRectangle = new Rectangle(roomXPosition, roomYPosition, map.Width, map.Height);
        }

        public bool IsVault => true;
        public Point Center => new Point((Left + Right) / 2, (Top+Bottom)/2);
        public int Left => BoundingRectangle.Left;
        public int Right => BoundingRectangle.Right;
        public int Top => BoundingRectangle.Top;
        public int Bottom => BoundingRectangle.Bottom;
        public int X => BoundingRectangle.X;
        public int Y => BoundingRectangle.Y;
        public int Width => BoundingRectangle.Width;
        public int Height => BoundingRectangle.Height;

        public void CreateRoom(DungeonMap map)
        {
            map.Copy(_map, Left, Top);
        }

        public Rectangle BoundingRectangle { get; set; }

        public bool Intersects(IRoom room)
        {
            return BoundingRectangle.Intersects(room.BoundingRectangle);
        }

        public bool Contains(int x , int y )
        {
            return BoundingRectangle.Contains(x, y);
        }
    }
}