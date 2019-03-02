using data_rogue_core.EntityEngineSystem;
using System;

namespace data_rogue_core.Systems
{
    public interface IScriptExecutor
    {
        void Execute(IEntity user, string script, Action onComplete = null);

        void ExecuteByName(IEntity user, string scriptName, Action onComplete = null);
    }
}
