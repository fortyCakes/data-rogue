﻿using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System.IO;
using System.Reflection;
using data_rogue_core.Data;
using System.Drawing;
using System.Linq;
using data_rogue_core.Forms.StaticForms;
using System;
using System.Collections.Generic;

namespace data_rogue_core
{
    public class SaveSystem : ISaveSystem
    {
        private readonly ISystemContainer _systemContainer;
        private readonly IWorldGenerator _worldGenerator;

        public SaveSystem(ISystemContainer systemContainer, IWorldGenerator worldGenerator)
        {
            _systemContainer = systemContainer;
            _worldGenerator = worldGenerator;
        }

        public void Load()
        {
            var directoryName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Saves");
            var fileName = Path.Combine(directoryName, "saveFile.sav");

            var loadedState = SaveStateSerializer.Deserialize(File.ReadAllText(fileName));

            _systemContainer.TimeSystem.CurrentTime = loadedState.Time;

            _systemContainer.Seed = loadedState.Seed;

            _systemContainer.MapSystem.Initialise();

            _systemContainer.EntityEngine.Initialise(_systemContainer);

            foreach (var savedEntity in loadedState.Entities)
            {
                var entity = EntitySerializer.Deserialize(_systemContainer, savedEntity);
                if (entity.Name == "Player")
                {
                    _systemContainer.PlayerSystem.Player = entity;
                }
            }

            foreach(var savedMap in loadedState.Maps)
            {
                var map = MapSerializer.Deserialize(_systemContainer, savedMap);

                _systemContainer.MapSystem.MapCollection.Add(map.MapKey, map);
            }

            _systemContainer.MessageSystem.Initialise();

            foreach (var savedMessage in loadedState.Messages)
            {
                var message = MessageSerializer.Deserialize(savedMessage);

                _systemContainer.MessageSystem.AllMessages.Add(message);
            }
        }

        public void Create(CharacterCreationForm characterCreationForm)
        {
            _worldGenerator.Create(_systemContainer, characterCreationForm);
        }

        public void Save()
        {
            var directoryName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Saves");
            var fileName = Path.Combine(directoryName, "saveFile.sav");

            Save(fileName);
        }

        public void Save(string fileName)
        {
            EnsureDirectoryExists(fileName);

            var saveState = GetSaveState();

            File.WriteAllText(fileName, SaveStateSerializer.Serialize(saveState));

            _systemContainer.MessageSystem.Write("Saved!", Color.Blue);
        }

        private static void EnsureDirectoryExists(string fileName)
        {
            var directoryName = Path.GetDirectoryName(fileName);

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }

        public SaveState GetSaveState()
        {
            return new SaveState
            {
                Entities = _systemContainer.EntityEngine.MutableEntities.Select(e => EntitySerializer.Serialize(e)).ToList(),
                Maps = _systemContainer.MapSystem.MapCollection.AllMaps.Select(m => MapSerializer.Serialize(m)).ToList(),
                Time = _systemContainer.TimeSystem.CurrentTime,
                Messages = _systemContainer.MessageSystem.AllMessages.Select(m => MessageSerializer.Serialize(m)).ToList()
            };
        }

        public void SaveMorgueFile(string morgueFileText)
        {
            var directoryName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Morgue");

            var time = DateTime.Now.ToUniversalTime().ToString("O").Replace(":", "");

            var fileName = Path.Combine(directoryName, $"morgue-{time}.txt");
            EnsureDirectoryExists(fileName);

            File.WriteAllText(fileName, morgueFileText);

            _systemContainer.MessageSystem.Write("Morgue file saved!", Color.Blue);
        }

        public void SaveHighScore(string name, decimal score)
        {
            var newScore = new HighScore(name, score);

            var highScores = GetHighScores();

            highScores.Add(newScore);
            highScores.Sort((x, y) => x.Score.CompareTo(y.Score));

            SaveHighScoreFile(highScores);
        }

        private void SaveHighScoreFile(List<HighScore> highScores)
        {
            var fileName = GetHighScoreFile();

            File.WriteAllLines(fileName, highScores.Select(s => s.Serialise()));
        }

        public List<HighScore> GetHighScores()
        {
            string fileName = GetHighScoreFile();

            if (File.Exists(fileName))
            {
                return File.ReadAllLines(fileName)
                    .Select(f => HighScore.Deserialise(f))
                    .ToList();
            }
            else
            {
                return new List<HighScore>();
            }
        }

        private static string GetHighScoreFile()
        {
            var directoryName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Saves");
            var fileName = Path.Combine(directoryName, "highscores.txt");
            return fileName;
        }
    }
}