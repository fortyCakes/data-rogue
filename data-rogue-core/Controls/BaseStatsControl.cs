using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Controls
{

    public class BaseInfoControl : BaseControl, IDataRogueInfoControl
    {
        public IEntity Entity { get; set; }

        public string Parameters { get; set; }

        public void SetData(IEntity entity, InfoDisplay display)
        {
            Entity = entity;
            Parameters = display.Parameters;
            Color = display.Color;
            BackColor = display.BackColor;
        }
    }
}
