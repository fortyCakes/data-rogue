using System;
using data_rogue_core.Forms;
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

            RenderMenuItems(form);

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

        private void RenderMenuItems(Form form)
        {
            // For now I will recklessly assume all of the controls fit on the form

            int yCoordinate = 5;

            foreach(FormData formData in form.FormData)
            {
                RenderSingleControl(ref yCoordinate, formData, form.Selected == formData.Name);
            }
        }

        private void RenderSingleControl(ref int yCoordinate, FormData formData, bool selected)
        {
            var foreColor = selected ? RLColor.Cyan : RLColor.White;

            switch (formData.FormDataType)
            {
                case Forms.FormDataType.Text:

                    Console.Print(1, yCoordinate, formData.Name + ": ", foreColor);
                    Console.Print(1 + formData.Name.Length + 3, yCoordinate, ((string)formData.Value).PadRight(30, '_'), RLColor.White);

                    yCoordinate += 2;
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
                    RenderSingleButton(ref xCoordinate, flag.ToString(), form.Selected == flag.ToString());
                }
            }


        }

        private void RenderSingleButton(ref int xCoordinate, string text, bool selected)
        {
            var foreColor = selected ? RLColor.Cyan : RLColor.White;

            Console.Set(xCoordinate, HEIGHT - 3, foreColor, RLColor.Black, 201);
            Console.Set(xCoordinate, HEIGHT - 2, foreColor, RLColor.Black, 186);
            Console.Set(xCoordinate, HEIGHT - 1, foreColor, RLColor.Black, 200);

            Console.Set(xCoordinate + text.Length + 1, HEIGHT - 3, foreColor, RLColor.Black, 187);
            Console.Set(xCoordinate + text.Length + 1, HEIGHT - 2, foreColor, RLColor.Black, 186);
            Console.Set(xCoordinate + text.Length + 1, HEIGHT - 1, foreColor, RLColor.Black, 188);

            for (int i = 0; i < text.Length; i++)
            {
                Console.Set(xCoordinate + i + 1, HEIGHT - 3, foreColor, RLColor.Black, 205);
                Console.Set(xCoordinate + i + 1, HEIGHT - 1, foreColor, RLColor.Black, 205);
            }

            Console.Print(xCoordinate + 1, HEIGHT - 2, text, foreColor, RLColor.Black);

            xCoordinate += text.Length + 3;
        }

        private void RenderTitleBar(Form form)
        {
            Console.Print(1, 2, form.Title, RLColor.White, RLColor.Black);
        }
    }
}