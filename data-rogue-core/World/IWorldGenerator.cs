using data_rogue_core.Forms.StaticForms;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public interface IWorldGenerator
    {
        void Create(ISystemContainer systemContainer, CharacterCreationForm characterCreationForm);
    }
}