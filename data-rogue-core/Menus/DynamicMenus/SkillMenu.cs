using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Systems;

namespace data_rogue_core.Menus.DynamicMenus
{
    public class SkillMenu : Menu
    {
        private ISystemContainer _systemContainer;
        private IEntity _entity;

        public override List<MenuAction> AvailableActions { get; set; } = new List<MenuAction> { MenuAction.Unequip };

        public SkillMenu(ISystemContainer systemContainer, IEntity skillEntity) : base(systemContainer.ActivitySystem, "Skills", null, GetSkillMenuItems(systemContainer, skillEntity))
        {
            _entity = skillEntity;
            _systemContainer = systemContainer;
            SelectedAction = MenuAction.Use;
            OnSelectCallback += DoSkillStuff;
        }

        private static MenuItem[] GetSkillMenuItems(ISystemContainer systemContainer, IEntity entity)
        {
            var cancelItem = new[] { new MenuItem("Cancel", null) };

            var skills = entity.Components.OfType<KnownSkill>();

            return skills.Select(i => ConvertSkillToMenuItem(i, systemContainer, entity)).Concat(cancelItem).ToArray();
        }

        private static MenuItem ConvertSkillToMenuItem(KnownSkill skill, ISystemContainer systemContainer, IEntity equippedEntity)
        {
            var skillEntity = systemContainer.SkillSystem.GetSkillFromKnown(skill);

            return new MenuItem($"{skillEntity.DescriptionName}", skill);
        }

        private void DoSkillStuff(MenuItem selectedItem, MenuAction selectedAction)
        {
            if (selectedItem.Text == "Cancel")
            {
                CloseActivity();
                return;
            }

            switch (selectedAction)
            {
                case MenuAction.Use:
                    CloseActivity();

                    KnownSkill knownSkill = (KnownSkill)selectedItem.Value;

                    ActionEventData actionEventData = new ActionEventData
                    {
                        Action = ActionType.UseSkill,
                        Parameters = knownSkill.Skill
                    };

                    _systemContainer.EventSystem.Try(EventType.Action, _entity, actionEventData);
                    
                    break;
                default:
                    throw new ApplicationException($"Unknown MenuAction in {nameof(SkillMenu)}");
            }
        }
    }
}
