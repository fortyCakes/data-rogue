using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using System;

namespace data_rogue_core.Components
{
    public class Actor : IEntityComponent
    {
        public ulong NextTick;
        public ulong Speed = 1000;

        public Faction Faction = new Faction("Default");
    }

    public class Faction : IEquatable<Faction>, ICustomFieldSerialization
    {
        public string FactionName;

        public Faction()
        {

        }

        public Faction(string factionName)
        {
            FactionName = factionName;
        }

        public void Deserialize(string value)
        {
            this.FactionName = value;
        }

        public bool Equals(Faction other)
        {
            if (other == null) return false;

            return FactionName == other.FactionName;
        }

        public string Serialize()
        {
            return FactionName;
        }
    }
}
