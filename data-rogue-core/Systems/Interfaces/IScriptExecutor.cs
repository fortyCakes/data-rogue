using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems
{
    public interface IScriptExecutor
    {
        void Execute(IEntity user, string script);

        void ExecuteByName(IEntity user, string scriptName);
    }
}
