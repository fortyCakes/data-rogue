using System;
using System.Reflection;
using RLNET;

namespace data_rogue_core.Display
{
    public static class Colors
    {
        public static RLColor FloorBackground = RLColor.Black;
        public static RLColor Floor = Swatch.AlternateDarkest;
        public static RLColor FloorBackgroundFov = Swatch.DbDark;
        public static RLColor FloorFov = Swatch.Alternate;

        public static RLColor WallBackground = Swatch.SecondaryDarkest;
        public static RLColor Wall = Swatch.Secondary;
        public static RLColor WallBackgroundFov = Swatch.SecondaryDarker;
        public static RLColor WallFov = Swatch.SecondaryLighter;

        public static RLColor TextHeading = RLColor.White;
        public static RLColor Text = Swatch.DbLight;
        public static RLColor Gold = Swatch.DbSun;

        public static RLColor DoorBackground = Swatch.ComplimentDarkest;
        public static RLColor Door = Swatch.ComplimentLighter;
        public static RLColor DoorBackgroundFov = Swatch.ComplimentDarker;
        public static RLColor DoorFov = Swatch.ComplimentLightest;

        public static RLColor Player = Swatch.DbLight;
        public static RLColor KoboldColor = Swatch.DbBrightWood;
        public static RLColor GoblinColor = Swatch.DbGrass;

        public static RLColor GetColor(string propertyName)
        {
            var prop = typeof(Colors).GetField(propertyName);

            return (RLColor)prop.GetValue(null);
        }
    }
}