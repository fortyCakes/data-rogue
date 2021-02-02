using System;
using System.Drawing;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Menus.DynamicMenus
{
    public class DebugMenu : Menu
    {
        private ISystemContainer _systemContainer;

        public DebugMenu(ISystemContainer systemContainer) : base(systemContainer.ActivitySystem, "Debug", null, GetDebugMenuItems())
        {
            _systemContainer = systemContainer;
            OnSelectCallback += GetCallback(systemContainer);
        }

        private MenuItemSelected GetCallback(ISystemContainer systemContainer)
        {
            return (item, action) => ApplyAction(systemContainer, item, action);
        }

        private void ApplyAction(ISystemContainer systemContainer, MenuItem item, MenuAction action)
        {
            switch(item.Text)
            {
                case "Cancel":
                    systemContainer.ActivitySystem.Pop();
                    return;
                case "Spawn":
                    var textInput = new TextInputActivity(systemContainer.ActivitySystem, "Spawn what entity?", ExecuteSpawnCommand);
                    textInput.InputText = "Monster:Bat";
                    systemContainer.ActivitySystem.Push(textInput);
                    break;
                default:
                    throw new ApplicationException($"Unhandled menu action {item.Text} in DebugMenu.");
            }
        }

        private void ExecuteSpawnCommand(string command)
        {
            var entityToSpawn = _systemContainer.PrototypeSystem.Get(command);
            if (entityToSpawn != null)
            {
                Targeting targetingData = new Targeting
                {
                    CellsHit = new VectorList { new Vector(0, 0) },
                    Range = 20
                };

                Action<MapCoordinate> spawnTarget = (target) => { _systemContainer.PositionSystem.SetPosition(entityToSpawn, target); };

                CloseActivity();
                _systemContainer.TargetingSystem.GetTarget(_systemContainer.PlayerSystem.Player, targetingData, spawnTarget);
            }
            else
            {
                _activitySystem.Push(new ToastActivity(_activitySystem, $"Could not find prototype for entity '{command}'", Color.Red));
            }
        }

        private static MenuItem[] GetDebugMenuItems()
        {
            var spawn = new MenuItem("Spawn", null);
            var learn = new MenuItem("Learn", null, false);
            var setCell = new MenuItem("SetCell", null, false);

            var cancelItem = new MenuItem("Cancel", null);

            return new MenuItem[]
            {
                spawn,
                learn,
                setCell,
                cancelItem
            };
        }
    }
}
