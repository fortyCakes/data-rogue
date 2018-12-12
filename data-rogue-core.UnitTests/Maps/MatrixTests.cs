using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Maps;
using FluentAssertions;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Maps
{
    [TestFixture]
    class MatrixTests
    {
        [Test]
        public void ReflectionInYAxisMatrix_Reflects()
        {
            var testVector = new Vector(1, 2);

            var reflectionMatrix = new Matrix(-1, 0, 0, 1);

            var result = reflectionMatrix * testVector;

            var expected = new Vector(-1, 2);

            result.Should().Be(expected);
        }

        [Test]
        public void ReflectionInXAxisMatrix_Reflects()
        {
            var testVector = new Vector(1, 2);

            var reflectionMatrix = new Matrix(1, 0, 0, -1);

            var result = reflectionMatrix * testVector;

            var expected = new Vector(1, -2);

            result.Should().Be(expected);
        }

        [Test]
        public void RotationBy90CW_Rotates()
        {
            var testVector = new Vector(1, 2);

            var reflectionMatrix = new Matrix(0, 1, -1, 0);

            var result = reflectionMatrix * testVector;

            var expected = new Vector(2, -1);

            result.Should().Be(expected);
        }
    }
}
