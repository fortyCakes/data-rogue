﻿using data_rogue_core.Data;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace data_rogue_core.EntityEngineSystem
{
    public static class EntitySerializer
    {
        private const string ENTITY_NAME_PATTERN = "^\"(.*)\"";

        public static string Serialize(IEntity entity)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"\"{entity.Name}\":{entity.EntityId}");

            foreach (var component in entity.Components)
            {
                stringBuilder.Append(ComponentSerializer.Serialize(component, 1));
            }

            return stringBuilder.ToString();
        }

        public static IEnumerable<IEntity> DeserializeAll(ISystemContainer systemContainer, IEnumerable<string> input)
        {
            var returnedEntities = new List<IEntity>();

            foreach (var page in input)
            {
                var entityTexts = SplitIntoEntities(page).Where(s => !string.IsNullOrWhiteSpace(s));

                returnedEntities.AddRange(entityTexts.Select(text => Deserialize(systemContainer, text)));
            }

            return returnedEntities;
        }

        private static IEnumerable<string> SplitIntoEntities(string input)
        {
            var stringBuilder = new StringBuilder();
            foreach (var line in input.Split('\n'))
            {
                if (Regex.IsMatch(line, ENTITY_NAME_PATTERN))
                {
                    if (stringBuilder.Length > 0)
                    {
                        yield return stringBuilder.ToString();
                    }

                    stringBuilder = new StringBuilder(line);
                    stringBuilder.AppendLine();
                }
                else
                {
                    stringBuilder.AppendLine(line);
                }
            }

            if (stringBuilder.Length > 0)
            {
                yield return stringBuilder.ToString();
            }
        }

        public static IEntity DeserializeOutsideEngine(string input)
        {
            var fakeSystemContainer = new SystemContainer();
            fakeSystemContainer.CreateSystems("");

            return Deserialize(fakeSystemContainer, input);
        }

        public static IEntity Deserialize(ISystemContainer systemContainer, string input)
        {
            var lines = input.SplitLines();

            EntityBuilder currentEntity = null;
            StringBuilder currentComponent = null;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || IsComment(line))
                {
                    continue;
                }

                if (line.StartsWith("\""))
                {
                    var identityMatch = Regex.Match(line, "^\"(.*)\":([0-9]*)");
                    if (identityMatch.Success)
                    {
                        var entityName = identityMatch.Groups[1].Value;
                        var entityId = uint.Parse(identityMatch.Groups[2].Value);

                        currentEntity = new EntityBuilder(entityName, entityId);
                    }
                    else
                    {
                        identityMatch = Regex.Match(line, ENTITY_NAME_PATTERN);
                        var entityName = identityMatch.Groups[1].Value;

                        currentEntity = new EntityBuilder(entityName);
                    }

                    continue;
                }

                if (line.StartsWith("["))
                {
                    var componentName = Regex.Match(line, "^\\[(.*)\\]").Groups[1].Value;
                    currentComponent = new StringBuilder();
                    currentComponent.AppendLine(line);
                    currentEntity?.Components.Add(currentComponent);

                    continue;
                }

                currentComponent.AppendLine(line);

            }

            return currentEntity.Build(systemContainer);
        }

        private static bool IsComment(string line)
        {
            return line.StartsWith("#");
        }
    }
}
