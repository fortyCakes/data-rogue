using data_rogue_core.Activities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Controls
{
    public class MessageLogControl : BaseInfoControl
    {
        public override bool FillsContainer => true;

        public int NumberOfMessages { get; set; }
    }
}
