using System;
using System.Linq;
using BearLib;
using BLTWrapper;
using data_rogue_core.Forms;
using data_rogue_core.Forms.StaticForms;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Utils;

namespace data_rogue_core.IOSystems.BLTTiles
{

    public class BLTTilesFormRenderer : IFormRenderer
    {
        private readonly ISpriteManager _spriteManager;
        private int _height;
        private int _width;

        public BLTTilesFormRenderer(ISpriteManager spriteManager)
        {
            _spriteManager = spriteManager;
        }

        public void Render(Form form)
        {
            BLT.Clear();

            _height = BLT.State(BLT.TK_HEIGHT);
            _width = BLT.State(BLT.TK_WIDTH);

            BLTTilesBackgroundRenderer_Old.RenderBackground(_width, _height, _spriteManager.Get("textbox_blue"));

            RenderTitleBar(form);

            RenderFormButtons(form);

            RenderFormControls(form);

            // throw new NotImplementedException();
        }
        private void RenderFormControls(Form form)
        {
            int yCoordinate = 12;

            foreach (var field in form.Fields)
            {
                RenderSingleControl(ref yCoordinate, field.Key, field.Value, form.FormSelection);
            }
        }

        private void RenderSingleControl(ref int yCoordinate, string fieldName, FormData fieldValue, FormSelection selection)
        {
            var selected = selection.SelectedItem == fieldName;
            if (selected)
            {
                BLT.Layer(BLTLayers.UIElements);
                BLT.Font("");
                BLT.Put(4, yCoordinate, _spriteManager.Tile("selector_left", TileDirections.Left));
            }

            BLT.Font("text");
            BLT.Layer(BLTLayers.Text);
            var nameText = fieldName + ":";
            var nameSize = BLT.Measure(nameText);

            BLT.Print(8, yCoordinate, fieldName + ":");

            switch (fieldValue.FormDataType)
            {
                case FormDataType.Text:

                    var size = BLTTextBox.Render(12 + nameSize.Width, yCoordinate, fieldValue.Value.ToString(), _spriteManager);

                    yCoordinate += size.Height + 2;

                    break;
                case FormDataType.MultipleChoice:

                    var text = fieldValue.Value.ToString();
                    var textSize = BLT.Measure(text);


                    BLT.Print(23 + nameSize.Width, yCoordinate, text);

                    if (selected)
                    {
                        BLT.Layer(BLTLayers.UIElements);
                        BLT.Font("");
                        BLT.Put(12 + nameSize.Width, yCoordinate - 2, _spriteManager.Tile("ui_arrow", TileDirections.Left));
                        BLT.Put(26 + nameSize.Width + textSize.Width, yCoordinate - 2, _spriteManager.Tile("ui_arrow", TileDirections.Right));
                    }

                    yCoordinate += 5;

                    break;

                case FormDataType.StatArray:

                    var statsFormData = fieldValue as StatsFormData;
                    var stats = statsFormData.Stats;

                    var statTotal = $"[[{statsFormData.CurrentTotalStat.ToString().PadLeft(4)}/{statsFormData.MaxTotalStat.ToString().PadRight(4)}]]";

                    BLT.Print(16 + nameSize.Width, yCoordinate, statTotal);

                    var longestStat = stats.Max(s => BLT.Measure(s.statName).Width);
                    yCoordinate += 4;

                    foreach (var stat in stats)
                    {
                        var statSelected = selection.SubItem == stat.statName;

                        BLT.Print(12, yCoordinate, (stat.statName + ": ").PadRight(longestStat + 2));
                        BLT.Print(12 + longestStat + 4, yCoordinate, (statSelected ? "[color=red]" : "") + "-");
                        BLT.Print(12 + longestStat + 8, yCoordinate, stat.statValue.ToString().PadBoth(4));
                        BLT.Print(12 + longestStat + 16, yCoordinate, (statSelected? "[color=green]" : "")+"+");

                        if (statSelected)
                        {
                            BLT.Layer(BLTLayers.UIElements);
                            BLT.Font("");
                            BLT.Put(6, yCoordinate, _spriteManager.Tile("selector_left", TileDirections.Left));
                            BLT.Layer(BLTLayers.Text);
                            BLT.Font("text");
                        }

                        yCoordinate += 4;
                    }

                    yCoordinate += 2;

                    break;
            }
        }

        private void RenderFormButtons(Form form)
        {
            var xCoordinate = 6;

            foreach (FormButton flag in (FormButton[])Enum.GetValues(typeof(FormButton)))
            {
                if (flag == FormButton.None) continue;

                if (form.Buttons.HasFlag(flag))
                {
                    var buttonSize = BLTButton.RenderButton(xCoordinate, _height - 12, flag.ToString(), form.FormSelection.SelectedItem == flag.ToString(), _spriteManager);

                    xCoordinate += buttonSize.Width + 4;
                }
            }
        }

        private void RenderTitleBar(Form form)
        {
            BLT.Layer((int)BLTLayers.Text);
            BLT.Font("textLarge");

            var x = 2;
            var y = 2;

            BLT.Print(x, y, form.Title);
        }
    }
}