using System.Data;
using data_rogue_core.Entities;
using RLNET;
using RogueSharp;

namespace data_rogue_core.Interfaces
{
    public interface IDrawable
    {
        RLColor Color { get; set; }
        char Symbol { get; set; }
        int X { get; set; }
        int Y { get; set; }

        void Draw(IRLConsoleWriter console, IMap map, int xOffset, int yOffset);
    }
}