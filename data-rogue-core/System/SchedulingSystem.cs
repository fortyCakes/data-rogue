using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Interfaces;

public class SchedulingSystem
{
    private int _time;
    private readonly SortedDictionary<int, List<IScheduleable>> _scheduleables;

    public SchedulingSystem()
    {
        _time = 0;
        _scheduleables = new SortedDictionary<int, List<IScheduleable>>();
    }

    // Add a new object to the schedule 
    // Place it at the current time plus the object's Time property.
    public void Add(IScheduleable scheduleable)
    {
        int key = _time + scheduleable.Time;
        if (!_scheduleables.ContainsKey(key))
        {
            _scheduleables.Add(key, new List<IScheduleable>());
        }
        _scheduleables[key].Add(scheduleable);
    }

    // Remove a specific object from the schedule.
    // Useful for when an monster is killed to remove it before it's action comes up again.
    public void Remove(IScheduleable scheduleable)
    {
        KeyValuePair<int, List<IScheduleable>> scheduleableListFound
          = new KeyValuePair<int, List<IScheduleable>>(-1, null);

        foreach (var scheduleablesList in _scheduleables)
        {
            if (scheduleablesList.Value.Contains(scheduleable))
            {
                scheduleableListFound = scheduleablesList;
                break;
            }
        }
        if (scheduleableListFound.Value != null)
        {
            scheduleableListFound.Value.Remove(scheduleable);
            if (scheduleableListFound.Value.Count <= 0)
            {
                _scheduleables.Remove(scheduleableListFound.Key);
            }
        }
    }

    // Get the next object whose turn it is from the schedule. Advance time if necessary
    public IScheduleable Get()
    {
        var firstScheduleableGroup = _scheduleables.First();
        var firstScheduleable = firstScheduleableGroup.Value.First();
        Remove(firstScheduleable);
        _time = firstScheduleableGroup.Key;
        return firstScheduleable;
    }

    // Get the current time (turn) for the schedule
    public int GetTime()
    {
        return _time;
    }

    // Reset the time and clear out the schedule
    public void Clear()
    {
        _time = 0;
        _scheduleables.Clear();
    }
}