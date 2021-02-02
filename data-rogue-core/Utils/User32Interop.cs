using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Utils
{
    using data_rogue_core.IOSystems;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public static class User32Interop
    {
        public static char ToAscii(KeyCombination keys)
        {
            var key = keys.Key.ToString().First();


            var outputBuilder = new StringBuilder(2);
            int result = ToAscii((uint)key, 0, GetKeyState(keys),
                outputBuilder, 0);
            if (result == 1)
                return outputBuilder[0];
            else
                throw new Exception("Invalid key");
        }

        private const byte HighBit = 0x80;
        private static byte[] GetKeyState(KeyCombination modifiers)
        {
            var keyState = new byte[256];

            if (modifiers.Shift)
            {
                keyState[(int)Keys.ShiftKey] = HighBit;
                keyState[(int)Keys.LShiftKey] = HighBit;
            }

            return keyState;
        }

        [DllImport("user32.dll")]
        private static extern int ToAscii(uint uVirtKey, uint uScanCode,
            byte[] lpKeyState,
            [Out] StringBuilder lpChar,
            uint uFlags);
    }
}
