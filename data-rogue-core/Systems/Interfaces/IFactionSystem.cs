using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using System;
using System.Windows.Forms;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IFactionSystem
    {
        Faction FactionOf(IEntity entity);

        bool IsSameFaction(IEntity entity, IEntity entity2);
    }
}
