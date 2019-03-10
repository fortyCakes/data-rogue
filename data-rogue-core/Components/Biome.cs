using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{

    public class Biome : IEntityComponent
    {
        public string Name;
    }

    public class Challenge: IEntityComponent
    {
        public int ChallengeRating = 0;
    }
}
