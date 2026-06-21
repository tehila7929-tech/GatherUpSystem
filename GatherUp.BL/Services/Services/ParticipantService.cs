using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL.Services
{
    public class ParticipantService
    {
        private readonly IRepository<Participant> _participants;
        private readonly IRepository<Event> _events;

        public ParticipantService(IRepository<Participant> participants, IRepository<Event> events)
        {
            _participants = participants;
            _events = events;
        }

        public void AddParticipantToEvent(Participant participant, int eventId)
        {
            var ev = _events.GetById(eventId) ?? throw new InvalidOperationException("האירוע לא נמצא.");

            if (ev.ParticipantIds.Contains(participant.Id))
                throw new InvalidOperationException("המשתתף כבר רשום לאירוע.");

            _participants.Add(participant);
            ev.ParticipantIds.Add(participant.Id);
            _events.Update(ev);
        }

        public void ConfirmAttendance(int participantId, bool isAttending)
        {
            var participant = _participants.GetById(participantId) ?? throw new InvalidOperationException("המשתתף לא נמצא.");
            participant.IsAttending = isAttending;
            _participants.Update(participant);
        }

        public IEnumerable<Participant> GetEventParticipants(int eventId)
        {
            var ev = _events.GetById(eventId) ?? throw new InvalidOperationException("האירוע לא נמצא.");
            return _participants.GetAll().Where(p => ev.ParticipantIds.Contains(p.Id));
        }

        public Participant GetParticipant(int participantId)
        {
            return _participants.GetById(participantId) ?? throw new InvalidOperationException("המשתתף לא נמצא.");
        }

        public void ConfirmPayment(int participantId, decimal amount)
        {
            var participant = _participants.GetById(participantId) ?? throw new InvalidOperationException("המשתתף לא נמצא.");
            participant.HasPaid = true;
            participant.AmountContributed = amount;
            _participants.Update(participant);
        }

        public IEnumerable<Participant> GetPendingParticipants(int eventId)
        {
            var ev = _events.GetById(eventId) ?? throw new InvalidOperationException("האירוע לא נמצא.");
            return _participants.GetAll().Where(p => ev.ParticipantIds.Contains(p.Id) && p.IsAttending == null);
        }
    }
}
