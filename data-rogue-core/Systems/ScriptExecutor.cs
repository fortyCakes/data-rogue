using data_rogue_core.EntityEngine;
using data_rogue_core.Systems.Interfaces;
using NLua;
using System;

namespace data_rogue_core.Systems
{
    public class ScriptExecutor : IScriptExecutor
    {

        public readonly ISystemContainer systemContainer;

        public ScriptExecutor (ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public void Execute(IEntity user, string script)
        {
            Lua state = new Lua();

            state["SystemContainer"] = systemContainer;
            state["User"] = user;

            state.DoString(script);
        }
    }
}
