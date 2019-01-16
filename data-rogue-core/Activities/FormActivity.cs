using System.Collections.Generic;
using data_rogue_core.Renderers;
using data_rogue_core.Renderers.ConsoleRenderers;

namespace data_rogue_core.Activities
{
    public class FormActivity : IActivity
    {
        public ActivityType Type => ActivityType.Form;
        public object Data => Form;
        public bool RendersEntireSpace => true;

        public Form Form { get; set; }
        public IFormRenderer Renderer { get; }

        public FormActivity(Form form, IRendererFactory rendererFactory)
        {
            Form = form;
            Renderer = (IFormRenderer)rendererFactory.GetRendererFor(Type);
        }
        
        public void Render()
        {
            Renderer.Render(Form);
        }
    }
}
