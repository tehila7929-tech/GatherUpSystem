using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure
{
    public class EventNotifier : IEventNotifier
    {
        public event Action<int>? AttendanceConfirmed;
        public event Action<int, decimal>? PaymentReceived;
        public event Action<int>? PollCreated;
        public event Action<int>? EventUpdated;

        public void RaiseAttendanceConfirmed(int participantId) => AttendanceConfirmed?.Invoke(participantId);
        public void RaisePaymentReceived(int participantId, decimal amount) => PaymentReceived?.Invoke(participantId, amount);
        public void RaisePollCreated(int pollId) => PollCreated?.Invoke(pollId);
        public void RaiseEventUpdated(int eventId) => EventUpdated?.Invoke(eventId);
    }
}
