using GatherUp.Core.DO;
using GatherUp.Core.Exceptions;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL.Services
{
    public class ParticipantService
    {
        private readonly IRepository<Participant> _participants;
        private readonly IRepository<Event> _events;
        private readonly IEventNotifier _notifier;
        private readonly IEmailService _email;

        public ParticipantService(
            IRepository<Participant> participants,
            IRepository<Event> events,
            IEventNotifier notifier,
            IEmailService email)
        {
            _participants = participants;
            _events = events;
            _notifier = notifier;
            _email = email;
        }

        public void AddParticipantToEvent(Participant participant, int eventId)
        {
            var ev = _events.GetById(eventId) ?? throw new NotFoundException("Event not found.");

            if (ev.ParticipantIds.Contains(participant.Id))
                throw new AlreadyExistsException("Participant is already registered for this event.");

            _participants.Add(participant);
            ev.ParticipantIds.Add(participant.Id);
            _events.Update(ev);
        }

        public void ConfirmAttendance(int participantId, bool isAttending)
        {
            var participant = _participants.GetById(participantId) ?? throw new NotFoundException("Participant not found.");
            participant.IsAttending = isAttending;
            _participants.Update(participant);
            if (isAttending) _notifier.RaiseAttendanceConfirmed(participantId);
        }

        public IEnumerable<Participant> GetEventParticipants(int eventId)
        {
            var ev = _events.GetById(eventId) ?? throw new NotFoundException("Event not found.");
            return _participants.GetAll().Where(p => ev.ParticipantIds.Contains(p.Id));
        }

        public Participant GetParticipant(int participantId)
        {
            return _participants.GetById(participantId) ?? throw new NotFoundException("Participant not found.");
        }

        public void ConfirmPayment(int participantId, decimal amount)
        {
            var participant = _participants.GetById(participantId) ?? throw new NotFoundException("Participant not found.");
            participant.HasPaid = true;
            participant.AmountContributed = amount;
            _participants.Update(participant);
            _notifier.RaisePaymentReceived(participantId, amount);
        }

        public IEnumerable<Participant> GetPendingParticipants(int eventId)
        {
            var ev = _events.GetById(eventId) ?? throw new NotFoundException("Event not found.");
            return _participants.GetAll().Where(p => ev.ParticipantIds.Contains(p.Id) && p.IsAttending == null);
        }

        /// <summary>
        /// Sends invitation emails to all participants who have not yet responded.
        /// </summary>
        public void SendInvitations(int eventId, string eventDescription, string bankDetails)
        {
            var ev = _events.GetById(eventId) ?? throw new NotFoundException("Event not found.");

            _participants.GetAll()
                .Where(p => ev.ParticipantIds.Contains(p.Id) && p.IsAttending == null)
                .ToList()
                .ForEach(p => _email.Send(p.Email,
                    $"📩 You are invited to: {ev.Name}\n" +
                    $"{eventDescription}\n" +
                    $"Please confirm your attendance via the system.\n" +
                    $"Payment details: {bankDetails}"));
        }

        /// <summary>
        /// Sends payment reminders to all participants who confirmed attendance but have not paid.
        /// </summary>
        public void SendPaymentReminders(int eventId, string bankDetails)
        {
            var ev = _events.GetById(eventId) ?? throw new NotFoundException("Event not found.");

            _participants.GetAll()
                .Where(p => ev.ParticipantIds.Contains(p.Id) && p.IsAttending == true && !p.HasPaid)
                .ToList()
                .ForEach(p => _email.Send(p.Email,
                    $"⏰ Reminder: Payment is due for event '{ev.Name}'.\n" +
                    $"Bank transfer details: {bankDetails}\n" +
                    $"Please complete your payment as soon as possible."));
        }

        public void UpdateContactPreference(int participantId, ContactPreference preference)
        {
            var participant = _participants.GetById(participantId) ?? throw new NotFoundException("Participant not found.");
            participant.ContactPreference = preference;
            _participants.Update(participant);
        }
    }
}
