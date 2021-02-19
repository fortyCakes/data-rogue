﻿using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class CanAddToMap : IEntityComponent
    {
        public string SettableProperty = null;

        public bool VisibleDuringPlay = true;
    }

    public class MapGenerationCommand: IEntityComponent
    {
        public string Command;
        public string Parameters;
    }
}
