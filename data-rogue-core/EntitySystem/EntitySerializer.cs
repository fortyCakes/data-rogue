﻿using data_rogue_core.Data;
using data_rogue_core.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace data_rogue_core.EntitySystem
{
    public static class EntitySerializer
    {
        private const string ENTITY_NAME_PATTERN = "^\"(.*)\"";

        public static string Serialize(Entity entity)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"\"{entity.Name}\":{entity.EntityId}");

            foreach (var component in entity.Components)
            {
                stringBuilder.Append(ComponentSerializer.Serialize(component, 1));
            }

            return stringBuilder.ToString();
        }

        public static List<Entity> DeserializeMultiple(string input, IEntityEngineSystem entityEngineSystem)
        {
            var entityTexts = SplitIntoEntities(input);

            return entityTexts.Select(text => Deserialize(text, entityEngineSystem)).ToList();
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

        public static Entity Deserialize(string input, IEntityEngineSystem entityEngineSystem)
        {
            var lines = input.SplitLines();

            EntityBuilder currentEntity = null;
            StringBuilder currentComponent = null;



            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue; // empty line
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
                }

                if (line.StartsWith("["))
                {
                    var componentName = Regex.Match(line, "^\\[(.*)\\]").Groups[1].Value;
                    currentComponent = new StringBuilder();
                    currentComponent.AppendLine(line);
                    currentEntity?.Components.Add(currentComponent);
                }

                if (line.StartsWith("    ") || line.StartsWith("\t"))
                {
                    currentComponent.AppendLine(line);
                }
            }

            return currentEntity.Build(entityEngineSystem);
        }
    }
}
