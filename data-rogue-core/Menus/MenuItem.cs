using System.Drawing;

namespace data_rogue_core.Menus
{
    public class MenuItem
    {
        public string Text { get; private set; }
        public bool Enabled { get; private set; }
        public Color Color { get; private set; }

        public MenuItem(string text, Color color, bool enabled = true)
        {
            Text = text;
            Color = color;
            Enabled = enabled;
        }
    }
}