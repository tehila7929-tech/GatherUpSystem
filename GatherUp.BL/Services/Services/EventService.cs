using GatherUp.Core.Interfaces;
using GatherUp.Core.DO;
using System.Collections.Generic;
using System.Linq;

namespace GatherUp.BL.Services
{
    public class EventService
    {
        private readonly IRepository<Event> _events;

        public EventService(IRepository<Event> events)
        {
            _events = events;
        }

        public void CreateEvent(Event newEvent)
        {
            _events.Add(newEvent);
        }

        public void UpdateEventDetails(Event updatedEvent)
        {
            if (_events.GetById(updatedEvent.Id) == null)
                throw new InvalidOperationException("האירוע לא נמצא.");
            _events.Update(updatedEvent);
        }

        public Dictionary<Event, UserRole> GetAllEventsForUser(int userId)
        {
            return _events.GetAll()
                .Where(e => e.ManagerId == userId || e.HostId == userId || e.ParticipantIds.Contains(userId))
                .ToDictionary(
                    e => e,
                    e => e.ManagerId == userId ? UserRole.Manager :
                         e.HostId == userId ? UserRole.Host :
                         UserRole.Participant
                );
        }
    }
}
