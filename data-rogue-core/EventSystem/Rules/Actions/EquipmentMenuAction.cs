﻿using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Menus.DynamicMenus;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class EquipmentMenuAction : ApplyActionRule
    {
        public EquipmentMenuAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.EquipmentMenu;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            _systemContainer.ActivitySystem.Push(new MenuActivity(new EquipmentMenu(_systemContainer, sender), _systemContainer.RendererSystem.RendererFactory));

            return false;
        }
    }
}
