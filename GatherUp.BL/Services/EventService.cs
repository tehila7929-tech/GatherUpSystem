using GatherUp.Core.DO;
using GatherUp.Core.Exceptions;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL.Services
{
    public class EventService
    {
        private readonly IRepository<Event> _events;
        private readonly IEventNotifier _notifier;

        public EventService(IRepository<Event> events, IEventNotifier notifier)
        {
            _events = events;
            _notifier = notifier;
        }

        public void CreateEvent(Event newEvent)
        {
            _events.Add(newEvent);
        }

        public Event GetById(int eventId)
        {
            return _events.GetById(eventId) ?? throw new NotFoundException("Event not found.");
        }

        public void UpdateEventDetails(Event updatedEvent)
        {
            if (_events.GetById(updatedEvent.Id) == null)
                throw new NotFoundException("Event not found.");
            _events.Update(updatedEvent);
            _notifier.RaiseEventUpdated(updatedEvent.Id);
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
