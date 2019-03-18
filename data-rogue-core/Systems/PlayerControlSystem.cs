using System.Linq;
using System.Text.RegularExpressions;
using data_rogue_core.Activities;
using data_rogue_core.Behaviours;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Menus.DynamicMenus;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;
using RLNET;

namespace data_rogue_core.Systems
{
    public class PlayerControlSystem : IPlayerControlSystem
    {
        private ISystemContainer _systemContainer;

        public MapCoordinate HoveredCoordinate { get; private set; }

        public PlayerControlSystem(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }


        public void HandleKeyPress(RLKeyPress keyPress)
        {
            if (_systemContainer.TimeSystem.WaitingForInput && keyPress != null)
            {
                switch (keyPress.Key)
                {
                    case RLKey.W:
                        MovePlayer( 0, -1);
                        break;
                    case RLKey.A:
                        MovePlayer(-1, 0);
                        break;
                    case RLKey.S:
                        if (keyPress.Shift)
                        {
                            Save();
                            _systemContainer.ActivitySystem.Push(new StaticTextActivity("Saved", _systemContainer.RendererSystem.RendererFactory));
                        }
                        else
                        {
                            MovePlayer(0, 1);
                        }

                        break;
                    case RLKey.D:
                        MovePlayer(1, 0);
                        break;
                    case RLKey.Y:
                        MovePlayer(-1, -1);
                        break;
                    case RLKey.U:
                        MovePlayer(1, -1);
                        break;
                    case RLKey.B:
                        MovePlayer(-1, 1);
                        break;
                    case RLKey.N:
                        MovePlayer(1, 1);
                        break;
                    case RLKey.L:
                        break;
                    case RLKey.R:
                        if (keyPress.Shift)
                        {
                            BeginRest();
                        }
                        break;
                    case RLKey.G:
                        if (keyPress.Shift)
                        {

                        }
                        else
                        {
                            GetItem();
                        }
                        break;
                    case RLKey.I:
                        if (keyPress.Shift)
                        {

                        }
                        else
                        {
                            ShowInventory();
                        }
                        break;
                    case RLKey.E:
                        if (keyPress.Shift)
                        {

                        }
                        else
                        {
                            ShowEquipment();
                        }
                        break;
                    case RLKey.Escape:
                        _systemContainer.ActivitySystem.Push(new MenuActivity(new MainMenu(
                                _systemContainer.ActivitySystem,
                                _systemContainer.PlayerSystem,
                                _systemContainer.SaveSystem,
                                _systemContainer.RendererSystem
                            ), _systemContainer.RendererSystem.RendererFactory));
                        break;
                    case RLKey.Period:
                        if (keyPress.Shift)
                        {
                            UseStairs(StairDirection.Down);
                        }
                        else
                        {
                            Wait(1000);
                        }
                        break;
                    case RLKey.Comma:
                        if (keyPress.Shift)
                        {
                            UseStairs(StairDirection.Up);
                        }
                        break;
                    case RLKey.Number1:
                        if (keyPress.Shift)
                        {

                        }
                        else
                        {
                            UseSkill(1);
                        }
                        break;
                    case RLKey.Number2:
                        if (keyPress.Shift)
                        {

                        }
                        else
                        {
                            UseSkill(2);
                        }
                        break;
                    case RLKey.Number3:
                        if (keyPress.Shift)
                        {

                        }
                        else
                        {
                            UseSkill(3);
                        }
                        break;
                    case RLKey.Number4:
                        if (keyPress.Shift)
                        {

                        }
                        else
                        {
                            UseSkill(4);
                        }
                        break;
                    case RLKey.Number5:
                        if (keyPress.Shift)
                        {

                        }
                        else
                        {
                            UseSkill(5);
                        }
                        break;
                }
            }
        }

        private void ShowEquipment()
        {
            IEntity player = _systemContainer.PlayerSystem.Player;

            _systemContainer.ActivitySystem.Push(new MenuActivity(new EquipmentMenu(_systemContainer, player), _systemContainer.RendererSystem.RendererFactory));
        }

        private void ShowInventory()
        {
            IEntity player = _systemContainer.PlayerSystem.Player;
            var inventory = player.Get<Inventory>();

            _systemContainer.ActivitySystem.Push(new MenuActivity(new InventoryMenu(_systemContainer, inventory), _systemContainer.RendererSystem.RendererFactory));
        }

