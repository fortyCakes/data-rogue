using System;
using data_rogue_core.Display;
using data_rogue_core.Entities;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using RLNET;
using RogueSharp;

namespace data_rogue_core.UnitTests.Entities
{
    [TestFixture]
    public class CounterBaseTests
    {
        private CounterBase _counterBase;

        [SetUp]
        public void Setup()
        {
            _counterBase = new CounterBase(100);
        }

        [Test]
        public void ToString_ReturnsCounterString()
        {
            _counterBase = new CounterBase(50, 100);

            _counterBase.ToString().Should().Be("50/100");
        }

        [Test]
        public void Construct_WithMaxHealth_UsesParameter()
        {
            _counterBase = new CounterBase(100, 200);

            _counterBase.CounterMax.Should().Be(200);
        }

        [Test]
        public void Construct_WithoutMaxHealth_Defaults()
        {
            _counterBase = new CounterBase(150);

            _counterBase.CounterMax.Should().Be(150);
        }

        [Test]
        public void TakeDamage_BelowZero_DoesNothing()
        {
            _counterBase = new CounterBase(150);

            _counterBase.TakeDamage(-1);

            _counterBase.CounterValue.Should().Be(150);
        }

        [Test]
        public void TakeDamage_BelowCurrentHealth_DoesDamage()
        {
            _counterBase = new CounterBase(150);

            _counterBase.TakeDamage(5);

            _counterBase.CounterValue.Should().Be(145);
        }

        [Test]
        public void TakeDamage_AboveCurrentHealth_SetsToZero()
        {
            _counterBase = new CounterBase(150);

            _counterBase.TakeDamage(155);

            _counterBase.CounterValue.Should().Be(0);
        }

        [Test]
        public void TakeDamage_CanUnderflow_GoesBelowZero()
        {
            _counterBase = new CounterBase(1, 100);

            _counterBase.TakeDamage(100, true);

            _counterBase.CounterValue.Should().Be(-99);
        }

        [Test]
        public void TakeDamage_AlreadyBelowZero_DoesntGoLower()
        {
            _counterBase = new CounterBase(-1, 100);

            _counterBase.TakeDamage(100, false);

            _counterBase.CounterValue.Should().Be(-1);
        }

        [Test]
        public void TakeDamage_CanUnderflow_AlreadyBelowZero_GoesLower()
        {
            _counterBase = new CounterBase(-1, 100);

            _counterBase.TakeDamage(100, true);

            _counterBase.CounterValue.Should().Be(-101);
        }

        [Test]
        public void Restore_BelowZero_DoesNothing()
        {
            _counterBase = new CounterBase(100, 150);

            _counterBase.Restore(-1);

            _counterBase.CounterValue.Should().Be(100);
        }

        [Test]
        public void Restore_Normal_DoesHealing()
        {
            _counterBase = new CounterBase(100, 150);

            _counterBase.Restore(10);

            _counterBase.CounterValue.Should().Be(110);
        }

        [Test]
        public void Restore_OverMax_SetsToMax()
        {
            _counterBase = new CounterBase(100, 150);

            _counterBase.Restore(9000);

            _counterBase.CounterValue.Should().Be(150);
        }

        [Test]
        public void Restore_OverMax_Allowed_SetsToOverMax()
        {
            _counterBase = new CounterBase(100, 150);

            _counterBase.Restore(9000, true);

            _counterBase.CounterValue.Should().Be(9100);
        }

    }
}