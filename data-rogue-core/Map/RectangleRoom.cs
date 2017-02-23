using data_rogue_core.Display;
using RogueSharp;

namespace data_rogue_core.Map
{
    class RectangleRoom : Rectangle, IRoom
    {
        public bool IsVault => false;
        public RectangleRoom(int x, int y, int width, int height) : base(x, y, width, height)
        {
            
        }

        public void CreateRoom(DungeonMap map)
        {
            for (int x = Left + 1; x < Right; x++)
            {
                for (int y = Top + 1; y < Bottom; y++)
                {
                    map.SetCellProperties(x, y, true, true, false, '.', Colors.Floor);
                }
            }
        }

        public Rectangle BoundingRectangle => this;
        public bool Intersects(IRoom room)
        {
            return this.Intersects(room.BoundingRectangle);
        }

    }
}