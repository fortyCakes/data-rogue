using data_rogue_core.Forms.StaticForms;

namespace data_rogue_core
{
    public interface ISaveSystem
    {
        void Create(CharacterCreationForm characterCreationForm);

        void Save();

        void Load();
    }
}