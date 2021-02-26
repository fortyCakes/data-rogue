using System;

namespace data_rogue_core.Data
{
    public class AlwaysCreateNewInstanceAttribute : Attribute
    {
        // Component fields with this attribute are always created fresh when the PrototypeSystem makes an instance
        // of the entity. It is used to stop Inventory and Equipment lists from being shared between entities.
    }
}