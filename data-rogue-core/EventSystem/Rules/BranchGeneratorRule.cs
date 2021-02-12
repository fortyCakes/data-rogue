using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Threading.Tasks;

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
        public uint RuleOrder => uint.MinValue;
        public EventRuleType RuleType => EventRuleType.BeforeEvent;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var portal = eventData as Portal;

            var branchEntity = _systemContainer.EntityEngine.Get(portal.BranchLink.Value);
            var branch = branchEntity.Get<Branch>();

            if (!branch.Generated)
            {
                var branchGenerationDisplayEntity = _systemContainer.EntityEngine.New("branchGenerationTracker",
                    new Appearance { Color = System.Drawing.Color.White, Glyph = '@' },
                    new SpriteAppearance { Bottom = "generic_person" },
                    new Animated(),
                    new Animation(),
                    new Description { Name = "Branch generation tracker", Detail= "Generating branch..." });

                _systemContainer.ActivitySystem.Push(new StaticTextActivity(_systemContainer.ActivitySystem, "Generating branch...", false, branchGenerationDisplayEntity));


                var generator = new BranchGenerator();

                Task<GeneratedBranch> generationTask = Task<GeneratedBranch>.Factory.StartNew(() => {
                    var generatedBranch = generator.Generate(_systemContainer, branchEntity);

                    foreach (Map map in generatedBranch.Maps)
                    {
                        _systemContainer.MapSystem.MapCollection.AddMap(map);
                    }

                    _systemContainer.ActivitySystem.Pop();

                    _systemContainer.EntityEngine.Destroy(branchGenerationDisplayEntity);

                    _systemContainer.EventSystem.Try(type, sender, eventData);

                    return generatedBranch;
                });

                return false;
            }

            return true;
        }
    }
}
