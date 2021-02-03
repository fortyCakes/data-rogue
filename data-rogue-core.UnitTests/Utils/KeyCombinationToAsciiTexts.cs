using data_rogue_core.IOSystems;
using data_rogue_core.Utils;
using FluentAssertions;
using NUnit.Framework;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace data_rogue_core.UnitTests.Utils
{
    [TestFixture]
    public class KeyCombinationToAsciiTexts
    {
        [Test]
        [TestCase(Key.A, false, false, false, 'a')]
        [TestCase(Key.A, true, false, false, 'A')]
        [TestCase(Key.Semicolon, false, false, false, ';')]
        [TestCase(Key.Semicolon, true, false, false, ':')]
        public void ConvertsKeyToCorrectChar(Key key, bool shift, bool alt, bool ctrl, char output)
        {
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(CultureInfo.GetCultureInfo("en-GB"));

            var keyCombination = new KeyCombination
            {
                Key = key,
                Shift = shift,
                Alt = alt,
                Ctrl = ctrl
            };

            var ascii = keyCombination.ToChar();

            ascii.Should().Be(output);
        }
    }
}
