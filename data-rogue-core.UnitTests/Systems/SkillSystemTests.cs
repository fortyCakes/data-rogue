using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Systems
{
    [TestFixture]
    public class SkillSystemTests
    {
        [SetUp]
        public void SetUp()
        {
            entityId = 0;
            learner = GetTestEntity();

            systemContainer = new SystemContainer();

            systemContainer.CreateSystems("seed");

            systemContainer.EntityEngine.Initialise(systemContainer);
        }

        uint entityId;
        IEntity learner;

        private SystemContainer systemContainer;

        [Test]
        public void LearnSkill_NoKnownSkills_HasKnownSkill()
        {
            var skill = GetTestSkill();

            systemContainer.SkillSystem.Learn(learner, skill);

            var knownSkills = learner.Components.OfType<KnownSkill>();

            knownSkills.Single().Skill.Should().Be(skill.Get<Prototype>().Name);
        }

        [Test]
        public void LearnSkill_OneKnownSkills_AddsKnownSkill()
        {
            var skill = GetTestSkill();
            var skill2 = GetTestSkill();

            systemContainer.SkillSystem.Learn(learner, skill);
            systemContainer.SkillSystem.Learn(learner, skill2);

            var knownSkills = learner.Components.OfType<KnownSkill>();

            var expected = new List<KnownSkill>
            {
                new KnownSkill { Order = 1, Skill = skill.Get<Prototype>().Name },
                new KnownSkill { Order = 2, Skill = skill2.Get<Prototype>().Name },
            };

            knownSkills.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void LearnSkill_AlreadyKnown_DoesNotAdd()
        {
            var skill = GetTestSkill();

            systemContainer.SkillSystem.Learn(learner, skill);
            systemContainer.SkillSystem.Learn(learner, skill);

            var knownSkills = learner.Components.OfType<KnownSkill>();

            var expected = new List<KnownSkill>
            {
                new KnownSkill { Order = 1, Skill = skill.Get<Prototype>().Name }
            };

            knownSkills.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Forget_KnownSkill_RemovesSkillAndRenumbers()
        {
            var skill = GetTestSkill();
            var skill2 = GetTestSkill();

            systemContainer.SkillSystem.Learn(learner, skill);
            systemContainer.SkillSystem.Learn(learner, skill2);

            systemContainer.SkillSystem.Forget(learner, skill);

            var knownSkills = learner.Components.OfType<KnownSkill>();

            var expected = new List<KnownSkill>
            {
                new KnownSkill { Order = 1, Skill = skill2.Get<Prototype>().Name }
            };

            knownSkills.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Forget_UnknownSkill_NoChange()
        {
            var skill = GetTestSkill();
            var skill2 = GetTestSkill();

            systemContainer.SkillSystem.Learn(learner, skill);

            systemContainer.SkillSystem.Forget(learner, skill2);

            var knownSkills = learner.Components.OfType<KnownSkill>();

            knownSkills.Single().Skill.Should().Be(skill.Get<Prototype>().Name);
        }

        private IEntity GetTestEntity()
        {
            return new Entity(entityId++, "Learner", new IEntityComponent[] {  });
        }

        private IEntity GetTestSkill()
        {
            var name = $"Skill {entityId}";
            return new Entity(entityId++, name, new IEntityComponent[] { new Skill(), new Prototype { Name = name } });
        }
    }
}