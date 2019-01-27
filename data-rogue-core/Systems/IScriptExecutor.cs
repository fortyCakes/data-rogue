using data_rogue_core.EntityEngine;

namespace data_rogue_core.Systems
{
    public interface IScriptExecutor
    {
        void Execute(IEntity user, string script);
    }
}
