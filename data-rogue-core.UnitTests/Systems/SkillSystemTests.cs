using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
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

            systemContainer = new SystemContainer();

            systemContainer.CreateSystems("seed");
            systemContainer.EventSystem = Substitute.For<IEventSystem>();
            systemContainer.EventSystem.Try(Arg.Any<EventType>(), Arg.Any<IEntity>(), Arg.Any<object>()).ReturnsForAnyArgs(true);

            systemContainer.EntityEngine.Initialise(systemContainer);

            learner = GetTestEntity();
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

        [Test]
        public void Use_PassesAppropriateScriptToExecutor()
        {
            systemContainer.MessageSystem = Substitute.For<IMessageSystem>();
            
            var skill = GetTestSkill();
            skill.Get<Skill>().ScriptName = "ScriptName";
            var script = systemContainer.EntityEngine.New("Script", new Prototype { Name = "ScriptName" }, new Script { Text = "SystemContainer.MessageSystem:Write('test')" });

            systemContainer.SkillSystem.Use(learner, skill.Name);

            systemContainer.MessageSystem.Received(1).Write("test");
        }

        [Test]
        public void OnComplete_RaisesSkillCompleteEvent()
        {
            systemContainer.EventSystem = Substitute.For<IEventSystem>();

            var skill = GetTestSkill();
            systemContainer.SkillSystem.OnComplete(learner, skill);

            systemContainer.EventSystem.Received(1).Try(EventType.CompleteSkill, learner, Arg.Any<CompleteSkillEventData>());
        }

        [Test]
        public void GetKnownSkillByIndex_HasSkill_ReturnsSkill()
        {
            var skill = GetTestSkill();
            var skill2 = GetTestSkill();

            systemContainer.SkillSystem.Learn(learner, skill);
            systemContainer.SkillSystem.Learn(learner, skill2);

            var result = systemContainer.SkillSystem.GetKnownSkillByIndex(learner, 2);

            var expected = learner.Components.OfType<KnownSkill>().Single(c => c.Skill == skill2.Name);

            result.Should().Be(expected);
        }

        [Test]
        public void GetKnownSkillByIndex_DoesNotHaveSkill_ReturnsNull()
        {
            var skill = GetTestSkill();
            var skill2 = GetTestSkill();

            systemContainer.SkillSystem.Learn(learner, skill);
            systemContainer.SkillSystem.Learn(learner, skill2);

            var result = systemContainer.SkillSystem.GetKnownSkillByIndex(learner, 3);

            result.Should().BeNull();
        }

        [Test]
        public void GetSkillFromKnown_ReturnsSkill()
        {
            var skill = GetTestSkill();
            systemContainer.SkillSystem.Learn(learner, skill);
            var known = systemContainer.SkillSystem.GetKnownSkillByIndex(learner, 1);

            var result = systemContainer.SkillSystem.GetSkillFromKnown(known);

            result.Should().Be(skill);
        }

        [Test]
        public void KnownSkills_ReturnsKnownSkills()
        {
            var skill = GetTestSkill();
            var skill2 = GetTestSkill();
            systemContainer.SkillSystem.Learn(learner, skill);
            systemContainer.SkillSystem.Learn(learner, skill2);

            var result = systemContainer.SkillSystem.KnownSkills(learner);

            result.Should().BeEquivalentTo(skill, skill2);
        }

        private IEntity GetTestEntity()
        {
            return systemContainer.EntityEngine.New("Learner");
        }

        private IEntity GetTestSkill()
        {
            var name = $"Skill {entityId++}";
            return systemContainer.EntityEngine.New(name, new Skill(), new Prototype { Name = name });
        }
    }
}