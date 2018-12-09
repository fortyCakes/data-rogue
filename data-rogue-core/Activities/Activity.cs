﻿namespace data_rogue_core.Activities
{
    public interface IActivity
    {
        ActivityType Type { get; }
        object Data { get; }
        bool RendersEntireSpace { get; }
        void Render();
    }
}