using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BearLib;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTShopItemControlDisplayer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(ShopItemControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var x = control.Position.X;
            var y = control.Position.Y;
            var display = control as ShopItemControl;

            var entity = display.Item;

            RenderBackgroundBox(x + BLTTilesIOSystem.TILE_SPACING * 2, y, control.ActivityIndex, new Size(BLTTilesIOSystem.TILE_SPACING * 2, BLTTilesIOSystem.TILE_SPACING * 2), spriteManager, display.Selected ? "textbox_blue_selected" : "textbox_blue");

            RenderEntityDetails(x, y, display, entity, systemContainer, spriteManager);

        }

        protected override Size LayoutInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(6 * BLTTilesIOSystem.TILE_SPACING, 4 * BLTTilesIOSystem.TILE_SPACING);
        }

        protected static void RenderEntityDetails(int x, int y, ShopItemControl display, IEntity shopItem, ISystemContainer systemContainer, ISpriteManager spriteManager)
        {
            BLT.Font("");
            SpriteAppearance appearance = shopItem.Has<SpriteAppearance>() ? shopItem.Get<SpriteAppearance>() : new SpriteAppearance { Bottom = "unknown" };
            AnimationFrame frame = shopItem.Has<Animated>() ? systemContainer.AnimationSystem.GetFrame(shopItem) : AnimationFrame.Idle0;
            Price price = shopItem.Get<Price>();

            var playerWealth = systemContainer.ItemSystem.CheckWealth(systemContainer.PlayerSystem.Player, price.Currency);

            var canAfford = playerWealth >= price.Amount;

            BLTLayers.Set(BLTLayers.UIElementPieces, display.ActivityIndex);
            string appearanceBottom = appearance.Bottom;
            RenderSpriteIfSpecified(x + 4 + BLTTilesIOSystem.TILE_SPACING * 2, y + 4, spriteManager, appearanceBottom, frame);

            BLTLayers.Set(BLTLayers.UIElementPieces + 1, display.ActivityIndex);
            string appearanceTop = appearance.Top;
            RenderSpriteIfSpecified(x + 4 + BLTTilesIOSystem.TILE_SPACING * 2, y + 4, spriteManager, appearanceTop, frame);

            BLTLayers.Set(BLTLayers.Text, display.ActivityIndex);
            BLT.Font("text");
            BLT.Print(new Rectangle(x + 2, y + BLTTilesIOSystem.TILE_SPACING * 2 + 2, 6*BLTTilesIOSystem.TILE_SPACING, 10), ContentAlignment.TopCenter, shopItem.GetBLTName());
            BLT.Color(canAfford ? Color.Gold : Color.Gray);
            BLT.Print(new Rectangle(x + 2, y + BLTTilesIOSystem.TILE_SPACING * 2 + 12, 6 * BLTTilesIOSystem.TILE_SPACING, 10), ContentAlignment.TopCenter, $"{price.Amount} {price.Currency}");
            BLT.Color(Color.White);

        }
    }
}