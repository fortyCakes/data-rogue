using data_rogue_core.IOSystems;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using OpenTK.Input;
using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Controls;
using System.Drawing;
using System;
using System.Diagnostics;
using System.Linq;
using data_rogue_core.Maps;
using System.Windows.Forms;
using LargeTextControl = data_rogue_core.Controls.LargeTextControl;

namespace data_rogue_core.Activities
{
    public class ToastActivity : BaseActivity
    {
        public override ActivityType Type => ActivityType.Toast;
        public override bool RendersEntireSpace => false;
        public override bool AcceptsInput => false;

        private readonly IActivitySystem _activitySystem;
        private readonly string _message;
        private readonly Color _color;
        private int _displayMs;
        private int _fadeOutMs;
        private Stopwatch _stopwatch;

        public ToastActivity(IActivitySystem activitySystem, string message, Color color, int displayMs = 500, int fadeOutMs = 1000)
        {
            _activitySystem = activitySystem;
            _message = message;
            _color = color;
            _displayMs = displayMs;
            _fadeOutMs = fadeOutMs;
            _stopwatch = new Stopwatch();
        }

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            if (_stopwatch.ElapsedMilliseconds > _displayMs + _fadeOutMs)
            {
                Close();
            }

            var x = width / 2;
            var y = height / 2;

            var textControl = new LargeTextControl
            {
                Position = new Rectangle(x, y, 0, 0),
                Parameters = _message,
                Entity = null,
                IsFocused = false,
                IsPressed = false
            };

            var controlRenderer = controlRenderers.Single(s => s.DisplayType == typeof(TextControl));
            var size = controlRenderer.GetSize(rendererHandle, textControl, systemContainer, playerFov);
            textControl.Position = new Rectangle(x - size.Width / 2, y - size.Height / 2, size.Width, size.Height);

            textControl.SetData(null, new InfoDisplay {BackColor = Color.Transparent, Color = GetColor(), ControlType = typeof(TextControl), Parameters = _message});

            return new List<IDataRogueControl>{textControl};
        }

        private Color GetColor()
        {
            return Color.FromArgb(GetAlpha(), _color.R, _color.G, _color.B);
        }

        private int GetAlpha()
        {
            if (!_stopwatch.IsRunning) _stopwatch.Start();

            if (_stopwatch.ElapsedMilliseconds < _displayMs) return 255;

            if (_stopwatch.ElapsedMilliseconds < _displayMs + _fadeOutMs)
            {
                var fadedMs = _stopwatch.ElapsedMilliseconds - _displayMs;
                return 255 - (255 * (int)fadedMs) / _fadeOutMs;
            }

            return 0;
        }

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
        }

        public override void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
        }

        private void Close()
        {
            _activitySystem.RemoveActivity(this);
        }
    }
}
