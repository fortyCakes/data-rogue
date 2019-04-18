﻿using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTTilesUnifiedRenderer : IUnifiedRenderer
    {
        private int _height;
        private int _width;
        private readonly ISpriteManager _spriteManager;
        private readonly List<IDataRogueControlRenderer> _controlRenderers;

        public BLTTilesUnifiedRenderer(List<IDataRogueControlRenderer> controlRenderers, ISpriteManager spriteManager)
        {
            _spriteManager = spriteManager;
            _controlRenderers = controlRenderers;
        }

        public void Render(ISystemContainer systemContainer, IActivity activity)
        {
            BLT.Clear();

            var playerFov = FOVHelper.CalculatePlayerFov(systemContainer);

            _height = BLT.State(BLT.TK_HEIGHT);
            _width = BLT.State(BLT.TK_WIDTH);

            foreach (var control in activity.GetLayout(systemContainer, _spriteManager, _controlRenderers, playerFov, _width, _height))
            {
                IDataRogueControlRenderer statsDisplayer = _controlRenderers.Single(s => s.DisplayType == control.GetType());
                statsDisplayer.Display(_spriteManager, control, systemContainer, playerFov);
            }
        }
    }
}
