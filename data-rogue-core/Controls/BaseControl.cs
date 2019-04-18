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
        
        public Color Color { get; protected set; }
        public Color BackColor { get; protected set; }

        public event PositionEventHandler Click;
    }

    public class BackgroundControl : BaseControl
    {
        public bool FillUnusedSpace { get; set; } = false;
    }
}
