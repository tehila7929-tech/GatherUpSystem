using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL.Services
{
    public class NotificationSubscription
    {
        private readonly IRepository<Participant> _participants;
        private readonly IEmailService _email;

        public NotificationSubscription(IEventNotifier notifier, IRepository<Participant> participants, IEmailService email)
        {
            _participants = participants;
            _email = email;

            notifier.AttendanceConfirmed += OnAttendanceConfirmed;
            notifier.PaymentReceived += OnPaymentReceived;
            notifier.PollCreated += OnPollCreated;
            notifier.EventUpdated += OnEventUpdated;
        }

        private void OnAttendanceConfirmed(int participantId)
        {
            var p = _participants.GetById(participantId);
            if (p != null)
                _email.Send(p.Email, $"Attendance confirmed by {p.Name}");
        }

        private void OnPaymentReceived(int participantId, decimal amount)
        {
            var p = _participants.GetById(participantId);
            if (p != null)
                _email.Send(p.Email, $"Payment of {amount} received from {p.Name}");
        }

        private void OnPollCreated(int pollId)
        {
            foreach (var p in _participants.GetAll().Where(p => p.ContactPreference != ContactPreference.None))
                _email.Send(p.Email, $"A new poll has been created (ID: {pollId})");
        }

        private void OnEventUpdated(int eventId)
        {
            foreach (var p in _participants.GetAll().Where(p => p.ContactPreference != ContactPreference.None))
                _email.Send(p.Email, $"Event details have been updated (ID: {eventId})");
        }
    }
}
