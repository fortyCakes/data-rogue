using BearLib;
using BLTWrapper;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public static class BLTTilesBackgroundRenderer
    {
        public static void RenderBackground(int _width, int _height, ISpriteSheet backgroundSpriteSheet)
        {
            BLT.Layer((int)BLTLayers.Background);
            BLT.Font("");

            var width = _width / BLTTilesIOSystem.TILE_SPACING;
            var height = _height / BLTTilesIOSystem.TILE_SPACING;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var directions = TileDirections.None;
                    if (x != 0) directions |= TileDirections.Left;
                    if (x != width - 1) directions |= TileDirections.Right;
                    if (y != 0) directions |= TileDirections.Up;
                    if (y != height - 1) directions |= TileDirections.Down;

                    var sprite = backgroundSpriteSheet.Tile(directions);

                    BLT.Put(x * BLTTilesIOSystem.TILE_SPACING, y * BLTTilesIOSystem.TILE_SPACING, sprite);
                }
            }
        }
    }
}