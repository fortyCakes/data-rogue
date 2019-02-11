using System.Drawing;

namespace data_rogue_core.Menus
{
    public class MenuItem
    {
        public string Text { get; private set; }
        public bool Enabled { get; private set; }
        public object Value { get; }

        public MenuItem(string text, object value = null, bool enabled = true)
        {
            Text = text;
            Enabled = enabled;
            Value = value;
        }
    }
}