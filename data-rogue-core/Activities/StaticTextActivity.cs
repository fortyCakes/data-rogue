using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
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
        public override ActivityType Type => ActivityType.TextInput;
        public override bool RendersEntireSpace => false;
        public override bool AcceptsInput => true;

        public virtual string Text { get; set; }
        public bool CloseOnKeyPress { get; }

        private readonly IActivitySystem _activitySystem;
        protected IEntity _displayEntity;

        public StaticTextActivity(IActivitySystem activitySystem, string staticText, bool closeOnKeyPress = true, IEntity displayEntity = null)
        {
            Text = staticText;
            CloseOnKeyPress = closeOnKeyPress;
            _activitySystem = activitySystem;
            _displayEntity = displayEntity;
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
            if (mouse.IsLeftClick && CloseOnKeyPress)
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
            var controls = new List<IDataRogueControl>();

            if (RendersEntireSpace)
            {
                controls.Add( new BackgroundControl { Position = new Rectangle(0, 0, width, height) });
                if (_displayEntity != null)
                {
                    var entityControl = new MenuEntityControl { Position = new Rectangle(renderer.ActivityPadding.Left, renderer.ActivityPadding.Top, 0, 0), Entity = _displayEntity };
                    var entitySize = SetSize(entityControl, renderer, systemContainer, rendererHandle, playerFov);

                    controls.Add(entityControl);
                    controls.Add(new TextControl { Position = new Rectangle(renderer.ActivityPadding.Left + entitySize.Width + renderer.ActivityPadding.Right, renderer.ActivityPadding.Top, width, height), Parameters = Text });
                }
                else
                {
                    controls.Add(new TextControl { Position = new Rectangle(renderer.ActivityPadding.Left, renderer.ActivityPadding.Top, width, height), Parameters = Text });
                }
            }
            else
            {
                Size entitySize = new Size(0, 0);
                if (_displayEntity != null)
                {
                    var entityControl = new MenuEntityControl { Position = new Rectangle(renderer.ActivityPadding.Left, renderer.ActivityPadding.Top, 0, 0), Entity = _displayEntity };
                    entitySize = SetSize(entityControl, renderer, systemContainer, rendererHandle, playerFov);
                    controls.Add(entityControl);
                }

                var textControl = new TextControl { Position = new Rectangle(renderer.ActivityPadding.Left + entitySize.Width + renderer.ActivityPadding.Right, renderer.ActivityPadding.Top, 0, 0), Parameters = Text };
                var textSize = SetSize(textControl, renderer, systemContainer, rendererHandle, playerFov);

                var finalWidth = textControl.Position.Right + renderer.ActivityPadding.Right;
                var finalHeight = textControl.Position.Bottom + renderer.ActivityPadding.Bottom;

                var backgroundControl = new BackgroundControl { Position = new Rectangle(0, 0, finalWidth, finalHeight) };
                controls.Add(textControl);
                controls.Add(backgroundControl);
                CenterControls(controls, width, height, finalWidth, finalHeight);

            }

            return controls;
        }

        private static Size SetSize(IDataRogueControl control, IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<MapCoordinate> playerFov)
        {
            var size = renderer.GetRendererFor(control).GetSize(rendererHandle, control, systemContainer, playerFov);
            control.Position = new Rectangle(control.Position.Location, size);
            return size;
        }

        private void CenterControls(List<IDataRogueControl> controls, int width, int height, int finalWidth, int finalHeight)
        {
            var x = width / 2 - finalWidth / 2;
            var y = height / 2 - finalHeight / 2;


            foreach (var control in controls)
            {
                control.Position = new Rectangle(control.Position.Left + x, control.Position.Top + y, control.Position.Width, control.Position.Height);
            }
        }

        protected virtual void Close()
        {
            _activitySystem.RemoveActivity(this);
        }
    }
}
