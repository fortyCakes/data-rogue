using BearLib;
using BLTWrapper;
using data_rogue_core.Menus;
using data_rogue_core.Renderers;
using System;
using System.Drawing;
using System.Text;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTTilesMenuRenderer : IMenuRenderer
    {
        private readonly ISpriteManager _spriteManager;
        private int _height;
        private int _width;

        public BLTTilesMenuRenderer(ISpriteManager spriteManager)
        {
            _spriteManager = spriteManager;
        }


        public void Render(Menu menu)
        {
            BLT.Clear();

            _height = BLT.State(BLT.TK_HEIGHT);
            _width = BLT.State(BLT.TK_WIDTH);

            BLTTilesBackgroundRenderer_Old.RenderBackground(_width, _height, _spriteManager.Get("textbox_blue"));

            RenderTitleBar(menu);

            RenderMenuActions(menu);

            RenderMenuText(menu);

            //throw new NotImplementedException();
        }

        private void RenderMenuActions(Menu menu)
        {
            if (menu.AvailableActions.Count > 1)
            {
                BLT.Layer((int)BLTLayers.Text);
                BLT.Font("text");

                var width = BLT.State(BLT.TK_WIDTH);

                StringBuilder text = new StringBuilder("[[");

                foreach (var action in menu.AvailableActions)
                {
                    var selected = action == menu.SelectedAction;

                    if (selected)
                    {
                        text.Append($"[color=#42a7f4]{action}[/color]|");
                    }
                    else
                    {
                        text.Append(action + "|");
                    }

                    text.Remove(text.Length - 1, 1);
                    text.Append("]]");

                    var size = BLT.Measure(text.ToString());

                    BLT.Print(width - size.Width - 2, 2, text.ToString());
                }
            }
        }

        private void RenderTitleBar(Menu menu)
        {
            BLT.Layer((int)BLTLayers.Text);
            BLT.Font("textLarge");

            var x = 2;
            var y = 2;

            if (menu.Centred)
            {
                var size = BLT.Measure(menu.MenuName);

                x = _width / 2 - size.Width / 2;
                y = _height / 2 - size.Height * 3 / 2;
            }


            BLT.Print(x, y, menu.MenuName);
        }

        private void RenderMenuText(Menu menu)
        {
            var font_height = 3;

            BLT.Layer((int)BLTLayers.Text);
            BLT.Font("text");

            var height = _height - 17;
            if (menu.Centred)
            {
                height = _height / 2 - 10;
                font_height *= 2;
            }

            int selectedIndex = menu.SelectedIndex;
            int itemCount = menu.MenuItems.Count;
            int itemsPerPage = height / font_height;
            int pageCount = (itemCount - 1) / itemsPerPage + 1;
            int page = selectedIndex / itemsPerPage;
            

            int menuOffset = menu.Centred ? _height / 2 : 11;
            for (int i = 0; i < itemsPerPage; i++)
            {
                var displayIndex = page * itemsPerPage + i;
                if (displayIndex < itemCount)
                {
                    MenuItem item = menu.MenuItems[displayIndex];
                    int y = menuOffset + i * font_height;

                    int x = 8;
                    var size = BLT.Measure(item.Text);

                    if (menu.Centred)
                    {
                        x = _width / 2 - size.Width / 2;
                    }

                    BLT.Print(x, y, item.Text);

                    if (displayIndex == selectedIndex)
                    {
                        RenderMenuSelector(x, y, size);
                    }
                }
            }

            if (pageCount > 1)
            {
                BLT.Print(2, height, $"(page {page} of {pageCount})");
            }

        }

        private void RenderMenuSelector(int baseX, int y, Size size)
        {
            
        }

        
    }
}