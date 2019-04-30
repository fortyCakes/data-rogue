using data_rogue_core.Activities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Controls
{
    public class BaseControl : IDataRogueControl
    {
        public bool IsFocused { get; set; }
        public bool IsPressed { get; set; }

        public Rectangle Position { get; set; }

        public Color Color { get; protected set; } = Color.White;
        public Color BackColor { get; protected set; } = Color.Black;

        public virtual void Click(object sender, PositionEventHandlerArgs eventArgs)
        {
            OnClick?.Invoke(sender, eventArgs);
        }
        public event PositionEventHandler OnClick;

        public bool Visible { get; set; } = true;
    }
}
