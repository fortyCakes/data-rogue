namespace data_rogue_core.Behaviours
{
    public interface IBehaviourFactory
    {
        IBehaviour Get(string behaviourName);
    }
}