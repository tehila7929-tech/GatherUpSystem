using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL.Services
{
    public class ParticipantService
    {
        private readonly IRepository<Participant> _participants;
        private readonly IRepository<Event> _events;
        private readonly IEventNotifier _notifier;

        public ParticipantService(IRepository<Participant> participants, IRepository<Event> events, IEventNotifier notifier)
        {
            _participants = participants;
            _events = events;
            _notifier = notifier;
        }

        public void AddParticipantToEvent(Participant participant, int eventId)
        {
            var ev = _events.GetById(eventId) ?? throw new InvalidOperationException("Event not found.");

            if (ev.ParticipantIds.Contains(participant.Id))
                throw new InvalidOperationException("Participant is already registered for this event.");

            _participants.Add(participant);
            ev.ParticipantIds.Add(participant.Id);
            _events.Update(ev);
        }

        public void ConfirmAttendance(int participantId, bool isAttending)
        {
            var participant = _participants.GetById(participantId) ?? throw new InvalidOperationException("Participant not found.");
            participant.IsAttending = isAttending;
            _participants.Update(participant);
            if (isAttending) _notifier.RaiseAttendanceConfirmed(participantId);
        }

        public IEnumerable<Participant> GetEventParticipants(int eventId)
        {
            var ev = _events.GetById(eventId) ?? throw new InvalidOperationException("Event not found.");
            return _participants.GetAll().Where(p => ev.ParticipantIds.Contains(p.Id));
        }

        public Participant GetParticipant(int participantId)
        {
            return _participants.GetById(participantId) ?? throw new InvalidOperationException("Participant not found.");
        }

        public void ConfirmPayment(int participantId, decimal amount)
        {
            var participant = _participants.GetById(participantId) ?? throw new InvalidOperationException("Participant not found.");
            participant.HasPaid = true;
            participant.AmountContributed = amount;
            _participants.Update(participant);
            _notifier.RaisePaymentReceived(participantId, amount);
        }

        public IEnumerable<Participant> GetPendingParticipants(int eventId)
        {
            var ev = _events.GetById(eventId) ?? throw new InvalidOperationException("Event not found.");
            return _participants.GetAll().Where(p => ev.ParticipantIds.Contains(p.Id) && p.IsAttending == null);
        }
    }
}
