using System;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using data_rogue_core.Activities;
using data_rogue_core.Controls;

namespace data_rogue_core.IOSystems
{
    public class RLNetLinesRenderer : RLNetControlRenderer
    {
        private char?[,] _lineChars;

        public override Type DisplayType => typeof(LinesControl);
        protected override void DisplayInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var size = display.Position.Size;

            var backgroundConsole = new RLConsole(size.Width, size.Height);

            CalculateLines(size, (display as LinesControl).Configuration);

            var foreColor = RLColor.White;
            var backColor = RLColor.Black;

            for (int x = 0; x < size.Width; x++)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    if (_lineChars[x, y].HasValue)
                    {
                        backgroundConsole.Print(x, y, _lineChars[x, y].ToString(), foreColor, backColor);
                    }
                }
            }

            RLConsole.Blit(backgroundConsole, 0, 0, backgroundConsole.Width, backgroundConsole.Height, console, 0, 0);
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return display.Position.Size;
        }

        private void CalculateLines(Size size, IOSystemConfiguration configuration)
        {

            byte connectTop = 1;
            byte connectRight = 2;
            byte connectBottom = 4;
            byte connectLeft = 8;

            var mapping = new Dictionary<byte, char>
            {
                {0, (char)254 },
                {1, (char)186 },
                {2, (char)205 },
                {3, (char)200 },
                {4, (char)186 },
                {5, (char)186 },
                {6, (char)201 },
                {7, (char)204 },
                {8, (char)205 },
                {9, (char)188 },
                {10, (char)205 },
                {11, (char)202 },
                {12, (char)187 },
                {13, (char)185 },
                {14, (char)203 },
                {15, (char)206 },
            };

            var lines = new bool[size.Width, size.Height];
            _lineChars = new char?[size.Width, size.Height];

            for (int x = 0; x < size.Width; x++)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    lines[x, y] = !IsInSubconsole(configuration, x, y);
                }
            }

            for (int x = 0; x < size.Width; x++)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    if (lines[x, y])
                    {
                        byte tileConnects = 0;
                        if (x != 0 && lines[x - 1, y]) tileConnects |= connectLeft;
                        if (y != 0 && lines[x, y - 1]) tileConnects |= connectTop;
                        if (y != size.Height - 1 && lines[x, y + 1]) tileConnects |= connectBottom;
                        if (x != size.Width - 1 && lines[x + 1, y]) tileConnects |= connectRight;

                        _lineChars[x, y] = mapping[tileConnects];
                    }
                    else
                    {
                        _lineChars[x, y] = null;
                    }
                }
            }
        }

        private bool IsInSubconsole(IOSystemConfiguration configuration, int x, int y)
        {
            foreach (var config in configuration.GameplayRenderingConfiguration)
            {
                if (config.Position.Contains(x, y))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
