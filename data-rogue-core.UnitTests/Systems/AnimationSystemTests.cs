using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Systems
{
    [TestFixture]
    public class AnimationSystemTests
    {
        private AnimationSystem _animationSystem;
        private IEntity _testObject;
        private Animated _animatedComponent;
        private IStopwatch _stopwatch;

        [SetUp]
        public void SetUp()
        {
            _stopwatch = Substitute.For<IStopwatch>();
            _stopwatch.ElapsedMilliseconds.ReturnsForAnyArgs(0);

            _animationSystem = new AnimationSystem(_stopwatch);
            _animationSystem.Initialise();

            _testObject = CreateTestObject();
            _animatedComponent = _testObject.Get<Animated>();

            _animationSystem.AddEntity(_testObject);
        }

        [Test]
        public void Tick_CurrentTickLow_IncreasesCurrentTick()
        {
            _animatedComponent.CurrentTick = 0;
            _stopwatch.ElapsedMilliseconds.Returns(AnimationSystem.TICK_LENGTH);

            _animationSystem.Tick();

            _animatedComponent.CurrentTick.Should().Be(1);
        }

        [Test]
        public void Tick_CurrentTickHigh_IncreasesCurrentFrameAndResetsTick()
        {
            _animatedComponent.CurrentTick = 29;
            _animatedComponent.FrameTicks = 30;
            _animatedComponent.CurrentFrame = 0;
            
            _stopwatch.ElapsedMilliseconds.Returns(AnimationSystem.TICK_LENGTH);
            _animationSystem.Tick();
            
            _animatedComponent.CurrentTick.Should().Be(0);
            _animatedComponent.CurrentFrame.Should().Be(1);
        }

        [Test]
        public void Tick_CurrentTickHigh_FrameHigh_ResetsCurrentFrameAndResetsTick()
        {
            _animatedComponent.CurrentTick = 29;
            _animatedComponent.FrameTicks = 30;
            _animatedComponent.CurrentFrame = 1;

            _stopwatch.ElapsedMilliseconds.Returns(AnimationSystem.TICK_LENGTH);
            _animationSystem.Tick();

            _animatedComponent.CurrentTick.Should().Be(0);
            _animatedComponent.CurrentFrame.Should().Be(0);
        }

        [Test]
        public void GetFrame_ReturnsCorrectFrame()
        {
            _animatedComponent.CurrentFrame = 1;

            _animationSystem.GetFrame(_testObject).Should().Be(1);
        }

        [Test]
        public void SetFrame_SetsFrame()
        {
            _animationSystem.SetFrame(_testObject, 1);

            _animatedComponent.CurrentFrame.Should().Be(1);
        }

        [Test]
        public void SetFrame_ResetsTick()
        {
            _animatedComponent.CurrentTick = 1;
            _animationSystem.SetFrame(_testObject, 1);

            _animatedComponent.CurrentTick.Should().Be(0);
        }

        private IEntity CreateTestObject()
        {
            var animatedEntity = new Entity(1, "animated entity", 
                new []{
                    new Animated
                    {
                        CurrentFrame = 0,
                        CurrentTick = 0,
                        Frames = new FrameList { AnimationFrame.Rest0, AnimationFrame.Rest1 },
                        FrameTicks = 30
                    }
                });
            return animatedEntity;
        }
    }
}
