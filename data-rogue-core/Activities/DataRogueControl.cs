using System.Drawing;

namespace data_rogue_core.Activities
{
    public abstract class DataRogueControl : IDataRogueControl
    {
        public bool IsFocused { get; set; }
        public bool IsPressed { get; set; }
        public Rectangle Position { get; set; } = new Rectangle();

        public Color Color { get; set; }
        public Color BackColor { get; set; }

        public event PositionEventHandler Click;
    }
}