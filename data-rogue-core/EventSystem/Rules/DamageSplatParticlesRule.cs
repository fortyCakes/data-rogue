using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace data_rogue_core.EventSystem.Rules
{
    public class DamageSplatParticlesRule : IEventRule
    {
        private const int NUM_SPLATS = 15;
        private ISystemContainer _systemContainer;

        public DamageSplatParticlesRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Damage };
        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.AfterSuccess;

        private IEventSystem EventRuleSystem { get; }
        public IMessageSystem MessageSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var location = sender.TryGet<Position>();
            var data = eventData as DamageEventData;

            for (int i = 0; i < NUM_SPLATS; i++)
            {
                CreateSplat(location);
            }
          

            return true;
        }

        private void CreateSplat(Position location)
        {
            var theta = Math.PI * _systemContainer.Random.ZeroToOne() * 2;
            int randomTime = (int)Math.Floor(500 * _systemContainer.Random.ZeroToOne());

            _systemContainer.ParticleSystem.CreateTextParticle(location.MapCoordinate, new List<AnimationMovement> {
                new AnimationMovement(new VectorDouble(0.5, 0.5), 1),
                new AnimationMovement(new VectorDouble(Math.Sin(theta), Math.Cos(theta)), randomTime),
                new AnimationMovement(new VectorDouble(0,0), 250)
               }, ".", Color.DarkRed);
        }
    }
}
