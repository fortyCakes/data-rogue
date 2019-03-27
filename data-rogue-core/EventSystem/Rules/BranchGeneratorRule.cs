using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class BranchGeneratorRule : IEventRule
    {
        private readonly ISystemContainer _systemContainer;

        public BranchGeneratorRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.UsePortal };
        public int RuleOrder => int.MinValue;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var portal = eventData as Portal;

            var branchEntity = _systemContainer.EntityEngine.Get(portal.BranchLink.Value);
            var branch = branchEntity.Get<Branch>();

            if (!branch.Generated)
            {
                _systemContainer.ActivitySystem.Push(new StaticTextActivity("Generating branch..."));

                var generator = new BranchGenerator();

                var generatedBranch = generator.Generate(_systemContainer, branchEntity);

                foreach (Map map in generatedBranch.Maps)
                {
                    _systemContainer.MapSystem.MapCollection.AddMap(map);
                }

                _systemContainer.ActivitySystem.Pop();
            }

            return true;
        }
    }
}
