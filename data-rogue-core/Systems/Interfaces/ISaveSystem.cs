using data_rogue_core.Data;
using data_rogue_core.Forms.StaticForms;
using System.Collections.Generic;

namespace data_rogue_core.Systems.Interfaces
{
    public interface ISaveSystem
    {
        void Create(CharacterCreationForm characterCreationForm);

        void Save();
        void SaveMorgueFile(string morgueFileText);

        void Load();
        SaveState GetSaveState();
        void SaveHighScore(string name, decimal score);

        List<HighScore> GetHighScores();
    }
}