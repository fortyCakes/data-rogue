using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using NLua;

namespace data_rogue_core.Systems
{
    public class ScriptExecutor : IScriptExecutor
    {
        public delegate void TargetCallback(MapCoordinate targetedCell);
        private TargetCallback targetCallback;

        public readonly ISystemContainer systemContainer;

        public ScriptExecutor (ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public void Execute(IEntity user, string script)
        {
            Lua state = new Lua();

            RegisterHandlers(state);

            RegisterValues(user, state);

            DoImports(state);

            SetupEnumeration(state);

            state.DoString(script);
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
            state.RegisterFunction("withTarget", this, GetType().GetMethod("SetTargetHandler"));
            state.RegisterFunction("requestTarget", this, GetType().GetMethod("RequestTarget"));
        }

        private void RegisterValues(IEntity user, Lua state)
        {
            state["SystemContainer"] = systemContainer;
            state["User"] = user;
        }

        private static void DoImports(Lua state)
        {
            state.LoadCLRPackage();
            state.DoString("import ('data-rogue-core', 'data_rogue_core.Systems.Interfaces')");
            state.DoString("import ('data-rogue-core', 'data_rogue_core.EventSystem')");
            state.DoString("import ('data-rogue-core', 'data_rogue_core.EventSystem.EventData')");
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
