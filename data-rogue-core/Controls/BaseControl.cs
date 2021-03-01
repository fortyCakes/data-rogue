using data_rogue_core.Activities;
using data_rogue_core.IOSystems;
using data_rogue_core.IOSystems.BLTTiles;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace data_rogue_core.Controls
{
    public abstract class BaseControl : IDataRogueControl
    {
        private IDataRogueControlRenderer _cachedRenderer;

        public bool IsFocused { get; set; }
        public bool IsPressed { get; set; }

        public Rectangle Position { get; set; }
        public Padding Padding { get; set; } = new Padding(0);
        public Padding Margin { get; set; } = new Padding(0);
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Top;

        public int ActivityIndex { get; set; }

        public Color Color { get; set; } = Color.White;
        public Color BackColor { get; set; } = Color.Black;

        public virtual void Click(object sender, PositionEventHandlerArgs eventArgs)
        {
            OnClick?.Invoke(sender, eventArgs);
        }
        
        public event PositionEventHandler OnClick;
        public event PositionEventHandler OnMouseDown;
        public event PositionEventHandler OnMouseUp;

        public bool Visible { get; set; } = true;

        public virtual bool CanHandleMouse => OnClick != null;
        public virtual bool FillsContainer => false;

        public virtual ActionEventData HandleMouse(MouseData mouse, IDataRogueControlRenderer renderer, ISystemContainer systemContainer) => null;

        public void MouseDown(object sender, PositionEventHandlerArgs eventArgs)
        {
            OnMouseDown?.Invoke(sender, eventArgs);
        }

        public void MouseUp(object sender, PositionEventHandlerArgs eventArgs)
        {
            OnMouseUp?.Invoke(sender, eventArgs);
        }

        public virtual bool Layout(List<IDataRogueControlRenderer> controlRenderers, ISystemContainer systemContainer, object handle, List<MapCoordinate> playerFov, Rectangle boundingBox)
        {
            var boxLessMargin = boundingBox.Pad(Margin);

            return GetCachedRenderer(controlRenderers).Layout(handle, this, systemContainer, playerFov, boxLessMargin, Padding, HorizontalAlignment, VerticalAlignment);
        }

        public virtual void Paint(List<IDataRogueControlRenderer> controlRenderers, object handle, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            GetCachedRenderer(controlRenderers).Paint(handle, this, systemContainer, playerFov);
        }

        protected IDataRogueControlRenderer GetCachedRenderer(List<IDataRogueControlRenderer> controlRenderers)
        {
            if (_cachedRenderer == null)
            {
                _cachedRenderer = controlRenderers.Single(r => r.DisplayType == GetType());
            }

            return _cachedRenderer;
        }
    }
}
