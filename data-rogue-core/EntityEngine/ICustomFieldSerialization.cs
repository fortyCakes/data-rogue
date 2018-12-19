namespace data_rogue_core.EntityEngine
{
    public interface ICustomFieldSerialization
    {
        string Serialize();

        void Deserialize(string value);
    }
}