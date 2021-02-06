namespace data_rogue_core.EventSystem.EventData
{
    public enum ActionType
    {
        None = 0,
        Wait,
        InventoryMenu,
        Select,
        MeleeAttack,
        Save,
        Enter,
        Move,
        EquipmentMenu,
        GetItem,
        WaitForInput,
        EscapeMenu,
        SkillMenu,
        UseSkill,
        RangedAttack,
        Rest,
        Examine,
        ResolveRangedAttack,
        FollowPath,
        PlayerStatus,
        Morgue,
        Interact,
        NextInteraction,
        Hotbar,
        DebugMenu,
        ChangeMapEditorTool,
        ChangeMapEditorCell,
        Open,
        ChangeMapEditorDefaultCell
    }
}