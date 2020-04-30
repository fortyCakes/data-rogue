using System;

[Flags]
public enum TargetingStatus
{
    NotTargeted = 0,
    Targetable = 1,
    Targeted = 2,
    MoveTo = 4
}