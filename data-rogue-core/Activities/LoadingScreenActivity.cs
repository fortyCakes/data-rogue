using System.Collections.Generic;
using data_rogue_core.IOSystems;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public class LoadingScreenActivity : IActivity
    {
        public ActivityType Type => ActivityType.StaticDisplay;
        public object Data => Text;
        public bool RendersEntireSpace => true;

        public bool Running => true;

        public string Text { get; set; }
        public IStaticTextRenderer Renderer { get; private set; }

        public bool CloseOnKeyPress => false;

        public LoadingScreenActivity(string staticText)
        {
            Text = staticText;
        }

        public void Render(ISystemContainer systemContainer)
        {
            Renderer.Render(Text, RendersEntireSpace);
        }

        public void Initialise(IRenderer renderer)
        {
            Renderer = (IStaticTextRenderer)renderer;
        }

        public void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
        }

        public void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
        }

        public void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
        }

        public IEnumerable<IDataRogueControl> GetLayout(int width, int height)
        {
            throw new System.NotImplementedException();
        }
    }
}
