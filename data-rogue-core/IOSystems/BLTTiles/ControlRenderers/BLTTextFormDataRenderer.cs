using System;
using System.Collections.Generic;
using System.Drawing;
using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Forms;
using data_rogue_core.Forms.StaticForms;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTTextFormDataRenderer : BLTControlRenderer
    {
        const int TILE_SIZE = BLTTilesIOSystem.TILE_SPACING/2;

        public override Type DisplayType => typeof(TextFormData);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var x = control.Position.X;
            var y = control.Position.Y;
            var text = (control as TextFormData).Value.ToString();

            var spriteSheet = spriteManager.Get("textbox_grey_small");
            BLT.Layer(BLTLayers.Text);
            BLT.Font("text");

            BLT.Print(x + 4, y, text);

            var textSize = BLT.Measure(new String('@', 30));

            var textBoxSize = textSize.Width + 4;

            if (textBoxSize % TILE_SIZE != 0)
            {
                textBoxSize = (textBoxSize / TILE_SIZE + 1) * TILE_SIZE;
            }

            var numberOfTiles = textBoxSize / TILE_SIZE;

            BLT.Layer(BLTLayers.UIElements);
            BLT.Font("");

            for (int i = 0; i < numberOfTiles; i++)
            {
                TileDirections direction = TileDirections.None;
                if (i != 0)
                {
                    direction |= TileDirections.Left;
                }

                if (i != numberOfTiles - 1)
                {
                    direction |= TileDirections.Right;
                }

                BLT.PutExt(x + TILE_SIZE * i, y - 2, 0, -2, spriteSheet.Tile(direction));
            }
        }

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            BLT.Font("text");

            var textSize = BLT.Measure(new String('@', 30));

            var textBoxSize = textSize.Width + 4;

            if (textBoxSize % TILE_SIZE != 0)
            {
                textBoxSize = (textBoxSize / TILE_SIZE + 1) * TILE_SIZE;
            }

            var numberOfTiles = textBoxSize / TILE_SIZE;

            return new Size(numberOfTiles * TILE_SIZE, TILE_SIZE);
        }
    }
}