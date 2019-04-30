using data_rogue_core.Controls;
using System.Collections.Generic;

namespace data_rogue_core.Forms
{
    public abstract class FormData : BaseControl
    {
        public string Name { get; set; }
        public FormDataType FormDataType;
        public int Order;
        public object Value;

        public FormData(string name, FormDataType formDataType, object value, int order)
        {
            FormDataType = formDataType;
            Value = value;
            Order = order;
            Name = name;
        }

        public bool HasSubFields { get; internal set; } = false;

        public virtual List<string> GetSubItems()
        {
            return new List<string>();
        }
    }
}