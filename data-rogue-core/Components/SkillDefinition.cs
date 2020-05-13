using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class Skill : IEntityComponent
    {
        public string ScriptName;
        public int Cost;
        public int Speed = 1000;
    }
}
