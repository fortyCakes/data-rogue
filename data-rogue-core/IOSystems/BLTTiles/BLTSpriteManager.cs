using BLTWrapper;
using System.Collections.Generic;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTSpriteManager : ISpriteManager
    {
        private Dictionary<string, ISpriteSheet> _spriteDictionary;

        public BLTSpriteManager()
        {
            _spriteDictionary = new Dictionary<string, ISpriteSheet>();
        }

        public void Add(ISpriteSheet spriteSheet)
        {
            _spriteDictionary.Add(spriteSheet.Name, spriteSheet);
        }

        public ISpriteSheet Get(string name)
        {
            return _spriteDictionary[name];
        }

        public int Tile(string name, TileDirections directions)
        {
            return _spriteDictionary[name].Tile(directions);
        }
    }
}