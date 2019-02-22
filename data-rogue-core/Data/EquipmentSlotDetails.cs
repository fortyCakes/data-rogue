using System;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Data
{
    public class EquipmentSlotDetails : ICustomFieldSerialization
    {
        public BodyPartType BodyPartType;
        public BodyPartLocation BodyPartLocation;
        public int Index = 1;

        public void Deserialize(string value)
        {
            var splits = value.Split('|');

            BodyPartType = (BodyPartType)Enum.Parse(typeof(BodyPartType), splits[0]);
            BodyPartLocation = (BodyPartLocation)Enum.Parse(typeof(BodyPartLocation), splits[1]);
            Index = int.Parse(splits[2]);
        }

        public string Serialize()
        {
            return ToString();
        }

        public override string ToString()
        {
            return $"{BodyPartType}|{BodyPartLocation}|{Index}";
        }

        public override bool Equals(object obj)
        {
            var asItem = obj as EquipmentSlotDetails;

            if (asItem == null) return false;

            return BodyPartType == asItem.BodyPartType && BodyPartLocation == asItem.BodyPartLocation && Index == asItem.Index;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = hash * 7 + BodyPartType.GetHashCode();
            hash = hash * 7 + BodyPartLocation.GetHashCode();
            hash = hash * 7 + Index.GetHashCode();

            return hash;
        }

        public static bool operator ==(EquipmentSlotDetails a, EquipmentSlotDetails b)
        {
            var aItem = a as EquipmentSlotDetails;
            var bItem = b as EquipmentSlotDetails;

            if (ReferenceEquals(a, null) && ReferenceEquals(bItem, null)) return true;
            if (ReferenceEquals(a, null) ^ ReferenceEquals(bItem, null)) return false;

            return aItem.Equals(bItem);
        }

        public static bool operator !=(EquipmentSlotDetails a, EquipmentSlotDetails b)
        {
            var aItem = a as EquipmentSlotDetails;
            var bItem = b as EquipmentSlotDetails;

            if (ReferenceEquals(a, null) && ReferenceEquals(bItem, null)) return false;
            if (ReferenceEquals(a, null) ^ ReferenceEquals(bItem, null)) return true;

            return !aItem.Equals(bItem);
        }

    }
}