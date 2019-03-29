using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Components;
using data_rogue_core.Renderers;
using RLNET;

namespace data_rogue_core.IOSystems
{
    public interface IIOSystem
    {
        IRendererFactory RendererFactory { get; }

        void Initialise(UpdateEventHandler onUpdate, UpdateEventHandler onRender);

        void Run();

        void Draw();

        KeyCombination GetKeyPress();

        MouseData GetMouseData();
    }
}
