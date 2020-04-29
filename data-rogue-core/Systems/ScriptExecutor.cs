using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.Systems
{
    public class ScriptExecutor : IScriptExecutor
    {
        public delegate void TargetCallback(MapCoordinate targetedCell);
        private TargetCallback targetCallback;

        private Action onCompleteAction = null;

        public readonly ISystemContainer systemContainer;

        public ScriptExecutor (ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public void Execute(IEntity user, string script, IEntity withEntity, Action onComplete)
        {
            if (onCompleteAction != null)
            {
                throw new ApplicationException("Can only await completion of one script at a time in ScriptExecutor");
            }

            onCompleteAction = onComplete;

            Lua state = new Lua();

            RegisterHandlers(state);

            RegisterValues(user, state, withEntity);

            DoImports(state);

            SetupEnumeration(state);

            state.DoString(script);
        }

        public void ExecuteByName(IEntity user, string scriptName, IEntity withEntity, Action onComplete)
        {
            var scriptEntity = systemContainer.PrototypeSystem.Get(scriptName);
            var script = scriptEntity.Get<Script>().Text;
            Execute(user, script, withEntity, onComplete);
        }

        private static void SetupEnumeration(Lua state)
        {
            state.DoString(@"function each(o)
               local e = o:GetEnumerator()
               return function()
                  if e:MoveNext() then
                    return e.Current
                 end
               end
            end");
        }

        private void RegisterHandlers(Lua state)
        {
            state.RegisterFunction("withTarget", this, GetType().GetMethod(nameof(SetTargetHandler)));
            state.RegisterFunction("requestTarget", this, GetType().GetMethod(nameof(RequestTarget)));
            state.RegisterFunction("onComplete", this, GetType().GetMethod(nameof(Complete)));
            state.RegisterFunction("makeAttack", this, GetType().GetMethod(nameof(MakeAttack)));
            state.RegisterFunction("attackCellsHit", this, GetType().GetMethod(nameof(AttackCellsHit)));
            state.RegisterFunction("attackAllCells", this, GetType().GetMethod(nameof(AttackAllCells)));
            state.RegisterFunction("isCellBlocked", this, GetType().GetMethod(nameof(IsCellBlocked)));
        }

        private void RegisterValues(IEntity user, Lua state, IEntity withEntity)
        {
            state["SystemContainer"] = systemContainer;
            state["User"] = user;
            state["Entity"] = withEntity;
        }

        private static void DoImports(Lua state)
        {
            state.LoadCLRPackage();
            state.DoString("import ('data-rogue-core', 'data_rogue_core.Systems.Interfaces')");
            state.DoString("import ('data-rogue-core', 'data_rogue_core.EventSystem')");
            state.DoString("import ('data-rogue-core', 'data_rogue_core.EventSystem.EventData')");
        }

        public void Complete()
        {
            onCompleteAction?.Invoke();
            onCompleteAction = null;
        }

        public void SetTargetHandler(TargetCallback callback)
        {
            targetCallback = callback;
        }

        public void RequestTarget(IEntity user, IEntity forSkill)
        {
            var targetingData = GetTargetingData(forSkill);

            systemContainer.TargetingSystem.GetTarget(user, targetingData, (MapCoordinate c) => targetCallback?.Invoke(c));
        }

        public void MakeAttack(IEntity attacker, IEntity defender, IEntity forSkill)
        {
            if (!defender.Has<Health>()) return;

            var attackDefinition = forSkill.Get<AttackDefinition>();

            var attackData = new AttackEventData
            {
                Accuracy = attackDefinition.Accuracy,
                AttackClass = attackDefinition.AttackClass,
                Attacker = attacker,
                AttackName = attackDefinition.AttackName,
                Damage = ResolveAttackDamage(attacker, attackDefinition.Damage),
                Defender = defender,
                Speed = attackDefinition.Speed,
                SpendTime = attackDefinition.SpendTime,
                Tags = attackDefinition.Tags?.Split(',')
            };

            systemContainer.EventSystem.Try(EventType.Attack, attacker, attackData);
        }

        private int? ResolveAttackDamage(IEntity attacker, string input)
        {
            int damage;

            if (int.TryParse(input, out damage))
            {
                return damage;
            }

            return (int)systemContainer.EventSystem.GetStat(attacker, input);
        }

        private static TargetingData GetTargetingData(IEntity forSkill)
        {
            var targeting = forSkill.Get<Targeting>();

            return new TargetingData
            {
                MoveToCell = targeting.MoveToCell,
                Range = targeting.Range,
                CellsHit = targeting.CellsHit,
                Rotatable = targeting.Rotatable,
                TargetOrigin = targeting.TargetOrigin,
                ValidVectors = targeting.ValidVectors,
                Friendly = targeting.Friendly,
                PathToTarget = targeting.PathToTarget
            };
        }

        public bool AttackCellsHit(MapCoordinate target, IEntity user, IEntity forSkill)
        {
            var anyTargets = false;
            var targeting = forSkill.Get<Targeting>();
            Matrix rotation = Matrix.Identity;

            if (targeting.Rotatable)
            {
                rotation = TargetingRotationHelper.GetSkillRotation(user, target);
            }

            foreach (var vector in targeting.CellsHit)
            {
                var vectorToCell = rotation * vector;

                var targetEntities = systemContainer.PositionSystem.EntitiesAt(target + vectorToCell);

                var targetFighters = systemContainer.FighterSystem.GetEntitiesWithFighter(targetEntities);

                foreach (var defender in targetFighters)
                {
                    anyTargets = true;
                    MakeAttack(user, defender, forSkill);
                }
            }

            return anyTargets;
        }

        public bool AttackAllCells(IEnumerable<MapCoordinate> coordinates, IEntity user, IEntity forSkill)
        {
            var anyTargets = false;

            foreach (var mapCoordinate in coordinates)
            {
                var targetEntities = systemContainer.PositionSystem.EntitiesAt(mapCoordinate);

                var targetFighters = systemContainer.FighterSystem.GetEntitiesWithFighter(targetEntities);

                foreach (var defender in targetFighters)
                {
                    anyTargets = true;
                    MakeAttack(user, defender, forSkill);
                }
            }

            return anyTargets;
        }

        public bool IsCellBlocked(MapCoordinate target, IEntity user)
        {
            return systemContainer.PositionSystem.IsBlocked(target, except: user);
        }
    }
}
