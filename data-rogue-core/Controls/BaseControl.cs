using data_rogue_core.Activities;
using data_rogue_core.IOSystems;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Controls
{
    public abstract class BaseControl : IDataRogueControl
    {
        public bool IsFocused { get; set; }
        public bool IsPressed { get; set; }

        public Rectangle Position { get; set; }

        public int ActivityIndex { get; set; }

        public Color Color { get; set; } = Color.White;
        public Color BackColor { get; set; } = Color.Black;

        public virtual void Click(object sender, PositionEventHandlerArgs eventArgs)
        {
            OnClick?.Invoke(sender, eventArgs);
        }

        public virtual ActionEventData HandleMouse(MouseData mouse, IDataRogueControlRenderer renderer, ISystemContainer systemContainer) => null;

        public event PositionEventHandler OnClick;

        public bool Visible { get; set; } = true;
        public virtual bool CanHandleMouse => false;

        public virtual bool FillsContainer => false;
    }
}
