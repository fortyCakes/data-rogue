using BLTWrapper;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public interface ISpriteManager
    {
        void Add(ISpriteSheet spriteSheet);

        ISpriteSheet Get(string name);

        int Tile(string name, TileDirections directions);
    }
}