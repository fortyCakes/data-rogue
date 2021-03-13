using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
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
        private ISystemContainer _systemContainer;
        private FlowContainerControl ShopItemsFlow;

        public ShopActivity(Rectangle position, Padding padding, ISystemContainer systemContainer, IEntity shop) : base(position, padding)
        {
            _systemContainer = systemContainer;
            this.shop = shop;
            SelectedItem = GetShopItems(systemContainer).First();

            OnLayout += ShopActivity_OnLayout;
        }

        public override void InitialiseControls()
        {
            backgroundControl = new BackgroundControl { Position = Position, Padding = new Padding(3) };

            var topFlow = new FlowContainerControl { FlowDirection = FlowDirection.BottomUp, VerticalAlignment = VerticalAlignment.Bottom };
            var downFlow = new FlowContainerControl { FlowDirection = FlowDirection.TopDown, VerticalAlignment = VerticalAlignment.Top };
            ShopItemsFlow = new FlowContainerControl { FlowDirection = FlowDirection.LeftToRight };

            titleControl = new LargeTextControl { Parameters = this.shop.DescriptionName };
            exitButton = new ButtonControl { Position = new Rectangle(), Text = "Exit" };
            exitButton.OnClick += ExitButton_OnClick;

            Controls.Add(backgroundControl);
            backgroundControl.Controls.Add(topFlow);

            topFlow.Controls.Add(exitButton);
            topFlow.Controls.Add(downFlow);

            downFlow.Controls.Add(titleControl);
            downFlow.Controls.Add(new Spacer());
            downFlow.Controls.Add(ShopItemsFlow);

            RefreshShopItemControls();
        }

        private void RefreshShopItemControls()
        {
            ShopItemsFlow.Controls.Clear();
            foreach (var item in GetShopItems(_systemContainer))
            {
                ShopItemsFlow.Controls.Add(new ShopItemControl { Item = item, Selected = item == _selectedItem });
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
        private IEntity _selectedItem;


        private void ShopActivity_OnLayout(object sender, EventArgs e)
        {
            var flowWidth = ShopItemsFlow.Position.Width;
            var itemWidth = ShopItemsFlow.Controls.First().Position.Width;

            if (flowWidth == 0 || itemWidth == 0)
            {
                itemsPerRow = 1;
            }
            else
            {
                itemsPerRow = Math.Max(1, flowWidth / itemWidth);
            }

            exitButton.IsFocused = SelectedItem == null;
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

        private IEntity SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                if (ShopItemsFlow != null)
                    foreach (ShopItemControl itemControl in ShopItemsFlow.Controls)
                    {
                        itemControl.Selected = itemControl.Item == _selectedItem;
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

                RefreshShopItemControls();
            }
        }

        private void TryPurchaseSelectedItem()
        {
            PurchaseItemEventData purchaseEvent = new PurchaseItemEventData { Shop = shop, PurchaseItem = SelectedItem };

            var ok = _systemContainer.EventSystem.Try(EventSystem.EventType.PurchaseItem, _systemContainer.PlayerSystem.Player, purchaseEvent);

            if (ok)
            {
                _systemContainer.ActivitySystem.Push(new ToastActivity(Position, Padding, _systemContainer.ActivitySystem, $"Purchased item: {SelectedItem.DescriptionName}", Color.White));
            }
        }

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
        }
    }
}
