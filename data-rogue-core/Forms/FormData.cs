using System.Collections.Generic;

namespace data_rogue_core.Forms
{
    public class FormData
    {
        public FormDataType FormDataType;
        public int Order;
        public object Value;

        public FormData(FormDataType formDataType, object value, int order)
        {
            FormDataType = formDataType;
            Value = value;
            Order = order;
        }

        public bool HasSubFields { get; internal set; } = false;

        public virtual List<string> GetSubItems()
        {
            return new List<string>();
        }
    }
}