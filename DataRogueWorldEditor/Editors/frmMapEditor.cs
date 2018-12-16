using data_rogue_core.Components;
using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;
using DataRogueWorldEditor.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Appearance = data_rogue_core.Components.Appearance;

namespace DataRogueWorldEditor.Editors
{
    public partial class frmMapEditor : WeifenLuo.WinFormsUI.Docking.DockContent, IEditor
    {
        private MapEditorTool _currentTool;

        private bool ShowMap { get; set; } = true;
        private bool ShowEnemies { get; set; } = true;
        private bool ShowItems { get; set; } = true;
        public Map Map { get; private set; }

        public int MapLeftX => Map.Cells.Min(c => c.Key.X);
        public int MapTopY => Map.Cells.Min(c => c.Key.Y);
        public int MapRightX => Map.Cells.Max(c => c.Key.X);
        public int MapBottomY => Map.Cells.Max(c => c.Key.Y);

        int MapDisplayHeight => (lblMap.Height - 20) / 13;
        int MapDisplayWidth => (lblMap.Width - 20) / 6;

        public int offsetX;
        public int offsetY;
        private IEntity _paintingWithCell;
        private MapCoordinate _selectedCoordinate;

        internal MapEditorTool SelectedTool
        {
            get => _currentTool;
            set
            {
                _currentTool = value;
                lblCurrentTool.Text = $"Current Tool: {_currentTool}";

                var toolControls = new[] { btnClearCell, btnSetCell, btnSelectCell };
                foreach (ToolStripButton control in toolControls)
                {
                    control.Checked = false;
                }

                if (_currentTool == MapEditorTool.SetCell) { btnSetCell.Checked = true; }
                if (_currentTool == MapEditorTool.ClearCell) { btnClearCell.Checked = true; }
                if (_currentTool == MapEditorTool.SelectCell) { btnSelectCell.Checked = true; }
            }
        }

        public IEntity PaintingWithCell { get => _paintingWithCell;
            set
            {
                _paintingWithCell = value;
                SetPaintingWithCellHilight(_paintingWithCell.Name);
            }
        }

        public BindingList<MapEditorGlyphBinding> GlyphEntities { get; set; }
        public IEntityEngineSystem EntityEngineSystem { get; }
        public MapCoordinate SelectedCoordinate
        {
            get => _selectedCoordinate;
            set
            {
                _selectedCoordinate = value;
                DisplayCellData(_selectedCoordinate);
            }
        }

        public frmMapEditor(string filename, IEntityEngineSystem entityEngineSystem)
        {
            InitializeComponent();

            var mapFileText = File.ReadAllText(filename);

            Map = MapSerializer.Deserialize(mapFileText, entityEngineSystem);

            SetData();

            SetOffsets();

            RenderMap();

            SelectedTool = MapEditorTool.SetCell;
            EntityEngineSystem = entityEngineSystem;
        }

        private void SetOffsets()
        {
            var mapHeight = MapBottomY - MapTopY;
            var mapWidth = MapRightX - MapLeftX;

            offsetX = mapWidth / 2 - MapDisplayWidth / 2;
            offsetY = mapHeight / 2 - MapDisplayHeight / 2;
        }

        private void SetData() //IEntityEngineSystem entityEngineSystem)
        {
            txtMapKey.Text = Map.MapKey.Key;

            txtDefaultCell.Text = Map.DefaultCell.Name;

            btnMap.Checked = ShowMap;
            btnEnemies.Checked = ShowEnemies;

            PaintingWithCell = Map.DefaultCell;

            var cellMapping = MapSerializer.GetMapGlyphs(Map);

            GlyphEntities = new BindingList<MapEditorGlyphBinding>(
                cellMapping.Select(m => new MapEditorGlyphBinding { Glyph = m.Value.ToString(), Entity = m.Key }).ToList());


            dgvGlyphs.DataSource = GlyphEntities;

        }

        private void btnMap_Click(object sender, EventArgs e)
        {
            ShowMap = !ShowMap;
            btnMap.Checked = ShowMap;
        }

        private void btnEnemies_Click(object sender, EventArgs e)
        {
            ShowEnemies = !ShowEnemies;
            btnEnemies.Checked = ShowEnemies;
        }

