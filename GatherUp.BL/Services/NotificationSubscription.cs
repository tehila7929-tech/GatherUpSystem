using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL.Services
{
    /// <summary>
    /// Singleton service that subscribes to domain events and sends email notifications.
    /// Must be resolved once at startup to register all event handlers.
    /// </summary>
    public class NotificationSubscription
    {
        private readonly IRepository<Participant> _participants;
        private readonly IRepository<EventManager> _managers;
        private readonly IEmailService _email;

        public NotificationSubscription(
            IEventNotifier notifier,
            IRepository<Participant> participants,
            IRepository<EventManager> managers,
            IEmailService email)
        {
            _participants = participants;
            _managers = managers;
            _email = email;

            notifier.AttendanceConfirmed += OnAttendanceConfirmed;
            notifier.PaymentReceived += OnPaymentReceived;
            notifier.PollCreated += OnPollCreated;
            notifier.EventUpdated += OnEventUpdated;
        }

        // Manager gets notified when a participant confirms attendance
        private void OnAttendanceConfirmed(int participantId)
        {
            var p = _participants.GetById(participantId);
            if (p == null) return;

            foreach (var manager in _managers.GetAll())
                _email.Send(manager.Email, $"✅ {p.Name} confirmed attendance.");
        }

        // Manager gets notified when a payment is made
        private void OnPaymentReceived(int participantId, decimal amount)
        {
            var p = _participants.GetById(participantId);
            if (p == null) return;

            foreach (var manager in _managers.GetAll())
                _email.Send(manager.Email, $"💰 Payment of ₪{amount} received from {p.Name}.");
        }

        // Participants who subscribed to notifications get notified on new poll
        private void OnPollCreated(int pollId)
        {
            _participants.GetAll()
                .Where(p => p.ContactPreference != ContactPreference.None)
                .ToList()
                .ForEach(p => _email.Send(p.Email, $"📊 A new poll has been created (Poll ID: {pollId}). Log in to vote!"));
        }

        // Participants who subscribed get notified on event update
        private void OnEventUpdated(int eventId)
        {
            _participants.GetAll()
                .Where(p => p.ContactPreference != ContactPreference.None)
                .ToList()
                .ForEach(p => _email.Send(p.Email, $"📢 Event details have been updated (Event ID: {eventId}). Check the latest info!"));
        }
    }
}
