using System;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Data
{
    public class EquipmentSlotDetails : ICustomFieldSerialization
    {
        public EquipmentSlot EquipmentSlot;
        public BodyPartType BodyPartType;
        public BodyPartLocation BodyPartLocation;
        public int Index = 1;

        public EquipmentSlotDetails()
        {
            // Only use for serialization
        }

        public EquipmentSlotDetails(EquipmentSlot slot, BodyPartType bodyPartType, BodyPartLocation location, int index)
        {
            EquipmentSlot = slot;
            BodyPartType = bodyPartType;
            BodyPartLocation = location;
            Index = index;
        }

        public void Deserialize(string value)
        {
            var splits = value.Split('|');

            EquipmentSlot = (EquipmentSlot)Enum.Parse(typeof(EquipmentSlot), splits[0]);
            BodyPartType = (BodyPartType)Enum.Parse(typeof(BodyPartType), splits[1]);
            BodyPartLocation = (BodyPartLocation)Enum.Parse(typeof(BodyPartLocation), splits[2]);
            Index = int.Parse(splits[3]);
        }

        public string Serialize()
        {
            return ToString();
        }

        public override string ToString()
        {
            return $"{EquipmentSlot}|{BodyPartType}|{BodyPartLocation}|{Index}";
        }

        public override bool Equals(object obj)
        {
            var asItem = obj as EquipmentSlotDetails;

            if (asItem == null) return false;

            return EquipmentSlot == asItem.EquipmentSlot && BodyPartType == asItem.BodyPartType && BodyPartLocation == asItem.BodyPartLocation && Index == asItem.Index;
        }

        public override int GetHashCode()
        {
            var hashCode = 1673211637;
            hashCode = hashCode * -1521134295 + EquipmentSlot.GetHashCode();
            hashCode = hashCode * -1521134295 + BodyPartType.GetHashCode();
            hashCode = hashCode * -1521134295 + BodyPartLocation.GetHashCode();
            hashCode = hashCode * -1521134295 + Index.GetHashCode();
            return hashCode;
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