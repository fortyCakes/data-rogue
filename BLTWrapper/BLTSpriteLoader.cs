using BearLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLTWrapper
{

    public class BLTSpriteLoader
    {
        private int _offset;

        public BLTSpriteLoader()
        {
            _offset = 0xE000;
        }

        public TilesetSpriteSheet LoadTileset_WallType(string name, string imageFile, int spriteWidth, int spriteHeight, int scaling)
        {
            var sheet = new TilesetSpriteSheet(name, _offset);

            LoadSpriteSheetFile(imageFile, spriteWidth, spriteHeight, scaling);

            // We assume loading a 6x3 tile set
            _offset += 6*3;

            return sheet;
        }

        public SingleSpriteSheet LoadSingleSprite(string name, string imageFile, int spriteWidth, int spriteHeight, int scaling)
        {
            var sheet = new SingleSpriteSheet(name, _offset);

            LoadSpriteSheetFile(imageFile, spriteWidth, spriteHeight, scaling);

            _offset += 1;

            return sheet;
        }

        private void LoadSpriteSheetFile(string imageFile, int spriteWidth, int spriteHeight, int scaling)
        {
            string configString = $"0x{_offset.ToString("X")}: {imageFile}, size={spriteWidth}x{spriteHeight}, resize={spriteWidth * scaling}x{spriteHeight * scaling}, resize-filter=nearest;";
            BLT.Set(configString);
        }

        public BoxTilesetSpriteSheet LoadTileset_BoxType(string name, string imageFile, int spriteWidth, int spriteHeight, int scaling)
        {
            var sheet = new BoxTilesetSpriteSheet(name, _offset);

            LoadSpriteSheetFile(imageFile, spriteWidth, spriteHeight, scaling);

            // We assume loading a 4x3 tile set
            _offset += 4*3;

            return sheet;
        }
    }

}
