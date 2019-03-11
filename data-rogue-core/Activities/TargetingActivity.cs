using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;
using System;

namespace data_rogue_core.Activities
{
    public class TargetingActivity : IActivity
    {
        public ActivityType Type => ActivityType.Targeting;
        public object Data => TargetingActivityData;

        public MapCoordinate CurrentTarget => TargetingActivityData.CurrentTarget;

        public bool RendersEntireSpace => false;

        private IRendererFactory rendererFactory;
        private IActivitySystem activitySystem;

        public ITargetingRenderer Renderer { get; set; }

        public TargetingActivityData TargetingActivityData;

        public TargetingActivity(TargetingData targetingData, Action<MapCoordinate> callback, ISystemContainer systemContainer)
        {
            this.rendererFactory = systemContainer.RendererSystem.RendererFactory;
            this.activitySystem = systemContainer.ActivitySystem;

            Renderer = (ITargetingRenderer)rendererFactory.GetRendererFor(Type);

            TargetingActivityData = new TargetingActivityData
            {
                TargetingData = targetingData,
                CurrentTarget = null,
                Callback = callback
            };
        }

        public void Render(ISystemContainer systemContainer)
        {
            Renderer.Render(systemContainer, TargetingActivityData);
        }

        public void Complete()
        {
            if (CurrentTarget != null)
            {
                TargetingActivityData.Callback(CurrentTarget);
            }
            else
            {
                // Target OnCancel behaviour?
            }

            if (activitySystem.Peek() == this)
            {
                activitySystem.Pop();
            }
        }
    }

    public class TargetingActivityData
    {
        public TargetingData TargetingData { get; internal set; }
        public MapCoordinate CurrentTarget { get; internal set; }
        public Action<MapCoordinate> Callback { get; set; }
    }
}