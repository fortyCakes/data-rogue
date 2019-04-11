
using data_rogue_core.Renderers;
using data_rogue_core.Forms;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public class FormActivity : IActivity
    {
        public ActivityType Type => ActivityType.Form;
        public object Data => Form;
        public bool RendersEntireSpace => true;

        public Form Form { get; set; }
        public IFormRenderer Renderer { get; private set; }

        public FormActivity(Form form)
        {
            Form = form;
            form.Activity = this;
        }

        public void Render(ISystemContainer systemContainer)
        {
            Renderer.Render(Form);
        }

        public void Initialise(IRenderer renderer)
        {
            Renderer = (IFormRenderer)renderer;
        }
    }
}
