using System;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class BranchGeneratorRule : IEventRule
    {
        private readonly ISystemContainer systemContainer;

        public BranchGeneratorRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.UsePortal };
        public int RuleOrder => Int32.MinValue;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var portal = eventData as Portal;

            var branch = systemContainer.EntityEngine.GetEntity(portal.BranchLink.Value).Get<Branch>();

            if (!branch.Generated)
            {
                Game.ActivityStack.Push(new StaticTextActivity("Generating branch...", Game.RendererFactory));

                var generator = BranchGeneratorFactory.GetGenerator(branch.GenerationType);

                var generatedBranch = generator.Generate(systemContainer, branch);

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
