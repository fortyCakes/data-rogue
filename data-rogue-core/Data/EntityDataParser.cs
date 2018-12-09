using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using data_rogue_core.EntitySystem;

namespace data_rogue_core.Data
{
    public class EntityDataParser : IEntityDataParser
    {
        private List<Type> ComponentTypes { get; }
        private IEntityEngineSystem EntityEngineSystem { get; }

        public EntityDataParser(List<Type> componentTypes, IEntityEngineSystem entityEngineSystem)
        {
            ComponentTypes = componentTypes;
            EntityEngineSystem = entityEngineSystem;
        }

        public List<IEntity> Parse(IEnumerable<string> lines)
        {
            var entityBuilders = new List<EntityBuilder>();
            EntityBuilder currentEntity = null;
            ComponentBuilder currentComponent = null;

            foreach (string line in lines)
            {
                if (line.StartsWith("#"))
                {
                    continue; // Comment
                }

                if (line.StartsWith("\""))
                {
                    var entityName = Regex.Match(line, "\"(.*)\"").Groups[1].Value;

                    currentEntity = new EntityBuilder(entityName);
                    entityBuilders.Add(currentEntity);
                }

                if (line.StartsWith("["))
                {
                    var componentName = Regex.Match(line, "\\[(.*)\\]").Groups[1].Value;
                    currentComponent = new ComponentBuilder(componentName);
                    currentEntity?.Components.Add(currentComponent);
                }

                if (line.StartsWith("    "))
                {
                    var matches = Regex.Match(line, "    (.*): ?(.*)");

                    var key = matches.Groups[1].Value;
                    var value = matches.Groups[2].Value;

                    currentComponent.Data.Add(key, value);
                }
            }

            return entityBuilders.Select(b => b.Build(EntityEngineSystem, ComponentTypes)).OfType<IEntity>().ToList();
        }
    }

    public class EntityBuilder
    {
        public string Name;

        public List<ComponentBuilder> Components = new List<ComponentBuilder>();

        public EntityBuilder(string entityName)
        {
            this.Name = entityName;
        }

        public Entity Build(IEntityEngineSystem engine, List<Type> componentTypes)
        {
            var components = Components.Select(c => c.Build(componentTypes));

            return engine.New(Name, components.ToArray());
        }
    }

    public class ComponentBuilder
    {
        private string Name;

        public ComponentBuilder(string componentName)
        {
            Name = componentName;
        }

        public Dictionary<string,string> Data { get; } = new Dictionary<string, string>();

        public IEntityComponent Build(List<Type> componentTypes)
        {
            var type = componentTypes.Single(t => t.Name == Name);

            IEntityComponent component = (IEntityComponent)Activator.CreateInstance(type);

            foreach (string key in Data.Keys)
            {
                var fieldInfo = component.GetType().GetField(key);
                Type fieldType = Nullable.GetUnderlyingType(fieldInfo.FieldType) ?? fieldInfo.FieldType;

                object safeValue = null;
                if (fieldType == typeof(Color))
                {
                    safeValue = ColorTranslator.FromHtml(Data[key]);
                }
                else if (Data[key] != null)
                {
                    safeValue = Convert.ChangeType(Data[key], fieldType);
                }
                
                fieldInfo.SetValue(component, safeValue);
            }

            return component;
        }
    }
}
