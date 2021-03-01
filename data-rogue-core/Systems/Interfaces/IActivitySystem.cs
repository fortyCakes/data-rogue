using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IActivitySystem
    {
        ActivityStack ActivityStack { get; }

        void Initialise(Rectangle defaultPosition, Padding defaultPadding);
        void Push(IActivity activity);
        IActivity Pop();
        IActivity Peek();

        Action QuitAction { get; set; }
        GameplayActivity GameplayActivity { get; }
        MapEditorActivity MapEditorActivity { get; }
        Rectangle DefaultPosition { get; }
        Padding DefaultPadding { get; }

        void RemoveActivity(IActivity activity);
        IActivity GetActivityAcceptingInput();
        IMapActivity GetMapActivity();

        void OpenShop(ISystemContainer systemContainer, IEntity shop);
    }
}