        private void btnItems_Click(object sender, EventArgs e)
        {
            ShowItems = !ShowItems;
            //btnItems.Checked = ShowItems;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void SaveAs()
        {
            throw new NotImplementedException();
        }

        private void RenderMap()
        {
            var stringBuilder = new StringBuilder();

            int height = (lblMap.Height - 20) / 13;
            int width = (lblMap.Width - 20) / 6;

            var leftX = offsetX - 1;
            var topY = offsetY - 1;

            for (int y = topY; y <= topY + height; y++)
            {
                for (int x = leftX; x <= leftX + width; x++)
                {
                    if (Map.CellExists(x, y))
                    {
                        var cellId = Map.CellAt(x, y).Name;
                        var glyph = GlyphEntities.Single(g => g.Entity == cellId);
                        stringBuilder.Append(glyph.Glyph);
                    }
                    else
                    {
                        stringBuilder.Append(" ");
                    }
                }
                stringBuilder.Append("\n");
            }

            var mapText = stringBuilder.ToString();

            lblMap.Text = mapText;

            DisplayCellData(SelectedCoordinate);
        }

        private void ApplyTool(MapEditorTool tool, MapCoordinate coordinate, MouseEventType mouseEventType, MouseButtons buttons)
        {
            bool isClicking = (mouseEventType == MouseEventType.Click || mouseEventType == MouseEventType.MouseMove ) && buttons == MouseButtons.Left;

            switch (tool)
            {
                case MapEditorTool.SetCell:
                    
                    if (isClicking)
                    {
                        Map.SetCell(coordinate, PaintingWithCell);
                    }
                    break;
                case MapEditorTool.ClearCell:
                    if (isClicking)
                    {
                        Map.ClearCell(coordinate);
                    }
                    break;
                case MapEditorTool.SelectCell:
                    if (isClicking)
                    {
                        SelectMapCell(coordinate);
                    }
                    break;
            }

            RenderMap();
        }

        private void SelectMapCell(MapCoordinate coordinate)
        {
            SelectedCoordinate = coordinate;
        }

        private void btnSetCell_Click(object sender, EventArgs e)
        {
            SelectedTool = MapEditorTool.SetCell;
        }

        private void btnClearCell_Click(object sender, EventArgs e)
        {
            SelectedTool = MapEditorTool.ClearCell;
        }

        private void dgvGlyphs_DoubleClickCell(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridView = (sender as DataGridView);
            var cellName = dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();

            var entity = EntityEngineSystem.GetEntityWithName(cellName);

            PaintingWithCell = entity;
        }

        private void SetPaintingWithCellHilight(string cellName)
        {
            foreach (DataGridViewRow row in dgvGlyphs.Rows)
            {
                if (row.Cells[1].Value?.ToString() == cellName)
                {
                    ColorRow(row, Color.CornflowerBlue);
                }
                else
                {
                    ColorRow(row, Color.White);
                }
            }
        }

        private static void ColorRow(DataGridViewRow row, Color color)
        {
            foreach (DataGridViewCell cell in row.Cells)
            {
                cell.Style.BackColor = color;
            }
        }

        private void lblMap_MouseClick(object sender, MouseEventArgs e)
        {
            ApplyToolAtCoordinates(e, MouseEventType.Click);
        }

        private void lblMap_MouseMove(object sender, MouseEventArgs e)
        {
            ApplyToolAtCoordinates(e, MouseEventType.MouseMove);
        }

        private void ApplyToolAtCoordinates(MouseEventArgs e, MouseEventType eventType)
        {
            int cellX = ((e.X - 8) / 6) - 1 + offsetX;
            int cellY = ((e.Y - 6) / 13) - 1 + offsetY;

            ApplyTool(SelectedTool, new MapCoordinate(Map.MapKey, cellX, cellY), eventType, e.Button);
        }

        private void btnScrollLeft_Click(object sender, EventArgs e)
        {
            offsetX--;
            RenderMap();
        }

        private void btnScrollRight_Click(object sender, EventArgs e)
        {
            offsetX++;
            RenderMap();
        }

        private void btnScrollUp_Click(object sender, EventArgs e)
        {
            offsetY--;
            RenderMap();
        }

        private void btnScrollDown_Click(object sender, EventArgs e)
        {
            offsetY++;
            RenderMap();
        }

        private void btnSelectCell_Click(object sender, EventArgs e)
        {
            SelectedTool = MapEditorTool.SelectCell;
        }
        
        private void DisplayCellData(MapCoordinate coordinate)
        {
            if (coordinate == null)
            {
                return;
            }

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"X: {coordinate.X} Y: {coordinate.Y}");

            if (Map.CellExists(coordinate.X, coordinate.Y))
            {
                var cell = Map.Cells[coordinate];

                var physical = cell.Get<Physical>();
                var appearance = cell.Get<Appearance>();

                stringBuilder.AppendLine($"{cell.Name}");
                stringBuilder.AppendLine($"Passable: {physical.Passable}");
                stringBuilder.AppendLine($"Transparent: {physical.Transparent}");
                stringBuilder.AppendLine($"Glyph: {appearance.Glyph}");
                stringBuilder.AppendLine($"Color: {ColorTranslator.ToHtml(appearance.Color)}");
                stringBuilder.AppendLine($"ZOrder: {appearance.ZOrder}");
            }
            else
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"(no cell set, default will be used)");
            }

            lblSelectedCell.Text = stringBuilder.ToString();
        }
    }

    public enum MouseEventType
    {
        Click,
        MouseDown,
        MouseUp,
        MouseMove
    }

    public class MapEditorGlyphBinding : INotifyPropertyChanged
    {
        private char _glyph;
        private string _entity;

        public string Glyph {
            get => _glyph.ToString();
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Glyph)));
                _glyph = value.First();
            }
        }
        public string Entity
        {
            get => _entity;
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Entity)));
                _entity = value;
            }
        }

        public MapEditorGlyphBinding()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    enum MapEditorTool
    {
        SetCell,
        ClearCell,
        SelectCell
    }
}
