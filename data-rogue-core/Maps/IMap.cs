﻿using System;
using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;

namespace data_rogue_core.Maps
{
    public interface IMap : ICloneable
    {
        int BottomY { get; }
        IEntity DefaultCell { get; set; }
        uint DefaultCellId { get; }
        int LeftX { get; }
        List<MapGenCommand> MapGenCommands { get; set; }
        MapKey MapKey { get; set; }
        Vector Origin { get; }
        int RightX { get; }
        int TopY { get; }

        Dictionary<MapCoordinate, IEntity> Cells { get; set; }
        HashSet<MapCoordinate> SeenCoordinates { get; set; }
        IEnumerable<Biome> Biomes { get; set; }
        double VaultWeight { get; set; }
        IEnumerable<MapKey> Vaults { get; set; }

        IEntity CellAt(int lookupX, int lookupY);
        IEntity CellAt(MapCoordinate coordinate);
        bool CellExists(int x, int y);
        bool CellExists(MapCoordinate coordinate);
        void ClearCell(MapCoordinate coordinate);
        List<MapCoordinate> FovFrom(IPositionSystem positionSystem, MapCoordinate mapCoordinate, int range, Func<Vector, bool> transparentTest = null);
        void RemoveCell(int x, int y);
        void RemoveCell(MapCoordinate mapCoordinate);
        void RemoveCellsInRange(int x1, int x2, int y1, int y2);
        void SetCell(int x, int y, IEntity cell);
        void SetCell(MapCoordinate coordinate, IEntity cell);
        void SetCellsInRange(int x1, int x2, int y1, int y2, IEntity cell);
        void SetSeen(int x, int y, bool seen = true);
        void SetSeen(MapCoordinate coordinate, bool seen = true);

        void InvalidateCache();
        void AddCommand(MapGenCommand entityCommand);

        void RemoveCommandsAt(int x, int y);
        void RemoveCommandsAt(MapCoordinate mapCoordinate);
        bool HasCommandAt(MapCoordinate mapCoordinate);


        IEnumerable<IEnumerable<MapCoordinate>> GetSections();
        void Spin(IRandom random);
        void Rotate(Matrix matrix);
        void PlaceSubMap(MapCoordinate position, IMap selectedVault);
        Size GetSize();
        bool IsLocationEmpty(MapCoordinate coordinate, Size vaultSize);
    }
}