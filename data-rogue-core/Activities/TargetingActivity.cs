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

        public bool RendersEntireSpace => false;
        public ITargetingRenderer Renderer { get; set; }

        private TargetingActivityData TargetingActivityData;

        public TargetingActivity(TargetingData targetingData, Action<MapCoordinate> callback, IRendererFactory rendererFactory)
        {
            Renderer = (ITargetingRenderer)rendererFactory.GetRendererFor(Type);

            TargetingActivityData = new TargetingActivityData
            {
                TargetingData = targetingData,
                CurrentTarget = null
            };
        }
        public void Render()
        {
            Renderer.Render(Game.WorldState, Game.SystemContainer, TargetingActivityData);
        }
    }

    public class TargetingActivityData
    {
        public TargetingData TargetingData { get; internal set; }
        public MapCoordinate CurrentTarget { get; internal set; }
    }
}