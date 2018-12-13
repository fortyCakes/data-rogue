using System;
using System.Threading;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;
using data_rogue_core.Menus;
using data_rogue_core.Systems;
using RLNET;

namespace data_rogue_core.EventSystem.Rules
{
    public class BranchGeneratorRule : IEventRule
    {
        private readonly IEntityEngineSystem _engine;
        private readonly IPositionSystem _positionSystem;
        private readonly string _seed;

        public BranchGeneratorRule(IEntityEngineSystem engine, IPositionSystem positionSystem, string seed)
        {
            _engine = engine;
            _positionSystem = positionSystem;
            _seed = seed;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.UsePortal };
        public int RuleOrder => Int32.MinValue;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var portal = eventData as Portal;

            var branch = _engine.GetEntityWithName(portal.BranchLink).Get<Branch>();

            if (!branch.Generated)
            {
                Game.ActivityStack.Push(new StaticTextActivity("Generating branch...", Game.RendererFactory));

                var generator = BranchGeneratorFactory.GetGenerator(branch.GenerationType);

                var generatedBranch = generator.Generate(branch, _engine, _positionSystem, _seed);

                foreach (Map map in generatedBranch.Maps)
                {
                    Game.WorldState.Maps.AddMap(map);
                }

                Game.ActivityStack.Pop();
            }

            return true;
        }
    }
}
