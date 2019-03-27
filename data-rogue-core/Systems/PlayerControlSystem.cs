using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Forms;
using data_rogue_core.Maps;
using data_rogue_core.Menus;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;
using RLNET;

namespace data_rogue_core.Systems
{
    public class PlayerControlSystem : IPlayerControlSystem
    {
        private ISystemContainer _systemContainer;
        private readonly IEntityDataProvider keyBindingsDataProvider;
        private List<KeyBinding> _keyBindings;
        private IActivitySystem _activitySystem;
        private ITargetingSystem _targetingSystem;

        public MapCoordinate HoveredCoordinate { get; private set; }

        public PlayerControlSystem(ISystemContainer systemContainer, IEntityDataProvider keyBindingsDataProvider)
        {
            this._systemContainer = systemContainer;
            this.keyBindingsDataProvider = keyBindingsDataProvider;
            _activitySystem = systemContainer.ActivitySystem;
            _targetingSystem = systemContainer.TargetingSystem;

        }

        public void Initialise()
        {
            SetKeyBindings();
        }

        private void SetKeyBindings()
        {
            var keyBindingsData = keyBindingsDataProvider.GetData();

            var bindings = EntitySerializer.DeserializeAll(_systemContainer, keyBindingsData).SingleOrDefault();

            if (bindings != null)
            {
                _keyBindings = bindings.Components.OfType<KeyBinding>().ToList();
            }
            else
            {
                _keyBindings = new List<KeyBinding>();
            }
        }

        public void HandleInput(KeyCombination keyPress, RLMouse mouse)
        {
            var actionData = HandleKeyPress(keyPress);

            IActivity currentActivity = _activitySystem.Peek();
            switch (currentActivity.Type)
            {
                case ActivityType.Menu:
                    (currentActivity.Data as Menu)?.HandleAction(actionData);
                    break;
                case ActivityType.StaticDisplay:
                    if (keyPress != null)
                    {
                        _activitySystem.Pop();
                    }
                    break;
                case ActivityType.Gameplay:
                    if (_systemContainer.TimeSystem.WaitingForInput && actionData != null)
                    {
                        _systemContainer.EventSystem.Try(EventType.Action, _systemContainer.PlayerSystem.Player, actionData);
                    }
                    HandleMouseInput(mouse);
                    break;
                case ActivityType.Form:
                    (currentActivity.Data as Form)?.HandleAction(actionData);
                    break;
                case ActivityType.Targeting:
                    _targetingSystem.HandleMouseInput(mouse);
                    break;
            }
        }

        private ActionEventData HandleKeyPress(KeyCombination keyPress)
        {
            var keyBinding = _keyBindings.SingleOrDefault(k => k.Key.Equals(keyPress));

            if (keyBinding == null) return null;

            var action = keyBinding.Action;

            ActionType actionType = ExtractActionType(action);
            var parameters = ExtractParameters(action);

            return new ActionEventData { Action = actionType, Parameters = parameters, KeyPress = keyPress, Speed = 1000 };
        }

                //switch(keyBinding.Action)
                //{

                //    case RLKey.S:
                //        if (keyPress.Shift)
                //        {
                //            Save();
                //            _systemContainer.ActivitySystem.Push(new StaticTextActivity("Saved", _systemContainer.RendererSystem.RendererFactory));
                //        }

                //        break;
                //    case RLKey.D:
                //        MovePlayer(1, 0);
                //        break;
                //    case RLKey.Y:
                //        MovePlayer(-1, -1);
                //        break;
                //    case RLKey.U:
                //        MovePlayer(1, -1);
                //        break;
                //    case RLKey.B:
                //        MovePlayer(-1, 1);
                //        break;
                //    case RLKey.N:
                //        MovePlayer(1, 1);
                //        break;
                //    case RLKey.L:
                //        break;
                //    case RLKey.R:
                //        if (keyPress.Shift)
                //        {
                //            BeginRest();
                //        }
                //        break;
                //    case RLKey.G:
                //        if (keyPress.Shift)
                //        {

                //        }
                //        else
                //        {
                //            GetItem();
                //        }
                //        break;
                //    case RLKey.I:
                //        if (keyPress.Shift)
                //        {

                //        }
                //        else
                //        {
                //            ShowInventory();
                //        }
                //        break;
                //    case RLKey.E:
                //        if (keyPress.Shift)
                //        {

                //        }
                //        else
                //        {
                //            ShowEquipment();
                //        }
                //        break;
                //    case RLKey.Escape:
                //        _systemContainer.ActivitySystem.Push(new MenuActivity(new MainMenu(
                //                _systemContainer.ActivitySystem,
                //                _systemContainer.PlayerSystem,
                //                _systemContainer.SaveSystem,
                //                _systemContainer.RendererSystem
                //            ), _systemContainer.RendererSystem.RendererFactory));
                //        break;
                //    case RLKey.Period:
                //        if (keyPress.Shift)
                //        {
                //            UseStairs(StairDirection.Down);
                //        }
                //        else
                //        {
                //            Wait(1000);
                //        }
                //        break;
                //    case RLKey.Comma:
                //        if (keyPress.Shift)
                //        {
                //            UseStairs(StairDirection.Up);
                //        }
                //        break;
                //    case RLKey.Number1:
                //        if (keyPress.Shift)
                //        {

                //        }
                //        else
                //        {
                //            UseSkill(1);
                //        }
                //        break;
                //    case RLKey.Number2:
                //        if (keyPress.Shift)
                //        {

                //        }
                //        else
                //        {
                //            UseSkill(2);
                //        }
                //        break;
                //    case RLKey.Number3:
                //        if (keyPress.Shift)
                //        {

                //        }
                //        else
                //        {
                //            UseSkill(3);
                //        }
                //        break;
                //    case RLKey.Number4:
                //        if (keyPress.Shift)
                //        {

                //        }
                //        else
                //        {
                //            UseSkill(4);
                //        }
                //        break;
                //    case RLKey.Number5:
                //        if (keyPress.Shift)
                //        {

                //        }
                //        else
                //        {
                //            UseSkill(5);
                //        }
                //        break;
                //}

        public static string ExtractParameters(string action)
        {
            if (action.Contains("("))
            {
                action = action.Remove(0, action.IndexOf('(') + 1);
                action = action.Remove(action.Length - 1);
            }
            else
            {
                return null;
            }

            return action;
        }

        public static ActionType ExtractActionType(string action)
        {
            if (action.Contains("("))
            {
                action = action.Remove(action.IndexOf('('));
            }

            return (ActionType)Enum.Parse(typeof(ActionType), action);
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

        public void HandleMouseInput(RLMouse mouse)
        {
            var x = mouse.X;
            var y = mouse.Y;

            if (_activitySystem.Peek() is GameplayActivity activity && _systemContainer.PlayerSystem.Player != null)
            {
                var gameplayRenderer = GetGameplayRenderer();

                var hoveredLocation = gameplayRenderer.GetMapCoordinateFromMousePosition(_systemContainer.RendererSystem.CameraPosition, x, y);

                SetHoveredLocation(hoveredLocation);
            }
                    
        }

        private IGameplayRenderer GetGameplayRenderer()
        {
            GameplayActivity gameplayActivity = (GameplayActivity)_activitySystem.ActivityStack.Single(a => a.Type == ActivityType.Gameplay);

            return gameplayActivity.Renderer;
        }

        private void SetHoveredLocation(MapCoordinate mapCoordinate)
        {
            HoveredCoordinate = mapCoordinate;
        }
    }
}