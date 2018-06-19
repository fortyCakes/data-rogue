using RLNET;

namespace data_rogue_core.Map
{
    public class DungeonCell : RogueSharp.Cell
    {
        public char Symbol { get; set; }
        public RLColor Color { get; set; }

        public DungeonCell(int x, int y, char symbol, RLColor color, bool isTransparent, bool isWalkable, bool isInFov) : base(x, y, isTransparent, isWalkable, isInFov)
        {
            Symbol = symbol;
            Color = color;
        }

        public DungeonCell(int x, int y, char symbol, RLColor color, bool isTransparent, bool isWalkable, bool isInFov, bool isExplored) : base(x, y, isTransparent, isWalkable, isInFov, isExplored)
        {
            Symbol = symbol;
            Color = color;
        }
    }
}
