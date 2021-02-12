using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.ComponentModel;
using System.Threading;
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
                var branchLoading = new BranchLoadingScreenActivity(_systemContainer);
                _systemContainer.ActivitySystem.Push(branchLoading);

                Action<string> setBranchLoadingStatus = (progressString) => { branchLoading.Text = progressString; };
                var progress = new Progress<string>(setBranchLoadingStatus);

                var task = Task.Run(() => GenerateBranch(branchEntity, sender, portal, progress));
                    
                return false;
            }

            return true;
        }

        private GeneratedBranch GenerateBranch(IEntity branchEntity, IEntity sender, Portal portal, IProgress<string> progress)
        {
            var generator = new BranchGenerator();

            var generatedBranch = generator.Generate(_systemContainer, branchEntity, progress);

            foreach (Map map in generatedBranch.Maps)
            {
                _systemContainer.MapSystem.MapCollection.AddMap(map);
            }

            _systemContainer.ActivitySystem.Pop();

            _systemContainer.PositionSystem.SetPosition(sender, portal.Destination);
            _systemContainer.EventSystem.Try(EventType.UsePortal, sender, portal);

            return generatedBranch;
        }
    }
}
