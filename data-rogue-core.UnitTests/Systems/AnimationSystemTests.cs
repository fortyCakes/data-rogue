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
        private IRandom _random;

        [SetUp]
        public void SetUp()
        {
            _stopwatch = Substitute.For<IStopwatch>();
            _stopwatch.ElapsedMilliseconds.ReturnsForAnyArgs(0);
            _random = Substitute.For<IRandom>();

            _animationSystem = new AnimationSystem(_stopwatch, _random);
            _animationSystem.Initialise();

            _testObject = CreateTestObject();
            _animatedComponent = _testObject.Get<Animated>();

            _animationSystem.AddEntity(_testObject);
        }

        [Test]
        public void Tick_CurrentTickLow_IncreasesCurrentTick()
        {
            _animatedComponent.CurrentAnimation.CurrentTick = 0;
            _stopwatch.ElapsedMilliseconds.Returns(AnimationSystem.TICK_LENGTH);

            _animationSystem.Tick();

            _animatedComponent.CurrentAnimation.CurrentTick.Should().Be(1);
        }

        [Test]
        public void Tick_CurrentTickHigh_IncreasesCurrentFrameAndResetsTick()
        {
            _animatedComponent.CurrentAnimation.CurrentTick = 29;
            _animatedComponent.CurrentAnimation.FrameTicks = 30;
            _animatedComponent.CurrentAnimation.CurrentFrame = 0;
            
            _stopwatch.ElapsedMilliseconds.Returns(AnimationSystem.TICK_LENGTH);
            _animationSystem.Tick();
            
            _animatedComponent.CurrentAnimation.CurrentTick.Should().Be(0);
            _animatedComponent.CurrentAnimation.CurrentFrame.Should().Be(1);
        }

        [Test]
        public void Tick_AnimationNotSet_SetsToDefault()
        {
            _animatedComponent.CurrentAnimation = null;
            _stopwatch.ElapsedMilliseconds.Returns(AnimationSystem.TICK_LENGTH);
            _animationSystem.Tick();

            _animatedComponent.CurrentAnimation.AnimationType.Should().Be(AnimationType.Idle);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void Tick_RandomiseTick_SometimesDecreasesCurrentTickMore(int randomReturn)
        {
            _animatedComponent.CurrentAnimation.CurrentTick = 29;
            _animatedComponent.CurrentAnimation.FrameTicks = 30;
            _animatedComponent.CurrentAnimation.CurrentFrame = 0;
            _animatedComponent.CurrentAnimation.RandomiseTicks = true;

            _random.PickOneFrom<int>().ReturnsForAnyArgs(randomReturn);

            _stopwatch.ElapsedMilliseconds.Returns(AnimationSystem.TICK_LENGTH);
            _animationSystem.Tick();

            _animatedComponent.CurrentAnimation.CurrentTick.Should().Be(-randomReturn);
            _animatedComponent.CurrentAnimation.CurrentFrame.Should().Be(1);
        }

        [Test]
        public void Tick_CurrentTickHigh_FrameHigh_ResetsCurrentFrameAndResetsTick()
        {
            _animatedComponent.CurrentAnimation.CurrentTick = 29;
            _animatedComponent.CurrentAnimation.FrameTicks = 30;
            _animatedComponent.CurrentAnimation.CurrentFrame = 1;

            _stopwatch.ElapsedMilliseconds.Returns(AnimationSystem.TICK_LENGTH);
            _animationSystem.Tick();

            _animatedComponent.CurrentAnimation.CurrentTick.Should().Be(0);
            _animatedComponent.CurrentAnimation.CurrentFrame.Should().Be(0);
        }

        [Test]
        public void GetFrame_ReturnsCorrectFrame()
        {
            _animatedComponent.CurrentAnimation.CurrentFrame = 1;

            _animationSystem.GetFrame(_testObject).Should().Be(1);
        }

        [Test]
        public void SetFrame_SetsFrame()
        {
            _animationSystem.SetFrame(_testObject, 1);

            _animatedComponent.CurrentAnimation.CurrentFrame.Should().Be(1);
        }

        [Test]
        public void SetFrame_ResetsTick()
        {
            _animatedComponent.CurrentAnimation.CurrentTick = 1;
            _animationSystem.SetFrame(_testObject, 1);

            _animatedComponent.CurrentAnimation.CurrentTick.Should().Be(0);
        }

        [Test]
        public void SetAnimation_ChangesAnimationType()
        {
            _animationSystem.SetAnimation(_testObject, AnimationType.Attack);

            _animatedComponent.CurrentAnimation.AnimationType.Should().Be(AnimationType.Attack);
            _animatedComponent.CurrentAnimation.CurrentTick.Should().Be(0);
            _animatedComponent.CurrentAnimation.CurrentFrame.Should().Be(0);
        }

        [Test]
        public void Tick_AnimationComplete_Repeat_Repeats()
        {
            _animatedComponent.CurrentAnimation.CurrentTick = 29;
            _animatedComponent.CurrentAnimation.FrameTicks = 30;
            _animatedComponent.CurrentAnimation.CurrentFrame = 1;

            _stopwatch.ElapsedMilliseconds.Returns(AnimationSystem.TICK_LENGTH);
            _animationSystem.Tick();

            _animatedComponent.CurrentAnimation.AnimationType.Should().Be(AnimationType.Idle);
        }

        [Test]
        public void Tick_AnimationComplete_NotRepeat_SwitchesToDefault()
        {
            _animationSystem.SetAnimation(_testObject, AnimationType.Attack);
            _animatedComponent.CurrentAnimation.CurrentTick = 29;
            _animatedComponent.CurrentAnimation.FrameTicks = 30;
            _animatedComponent.CurrentAnimation.CurrentFrame = 0;

            _stopwatch.ElapsedMilliseconds.Returns(AnimationSystem.TICK_LENGTH);
            _animationSystem.Tick();

            _animatedComponent.CurrentAnimation.AnimationType.Should().Be(AnimationType.Idle);
        }

        private IEntity CreateTestObject()
        {
            var animated = new Animated();
            var animation = new Animation
            {
                AnimationType = AnimationType.Idle,
                CurrentFrame = 0,
                CurrentTick = 0,
                Frames = new FrameList {AnimationFrame.Idle0, AnimationFrame.Idle1},
                FrameTicks = 30,
                Repeat = true
            };

            var animation2 = new Animation
            {
                AnimationType = AnimationType.Attack,
                CurrentFrame = 0,
                CurrentTick = 0,
                Frames = new FrameList { AnimationFrame.Attack0 },
                FrameTicks = 30,
                Repeat = false
            };

            animated.CurrentAnimation = animation;

            var animatedEntity = new Entity(1, "animated entity", 
                new IEntityComponent[]{ animated, animation, animation2 });
            return animatedEntity;
        }
    }
}
