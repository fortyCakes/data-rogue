using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using NLua;
using System;

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

        public void RequestTarget(IEntity user, TargetingData targetingData)
        {
            systemContainer.TargetingSystem.GetTarget(user, targetingData, (MapCoordinate c) => targetCallback?.Invoke(c));
        }
    }
}
