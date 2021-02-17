using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Utils
{
    public static class BLTNameExtensions
    {
        public static string GetBLTName(this IEntity hoveredEntity)
        {
            return $"[color={hoveredEntity.DescriptionColor.ToHexCode()}]{hoveredEntity.DescriptionName}[/color]";
        }
    }
}
