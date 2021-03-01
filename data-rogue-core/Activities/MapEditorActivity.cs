using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.Controls.MapEditorTools;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Forms;
using data_rogue_core.Forms.StaticForms;
using data_rogue_core.IOSystems;
using data_rogue_core.IOSystems.BLTTiles;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using Form = data_rogue_core.Forms.Form;

namespace data_rogue_core.Activities
{
    public class MapEditorActivity: BaseActivity, IMapActivity
    {
        private IMap _map;

        private IMap Map {
            get => _map;
            set
            {
                _map = value;
                _mapName = _map.MapKey.Key;
                PrimaryCell = _systemContainer.PrototypeSystem.Get("Cell:Wall");
                SecondaryCell = _systemContainer.PrototypeSystem.Get("Cell:Empty");
                CameraPosition = new MapCoordinate(_map.MapKey, 0, 0);
            }
        }

        public string _mapName;
        private IEntity _primaryCell;
        private IEntity _secondaryCell;
        private MapCoordinate _mousePosition;
        private readonly ISystemContainer _systemContainer;

        public MapEditorActivity(Rectangle position, Padding padding, ISystemContainer systemContainer) : base(position, padding)
        {
            _systemContainer = systemContainer;
            NewMap();
        }

        public override void InitialiseControls()
        {
            Controls.Add(new MapControl { Position = Position });
            Controls.Add(new MapEditorHighlightControl { Position = Position });
            Controls.Add(new MapEditorToolbarControl { Position = Position });
            Controls.Add(new MapEditorCellPickerControl { Position = Position, HorizontalAlignment = HorizontalAlignment.Right });
            Controls.Add(new MessageLogControl { Position = Position, NumberOfMessages = 10, VerticalAlignment = VerticalAlignment.Bottom });
        }

        public static IEnumerable<IMapEditorTool> GetToolbarControls() => new List<IMapEditorTool>
        {
            new PenTool(),
            new EraserTool(),
            new LineTool(),
            new SquareTool(),
            new FilledSquareTool(),
            new CircleTool(),
            new FilledCircleTool(),
            new FillTool(),
            new EntityTool(),
            new TinyEraserTool()
        };

        public List<MapCoordinate> GetHighlightedCells()
        {
            return CurrentTool.GetTargetedCoordinates(Map, _mousePosition).ToList();
        }

        public List<MapCoordinate> GetSecondaryCells()
        {
            return CurrentTool.GetInternalCoordinates(Map, _mousePosition).ToList();
        }

        public override ActivityType Type => ActivityType.MapEditor;
        public override bool RendersEntireSpace => true;
        
        public override bool AcceptsInput => true;

        public IMapEditorTool CurrentTool { get; set; } = new PenTool();

        public IEntity PrimaryCell {
            get { return _primaryCell; }
            set {
                _primaryCell = value;
                _systemContainer?.MessageSystem.Write("Current cell set to " + (_primaryCell == null ? "null" : _primaryCell.DescriptionName));
            }
        }

        public void NewMap()
        {
            _systemContainer.MapSystem.Initialise();
            Map = new Map("NewMap", _systemContainer.PrototypeSystem.Get("Cell:Void"));
            _systemContainer.MapSystem.MapCollection.Add(Map.MapKey, Map);
        }

        public IEntity SecondaryCell
        {
            get { return _secondaryCell; }
            set
            {
                _secondaryCell = value;
                _systemContainer?.MessageSystem.Write("Secondary cell set to " + (_secondaryCell == null ? "null" : _secondaryCell.DescriptionName));
            }
        }

        public IEntity DefaultCell => _map.DefaultCell;

        public void ApplyTool(MapCoordinate mapCoordinate, IEntity cell, IEntity alternateCell)
        {
            CurrentTool.Apply(_map, mapCoordinate, cell, alternateCell, _systemContainer);
        }

        public MapCoordinate CameraPosition { get; set; }

        public void SetTool(string toolName)
        {
            CurrentTool = GetToolbarControls().SingleOrDefault(s => s.Entity.DescriptionName == toolName);
        }

