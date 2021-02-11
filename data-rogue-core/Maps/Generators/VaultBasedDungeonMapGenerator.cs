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

        public VaultBasedDungeonMapGenerator(ISystemContainer systemContainer, string floorCell, string wallCell, int numberOfVaults, int maxTries, IEntity branch)
        {
            _systemContainer = systemContainer;
            _floorCell = systemContainer.PrototypeSystem.Get(floorCell);
            _wallCell = systemContainer.PrototypeSystem.Get(wallCell);
            _numberOfVaults = numberOfVaults;
            _branch = branch;

            _lineDrawing = new BresenhamLineDrawingAlgorithm();
            _tunnelPathfinding = new AStarPathfindingAlgorithm(false, PassableToTunneling);
        }

        public IMap Generate(string mapName, IRandom random)
        {
            IMap map = new Map(mapName, _wallCell);
            var vaultPlacements = new List<VaultPlacement>();

            List<IMap> validVaults = GetValidVaults();

            PlaceVaults(random, map, vaultPlacements, validVaults);

            ConnectAsManyVaultsAsPossible(random, map, vaultPlacements);

            ConnectDisconnectedSections(random, map, vaultPlacements);

            map.Vaults = vaultPlacements.Select(v => v.Vault.MapKey);

            return map;
        }

        private void PlaceVaults(IRandom random, IMap map, List<VaultPlacement> vaultPlacements, List<IMap> validVaults)
        {
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

            while (mapSections.Count() > 1 && attempts++ < 1000)
            {
                var firstSection = random.PickOne(mapSections);
                var secondSection = random.PickOne(mapSections.Except(new[] { firstSection }).ToList());

                TryConnectSections(map, random, firstSection, secondSection, vaultPlacements);

                mapSections = map.GetSections().ToList();
            }
        }

        private void TryConnectSections(IMap map, IRandom random, IEnumerable<MapCoordinate> firstSection, IEnumerable<MapCoordinate> secondSection, List<VaultPlacement> vaultPlacements)
        {
            var connectionPoints = vaultPlacements.SelectMany(v => v.VaultConnectionPoints.Select(cp => cp.Coordinate));

            var firstSectionConnectionPoint = random.PickOne(firstSection.Intersect(connectionPoints).ToList());
            var secondSectionConnectionPoint = random.PickOne(secondSection.Intersect(connectionPoints).ToList());

            var path = _tunnelPathfinding.Path(map, firstSectionConnectionPoint, secondSectionConnectionPoint);

            if (path != null && path.Any())
            {
                CarveTunnel(map, path);
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
            var placedVault = selectedVault.Clone();
            placedVault.Spin(random);

            var position = FindPosition(map, selectedVault, random);

            if (position != null)
            {
                map.PlaceSubMap(position, selectedVault);

                return new VaultPlacement(placedVault, position);
            }

            return null;
        }

        private MapCoordinate FindPosition(IMap map, IMap selectedVault, IRandom random)
        {
            
            var vaultSize = selectedVault.GetSize();

            for (int i = 0; i <= _maxTries; i++)
            {
                var coordinate = PickRandomCoordinate(map, random, vaultSize, i);

                if (map.IsEmpty(coordinate, vaultSize))
                {
                    return coordinate;
                }
            }

            return null;
        }

        private MapCoordinate PickRandomCoordinate(IMap map, IRandom random, Size size, int i)
        {
            var key = map.MapKey;
            var temperature = 1 + (double)i / 10;
            var randWidth = (int)(size.Width * temperature);
            var randHeight = (int)(size.Height * temperature);

            return new MapCoordinate(key, random.Between(-randWidth, randWidth), random.Between(-randHeight, randHeight));
        }

        private IMap SelectVault(List<IMap> validVaults, IRandom random)
        {
            return random.PickOne<IMap>(validVaults);
        }

        private bool PassableToTunneling(IMap map, AStarPathfindingAlgorithm.AStarLocation location)
        {
            var coordinate = new MapCoordinate(map.MapKey, location.X, location.Y);
            return !map.CellExists(coordinate);
        }
    }
}
