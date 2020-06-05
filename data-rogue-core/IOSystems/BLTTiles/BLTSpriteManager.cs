using BLTWrapper;
using System.Collections.Generic;
using data_rogue_core.Components;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTSpriteManager : ISpriteManager
    {
        private Dictionary<string, ISpriteSheet> _spriteDictionary;
        private Dictionary<ISpriteSheet, FrameList> _frameCache = new Dictionary<ISpriteSheet, FrameList>();

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

        public int Tile(string name, TileDirections directions, AnimationFrame frame)
        {
            var spriteSheet = _spriteDictionary[name];
            if (_frameCache.ContainsKey(spriteSheet))
            {
                var frameIndex = _frameCache[spriteSheet].IndexOf(frame);

                return spriteSheet.Tile(directions, frameIndex);
            }

            return spriteSheet.Tile(directions);
        }

        public int Tile(string name, AnimationFrame frame)
        {
            return Tile(name, TileDirections.None, frame);
        }

        public int Tile(string name, TileDirections directions)
        {
            return Tile(name, directions, 0);
        }

        public int Tile(string name)
        {
            return Tile(name, TileDirections.None, 0);
        }

        public void AddAnimated(ISpriteSheet spriteSheet, FrameList sheetFrames)
        {
            _frameCache.Add(spriteSheet, sheetFrames);
            Add(spriteSheet);
        }
    }
}