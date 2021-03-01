using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        public ShopActivity(Rectangle position, Padding padding, ISystemContainer systemContainer, IEntity shop) : base(position, padding)
        {
            _systemContainer = systemContainer;
            this.shop = shop;
            SelectedItem = GetShopItems(systemContainer).First();
        }

        public override void InitialiseControls()
        {
            backgroundControl = new BackgroundControl { Position = Position };

            var topFlow = new FlowContainerControl { FlowDirection = FlowDirection.BottomUp };
            var downFlow = new FlowContainerControl { FlowDirection = FlowDirection.TopDown };
            var sideFlow = new FlowContainerControl { FlowDirection = FlowDirection.LeftToRight };

            titleControl = new LargeTextControl { Parameters = this.shop.DescriptionName };
            exitButton = new ButtonControl { Position = new Rectangle(), Text = "Exit" };
            exitButton.OnClick += ExitButton_OnClick;

            Controls.Add(backgroundControl);
            backgroundControl.Controls.Add(topFlow);

            topFlow.Controls.Add(exitButton);
            topFlow.Controls.Add(downFlow);

            downFlow.Controls.Add(titleControl);
            downFlow.Controls.Add(sideFlow);

            foreach (var item in GetShopItems(_systemContainer))
            {
                sideFlow.Controls.Add(new ShopItemControl { Item = item });
            }
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

        public BackgroundControl backgroundControl = new BackgroundControl();
        public LargeTextControl titleControl;
        public ButtonControl exitButton;

        private int itemsPerRow = 1;

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
                _systemContainer.ActivitySystem.Push(new ToastActivity(Position, Padding, _systemContainer.ActivitySystem, $"Purchased item:{SelectedItem.DescriptionName}", Color.White));
            }
        }

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
        }
    }
}
