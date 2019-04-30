using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using data_rogue_core.Controls;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using OpenTK.Input;

namespace data_rogue_core.Activities
{
    public class StaticTextActivity : BaseActivity
    {
        public override ActivityType Type => ActivityType.StaticDisplay;
        public override object Data => Text;
        public override bool RendersEntireSpace => false;

        public string Text { get; set; }
        public bool CloseOnKeyPress { get; }

        private readonly IActivitySystem _activitySystem;

        public StaticTextActivity(IActivitySystem activitySystem, string staticText, bool closeOnKeyPress = true)
        {
            Text = staticText;
            CloseOnKeyPress = closeOnKeyPress;
            _activitySystem = activitySystem;
        }
        
        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            if (keyboard != null && keyboard.Key != Key.Unknown && CloseOnKeyPress)
            {
                Close();
            }
        }

        public override void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            if (mouse.IsLeftClick)
            {
                Close();
            }
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            //throw new System.NotImplementedException();
        }

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            yield return new BackgroundControl { Position = new Rectangle(0, 0, width, height) };

            yield return new TextControl { Position = new Rectangle(1, 1, width, height), Parameters = Text };
        }

        private void Close()
        {
            _activitySystem.RemoveActivity(this);
        }
    }
}
