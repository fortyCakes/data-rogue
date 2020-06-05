using BLTWrapper;
using data_rogue_core.Components;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public interface ISpriteManager
    {
        void Add(ISpriteSheet spriteSheet);

        ISpriteSheet Get(string name);

        int Tile(string name, TileDirections directions, AnimationFrame frame);
        int Tile(string name, AnimationFrame frame);
        int Tile(string name, TileDirections directions);
        int Tile(string name);
    }
}