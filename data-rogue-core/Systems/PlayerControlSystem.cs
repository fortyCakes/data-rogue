﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem;
using data_rogue_core.Maps;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Systems.Interfaces;
using RLNET;

namespace data_rogue_core.Systems
{
    public class PlayerControlSystem : IPlayerControlSystem
    {

        private readonly IPositionSystem PositionSystem;
        private readonly IEventRuleSystem EventSystem;
        private readonly ITimeSystem TimeSystem;

        public PlayerControlSystem(IPositionSystem positionSystem, IEventRuleSystem eventRuleSystem, ITimeSystem timeSystem)
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
                    case RLKey.L:
                        Game.ActivityStack.Push(new StaticTextActivity(@"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.", Game.RendererFactory));
                        break;
                    case RLKey.Escape:
                        Game.ActivityStack.Push(MainMenu.GetMainMenu());
                        break;
                    case RLKey.Period:
                        UseStairs(StairDirection.Down);
                        break;
                    case RLKey.Comma:
                        UseStairs(StairDirection.Up);
                        break;
                }
            }
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

            if (EventSystem.Try(EventType.Move, player, vector))
            {
                PositionSystem.Move(player.Get<Position>(), vector);
                EventSystem.Try(EventType.SpendTime, player, 1000);
            }
        }
    }
}