using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Maps
{
    public static class MapSerializer
    {
        private const char DEFAULT_CELL_GLYPH = ' ';
        private const char GLYPH_SEPARATOR = ':';

        public static string Serialize(Map map)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Map:\"{map.MapKey.Key}\"");

            stringBuilder.AppendLine($"default:" + map.DefaultCell.Name);

            stringBuilder.AppendLine($"{map.LeftX},{map.TopY}");

            foreach(MapGenCommand command in map.MapGenCommands)
            {
                stringBuilder.AppendLine(command.ToString());
            }

            var glyphs = GetMapGlyphs(map);

            foreach(var glyph in glyphs)
            {
                stringBuilder.AppendLine($"{glyph.Value}:{glyph.Key}");
            }

            for (int y = map.TopY; y <= map.BottomY; y++)
            {
                for (int x = map.LeftX; x <= map.RightX; x++)
                {
                    if (map.CellExists(x, y))
                    {
                        var cellId = map.CellAt(x, y).Name;
                        var glyph = glyphs[cellId];
                        stringBuilder.Append(glyph);
                    }
                    else
                    {
                        stringBuilder.Append(DEFAULT_CELL_GLYPH);
                    }
                }
                stringBuilder.AppendLine();
            }

            if (map.SeenCoordinates.Any())
            {
                var leftXSeen = map.SeenCoordinates.Min(c => c.X);
                var topYSeen = map.SeenCoordinates.Min(c => c.Y);
                var rightXSeen = map.SeenCoordinates.Max(c => c.X);
                var bottomYSeen = map.SeenCoordinates.Max(c => c.Y);

                stringBuilder.AppendLine($"Seen:{leftXSeen},{topYSeen}");

                for (int y = topYSeen; y <= bottomYSeen; y++)
                {
                    for (int x = leftXSeen; x <= rightXSeen; x++)
                    {
                        if (map.SeenCoordinates.Contains(new MapCoordinate(map.MapKey, x, y)))
                        {
                            stringBuilder.Append("+");
                        }
                        else
                        {
                            stringBuilder.Append(" ");
                        }
                    }
                    stringBuilder.AppendLine();
                }
            }

            return stringBuilder.ToString();
        }
        
        public static Map Deserialize(string savedMap, IEntityEngine entityEngineSystem, IPrototypeSystem prototypeSystem, string mapNameOverride = null)
        {
            var lines = savedMap.Split('\n');

            var mapName = mapNameOverride ?? Extract(lines[0], "Map:\"(.*)\"");
            var defaultCellName = Extract(lines[1], "default:(.*)").Trim();
            var coordinateMatch = Regex.Match(lines[2], "(-?[0-9]),(-?[0-9])");
            var leftX = int.Parse(coordinateMatch.Groups[1].Value);
            var topY = int.Parse(coordinateMatch.Groups[2].Value);

            IEntity defaultCell = prototypeSystem.Create(defaultCellName);

            var lineIndex = 3;

            var commands = GetCommandsInMap(lines, ref lineIndex);

            var glyphDictionary = GetCellsInMap(entityEngineSystem, prototypeSystem, lines, ref lineIndex);

            Map deserialisedMap = new Map(mapName, defaultCell);

            bool seenMode = false;
            int yOffset = 0;

            for (int j = 0; j + lineIndex < lines.Length; j++)
            {
                var line = lines[lineIndex + j].Trim('\r', '\n');

                if (line.StartsWith("Seen:"))
                {
                    seenMode = true;
                    var match = Regex.Match(line, "Seen:(.*),(.*)");
                    leftX = int.Parse(match.Groups[1].Value);
                    topY = int.Parse(match.Groups[2].Value);
                    yOffset = -j-1;
                    continue;
                }

                if (seenMode)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        char glyph = line[i];
                        if (glyph == '+')
                        {
                            deserialisedMap.SetSeen(leftX + i, topY + j + yOffset);
                        }
                    }
                }
                else
                {

                    for (int i = 0; i < line.Length; i++)
                    {
                        char glyph = line[i];
                        if (glyph == DEFAULT_CELL_GLYPH)
                        {
                            continue;
                        }

                        int x = leftX + i;
                        int y = topY + j;

                        IEntity cell = glyphDictionary[glyph];

                        deserialisedMap.SetCell(x, y, cell);
                    }
                }
            }

            deserialisedMap.MapGenCommands = commands;

            return deserialisedMap;
        }

        private static List<MapGenCommand> GetCommandsInMap(string[] lines, ref int lineIndex)
        {
            var commandsInMap = new List<MapGenCommand>();

            Match match;
            while ((match = Regex.Match(lines[lineIndex], @"(\d*),(\d*): (.*)\((.*)\)")).Success)
            {
                var x = int.Parse(match.Groups[1].Value);
                var y = int.Parse(match.Groups[2].Value);

                var command = (MapGenCommandType)Enum.Parse(typeof(MapGenCommandType), match.Groups[3].Value);

                var parameters = match.Groups[4].Value;

                commandsInMap.Add(new MapGenCommand
                {
                    Vector = new Vector(x, y),
                    MapGenCommandType = command,
                    Parameters = parameters
                });

                lineIndex++;
            }

            return commandsInMap;
        }

        private static Dictionary<char, IEntity> GetCellsInMap(IEntityEngine entityEngineSystem, IPrototypeSystem prototypeSystem, string[] lines, ref int lineIndex)
        {
            var cellsInMap = new Dictionary<char, IEntity>();

            Match match;

            while ((match = Regex.Match(lines[lineIndex], "(.):(.*)")).Success)
            {
                char glyph = match.Groups[1].Value.First();
                string entityName = match.Groups[2].Value.Trim();
                IEntity entity = prototypeSystem.Create(entityName);

                cellsInMap.Add(glyph, entity);

                lineIndex++;
            }

            return cellsInMap;
        }
        private static string Extract(string input, string pattern)
        {
            return Regex.Match(input, pattern).Groups[1].Value;
        }

        public static Dictionary<string, char> GetMapGlyphs(Map map)
        {
            var mapGlyphs = new Dictionary<string, char>();

            var distinctCells = map.Cells.Values
                .Union(new List<IEntity> { map.DefaultCell })
                .Distinct()
                .OrderBy(e => e.EntityId);

            var usedGlyphs = $"{DEFAULT_CELL_GLYPH}{GLYPH_SEPARATOR}"; 

            foreach (var cell in distinctCells)
            {
                var appearance = cell.Get<Appearance>();
                char persistGlyph;

                if (usedGlyphs.Contains(appearance.Glyph))
                {
                    persistGlyph = GetUniqueGlyph(usedGlyphs);
                }
                else
                {
                    persistGlyph = appearance.Glyph;
                }

                usedGlyphs += persistGlyph;
                mapGlyphs.Add(cell.Name, persistGlyph);
            }

            return mapGlyphs;
        }

        private static char GetUniqueGlyph(string usedGlyphs)
        {
            const string glyphsToTry = ",.~@}{?><!£$%^&*()#[];1234567890|`QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";

            return glyphsToTry.First(glyph => !usedGlyphs.Contains(glyph));
        }
    }
}