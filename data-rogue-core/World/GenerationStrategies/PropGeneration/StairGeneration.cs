﻿using System;
using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.World.GenerationStrategies
{

    public class StairGeneration : BaseEntityGenerationStrategy
    {
        private class ItemList: Dictionary<int, HashSet<IEntity>> { }

        public override void Generate(ISystemContainer systemContainer, GeneratedBranch generatedBranch, IEntity branch, IRandom random, IProgress<string> progress)
        {
            progress.Report("Placing stairs");

            BranchGenerator.PlaceStairs(systemContainer, generatedBranch, random);
        }
        
    }
}