        private void GetItem()
        {
            IEntity player = _systemContainer.PlayerSystem.Player;
            var playerMapCoordinate = player.Get<Position>().MapCoordinate;

            var items = _systemContainer.PositionSystem.EntitiesAt(playerMapCoordinate)
                .Except(new[] { player })
                .Where(e => e.Has<Item>() || e.Has<Wealth>())
                .OrderBy(e => e.EntityId);

            var firstItem = items.FirstOrDefault();

            if (firstItem != null)
            {
                if (firstItem.Has<Wealth>())
                {
                    var wealth = firstItem.Get<Wealth>();

                    var eventData = new PickupWealthEventData { Item = firstItem };

                    var ok = _systemContainer.EventSystem.Try(EventType.PickUpWealth, player, eventData);

                    if (ok)
                    {
                        _systemContainer.MessageSystem.Write($"You pick up {wealth.Amount} {wealth.Currency}.");

                        _systemContainer.ItemSystem.TransferWealth(firstItem, player, wealth.Currency, wealth.Amount);

                        _systemContainer.EntityEngine.Destroy(firstItem);
                    }
                }
                else
                {
                    var eventData = new PickupItemEventData { Item = firstItem };

                    var ok = _systemContainer.EventSystem.Try(EventType.PickUpItem, player, eventData);

                    if (ok)
                    {
                        var inventory = player.Get<Inventory>();

                        var done = _systemContainer.ItemSystem.MoveToInventory(firstItem, inventory);

                        if (done)
                        {
                            _systemContainer.MessageSystem.Write($"You pick up the {firstItem.Get<Description>().Name}.");

                            _systemContainer.EventSystem.Try(EventType.SpendTime, player, new SpendTimeEventData { Ticks = 1000 });
                        }
                    }
                }
            }
        }

        private void BeginRest()
        {
            IEntity player = _systemContainer.PlayerSystem.Player;

            player.Get<PlayerRestBehaviour>().Resting = true;
            Wait(1000);
        }

        private void UseSkill(int index)
        {
            IEntity player = _systemContainer.PlayerSystem.Player;

            var skill = _systemContainer.SkillSystem.GetKnownSkillByIndex(player, index);

            if (skill != null)
            {
                var ok = _systemContainer.EventSystem.Try(EventType.SelectSkill, player, new ActivateSkillEventData() { SkillName = skill.Skill });

                if (ok)
                {
                    _systemContainer.SkillSystem.Use(player, skill.Skill);
                }
            }
        }

        private void Wait(int ticks)
        {
            _systemContainer.EventSystem.Try(EventType.SpendTime, _systemContainer.PlayerSystem.Player, new SpendTimeEventData{Ticks = ticks});
        }

        private void UseStairs(StairDirection direction)
        {
            IEntity player = _systemContainer.PlayerSystem.Player;
            var playerMapCoordinate = player.Get<Position>().MapCoordinate;

            var stairs = _systemContainer.PositionSystem
                .EntitiesAt(playerMapCoordinate)
                .Where(e => e.Has<Stairs>())
                .Select(e => e.Get<Stairs>())
                .SingleOrDefault();

            if (stairs != null && stairs.Direction == direction)
            {
                if (_systemContainer.EventSystem.Try(EventType.ChangeFloor, player, direction))
                {
                    player.Get<Position>().MapCoordinate = stairs.Destination;
                }
            }
            else
            {
                var portal = _systemContainer.PositionSystem
                    .EntitiesAt(playerMapCoordinate)
                    .Where(e => e.Has<Portal>())
                    .Select(e => e.Get<Portal>())
                    .SingleOrDefault();

                if (portal != null)
                {
                    if (_systemContainer.EventSystem.Try(EventType.UsePortal, player, portal))
                    {
                        player.Get<Position>().MapCoordinate = portal.Destination;
                    }
                }
            }
        }

        private void DebugChangeFloor(int change)
        {
            var playerMapCoordinate = _systemContainer.PlayerSystem.Player.Get<Position>().MapCoordinate;

            var match = Regex.Match(playerMapCoordinate.Key.Key, "^(.*):([0-9]*)$");

            string newMap = $"{match.Groups[1].Value}:{int.Parse(match.Groups[2].Value) + change}";
            if (_systemContainer.MapSystem.MapCollection.Any(m => m.Key.Key == newMap))
            {
                playerMapCoordinate.Key = new MapKey(newMap);
            }
        }

        private void Save()
        {
            _systemContainer.SaveSystem.Save();
        }

        private void MovePlayer(int x, int y)
        {
            var player = _systemContainer.PlayerSystem.Player;
            var vector = new Vector(x, y);

            _systemContainer.EventSystem.Try(EventType.Move, player, vector);
        }

        public void HandleMouseInput(RLMouse mouse)
        {
            var x = mouse.X;
            var y = mouse.Y;

            if (_systemContainer.ActivitySystem.Peek() is GameplayActivity activity)
            {
                var gameplayRenderer = _systemContainer.RendererSystem.RendererFactory.GetRendererFor(ActivityType.Gameplay) as IGameplayRenderer;

                var hoveredLocation = gameplayRenderer.GetMapCoordinateFromMousePosition(_systemContainer.RendererSystem.CameraPosition, x, y);

                SetHoveredLocation(hoveredLocation);
            }
                    
        }

        private void SetHoveredLocation(MapCoordinate mapCoordinate)
        {
            HoveredCoordinate = mapCoordinate;
        }
    }
}