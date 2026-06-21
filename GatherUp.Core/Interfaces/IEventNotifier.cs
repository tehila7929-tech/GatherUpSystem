namespace GatherUp.Core.Interfaces
{
    public interface IEventNotifier
    {
        event Action<int> AttendanceConfirmed;
        event Action<int, decimal> PaymentReceived;
        event Action<int> PollCreated;
        event Action<int> EventUpdated;

        void RaiseAttendanceConfirmed(int participantId);
        void RaisePaymentReceived(int participantId, decimal amount);
        void RaisePollCreated(int pollId);
        void RaiseEventUpdated(int eventId);
    }
}
