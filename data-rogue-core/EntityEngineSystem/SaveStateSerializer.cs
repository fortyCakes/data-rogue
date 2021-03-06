﻿using data_rogue_core.Utils;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace data_rogue_core
{
    public class SaveStateSerializer
    {
        private const string ENTITY_START_PATTERN = "^\"(.*)\"";
        private const string MAP_START_PATTERN = "^Map:\"(.*)\"";

        public static string Serialize(SaveState state)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Seed: {state.Seed}");
            stringBuilder.AppendLine($"Time: {state.Time}");

            stringBuilder.AppendLine($"===== ENTITIES =====");
            foreach (var entity in state.Entities)
            {
                stringBuilder.Append(entity);
            }

            stringBuilder.AppendLine($"===== MAPS =====");
            foreach (var map in state.Maps)
            {
                stringBuilder.Append(map);
            }

            stringBuilder.AppendLine("===== MESSAGES =====");
            foreach (var message in state.Messages)
            {
                stringBuilder.AppendLine(message);
            }

            return stringBuilder.ToString();
        }

        public static SaveState Deserialize(string serialized)
        {
            var lines = serialized.SplitLines();

            var state = new SaveState();

            state.Seed = Regex.Match(lines.First(), "^Seed: (.*)").Groups[1].Value;
            
            state.Time = ulong.Parse(Regex.Match(lines.Skip(1).First(), "^Time: (.*)").Groups[1].Value);

            int lineIndex;

            var textBuilder = new StringBuilder();
            for (lineIndex = 3; lineIndex < lines.Length; lineIndex++)
            {
                var line = lines[lineIndex];

                if (line == "===== MAPS =====")
                {
                    lineIndex++;
                    break;
                }

                var entityStartMatch = Regex.Match(line, ENTITY_START_PATTERN);

                if (entityStartMatch.Success)
                {
                    AddEntityIfNotEmpty(state, textBuilder);
                    textBuilder = new StringBuilder();
                }

                textBuilder.AppendLine(line);
            }

            AddEntityIfNotEmpty(state, textBuilder);

            textBuilder = new StringBuilder();

            for (; lineIndex < lines.Length; lineIndex++)
            {
                var line = lines[lineIndex];

                if (line == "===== MESSAGES =====")
                {
                    lineIndex++;
                    break;
                }

                var mapStartMatch = Regex.Match(line, MAP_START_PATTERN);

                if (mapStartMatch.Success)
                {
                    AddMapIfNotEmpty(state, textBuilder);
                    textBuilder = new StringBuilder();
                }

                textBuilder.AppendLine(line);
            }

            AddMapIfNotEmpty(state, textBuilder);

            for (; lineIndex < lines.Length; lineIndex++)
            {
                var line = lines[lineIndex];

                if (!string.IsNullOrWhiteSpace(line))
                {
                    state.Messages.Add(line);
                }
        }

            return state;
        }

        private static void AddMapIfNotEmpty(SaveState state, StringBuilder textBuilder)
        {
            if (textBuilder.Length > 0)
            {
                state.Maps.Add(textBuilder.ToString());
            }
        }

        private static void AddEntityIfNotEmpty(SaveState state, StringBuilder textBuilder)
        {
            if (textBuilder.Length > 0)
            {
                state.Entities.Add(textBuilder.ToString());
            }
        }
    }
}