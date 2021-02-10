using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.Controls.MapEditorTools;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.IOSystems;
using data_rogue_core.IOSystems.BLTTiles;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

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
                PrimaryCell = _map.DefaultCell;
                SecondaryCell = _map.DefaultCell;
                CameraPosition = new MapCoordinate(_map.MapKey, 0, 0);
            }
        }

        public string _mapName;
        private IEntity _primaryCell;
        private IEntity _secondaryCell;
        private MapCoordinate _mousePosition;
        private readonly ISystemContainer _systemContainer;

        public MapEditorActivity(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
            NewMap();
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

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            var config = GetRenderingConfiguration(width, height);

            var controls = ControlFactory.GetControls(config, renderer, systemContainer, rendererHandle, controlRenderers, playerFov, systemContainer.ActivitySystem.ActivityStack.IndexOf(this));

            return controls;
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
            if (action.Action == ActionType.ChangeMapEditorCell)
            {
                ShowChangePrimaryCellDialogue();
            }

            if (action.Action == ActionType.Move)
            {
                CameraPosition += Vector.Parse(action.Parameters);
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
        }

        private IEnumerable<IEntity> AllCells => _systemContainer.EntityEngine.AllEntities.Where(e => e.Has<Cell>());

        public void ShowChangeDefaultCellDialogue()
        {
            var inputActivity = new EntityPickerMenuActivity(AllCells, _systemContainer, "Choose a new default cell:", SetDefaultCell);

            _systemContainer.ActivitySystem.Push(inputActivity);
        }

        public void ShowChangePrimaryCellDialogue()
        {
            var inputActivity = new EntityPickerMenuActivity(AllCells, _systemContainer, "Choose a cell to use:", SetPrimaryCell);

            _systemContainer.ActivitySystem.Push(inputActivity);
        }

        public void ShowChangeSecondaryCellDialogue()
        {
            var inputActivity = new EntityPickerMenuActivity(AllCells, _systemContainer, "Choose a cell to use:", SetSecondaryCell);

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

        public IEnumerable<IRenderingConfiguration> GetRenderingConfiguration(int width, int height)
        {
            return new List<IRenderingConfiguration>
            {
                new MapEditorConfiguration {Position=new Rectangle(0,0, width, height)},
                new MapEditorTargetingConfiguration {Position=new Rectangle(0,0, width, height)},
                new StatsConfiguration {Position = new Rectangle(0,0,width, height),
                    Displays = new List<InfoDisplay> {
                        new InfoDisplay { ControlType=typeof(MapEditorToolbarControl) }
                    }
                },
                new StatsConfiguration{ Position = new Rectangle(width - 6 * 8, 0, 6 * 8, 24), Displays = new List<InfoDisplay>{
                    new InfoDisplay { ControlType=typeof(MapEditorCellPickerControl)}
                } },
                new MessageConfiguration{Position = new Rectangle(1,height-50, width, 25), NumberOfMessages = 10}
            };
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