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
using System.Windows.Forms.VisualStyles;

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
        private LargeTextControl _textControl;

        public ToastActivity(Rectangle position, Padding padding, IActivitySystem activitySystem, string message, Color color, int displayMs = 500, int fadeOutMs = 1000) : base(position, padding)
        {
            _activitySystem = activitySystem;
            _message = message;
            _color = color;
            _displayMs = displayMs;
            _fadeOutMs = fadeOutMs;
            _stopwatch = new Stopwatch();

            OnLayout += ToastActivity_OnLayout;
        }

        public ToastActivity(IActivitySystem activitySystem, string message, Color color, int displayMs = 500, int fadeOutMs = 1000) : base(activitySystem.DefaultPosition, activitySystem.DefaultPadding)
        {
            _activitySystem = activitySystem;
            _message = message;
            _color = color;
            _displayMs = displayMs;
            _fadeOutMs = fadeOutMs;
            _stopwatch = new Stopwatch();

            OnLayout += ToastActivity_OnLayout;
        }


        private void ToastActivity_OnLayout(object sender, EventArgs e)
        {
            UpdateAnimatedControls();
        }

        public override void InitialiseControls()
        {
            _textControl = new LargeTextControl
            {
                Parameters = _message,
                BackColor = Color.Transparent,
                Color = GetColor(),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            Controls.Add(_textControl);
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

        public void UpdateAnimatedControls()
        {
            _textControl.Color = GetColor();

            if (_stopwatch.ElapsedMilliseconds > _displayMs + _fadeOutMs)
            {
                Close();
            }
        }

        private void Close()
        {
            _activitySystem.RemoveActivity(this);
        }
    }
}
