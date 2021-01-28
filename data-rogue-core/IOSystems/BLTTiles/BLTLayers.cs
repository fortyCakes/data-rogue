using BearLib;
using System;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public static class BLTLayers
    {
        public static int Base = 0;
        public static int Background = 1;
        public static int MapTileBottom = 2;
        public static int MapTileTop = 3;
        public static int MapEntityBottom = 4;
        public static int MapEntityTop = 5;
        public static int MapParticles = 6;
        public static int MapShade = 8;

        public static int UIElements = 12;
        public static int UIElementPieces = 13;
        public static int UIMasks = 14;
        public static int Text = 15;
        public static int Top = 24;

        public static int FULL_LAYER_SIZE = 25;

        public static void Set(int layer, int activityIndex)
        {
            if (activityIndex > 11)
            {
                throw new ApplicationException("Can't have more than 11 layers of activities");
            }

            BLT.Layer(layer + activityIndex * FULL_LAYER_SIZE);
        }
    }
}