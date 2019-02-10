using System;
using System.Linq;
using System.Text.RegularExpressions;
using data_rogue_core.Activities;
using data_rogue_core.Behaviours;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;
using RLNET;

namespace data_rogue_core.Systems
{
    public class PlayerControlSystem : IPlayerControlSystem
    {

        private readonly IPositionSystem positionSystem;
        private readonly IEventSystem eventSystem;
        private readonly IMessageSystem messageSystem;
        private readonly ITimeSystem timeSystem;
        private readonly IItemSystem itemSystem;

        public MapCoordinate HoveredCoordinate { get; private set; }

        public PlayerControlSystem(IPositionSystem positionSystem, IEventSystem eventSystem, ITimeSystem timeSystem, IItemSystem itemSystem, IMessageSystem messageSystem)
        {
            this.timeSystem = timeSystem;
            this.itemSystem = itemSystem;
            this.positionSystem = positionSystem;
            this.eventSystem = eventSystem;
            this.messageSystem = messageSystem;
        }


        public void HandleKeyPress(RLKeyPress keyPress)
        {
            if (timeSystem.WaitingForInput && keyPress != null)
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
                            Game.ActivityStack.Push(new StaticTextActivity("Saved", Game.RendererFactory));
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
                    case RLKey.Escape:
                        Game.ActivityStack.Push(MainMenu.GetMainMenu());
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

        private void GetItem()
        {
            IEntity player = Game.WorldState.Player;
            var playerMapCoordinate = player.Get<Position>().MapCoordinate;

            var items = positionSystem.EntitiesAt(playerMapCoordinate)
                .Except(new[] { player })
                .Where(e => e.Has<Item>())
                .OrderBy(e => e.EntityId);

            var firstItem = items.First();

            var eventData = new PickupItemEventData { Item = firstItem };

            var ok = eventSystem.Try(EventType.PickUpItem, player, eventData);

            if (ok)
            {
                var inventory = player.Get<Inventory>();

                itemSystem.MoveToInventory(firstItem, inventory);

                messageSystem.Write($"You pick up the {firstItem.Get<Description>().Name}.");

                eventSystem.Try(EventType.SpendTime, player, new SpendTimeEventData { Ticks = 1000 });
            }
        }

        private void BeginRest()
        {
            IEntity player = Game.WorldState.Player;

            player.Get<PlayerRestBehaviour>().Resting = true;
            Wait(1000);
        }

        private void UseSkill(int index)
        {
            eventSystem.Try(EventType.ActivateSkill, Game.WorldState.Player, new ActivateSkillEventData() { SkillIndex = index });
        }

        private void Wait(int ticks)
        {
            eventSystem.Try(EventType.SpendTime, Game.WorldState.Player, new SpendTimeEventData{Ticks = ticks});
        }

        private void UseStairs(StairDirection direction)
        {
            IEntity player = Game.WorldState.Player;
            var playerMapCoordinate = player.Get<Position>().MapCoordinate;

            var stairs = positionSystem
                .EntitiesAt(playerMapCoordinate)
                .Where(e => e.Has<Stairs>())
                .Select(e => e.Get<Stairs>())
                .SingleOrDefault();

            if (stairs != null && stairs.Direction == direction)
            {
                if (eventSystem.Try(EventType.ChangeFloor, player, direction))
                {
                    player.Get<Position>().MapCoordinate = stairs.Destination;
                }
            }
            else
            {
                var portal = positionSystem
                    .EntitiesAt(playerMapCoordinate)
                    .Where(e => e.Has<Portal>())
                    .Select(e => e.Get<Portal>())
                    .SingleOrDefault();

                if (portal != null)
                {
                    if (eventSystem.Try(EventType.UsePortal, player, portal))
                    {
                        player.Get<Position>().MapCoordinate = portal.Destination;
                    }
                }
            }
        }

        private void DebugChangeFloor(int change)
        {
            var playerMapCoordinate = Game.WorldState.Player.Get<Position>().MapCoordinate;

            var match = Regex.Match(playerMapCoordinate.Key.Key, "^(.*):([0-9]*)$");

            string newMap = $"{match.Groups[1].Value}:{int.Parse(match.Groups[2].Value) + change}";
            if (Game.WorldState.Maps.Any(m => m.Key.Key == newMap))
            {
                playerMapCoordinate.Key = new MapKey(newMap);
            }
        }

        private void Save()
        {
            SaveSystem.Save(Game.WorldState);
        }

        private void MovePlayer(int x, int y)
        {
            var player = Game.WorldState.Player;
            var vector = new Vector(x, y);

            eventSystem.Try(EventType.Move, player, vector);
        }

        public void HandleMouseInput(RLMouse mouse)
        {
            var x = mouse.X;
            var y = mouse.Y;

            if (Game.ActivityStack.Peek() is GameplayActivity activity)
            {
                var gameplayRenderer = Game.RendererFactory.GetRendererFor(ActivityType.Gameplay) as IGameplayRenderer;

                var hoveredLocation = gameplayRenderer.GetMapCoordinateFromMousePosition(Game.WorldState, x, y);

                SetHoveredLocation(hoveredLocation);
            }
                    
        }

        private void SetHoveredLocation(MapCoordinate mapCoordinate)
        {
            HoveredCoordinate = mapCoordinate;
        }
    }
}