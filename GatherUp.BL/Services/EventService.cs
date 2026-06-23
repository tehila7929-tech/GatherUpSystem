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
            var all = _events.GetAll();
            if (newEvent.Id == 0 || all.Any(e => e.Id == newEvent.Id))
                newEvent.Id = all.Any() ? all.Max(e => e.Id) + 1 : 1;
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

        public IEnumerable<object> GetAllEventsForUser(HashSet<int> ids)
        {
            return _events.GetAll()
                .Where(e => ids.Contains(e.ManagerId) || (e.HostId.HasValue && ids.Contains(e.HostId.Value)) || e.ParticipantIds.Any(ids.Contains))
                .Select(e =>
                {
                    string role = ids.Contains(e.ManagerId)                              ? "Manager" :
                                  e.HostId.HasValue && ids.Contains(e.HostId.Value)      ? "Host"    : "Participant";
                    return (object)new { Event = e, Role = role };
                });
        }
    }
}
