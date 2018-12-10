using System;
using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntitySystem;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

            var leftX = map.Cells.Min(c => c.Key.X);
            var rightX = map.Cells.Max(c => c.Key.X);
            var topY = map.Cells.Min(c => c.Key.Y);
            var bottomY = map.Cells.Max(c => c.Key.Y);

            stringBuilder.AppendLine($"{leftX},{topY}");

            var glyphs = GetMapGlyphs(map);

            foreach(var glyph in glyphs)
            {
                stringBuilder.AppendLine($"{glyph.Value}:{glyph.Key}");
            }

            for (int y = topY; y <= bottomY; y++)
            {
                for (int x = leftX; x <= rightX; x++)
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

            return stringBuilder.ToString();
        }
        
        public static Map Deserialize(string savedMap, IEntityEngineSystem entityEngineSystem, string mapNameOverride = null)
        {
            var lines = savedMap.Split('\n');

            var mapName = mapNameOverride ?? Extract(lines[0], "Map:\"(.*)\"");
            var defaultCellId = Extract(lines[1], "default:(.*)").Trim();
            var coordinateMatch = Regex.Match(lines[2], "(-?[0-9]),(-?[0-9])");
            var leftX = int.Parse(coordinateMatch.Groups[1].Value);
            var topY = int.Parse(coordinateMatch.Groups[2].Value);

            IEntity defaultCell = entityEngineSystem.GetEntitiesWithName(defaultCellId).Single();

            var lineIndex = 3;

            var glyphDictionary = GetCellsInMap(entityEngineSystem, lines, ref lineIndex);

            Map deserialisedMap = new Map(mapName, defaultCell);

            for (int j = 0; j+lineIndex < lines.Length; j++)
            {
                var line = lines[lineIndex+j].Trim();

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

            return deserialisedMap;
        }

        private static Dictionary<char, IEntity> GetCellsInMap(IEntityEngineSystem entityEngineSystem, string[] lines, ref int lineIndex)
        {
            var cellsInMap = new Dictionary<char, IEntity>();

            

            Match match;

            while ((match = Regex.Match(lines[lineIndex], "(.):(.*)")).Success)
            {
                char glyph = match.Groups[1].Value.First();
                string entityName = match.Groups[2].Value.Trim();
                IEntity entity = entityEngineSystem.GetEntitiesWithName(entityName).Single();

                cellsInMap.Add(glyph, entity);

                lineIndex++;
            }

            return cellsInMap;
        }

        private static string Extract(string input, string pattern)
        {
            return Regex.Match(input, pattern).Groups[1].Value;
        }

        private static Dictionary<string, char> GetMapGlyphs(Map map)
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