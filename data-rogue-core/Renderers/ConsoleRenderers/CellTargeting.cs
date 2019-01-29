using System;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    [Flags]
    public enum CellTargeting
    {
        None = 0,
        Targetable = 1,
        CurrentTarget = 2
    }
}