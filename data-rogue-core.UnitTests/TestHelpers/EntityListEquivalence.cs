using System.Collections.Generic;
using System.Linq;
using data_rogue_core.EntitySystem;
using FluentAssertions;
using FluentAssertions.Equivalency;

namespace data_rogue_core.UnitTests.Data
{
    partial class EntitySerializerTests
    {
        public class EntityListEquivalence : IEquivalencyStep
        {
            public bool CanHandle(IEquivalencyValidationContext context, IEquivalencyAssertionOptions config)
            {
                return context.Subject is List<Entity> && context.Expectation is List<Entity>;
            }

            public bool Handle(IEquivalencyValidationContext context, IEquivalencyValidator parent, IEquivalencyAssertionOptions config)
            {
                var subjectEntities = context.Subject.As<List<Entity>>();

                var expectationEntities = context.Expectation.As<List<Entity>>();

                subjectEntities.Count.Should().Be(expectationEntities.Count);

                for(int i = 0; i < subjectEntities.Count; i++)
                {
                    Compare(subjectEntities[i], expectationEntities[i]);
                }

                return true;
            }

            private void Compare(Entity entity1, Entity entity2)
            {
                foreach(var type in entity1.Components.Select(c => c.GetType()))
                {
                    var component1 = entity1.Components.Single(s => s.GetType() == type);
                    var component2 = entity2.Components.Single(s => s.GetType() == type);

                    foreach(var key in type.GetFields().Where(f => f.IsPublic))
                    {
                        var value1 = key.GetValue(component1);
                        var value2 = key.GetValue(component2);

                        value1.Should().Be(value2);
                    }
                }
            }
        }
    }
}
