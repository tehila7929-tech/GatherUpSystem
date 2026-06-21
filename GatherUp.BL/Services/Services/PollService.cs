using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL.Services
{
    public class PollService
    {
        private readonly IRepository<Poll> _polls;
        private readonly IRepository<Event> _events;

        public PollService(IRepository<Poll> polls, IRepository<Event> events)
        {
            _polls = polls;
            _events = events;
        }

        public void CreatePoll(Poll poll, int eventId)
        {
            var ev = _events.GetById(eventId) ?? throw new InvalidOperationException("האירוע לא נמצא.");
            _polls.Add(poll);
            ev.PollIds.Add(poll.Id);
            _events.Update(ev);
        }

        public void AddQuestion(int pollId, PollQuestion question)
        {
            var poll = _polls.GetById(pollId) ?? throw new InvalidOperationException("הסקר לא נמצא.");
            poll.Questions.Add(question);
            _polls.Update(poll);
        }

        public void SubmitAnswer(int pollId, int questionId, int participantId, string answer)
        {
            var poll = _polls.GetById(pollId) ?? throw new InvalidOperationException("הסקר לא נמצא.");
            var question = poll.Questions.FirstOrDefault(q => q.Id == questionId)
                ?? throw new InvalidOperationException("השאלה לא נמצאת.");

            question.SetAnswer(participantId, answer);
            _polls.Update(poll);
        }

        public (Poll poll, Dictionary<int, Dictionary<string, double>> resultsByQuestion) GetPollResults(int pollId)
        {
            var poll = _polls.GetById(pollId) ?? throw new InvalidOperationException("הסקר לא נמצא.");

            var results = poll.Questions.ToDictionary(
                q => q.Id,
                q =>
                {
                    int totalAnswers = q.Answers.Count;
                    return q.Options.ToDictionary(
                        option => option,
                        option => totalAnswers == 0 ? 0.0 :
                            Math.Round(q.Answers.Count(a => a.Answer == option) * 100.0 / totalAnswers, 1)
                    );
                }
            );

            return (poll, results);
        }
    }
}
