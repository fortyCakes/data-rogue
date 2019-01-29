using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using NLua;
using System;
using System.Drawing;

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

            state.RegisterFunction("withTarget", this, GetType().GetMethod("SetTargetHandler"));
            state.RegisterFunction("requestTarget", this, GetType().GetMethod("RequestTarget"));

            state["SystemContainer"] = systemContainer;
            state["User"] = user;

            state.LoadCLRPackage();
            state.DoString("import ('data-rogue-core', 'data_rogue_core.Systems.Interfaces')");

            state.DoString(script);
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
