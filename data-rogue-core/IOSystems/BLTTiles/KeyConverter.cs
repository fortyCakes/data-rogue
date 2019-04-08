using BearLib;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public static class KeyConverter
    {
        private static Dictionary<int, Key> mapping = new Dictionary<int, Key>
        {
            {BLT.TK_A, Key.A },
            {BLT.TK_B, Key.B },
            {BLT.TK_C, Key.C },
            {BLT.TK_D, Key.D },
            {BLT.TK_E, Key.E },
            {BLT.TK_F, Key.F },
            {BLT.TK_G, Key.G },
            {BLT.TK_H, Key.H },
            {BLT.TK_I, Key.I },
            {BLT.TK_J, Key.J },
            {BLT.TK_K, Key.K },
            {BLT.TK_L, Key.L },
            {BLT.TK_M, Key.M },
            {BLT.TK_N, Key.N },
            {BLT.TK_O, Key.O },
            {BLT.TK_P, Key.P },
            {BLT.TK_Q, Key.Q },
            {BLT.TK_R, Key.R },
            {BLT.TK_S, Key.S },
            {BLT.TK_T, Key.T },
            {BLT.TK_U, Key.U },
            {BLT.TK_V, Key.V },
            {BLT.TK_W, Key.W },
            {BLT.TK_X, Key.X },
            {BLT.TK_Y, Key.Y },
            {BLT.TK_Z, Key.Z },
            {BLT.TK_1, Key.Number1 },
            {BLT.TK_2, Key.Number2 },
            {BLT.TK_3, Key.Number3 },
            {BLT.TK_4, Key.Number4 },
            {BLT.TK_5, Key.Number5 },
            {BLT.TK_6, Key.Number6 },
            {BLT.TK_7, Key.Number7 },
            {BLT.TK_8, Key.Number8 },
            {BLT.TK_9, Key.Number9 },
            {BLT.TK_0, Key.Number0 },
            {BLT.TK_PERIOD, Key.Period },
            {BLT.TK_COMMA, Key.Comma },
            {BLT.TK_SLASH, Key.Slash },
            {BLT.TK_BACKSLASH, Key.BackSlash },
            {BLT.TK_BACKSPACE, Key.BackSpace },
            {BLT.TK_DOWN, Key.Down },
            {BLT.TK_UP, Key.Up },
            {BLT.TK_LEFT, Key.Left },
            {BLT.TK_RIGHT, Key.Right },
            {BLT.TK_ENTER, Key.Enter }

        };

        public static Key FromBLTInput(int input)
        {
            if (mapping.ContainsKey(input))
                return mapping[input];
            else return Key.Unknown;
        }
    }
}