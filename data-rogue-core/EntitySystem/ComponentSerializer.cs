using data_rogue_core.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace data_rogue_core.EntitySystem
{
    public static class ComponentSerializer
    {
        private const string VALUE_PATTERN = "([a-zA-Z]*): (.*)";
        private const string NAME_PATTERN = "\\[([a-zA-Z0-9]*)\\]";

        public static string Serialize(IEntityComponent component, int depth)
        {
            var stringBuilder = new StringBuilder();

            var componentType = component.GetType();

            stringBuilder.AppendLine($"[{componentType.Name}]");

            var fields = componentType.GetFields(BindingFlags.Public | BindingFlags.Instance).OrderBy(f => f.Name);

            foreach (var field in fields)
            {
                object value = field.GetValue(component);
                string stringValue = null;

                Type fieldType = field.FieldType;
                if (fieldType == typeof(Color))
                {
                    stringValue = ColorTranslator.ToHtml((Color)value);
                }
                else if (typeof(ICustomFieldSerialization).IsAssignableFrom(fieldType))
                {
                    stringValue = ((ICustomFieldSerialization)value).Serialize();
                }
                else if (IsNullOrDefault(value))
                {
                    stringValue = null;
                }
                else
                {
                    stringValue = value.ToString();
                }

                if (stringValue != null)
                {
                    stringBuilder.AppendLine($"{field.Name}: {stringValue}");
                }
            }

            return stringBuilder.ToString();
        }

        public static IEntityComponent Deserialize(string input, IEntityEngineSystem entityEngineSystem, int depth)
        {
            var lines = input.SplitLines();

            var name = Regex.Match(lines[0], NAME_PATTERN).Groups[1].Value;

            var type = entityEngineSystem.ComponentTypes.Single(t => t.Name == name);

            IEntityComponent component = (IEntityComponent)Activator.CreateInstance(type);

            BindValuesRecursively(component, lines.Skip(1), depth);

            return component;
        }

        private static void BindValuesRecursively(IEntityComponent component, IEnumerable<string> lines, int depth)
        {
            foreach (var line in lines)
            {
                var expandedLine = line.Replace("\t", "     ");

                var lineData = expandedLine.Substring(depth * 4);

                var valueMatch = Regex.Match(lineData, VALUE_PATTERN);

                if (valueMatch.Success)
                {
                    var key = valueMatch.Groups[1].Value;
                    var value = valueMatch.Groups[2].Value.TrimEnd();

                    BindSingleValue(component, key, value);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        private static void BindSingleValue(IEntityComponent component, string key, string value)
        {
            var fieldInfo = component.GetType().GetField(key);
            Type fieldType = Nullable.GetUnderlyingType(fieldInfo.FieldType) ?? fieldInfo.FieldType;

            object safeValue = null;
            if (fieldType == typeof(Color))
            {
                safeValue = ColorTranslator.FromHtml(value);
            }
            else if (typeof(ICustomFieldSerialization).IsAssignableFrom(fieldType))
            {
                var instance = Activator.CreateInstance(fieldType);
                ((ICustomFieldSerialization)instance).Deserialize(value);
                safeValue = instance;
            }
            else if (value != null)
            {
                safeValue = Convert.ChangeType(value, fieldType);
            }

            fieldInfo.SetValue(component, safeValue);
        }

        public static bool IsNullOrDefault<T>(T argument)
        {
            // deal with normal scenarios
            if (argument == null) return true;
            if (object.Equals(argument, default(T))) return true;

            // deal with non-null nullables
            Type methodType = typeof(T);
            if (Nullable.GetUnderlyingType(methodType) != null) return false;

            // deal with boxed value types
            Type argumentType = argument.GetType();
            if (argumentType.IsValueType && argumentType != methodType)
            {
                object obj = Activator.CreateInstance(argument.GetType());
                return obj.Equals(argument);
            }

            return false;
        }
    }
}
