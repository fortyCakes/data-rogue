using System;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class EnterAction: ApplyActionRule
    {
        public EnterAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.Enter;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var mapCoordinate = sender.Get<Position>().MapCoordinate;
            var direction = (StairDirection)Enum.Parse(typeof(StairDirection), eventData.Parameters);

            var stairs = _systemContainer.PositionSystem
                .EntitiesAt(mapCoordinate)
                .Where(e => e.Has<Stairs>())
                .Select(e => e.Get<Stairs>())
                .SingleOrDefault();

            if (stairs != null && stairs.Direction == direction)
            {
                if (_systemContainer.EventSystem.Try(EventType.ChangeFloor, sender, direction))
                {
                    sender.Get<Position>().MapCoordinate = stairs.Destination;
                }
            }
            else
            {
                var portal = _systemContainer.PositionSystem
                    .EntitiesAt(mapCoordinate)
                    .Where(e => e.Has<Portal>())
                    .Select(e => e.Get<Portal>())
                    .SingleOrDefault();

                if (portal != null)
                {
                    if (_systemContainer.EventSystem.Try(EventType.UsePortal, sender, portal))
                    {
                        sender.Get<Position>().MapCoordinate = portal.Destination;
                    }
                }
            }

            return true;
        }
    }
}
