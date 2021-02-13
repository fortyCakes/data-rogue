using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.Maps.Generators
{
    /// <summary>
    /// A vault-based map generator.
    /// 
    /// Build around the origin (0,0).
    ///
    /// Select a random vault (matching the brach biome) and rotate it randomly.
    /// We pick a random point on the map and check if the vault fits there.
    /// A vault fits if the map has no defined tiles in the vault's bounding box.
    /// If it doesn't fit, we try a few times to find somewhere that it does,
    /// trying higher "temperature" each time. (Eventually give up.)
    /// 
    /// If we find a fit, place the vault on the map and repeat until we have
    /// enough vaults placed.
    /// 
    /// Then iterate through the vault connections and connect them with
    /// corridors of floor tiles.
    /// 
    /// </summary>
    public class VaultBasedDungeonMapGenerator : IMapGenerator
    {
        private ISystemContainer _systemContainer;
        private IEntity _floorCell;
        private IEntity _wallCell;
        private readonly int _numberOfVaults;
        private readonly IEntity _branch;
        private readonly ILineDrawingAlgorithm _lineDrawing;
        private readonly IPathfindingAlgorithm _tunnelPathfinding;
        private readonly int _maxTries;
        private readonly double _vaultChance;

        public VaultBasedDungeonMapGenerator(ISystemContainer systemContainer, string floorCell, string wallCell, int numberOfVaults, int maxTries, double vaultChance, IEntity branch)
        {
            _systemContainer = systemContainer;
            _floorCell = systemContainer.PrototypeSystem.Get(floorCell);
            _wallCell = systemContainer.PrototypeSystem.Get(wallCell);
            _numberOfVaults = numberOfVaults;
            _branch = branch;
            _maxTries = maxTries;

            _lineDrawing = new BresenhamLineDrawingAlgorithm();
            _tunnelPathfinding = new AStarPathfindingAlgorithm(false, PassableToTunneling, 500);
            _vaultChance = vaultChance;
        }

        public IMap Generate(string mapName, IRandom random, IProgress<string> progress)
        {
            progress.Report("Generating branch... Starting generation");

            IMap map = new Map(mapName, _wallCell);
            var vaultPlacements = new List<VaultPlacement>();

            progress.Report("Generating branch... Getting vault list");
            List<IMap> validVaults = GetValidVaults();

            progress.Report("Generating branch... Placing vaults");
            PlaceVaults(random, map, vaultPlacements, validVaults);

            progress.Report("Generating branch... Adding initial vault connections");
            ConnectAsManyVaultsAsPossible(random, map, vaultPlacements);

            progress.Report("Generating branch... Connecting disjoint sections");
            ConnectDisconnectedSections(random, map, vaultPlacements);

            progress.Report("Generating branch... Connecting unused doors");
            ConnectUnusedConnectionPoints(random, map, vaultPlacements, progress);

            progress.Report("Generating branch... Removing unused doors");
            PlaceWallOnUnusedConnectionPoints(map, vaultPlacements);

            map.Vaults = vaultPlacements.Select(v => v.Vault.MapKey);

            progress.Report("Generating branch... Done!");
            return map;
        }

        private void PlaceWallOnUnusedConnectionPoints(IMap map, List<VaultPlacement> vaultPlacements)
        {
            var connectionPoints = vaultPlacements.SelectMany(v => v.VaultConnectionPoints.Select(cp => cp.Coordinate)).ToList();

            foreach(var connectionPoint in connectionPoints)
            {
                map.SetCell(connectionPoint, _wallCell);
            }
        }

        private void PlaceVaults(IRandom random, IMap map, List<VaultPlacement> vaultPlacements, List<IMap> validVaults)
        {
            SetGenerationStatus("Placing vaults");
            for (int i = 0; i <= _numberOfVaults; i++)
            {
                var selectedVault = SelectVault(validVaults, random);

                var vaultPlacement = PlaceVault(map, selectedVault, random);

                if (vaultPlacement != null)
                {
                    vaultPlacements.Add(vaultPlacement);
                }
            }
        }

        private void ConnectAsManyVaultsAsPossible(IRandom random, IMap map, List<VaultPlacement> vaultPlacements)
        {
            var possibleVaultConnections = GetPossibleVaultConnections(map, vaultPlacements).ToList();

            while (possibleVaultConnections.Any())
            {
                var vaultConnection = SelectVaultConnection(possibleVaultConnections, random);

                AddVaultConnectionToMap(map, vaultConnection);

                vaultConnection.RemovePossibleConnectionPoints();

                possibleVaultConnections = GetPossibleVaultConnections(map, vaultPlacements).ToList();
            }
        }

        private void ConnectDisconnectedSections(IRandom random, IMap map, List<VaultPlacement> vaultPlacements)
        {
            var mapSections = map.GetSections().ToList();
            var attempts = 0;

            while (mapSections.Count() > 1 && attempts++ < _maxTries)
            {
                var firstSection = random.PickOne(mapSections);
                var secondSection = random.PickOne(mapSections.Except(new[] { firstSection }).ToList());

                var connected = TryConnectSections(map, random, firstSection, secondSection, vaultPlacements);

                if (connected)
                {
                    mapSections.Remove(firstSection);
                    mapSections.Remove(secondSection);

                    var combinedSection = firstSection.ToList();
                    combinedSection.AddRange(secondSection);

                    mapSections.Add(combinedSection);
                }
            }
        }

        private void ConnectUnusedConnectionPoints(IRandom random, IMap map, List<VaultPlacement> vaultPlacements, IProgress<string> progress)
        {
            var connectionPoints = vaultPlacements.SelectMany(v => v.VaultConnectionPoints.Select(cp => cp.Coordinate)).ToList();

            int i = 0;

            while(connectionPoints.Count() > 1 && i++ < _maxTries)
            {
                progress.Report($"Generating branch... Connecting unused doors ({i}/{_maxTries})");

                var firstPoint = random.PickOne(connectionPoints);
                var secondPoint = random.PickOne(connectionPoints.Except(new[] { firstPoint }).ToList());

                TryCarveTunnel(map, firstPoint, secondPoint, vaultPlacements);
            }
        }

        private bool TryConnectSections(IMap map, IRandom random, IEnumerable<MapCoordinate> firstSection, IEnumerable<MapCoordinate> secondSection, List<VaultPlacement> vaultPlacements)
        {
            var connectionPoints = vaultPlacements.SelectMany(v => v.VaultConnectionPoints.Select(cp => cp.Coordinate));

            var firstSectionValidConnections = firstSection.Intersect(connectionPoints).ToList();
            var secondSectionValidConnections = secondSection.Intersect(connectionPoints).ToList();

            if (firstSectionValidConnections.Any() && secondSectionValidConnections.Any())
            {
                var firstSectionConnectionPoint = random.PickOne(firstSectionValidConnections);
                var secondSectionConnectionPoint = random.PickOne(secondSectionValidConnections);

                return TryCarveTunnel(map, firstSectionConnectionPoint, secondSectionConnectionPoint, vaultPlacements);
            }

            return false;
        }

        private bool TryCarveTunnel(IMap map, MapCoordinate firstPoint, MapCoordinate secondPoint, List<VaultPlacement> vaultPlacements)
        {
            var path = _tunnelPathfinding.Path(map, firstPoint, secondPoint);

            if (path != null && path.Any())
            {
                CarveTunnel(map, path);

                RemoveConnectionPoints(vaultPlacements, firstPoint, secondPoint);

                return true;
            }
            else
            {
                return false;
            }
        }

        private void RemoveConnectionPoints(List<VaultPlacement> vaultPlacements, MapCoordinate firstPoint, MapCoordinate secondPoint)
        {
            foreach(var vault in vaultPlacements)
            {
                var toRemove = vault.VaultConnectionPoints.FirstOrDefault(f => f.Coordinate == firstPoint);
                if (toRemove != null)
                {
                    vault.VaultConnectionPoints.Remove(toRemove);
                }
                toRemove = vault.VaultConnectionPoints.FirstOrDefault(f => f.Coordinate == secondPoint);
                if (toRemove != null)
                {
                    vault.VaultConnectionPoints.Remove(toRemove);
                }
            }
        }

        private List<IMap> GetValidVaults()
        {
            var biomes = _branch.Components.OfType<Biome>();
            var validVaults = _systemContainer.MapSystem.Vaults
                .Where(v => BiomesMatch(v, biomes))
                .ToList();

            if (!validVaults.Any())
            {
                throw new ApplicationException($"No valid vaults found for branch {_branch.Name}");
            }

            return validVaults;
        }

        private static bool BiomesMatch(IMap vault, IEnumerable<Biome> branchBiomes)
        {
            return !vault.Biomes.Any() || vault.Biomes.Any(b => branchBiomes.Any(biome => biome.Name == b.Name));
        }

        private void AddVaultConnectionToMap(IMap map, VaultPossibleConnection vaultConnection)
        {
            if (vaultConnection.IsRightAngle)
            {
                var corner = vaultConnection.GetCorner();

                CarveCornerTunnel(map, vaultConnection.First.Coordinate, corner);
                CarveCornerTunnel(map, vaultConnection.Second.Coordinate, corner);
            }
            else // We're straight across.
            {
                var (firstCorner, secondCorner) = vaultConnection.GetCorners();

                CarveCornerTunnel(map, vaultConnection.First.Coordinate, firstCorner);
                CarveCornerTunnel(map, firstCorner, secondCorner);
                CarveCornerTunnel(map, secondCorner, vaultConnection.Second.Coordinate);
            }
        }

        private void CarveCornerTunnel(IMap map, MapCoordinate coordinate, MapCoordinate corner)
        {
            var points = _lineDrawing.DrawLine(coordinate, corner);

            CarveTunnel(map, points);
        }

        private void CarveTunnel(IMap map, IEnumerable<MapCoordinate> path)
        {
            foreach (var point in path)
            {
                map.SetCell(point, _floorCell);
            }
        }

        private VaultPossibleConnection SelectVaultConnection(IList<VaultPossibleConnection> possibleVaultConnections, IRandom random)
        {
            return random.PickOne(possibleVaultConnections);
        }

        private IEnumerable<VaultPossibleConnection> GetPossibleVaultConnections(IMap map, List<VaultPlacement> vaultPlacements)
        {
            foreach(var vault in vaultPlacements)
            {
                foreach(var otherVault in vaultPlacements.Except(new[] { vault }))
                {
                    foreach (var connectFrom in vault.VaultConnectionPoints)
                    {
                        foreach(var connectTo in otherVault.VaultConnectionPoints)
                        {
                            var pointsBetween = _lineDrawing.DrawLine(connectFrom.Coordinate, connectTo.Coordinate).Except(new[] { connectFrom.Coordinate, connectTo.Coordinate });

                            if (!pointsBetween.Any(p => map.CellExists(p)))
                            {
                                var connection = new VaultPossibleConnection
                                {
                                    FirstVault = vault,
                                    SecondVault = otherVault,
                                    First = connectFrom,
                                    Second = connectTo
                                };

                                yield return connection;
                            }
                        }
                    }
                }
            }
        }

        private VaultPlacement PlaceVault(IMap map, IMap selectedVault, IRandom random)
        {
            var placedVault = (IMap)selectedVault.Clone();
            placedVault.Spin(random);

            var position = FindPosition(map, placedVault, random);

            if (position != null)
            {
                map.PlaceSubMap(position, placedVault);

                var placement = new VaultPlacement(placedVault, position);

                Console.WriteLine($"Placed {placedVault.MapKey} on {map.MapKey}, connections are:{string.Concat(placement.VaultConnectionPoints.Select(cp => $"({cp.Coordinate.X},{cp.Coordinate.Y})").ToArray()) }");

                return placement;
            }

            return null;
        }

        private MapCoordinate FindPosition(IMap map, IMap selectedVault, IRandom random)
        {
            
            var vaultSize = selectedVault.GetSize();

            for (int i = 0; i <= _maxTries; i++)
            {
                var coordinate = PickRandomCoordinate(map, random, i);

                // We check that the area around where the vault will be is empty as well. This lets us place corridors etc in it.
                Size largerSize = new Size(vaultSize.Width + 2, vaultSize.Height + 2);
                if (map.IsLocationEmpty(coordinate - new Vector(1,1), largerSize))
                {
                    return coordinate;
                }
            }

            return null;
        }

        private MapCoordinate PickRandomCoordinate(IMap map, IRandom random, int i)
        {
            var size = map.GetSize();
            var key = map.MapKey;
            var temperature = (double)i / 20;
            var randWidth = (int)(size.Width * temperature);
            var randHeight = (int)(size.Height * temperature);

            return new MapCoordinate(key, random.Between(-randWidth, randWidth), random.Between(-randHeight, randHeight));
        }

        private IMap SelectVault(List<IMap> validVaults, IRandom random)
        {
            if (random.PercentageChance(_vaultChance))
            {
                return random.PickOne(validVaults);
            }
            else
            {
                return CreateBlankRoom(random);
            }
        }

        private IMap CreateBlankRoom(IRandom random)
        {
            var map = new Map("Blank Room", _wallCell);

            var xSize = random.Between(3, 12);
            var ySize = random.Between(3, 12);

            for (int x = 0; x <= xSize; x++)
                for (int y = 0; y <= ySize; y++)
                {
                    map.SetCell(new MapCoordinate(map.MapKey, x, y), _floorCell);
                    if (IsOnWall(xSize, ySize, x, y) && random.PercentageChance(0.1))
                    {
                        map.MapGenCommands.Add(new MapGenCommand { MapGenCommandType = MapGenCommandType.Entity, Parameters = "Props:VaultConnection", Vector = new Vector(x, y) });
                    }
                }

            return map;
        }

        private static bool IsOnWall(int xSize, int ySize, int x, int y)
        {
            return (x == 0 || x == xSize) ^ (y == 0 || y == ySize);
        }

        private bool PassableToTunneling(IMap map, AStarPathfindingAlgorithm.AStarLocation location)
        {
            var coordinate = new MapCoordinate(map.MapKey, location.X, location.Y);
            return !map.CellExists(coordinate);
        }

        private void SetGenerationStatus(string status)
        {
        }
    }
}
