﻿using data_rogue_core.Forms;
using System;
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
    }

    public class MultipleChoiceFormData : FormData
    {
        public MultipleChoiceFormData(object value, int order, List<object> validValues) : base(FormDataType.MultipleChoice, value, order)
        {
            ValidValues = validValues;
        }

        public List<object> ValidValues { get; }

        public void ChangeSelection(int change)
        {
            var index = ValidValues.IndexOf(Value);

            index += change;

            while (index > ValidValues.Count - 1)
            {
                index -= ValidValues.Count;
            }

            while (index < 0)
            {
                index += ValidValues.Count;
            }

            Value = ValidValues[index];
        }
    }
}