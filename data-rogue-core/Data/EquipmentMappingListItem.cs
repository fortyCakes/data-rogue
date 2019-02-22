using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Data
{
    public class EquipmentMappingListItem : ICustomFieldSerialization
    {
        public EquipmentSlotDetails Slot;
        public uint EquipmentId;

        public void Deserialize(string value)
        {
            var splits = value.Split(':');
            Slot = new EquipmentSlotDetails();
            Slot.Deserialize(splits[0]);
            EquipmentId = uint.Parse(splits[1]);
        }

        public override bool Equals(object obj)
        {
            var asItem = obj as EquipmentMappingListItem;

            if (asItem == null) return false;

            return Slot == asItem.Slot && EquipmentId == asItem.EquipmentId;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = hash * 7 + Slot.GetHashCode();
            hash = hash * 7 + EquipmentId.GetHashCode();

            return hash;
        }

        public string Serialize()
        {
            return ToString();
        }

        public override string ToString()
        {
            return $"{Slot.Serialize()}:{EquipmentId}";
        }

        public static bool operator ==(EquipmentMappingListItem a, EquipmentMappingListItem b)
        {
            var aItem = a as EquipmentMappingListItem;
            var bItem = b as EquipmentMappingListItem;

            if (ReferenceEquals(a, null) && ReferenceEquals(bItem, null)) return true;
            if (ReferenceEquals(a, null) ^ ReferenceEquals(bItem, null)) return false;

            return aItem.Equals(bItem);
        }

        public static bool operator !=(EquipmentMappingListItem a, EquipmentMappingListItem b)
        {
            var aItem = a as EquipmentMappingListItem;
            var bItem = b as EquipmentMappingListItem;

            if (ReferenceEquals(a, null) && ReferenceEquals(bItem, null)) return false;
            if (ReferenceEquals(a, null) ^ ReferenceEquals(bItem, null)) return true;

            return !aItem.Equals(bItem);
        }
    }
}
