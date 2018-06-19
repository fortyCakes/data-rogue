using data_rogue_core.Entities;

namespace data_rogue_core.Interfaces
{
    public interface IActor
    {
        string Name { get; set; }
        int Awareness { get; set; }
        int Attack { get; set; }
        int Speed { get; set; }
        HealthCounter HealthCounter { get; }
        int CurrentHealth { get; }
        int MaxHealth { get; }
        int Gold { get; set; }
        int DefenseChance { get; set; }
        int Defense { get; set; }
        int AttackChance { get; set; }
    }
}
