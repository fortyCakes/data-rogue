using System;
using System.Linq;
using System.Text.RegularExpressions;
using data_rogue_core.Activities;
using data_rogue_core.Behaviours;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Systems.Interfaces;
using RLNET;

namespace data_rogue_core.Systems
{
    public class PlayerControlSystem : IPlayerControlSystem
    {

        private readonly IPositionSystem PositionSystem;
        private readonly IEventSystem EventSystem;
        private readonly ITimeSystem TimeSystem;

        public PlayerControlSystem(IPositionSystem positionSystem, IEventSystem eventRuleSystem, ITimeSystem timeSystem)
        {
            TimeSystem = timeSystem;
            PositionSystem = positionSystem;
            EventSystem = eventRuleSystem;
        }


        public void HandleKeyPress(RLKeyPress keyPress)
        {
            if (TimeSystem.WaitingForInput && keyPress != null)
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

        private void BeginRest()
        {
            IEntity player = Game.WorldState.Player;

            player.Get<PlayerRestBehaviour>().Resting = true;
            Wait(1000);
        }

        private void UseSkill(int index)
        {
            EventSystem.Try(EventType.ActivateSkill, Game.WorldState.Player, new ActivateSkillEventData() { SkillIndex = index });
        }

        private void Wait(int ticks)
        {
            EventSystem.Try(EventType.SpendTime, Game.WorldState.Player, new SpendTimeEventData{Ticks = ticks});
        }

        private void UseStairs(StairDirection direction)
        {
            IEntity player = Game.WorldState.Player;
            var playerMapCoordinate = player.Get<Position>().MapCoordinate;

            var stairs = PositionSystem
                .EntitiesAt(playerMapCoordinate)
                .Where(e => e.Has<Stairs>())
                .Select(e => e.Get<Stairs>())
                .SingleOrDefault();

            if (stairs != null && stairs.Direction == direction)
            {
                if (EventSystem.Try(EventType.ChangeFloor, player, direction))
                {
                    player.Get<Position>().MapCoordinate = stairs.Destination;
                }
            }
            else
            {
                var portal = PositionSystem
                    .EntitiesAt(playerMapCoordinate)
                    .Where(e => e.Has<Portal>())
                    .Select(e => e.Get<Portal>())
                    .SingleOrDefault();

                if (portal != null)
                {
                    if (EventSystem.Try(EventType.UsePortal, player, portal))
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

            EventSystem.Try(EventType.Move, player, vector);
        }
    }
}