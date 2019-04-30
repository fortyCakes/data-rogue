using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.Forms.StaticForms
{
    public abstract class SubSelectableFormData : FormData
    {
        public SubSelectableFormData(string name, FormDataType formDataType, object value, int order) : base(name, formDataType, value, order)
        {
        }

        public string SubSelection { get; set; }
    }
}