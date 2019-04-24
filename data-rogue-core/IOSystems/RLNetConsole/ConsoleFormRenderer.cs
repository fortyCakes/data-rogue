using System;
using System.Linq;
using data_rogue_core.Forms;
using data_rogue_core.Forms.StaticForms;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public class ConsoleFormRenderer : BaseConsoleRenderer, IFormRenderer
    {
        private readonly int WIDTH;
        private readonly int HEIGHT;

        public ConsoleFormRenderer(RLConsole console) : base(console)
        {
            WIDTH = console.Width;
            HEIGHT = console.Height;
        }
        
        public void Render(Form form)
        {
            Console.Clear();

            RenderTitleBar(form);

            RenderButtons(form);

            RenderFormControls(form);

            RenderLines();
        }

        private void RenderLines()
        {
            for (int i = 0; i <= WIDTH; i++)
            {
                Console.Set(i, 3, RLColor.White, RLColor.Black, 196);
                Console.Set(i, HEIGHT - 4, RLColor.White, RLColor.Black, 196);
            }
        }

        private void RenderFormControls(Form form)
        {
            // For now I will recklessly assume all of the controls fit on the form

            int yCoordinate = 5;

            foreach(var field in form.Fields)
            {
                RenderSingleControl(ref yCoordinate, field.Key, field.Value, form.FormSelection);
            }
        }

        private void RenderSingleControl(ref int yCoordinate, string fieldName, FormData formData, FormSelection selection)
        {
            var selected = selection.SelectedItem == fieldName;
            var foreColor = selected ? RLColor.Cyan : RLColor.White;

            switch (formData.FormDataType)
            {
                case FormDataType.Text:

                    Console.Print(1, yCoordinate, fieldName + ": ", foreColor);
                    Console.Print(1 + fieldName.Length + 3, yCoordinate, ((string)formData.Value).PadRight(30, '_'), RLColor.White);

                    yCoordinate += 2;
                    break;
                case FormDataType.MultipleChoice:
                    
                    Console.Print(1, yCoordinate, fieldName + ": ", foreColor);
                    Console.Print(fieldName.Length + 5, yCoordinate, ((string)formData.Value).PadRight(28, ' '), RLColor.White);
                    Console.Print(fieldName.Length + 4, yCoordinate, "[", foreColor);
                    Console.Set(fieldName.Length + 3, yCoordinate, foreColor, null, selected ? 27 : 0);
                    Console.Set(fieldName.Length + 32, yCoordinate, foreColor, null, selected ? 26 : 0);
                    Console.Print(fieldName.Length + 31, yCoordinate, "]", foreColor);

                    yCoordinate += 2;
                    break;
                case FormDataType.StatArray:
                    var statsFormData = formData as StatsFormData;
                    var stats = statsFormData.Stats;

                    Console.Print(1, yCoordinate, fieldName + ": ", foreColor);
                    Console.Print(fieldName.Length + 7, yCoordinate, "[     /     ]", foreColor);
                    Console.Print(fieldName.Length + 9, yCoordinate, statsFormData.CurrentTotalStat.ToString().PadLeft(4), foreColor);
                    Console.Print(fieldName.Length + 14, yCoordinate, statsFormData.MaxTotalStat.ToString().PadRight(4), foreColor);
                    yCoordinate += 1;
                    var longestStat = stats.Max(s => s.statName.Length);

                    foreach(var stat in stats)
                    {
                        var statSelected = selection.SubItem == stat.statName;
                        var statForeColor = statSelected ? RLColor.Cyan : RLColor.White;

                        Console.Print(2, yCoordinate, (stat.statName + ": ").PadRight(longestStat+2), statForeColor);
                        Console.Print(2 + longestStat + 2, yCoordinate, "-", statSelected ? RLColor.Red : RLColor.White);
                        Console.Print(2 + longestStat + 4, yCoordinate, stat.statValue.ToString().PadBoth(4), statForeColor);
                        Console.Print(2 + longestStat + 10, yCoordinate, "+", statSelected ? RLColor.Green : RLColor.White);

                        yCoordinate += 1;
                    }

                    yCoordinate += 1;

                    break;
                default:
                    throw new NotImplementedException();
                 
            }
        }

        private void RenderButtons(Form form)
        {
            var xCoordinate = 2;

            foreach (FormButton flag in (FormButton[])Enum.GetValues(typeof(FormButton)))
            {
                if (flag == FormButton.None) continue;

                if (form.Buttons.HasFlag(flag))
                {
                    RenderSingleButton(ref xCoordinate, flag.ToString(), form.FormSelection.SelectedItem == flag.ToString());
                }
            }


        }

        private void RenderSingleButton(ref int xCoordinate, string text, bool selected)
        {
            
        }

        private void RenderTitleBar(Form form)
        {
            Console.Print(1, 2, form.Title, RLColor.White, RLColor.Black);
        }
    }
}