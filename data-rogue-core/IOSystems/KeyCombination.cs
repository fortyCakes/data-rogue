using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using data_rogue_core.EntityEngineSystem;
using OpenTK.Input;

namespace data_rogue_core.IOSystems
{
    public class KeyCombination : ICustomFieldSerialization, IEquatable<KeyCombination>
    {
        public Key Key;
        public bool Shift;
        public bool Ctrl;
        public bool Alt;

        public void Deserialize(string value)
        {
            if (value.StartsWith("Ctrl+"))
            {
                value = value.Remove(0, 5);
                Ctrl = true;
            }

            if (value.StartsWith("Shift+"))
            {
                value = value.Remove(0, 6);
                Shift = true;
            }

            if (value.StartsWith("Alt+"))
            {
                value = value.Remove(0, 4);
                Alt = true;
            }

            Key = (Key)Enum.Parse(typeof(Key), value);
        }

        public string Serialize()
        {
            return (Ctrl ? "Ctrl+" : "") + (Shift ? "Shift+" : "") + (Alt ? "Alt+" : "") + Key.ToString();
        }

        public char? ToChar()
        {
            if (Ctrl || Alt)
            {
                return null;
            }

            if (Key >= Key.A && Key <= Key.Z)
            {
                var theChar = Key.ToString();

                if (Shift) return theChar.ToUpper().First(); else return theChar.ToLower().First();
            }
            
            switch (Key)
            {
                case Key.Semicolon: return Shift ? ':' : ';';
                case Key.Comma: return Shift ? '<' : ',';
                case Key.Period: return Shift ? '>' : '.';
            }

            return null;
        }

        public bool Equals(KeyCombination other)
        {
            if (this == other) return true;

            if (this == null || other == null) return false;

            return this.Key == other.Key && this.Shift == other.Shift && this.Ctrl == other.Ctrl && this.Alt == other.Alt;
        }
    }
}