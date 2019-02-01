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
        public ITargetingRenderer Renderer { get; set; }

        public TargetingActivityData TargetingActivityData;

        public TargetingActivity(TargetingData targetingData, Action<MapCoordinate> callback, IRendererFactory rendererFactory)
        {
            Renderer = (ITargetingRenderer)rendererFactory.GetRendererFor(Type);

            TargetingActivityData = new TargetingActivityData
            {
                TargetingData = targetingData,
                CurrentTarget = null,
                Callback = callback
            };
        }
        public void Render()
        {
            Renderer.Render(Game.WorldState, Game.SystemContainer, TargetingActivityData);
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

            if (Game.ActivityStack.Peek() == this)
            {
                Game.ActivityStack.Pop();
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