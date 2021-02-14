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
                    var spawnEntity = new TextInputActivity(systemContainer.ActivitySystem, "Spawn what entity?", ExecuteSpawnCommand);
                    spawnEntity.InputText = "Monster:Bat";
                    systemContainer.ActivitySystem.Push(spawnEntity);
                    break;
                case "Learn":
                    var learnSkill = new TextInputActivity(systemContainer.ActivitySystem, "Learn what skill?", ExecuteLearnCommand);
                    learnSkill.InputText = "Monster:Bat";
                    systemContainer.ActivitySystem.Push(learnSkill);
                    break;
                case "SetMapCell":
                    var setCell = new TextInputActivity(systemContainer.ActivitySystem, "Set cell to what?", ExecuteSetCellCommand);
                    setCell.InputText = "Cell:Empty";
                    systemContainer.ActivitySystem.Push(setCell);
                    break;
                case "God Mode":
                    systemContainer.EntityEngine.AddComponent(systemContainer.PlayerSystem.Player, new GodMode());
                    ShowToast("GodMode enabled");
                    break;
                case "Toggle NoClip":
                    systemContainer.PlayerSystem.Player.Get<Physical>().Passable = !systemContainer.PlayerSystem.Player.Get<Physical>().Passable;
                    ShowToast("NoClip " + (systemContainer.PlayerSystem.Player.Get<Physical>().Passable ? "enabled" : "disabled"));
                    break;
                case "Reveal Map":
                    var map = systemContainer.MapSystem.MapCollection[systemContainer.RendererSystem.CameraPosition.Key];
                    ShowToast("Map revealed");

                    foreach(var cell in map.Cells)
                    {
                        map.SetSeen(cell.Key);
                        if (cell.Value.Get<Physical>().Transparent)
                        {
                            foreach (var adjacentVector in Vector.GetAdjacentCellVectors())
                            {
                                map.SetSeen(cell.Key + adjacentVector);
                            }
                        }
                    }

                    break;
                default:
                    throw new ApplicationException($"Unhandled menu action {item.Text} in DebugMenu.");
            }
        }

        private void ShowToast(string message)
        {
            _systemContainer.ActivitySystem.ActivityStack.Push(new ToastActivity(_systemContainer.ActivitySystem, message, Color.White));
        }

        private void ExecuteSpawnCommand(string command)
        {
            var entityToSpawn = _systemContainer.PrototypeSystem.TryGet(command);

            if (entityToSpawn != null)
            {
                Action<MapCoordinate> spawnTarget = (target) => { _systemContainer.PositionSystem.SetPosition(entityToSpawn, target); };
                GetTargetForDebugAction(spawnTarget);
            }
            else
            {
                _activitySystem.Push(new ToastActivity(_activitySystem, $"Could not find prototype for entity '{command}'", Color.Red));
            }
        }

        private void GetTargetForDebugAction(Action<MapCoordinate> spawnTarget)
        {
            Targeting targetingData = new Targeting
            {
                CellsHit = new VectorList { new Vector(0, 0) },
                Range = 20
            };
            CloseActivity();
            _systemContainer.TargetingSystem.GetTarget(_systemContainer.PlayerSystem.Player, targetingData, spawnTarget);
        }

        private void ExecuteLearnCommand(string command)
        {
            var skill = _systemContainer.PrototypeSystem.Get(command);
            _systemContainer.SkillSystem.Learn(_systemContainer.PlayerSystem.Player, skill);
        }

        private void ExecuteSetCellCommand(string command)
        {
            var cell = _systemContainer.PrototypeSystem.Get(command);
            if (cell != null)
            {
                Action<MapCoordinate> spawnTarget = (target) => { _systemContainer.MapSystem.MapCollection[target.Key].SetCell(target, cell); };
                GetTargetForDebugAction(spawnTarget);
            }
            else
            {
                _activitySystem.Push(new ToastActivity(_activitySystem, $"Could not find prototype for entity '{command}'", Color.Red));
            }
        }

        private static MenuItem[] GetDebugMenuItems()
        {
            var spawn = new MenuItem("Spawn", null);
            var learn = new MenuItem("Learn", null);
            var setCell = new MenuItem("SetMapCell", null);

            var godmode = new MenuItem("God Mode", null);
            var noclip = new MenuItem("Toggle NoClip", null);
            var seeAll = new MenuItem("Reveal Map");

            var cancelItem = new MenuItem("Cancel", null);

            return new MenuItem[]
            {
                spawn,
                learn,
                setCell,
                godmode,
                noclip,
                seeAll,
                cancelItem
            };
        }
    }
}