        public override void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            if (mouse.MouseActive)
            {
                _mousePosition = systemContainer.RendererSystem.Renderer.GetMapEditorMapCoordinateFromMousePosition(systemContainer.RendererSystem.CameraPosition, mouse.X, mouse.Y);
            }
            base.HandleMouse(systemContainer, mouse);
        }
        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            // Keyboard interaction handled through keybinding -> Actions
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            if (action != null)
            {
                if (action.Action == ActionType.ChangeMapEditorCell)
                {
                    ShowChangePrimaryCellDialogue();
                }

                if (action.Action == ActionType.Move)
                {
                    CameraPosition += Vector.Parse(action.Parameters);
                }

                if (action.Action == ActionType.CreateNew)
                {
                    NewMap();
                }

                if (action.Action == ActionType.Save)
                {
                    SaveMap();
                }

                if (action.Action == ActionType.Open)
                {
                    OpenMap();
                }

                if (action.Action == ActionType.ChangeMapEditorDefaultCell)
                {
                    ShowChangeDefaultCellDialogue();
                }

                if (action.Action == ActionType.EditDetails)
                {
                    ShowMapInfoForm();
                }

                if (action.Action == ActionType.EscapeMenu)
                {
                    _systemContainer.ActivitySystem.RemoveActivity(this);
                }
            }
        }

        private void ShowMapInfoForm()
        {
            var mapInfoForm = new MapInfoForm(_systemContainer, _map, MapInfoFormCallback);
            _systemContainer.ActivitySystem.Push(new FormActivity(Position, Padding, mapInfoForm));
        }

        private void MapInfoFormCallback(FormButton selectedButton, Form form)
        {
            if (selectedButton == FormButton.Ok)
            {
                var mapInfoForm = form as MapInfoForm;

                SetMapName(mapInfoForm.Fields["Map Name"].Value.ToString());
                SetMapBiomes(mapInfoForm.Fields["Biomes"].Value.ToString());
                SetMapWeight(mapInfoForm.Fields["VaultWeight"].Value.ToString());
            }

            _systemContainer.ActivitySystem.ActivityStack.Pop();
        }

        private void SetMapWeight(string formData)
        {
            _map.VaultWeight = double.Parse(formData);
        }

        private void SetMapBiomes(string formData)
        {
            var biomes = formData.Split(',').Select(b => new Biome { Name = b });
            _map.Biomes = biomes.ToList();
        }

        private void SetMapName(string formData)
        {
            var newKey = new MapKey(formData);

            _map.MapKey = newKey;
            foreach(var cell in _map.Cells)
            {
                cell.Key.Key = newKey;
            }
        }

        private IEnumerable<IEntity> AllCells => _systemContainer.EntityEngine.AllEntities.Where(e => e.Has<Cell>());

        public void ShowChangeDefaultCellDialogue()
        {
            var inputActivity = new EntityPickerMenuActivity(Position, Padding, AllCells, _systemContainer, "Choose a new default cell:", SetDefaultCell);

            _systemContainer.ActivitySystem.Push(inputActivity);
        }

        public void ShowChangePrimaryCellDialogue()
        {
            var inputActivity = new EntityPickerMenuActivity(Position, Padding, AllCells, _systemContainer, "Choose a cell to use:", SetPrimaryCell);

            _systemContainer.ActivitySystem.Push(inputActivity);
        }

        public void ShowChangeSecondaryCellDialogue()
        {
            var inputActivity = new EntityPickerMenuActivity(Position, Padding, AllCells, _systemContainer, "Choose a cell to use:", SetSecondaryCell);

            _systemContainer.ActivitySystem.Push(inputActivity);
        }

        private void SetDefaultCell(IEntity parameter)
        {
            _map.DefaultCell = parameter;
        }

        private void SetPrimaryCell(IEntity parameter)
        {
            PrimaryCell = parameter;
        }

        private void SetSecondaryCell(IEntity parameter)
        {
            SecondaryCell = parameter;
        }

        public void SaveMap()
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Map file|*.map";
            saveDialog.Title = "Save map...";
            saveDialog.ShowDialog();

            if (saveDialog.FileName != "")
            {
                var serialisedMap = MapSerializer.Serialize(_map);
                File.WriteAllText(saveDialog.FileName, serialisedMap);
                _systemContainer.MessageSystem.Write($"Saved to {saveDialog.FileName}", Color.Blue);
            }           
        }

        public void OpenMap()
        {
            var openDialog = new OpenFileDialog();
            openDialog.Filter = "Map file|*.map";
            openDialog.Title = "Open map...";
            openDialog.ShowDialog();

            if (openDialog.FileName != "")
            {
                var serialisedMap = File.ReadAllText(openDialog.FileName);
                Map = MapSerializer.Deserialize(_systemContainer, serialisedMap);
                _systemContainer.MapSystem.MapCollection.Clear();
                _systemContainer.MapSystem.MapCollection.Add(Map.MapKey, Map);
                _systemContainer.MessageSystem.Write($"Opened {openDialog.FileName}", Color.Blue);
            }
        }

        
    }
}