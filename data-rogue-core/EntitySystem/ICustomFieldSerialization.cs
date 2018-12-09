namespace data_rogue_core.EntitySystem
{
    public interface ICustomFieldSerialization
    {
        string Serialize();

        void Deserialize(string value);
    }
}