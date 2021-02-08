using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        public string _mapName;
        private IEntity _primaryCell;
        private IEntity _secondaryCell;
        private readonly ISystemContainer _systemContainer;

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

        public void ApplyTool(MapCoordinate mapCoordinate, IEntity cell)
        {
            CurrentTool.Apply(_map, mapCoordinate, cell);
        }

        public MapCoordinate CameraPosition { get; set; }

        public MapEditorActivity(ISystemContainer systemContainer, IMap map)
        {
            _map = map;
            _mapName = map.MapKey.Key;
            PrimaryCell = map.DefaultCell;
            _systemContainer = systemContainer;
            CameraPosition = new MapCoordinate(_map.MapKey, 0, 0);
        }

        public static IEnumerable<IMapEditorTool> GetToolbarControls() => new List<IMapEditorTool>
        {
            new PenTool(),
            new EraserTool()
        };

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

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            //throw new System.NotImplementedException();
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

        public void ShowChangeDefaultCellDialogue()
        {
            var inputActivity = new MapEditorCellMenuActivity(_systemContainer, "Choose a new default cell:", SetDefaultCell);

            _systemContainer.ActivitySystem.Push(inputActivity);
        }

        public void ShowChangePrimaryCellDialogue()
        {
            var inputActivity = new MapEditorCellMenuActivity(_systemContainer, "Choose a cell to use:", SetPrimaryCell);

            _systemContainer.ActivitySystem.Push(inputActivity);
        }

        public void ShowChangeSecondaryCellDialogue()
        {
            var inputActivity = new MapEditorCellMenuActivity(_systemContainer, "Choose a cell to use:", SetSecondaryCell);

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
                new StatsConfiguration {Position = new Rectangle(0,0,width, height),
                    Displays = new List<InfoDisplay> {
                        new InfoDisplay { ControlType=typeof(MapEditorToolbarControl) }
                    }
                },
                new StatsConfiguration{ Position = new Rectangle(width - 6 * 8, 0, 6 * 8, 24), Displays = new List<InfoDisplay>{
                    new InfoDisplay { ControlType=typeof(MapEditorSelectedCellControl)}
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
                _map = MapSerializer.Deserialize(_systemContainer, serialisedMap);
                _systemContainer.MessageSystem.Write($"Opened {openDialog.FileName}", Color.Blue);
            }
        }
    }
}