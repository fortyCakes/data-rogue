using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public class ShopActivity : BaseActivity
    {
        private IEntity shop;
        private IEntity SelectedItem;
        private ISystemContainer _systemContainer;

        public ShopActivity(ISystemContainer systemContainer, IEntity shop)
        {
            _systemContainer = systemContainer;
            this.shop = shop;
            SelectedItem = GetShopItems(systemContainer).First();

            InitialiseControls();
        }

        private void InitialiseControls()
        {
            titleControl = new LargeTextControl { Parameters = this.shop.DescriptionName };
            exitButton = new ButtonControl { Position = new Rectangle(), Text = "Exit" };
            exitButton.OnClick += ExitButton_OnClick;
        }

        private void ExitButton_OnClick(object sender, PositionEventHandlerArgs args)
        {
            CloseActivity();
        }

        private void CloseActivity()
        {
            _systemContainer.ActivitySystem.RemoveActivity(this);
        }

        public override ActivityType Type => ActivityType.Shop;

        public override bool RendersEntireSpace => true;

        public override bool AcceptsInput => true;

        public IDataRogueControl backgroundControl = new BackgroundControl();
        public IDataRogueControl titleControl;
        public IDataRogueControl exitButton;

        private int itemsPerRow = 1;

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            var controls = new List<IDataRogueControl>();

            backgroundControl.Position = new Rectangle(0, 0, width, height);
            controls.Add(backgroundControl);

            var left = renderer.ActivityPadding.Left;
            var top = renderer.ActivityPadding.Top;

            titleControl.Position = new Rectangle(new Point(left, top), titleControl.Position.Size);
            SetSize(titleControl, systemContainer, rendererHandle, controlRenderers, playerFov);
            controls.Add(titleControl);

            top += titleControl.Position.Height + renderer.ActivityPadding.Top;

            ShopItemControl shopItemControl = null;

            foreach(var item in GetShopItems(systemContainer))
            {
                shopItemControl = new ShopItemControl { Position = new Rectangle(left, top, 0, 0), Item = item, Selected = (item == SelectedItem) };
                SetSize(shopItemControl, systemContainer, rendererHandle, controlRenderers, playerFov);

                controls.Add(shopItemControl);

                left += shopItemControl.Position.Width + renderer.ActivityPadding.Left;

                if (left > width - shopItemControl.Position.Width - renderer.ActivityPadding.Left - renderer.ActivityPadding.Right)
                {
                    left = renderer.ActivityPadding.Left;
                    top += shopItemControl.Position.Height + renderer.ActivityPadding.Top;
                }
            }

            this.itemsPerRow = (width - renderer.ActivityPadding.Left - renderer.ActivityPadding.Right) / (shopItemControl.Position.Width);

            var button = exitButton;
            SetSize(button, systemContainer, rendererHandle, controlRenderers, playerFov);
            button.IsFocused = SelectedItem == null;

            button.Position = new Rectangle(renderer.ActivityPadding.Left, height - button.Position.Height - renderer.ActivityPadding.Bottom, button.Position.Width, button.Position.Height);
            controls.Add(button);

            return controls;
        }

        private IEnumerable<IEntity> GetShopItems(ISystemContainer systemContainer)
        {
            return systemContainer.ItemSystem.GetInventory(shop);
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            if (action != null)
            {
                switch(action.Action)
                {
                    case ActionType.Move:
                        var vector = Vector.Parse(action.Parameters);

                        if (vector == Vector.Right) { MoveNext(); }
                        if (vector == Vector.Left) { MovePrevious(); }
                        if (vector == Vector.Up) { MoveUp(); }
                        if (vector == Vector.Down) { MoveDown(); }
                        break;
                    case ActionType.Select:
                        Select();
                        break;
                }
            }
        }

        private void MoveDown()
        {
            var shopItems = GetShopItems(_systemContainer).ToList();

            if (SelectedItem == null)
            {
                SelectedItem = shopItems.First();
            }
            else
            {
                var newIndex = shopItems.IndexOf(SelectedItem) + itemsPerRow;
                if (newIndex >= shopItems.Count())
                {
                    SelectedItem = null;
                }
                else
                {
                    SelectedItem = shopItems[newIndex];
                }
            }
        }

        private void MoveUp()
        {
            var shopItems = GetShopItems(_systemContainer).ToList();

            if (SelectedItem == null)
            {
                SelectedItem = shopItems.Last();
            }
            else
            {
                var newIndex = shopItems.IndexOf(SelectedItem) - itemsPerRow;
                if (newIndex < 0)
                {
                    SelectedItem = null;
                }
                else
                {
                    SelectedItem = shopItems[newIndex];
                }
            }
        }

        private void MoveNext()
        {
            var shopItems = GetShopItems(_systemContainer).ToList();

            if (SelectedItem == null)
            {
                SelectedItem = shopItems.First();
            }
            else
            {
                var newIndex = shopItems.IndexOf(SelectedItem) + 1;
                if (newIndex >= shopItems.Count())
                {
                    SelectedItem = null;
                }
                else
                {
                    SelectedItem = shopItems[newIndex];
                }
            }
        }

        private void MovePrevious()
        {
            var shopItems = GetShopItems(_systemContainer).ToList();

            if (SelectedItem == null)
            {
                SelectedItem = shopItems.Last();
            }
            else
            {
                var newIndex = shopItems.IndexOf(SelectedItem) - 1;
                if (newIndex < 0)
                {
                    SelectedItem = null;
                }
                else
                {
                    SelectedItem = shopItems[newIndex];
                }
                
            }
        }

        private void Select()
        {
            if (SelectedItem == null)
            {
                CloseActivity();
            }
            else
            {
                TryPurchaseSelectedItem();
                SelectedItem = GetShopItems(_systemContainer).FirstOrDefault();
            }
        }

        private void TryPurchaseSelectedItem()
        {
            PurchaseItemEventData purchaseEvent = new PurchaseItemEventData { Shop = shop, PurchaseItem = SelectedItem };

            var ok = _systemContainer.EventSystem.Try(EventSystem.EventType.PurchaseItem, _systemContainer.PlayerSystem.Player, purchaseEvent);

            if (ok)
            {
                _systemContainer.ActivitySystem.Push(new ToastActivity(_systemContainer.ActivitySystem, $"Purchased item:{SelectedItem.DescriptionName}", Color.White));
            }
        }

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
        }
    }
}
