using RogueSharp;

namespace data_rogue_core.Map
{
    public interface IRoom
    {
        bool IsVault { get; }
        Point Center { get; }
        int Left { get; }
        int Right { get; }
        int Top { get; }
        int Bottom { get; }
        int X { get; }
        int Y { get; }
        int Width { get; }
        int Height { get; }

        Rectangle BoundingRectangle { get; }

        bool Intersects(IRoom room);

        void CreateRoom(DungeonMap map);
        bool Contains(int x, int y);
    }
}