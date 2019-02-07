using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Appearance = data_rogue_core.Components.Appearance;

namespace DataRogueWorldEditor.Editors
{
    public partial class frmMapEditor : WeifenLuo.WinFormsUI.Docking.DockContent, IEditor
    {
        public Map Map { get; private set; }

        public bool IsDirty
        {
            get => _isDirty;
            set {
                _isDirty = value;
                SetTabTitle();
            }
        }


        private string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                SetTabTitle();
            }
        }

        private MapEditorTool _currentTool;

        private bool ShowMap { get; set; } = true;
        private bool ShowEntities { get; set; } = true;
        private bool ShowItems { get; set; } = true;

        int MapDisplayHeight => (lblMap.Height - 20) / 13;
        int MapDisplayWidth => (lblMap.Width - 20) / 6;

        public int offsetX;
        public int offsetY;
        private IEntity _paintingWithCell;
        private MapCoordinate _selectedCoordinate;
        private string _fileName;
        private bool _isDirty;
        private MapCoordinate _oldMapGenCoordinate;
        private ISystemContainer SystemContainer;

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

        public IEntity PaintingWithCell
        {
            get => _paintingWithCell;
            set
            {
                _paintingWithCell = value;
                SetPaintingWithCellHilight(_paintingWithCell.Name);
            }
        }

        public BindingList<MapEditorGlyphBinding> GlyphEntities { get; set; }
        public BindingList<MapGenCommand> MapGenCommands { get; private set; }
        public IEntityEngine EntityEngineSystem { get; }
        public IPrototypeSystem PrototypeSystem { get; }

        public MapCoordinate SelectedCoordinate
        {
            get => _selectedCoordinate;
            set
            {
                _selectedCoordinate = value;
                DisplayCellData(_selectedCoordinate);
            }
        }

        public frmMapEditor(ISystemContainer systemContainer, string filename)
        {
            SystemContainer = systemContainer;

            InitializeComponent();

            FileName = filename;

            if (string.IsNullOrEmpty(FileName))
            {
                Map = new Map("new map key", SystemContainer.PrototypeSystem.Create("Cell:Wall"));
            }
            else
            {
                var mapFileText = File.ReadAllText(FileName);

                Map = MapSerializer.Deserialize(SystemContainer, mapFileText);
            }

            dgvCommands.AutoGenerateColumns = false;

            colCommandTypes.DataSource = Enum.GetValues(typeof(MapGenCommandType));
            colCommandTypes.ValueType = typeof(MapGenCommandType);
            colCommandTypes.DataPropertyName = "MapGenCommandType";

            SetData();

            SetOffsets();

            RenderMap();

            SelectedTool = MapEditorTool.SetCell;
            EntityEngineSystem = SystemContainer.EntityEngine;
            PrototypeSystem = SystemContainer.PrototypeSystem;
        }

        private void SetOffsets()
        {
            var mapHeight = Map.BottomY - Map.TopY;
            var mapWidth = Map.RightX - Map.LeftX;

            offsetX = mapWidth / 2 - MapDisplayWidth / 2;
            offsetY = mapHeight / 2 - MapDisplayHeight / 2;
        }

        private void SetData() //IEntityEngineSystem entityEngineSystem)
        {
            SetTabTitle();

            txtMapKey.Text = Map.MapKey.Key;

            txtDefaultCell.Text = Map.DefaultCell.Name;

            btnMap.Checked = ShowMap;
            btnEntities.Checked = ShowEntities;

            var cellMapping = MapSerializer.GetMapGlyphs(Map);

            GlyphEntities = new BindingList<MapEditorGlyphBinding>(
                cellMapping.Select(m => new MapEditorGlyphBinding { Glyph = m.Value.ToString(), Entity = m.Key }).ToList());

            dgvGlyphs.DataSource = GlyphEntities;

            PaintingWithCell = Map.DefaultCell;
        }

        private void SetTabTitle()
        {
            this.Text = Path.GetFileName(FileName) + (IsDirty ? "*" : "");
        }

        private void btnMap_Click(object sender, EventArgs e)
        {
            ShowMap = !ShowMap;
            btnMap.Checked = ShowMap;
            RenderMap();
        }

        private void btnEnemies_Click(object sender, EventArgs e)
        {
            ShowEntities = !ShowEntities;
            btnEntities.Checked = ShowEntities;
            RenderMap();
        }

        private void btnItems_Click(object sender, EventArgs e)
        {
            ShowItems = !ShowItems;
            //btnItems.Checked = ShowItems;
        }

        public void Save()
        {
            DoSave(false);
        }

        public void SaveAs()
        {
            DoSave(true);
        }

        private void DoSave(bool forceSaveDialog)
        {
            UpdateMapGenCommandBinding(_oldMapGenCoordinate);

            Map.MapKey = new MapKey(txtMapKey.Text);

            Map.DefaultCell = PrototypeSystem.Create(txtDefaultCell.Text);

            var serialisedMap = MapSerializer.Serialize(Map);

            if (FileName == null || forceSaveDialog)
            {
                var ok = DisplaySaveFileDialog();
                if (!ok)
                {
                    return;
                }
            }

            File.WriteAllText(FileName, serialisedMap);

            IsDirty = false;
        }

        private bool DisplaySaveFileDialog()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save Map",
                Filter = "data-rogue map file|*.map"
            };

            var result = dialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
            {
                FileName = dialog.FileName;
                return true;
            }

            return false;
        }

        private void RenderMap()
        {
            var mapStringBuilder = new StringBuilder();
            var entityStringBuilder = new StringBuilder();

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
                        mapStringBuilder.Append(glyph.Glyph);
                    }
                    else
                    {
                        mapStringBuilder.Append(" ");
                    }

                    if (Map.MapGenCommands.Any(c => c.Vector == new Vector(x+3,y)))
                    {
                        entityStringBuilder.Append("╬");
                    }
                    else
                    {
                        entityStringBuilder.Append(" ");
                    }
                }
                mapStringBuilder.Append("\n");
                entityStringBuilder.Append("\n");
            }

            var mapText = ShowMap ? mapStringBuilder.ToString() : "";
            var entityText = ShowEntities ? entityStringBuilder.ToString() : "";

            lblMap.Text = mapText;
            lblMapEntities.Text = entityText;

            DisplayCellData(SelectedCoordinate);

            lblMapEntities.Refresh();
        }

        private void ApplyTool(MapEditorTool tool, MapCoordinate coordinate, MouseEventType mouseEventType, MouseButtons buttons)
        {
            bool isClicking = (mouseEventType == MouseEventType.Click || mouseEventType == MouseEventType.MouseMove) && buttons == MouseButtons.Left;

            switch (tool)
            {
                case MapEditorTool.SetCell:

                    if (isClicking)
                    {
                        Map.SetCell(coordinate, PaintingWithCell);
                        IsDirty = true;
                    }
                    break;
                case MapEditorTool.ClearCell:
                    if (isClicking)
                    {
                        Map.ClearCell(coordinate);
                        IsDirty = true;
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
            var cellName = dataGridView.Rows[e.RowIndex].Cells[1].Value?.ToString();

            if (string.IsNullOrEmpty(cellName)) return;

            var entity = PrototypeSystem.Create(cellName);

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
            ApplyToolAtCoordinates(e.X, e.Y, e.Button, MouseEventType.Click);
        }

        private void lblMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ApplyToolAtCoordinates(e.X, e.Y, e.Button, MouseEventType.MouseMove);
            }
        }

        private void ApplyToolAtCoordinates(int x, int y, MouseButtons buttons, MouseEventType eventType)
        {
            int cellX = ((x - 3) / 6) - 1 + offsetX;
            int cellY = ((y - 1) / 13) - 1 + offsetY;

            ApplyTool(SelectedTool, new MapCoordinate(Map.MapKey, cellX, cellY), eventType, buttons);
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

            if (coordinate != _oldMapGenCoordinate)
            {
                UpdateMapGenCommandBinding(coordinate);
            }

            lblSelectedCell.Text = stringBuilder.ToString();
        }

        private void UpdateMapGenCommandBinding(MapCoordinate newCoordinate)
        {
            if (_oldMapGenCoordinate != null)
            {
                Map.MapGenCommands = Map.MapGenCommands.Where(c => !(c.Vector.X == _oldMapGenCoordinate.X && c.Vector.Y == _oldMapGenCoordinate.Y)).ToList();
            }

            if (MapGenCommands?.Any() == true)
            {
                Map.MapGenCommands.AddRange(MapGenCommands.Where(c => c.MapGenCommandType != MapGenCommandType.Null).Select(c => MakeNewCommand(c, _oldMapGenCoordinate)));
            }

            MapGenCommands = new BindingList<MapGenCommand>(Map.MapGenCommands.Where(c => c.Vector.X == newCoordinate.X && c.Vector.Y == newCoordinate.Y).ToList());
            dgvCommands.DataSource = MapGenCommands;

            _oldMapGenCoordinate = newCoordinate;
        }

        private MapGenCommand MakeNewCommand(MapGenCommand c, MapCoordinate coordinate)
        {
            return new MapGenCommand { MapGenCommandType = c.MapGenCommandType, Parameters = c.Parameters, Vector = new Vector(coordinate.X, coordinate.Y) };
        }

        private void txtMapKey_TextChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        private void txtDefaultCell_TextChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        private void dgvGlyphs_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            IsDirty = true;
        }

        private void lblMap_Paint(object sender, PaintEventArgs e)
        {
            Label lbl = sender as Label;
            e.Graphics.Clear(lbl.BackColor);

            TextRenderer.DrawText(e.Graphics, lbl.Text, lbl.Font,
                lbl.ClientRectangle,
                lbl.ForeColor,
                lbl.BackColor, TextFormatFlags.Default);
        }

        private void lblMapEntities_Paint(object sender, PaintEventArgs e)
        {
            lblMapEntities.Refresh();
        }

        private void lblMapEntities_MouseClick(object sender, MouseEventArgs e)
        {
             ApplyToolAtCoordinates(e.X + lblMapEntities.Left, e.Y + lblMapEntities.Top, e.Button, MouseEventType.Click);
        }

        private void lblMapEntities_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ApplyToolAtCoordinates(e.X + lblMapEntities.Left, e.Y + lblMapEntities.Top, e.Button, MouseEventType.MouseMove);
            }
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
