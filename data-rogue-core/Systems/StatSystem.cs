using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using System;
using System.Linq;

namespace data_rogue_core
{
    public class StatSystem : IStatSystem
    {
        private readonly IEntityEngine _engine;

        public StatSystem(IEntityEngine engine)
        {
            _engine = engine;
        }

        public int GetStat(IEntity entity, string statName)
        {
            return GetStatComponent(entity, statName)?.Value ?? 0;
        }

        public void IncreaseStat(IEntity entity, string statName, int value)
        {
            var stat = GetStatComponent(entity, statName);

            if (stat != null)
            {
                stat.Value += value;
            }
            else
            {
                AddStatComponent(entity, statName, value);
            }
        }

        public void SetStat(IEntity entity, string statName, int value)
        {
            var stat = GetStatComponent(entity, statName);

            if (stat != null)
            { 
                stat.Value = value;
            }
            else
            {
                AddStatComponent(entity, statName, value);
            }
        }

        private void AddStatComponent(IEntity entity, string statName, int value)
        {
            _engine.AddComponent(entity, new Stat { Name = statName, Value = value });
        }

        private static Stat GetStatComponent(IEntity entity, string statName)
        {
            return entity.Components.OfType<Stat>().SingleOrDefault(s => s.Name == statName);
        }
    }
}