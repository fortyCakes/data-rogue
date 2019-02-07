namespace data_rogue_core.EntityEngineSystem
{
    public interface ICustomFieldSerialization
    {
        string Serialize();

        void Deserialize(string value);
    }
}