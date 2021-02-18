using data_rogue_core.EntityEngineSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_one.Components
{
    public class EnchantmentGeneration : IEntityComponent
    {
        public string Prefix;
        public string Suffix;
        public int EnchantPower = 1;
        public string DescriptionLine;
    }
}
