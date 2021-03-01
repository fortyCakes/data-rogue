using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
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
    public class BLTMultipleChoiceFormDataRenderer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(MultipleChoiceFormData);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as MultipleChoiceFormData;
            var x = control.Position.X;
            var y = control.Position.Y;

            BLT.Font("text");
            var text = display.Value.ToString();
            var textSize = BLT.Measure(text);

            BLT.Print(x + 10, y, text);

            if (control.IsFocused)
            {
                BLTLayers.Set(BLTLayers.UIElements, control.ActivityIndex);
                BLT.Font("");
                BLT.Put(x, y - 2, spriteManager.Tile("ui_arrow", TileDirections.Left));
                BLT.Put(x + textSize.Width + 12, y - 2, spriteManager.Tile("ui_arrow", TileDirections.Right));
            }

        }

        protected override Size Measure(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov, Rectangle boundingBox, Padding padding, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            var display = control as MultipleChoiceFormData;
            
            var text = display.Value.ToString();
            BLT.Font("text");

            var textSize = BLT.Measure(text);

            return new Size(textSize.Width + 6, textSize.Height);
        }
    }
}