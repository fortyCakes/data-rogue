namespace data_rogue_core.EventSystem.EventData
{
    public enum Stat
    {
        Muscle,
        Agility,
        Tension,
        Willpower,
        Intellect
    }

    public class GetStatEventData
    {
        public Stat Stat;
        public decimal Value;
    }
}
