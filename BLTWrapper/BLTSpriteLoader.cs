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

        public TilesetSpriteSheet LoadTileset_WallType(string name, string imageFile, int spriteWidth, int spriteHeight, int scaling, int spacing)
        {
            var sheet = new TilesetSpriteSheet(name, _offset);

            LoadSpriteSheetFile(imageFile, spriteWidth, spriteHeight, scaling, spacing);

            // We assume loading a 6x3 tile set
            _offset += 6*3;

            return sheet;
        }

        public SingleSpriteSheet LoadSingleSprite(string name, string imageFile, int spriteWidth, int spriteHeight, int scaling, int spacing)
        {
            var sheet = new SingleSpriteSheet(name, _offset);

            LoadSpriteSheetFile(imageFile, spriteWidth, spriteHeight, scaling, spacing);

            _offset += 1;

            return sheet;
        }

        private void LoadSpriteSheetFile(string imageFile, int spriteWidth, int spriteHeight, int scaling, int spacing)
        {
            string configString = $"0x{_offset.ToString("X")}: {imageFile}, size={spriteWidth}x{spriteHeight}, resize={spriteWidth * scaling}x{spriteHeight * scaling}, resize-filter=nearest, spacing={spacing}x{spacing};";
            var ok = BLT.Set(configString);

            if (!ok)
                throw new Exception("BLT wasn't OK with sprite loading");
        }

        public BoxTilesetSpriteSheet LoadTileset_BoxType(string name, string imageFile, int spriteWidth, int spriteHeight, int scaling, int spacing)
        {
            var sheet = new BoxTilesetSpriteSheet(name, _offset);

            LoadSpriteSheetFile(imageFile, spriteWidth, spriteHeight, scaling, spacing);

            // We assume loading a 4x3 tile set
            _offset += 4*3;

            return sheet;
        }
    }

}
