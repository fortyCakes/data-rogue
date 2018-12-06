using data_rogue_core.EntitySystem;

namespace data_rogue_core
{
    public interface ISystem
    {
        void AddEntity(Entity entity);
        void RemoveEntity(Entity entity);

        SystemComponents SystemComponents {get;}
    }
}