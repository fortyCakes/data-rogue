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
                    TileDirections directions = BLTTileDirectionHelper.GetDirections(x, width, y, height);

                    var sprite = backgroundSpriteSheet.Tile(directions);

                    BLT.Put(x * BLTTilesIOSystem.TILE_SPACING, y * BLTTilesIOSystem.TILE_SPACING, sprite);
                }
            }
        }
    }
}