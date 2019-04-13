using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Renderers;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems;
using OpenTK.Input;
using RLNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public delegate void GameLoopEventHandler(object sender, GameLoopEventArgs e);
}
