using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL.Services
{
    public class PollService
    {
        private readonly IRepository<Poll> _polls;
        private readonly IRepository<Event> _events;
        private readonly IEventNotifier _notifier;

        public PollService(IRepository<Poll> polls, IRepository<Event> events, IEventNotifier notifier)
        {
            _polls = polls;
            _events = events;
            _notifier = notifier;
        }

        public void CreatePoll(Poll poll, int eventId)
        {
            var ev = _events.GetById(eventId) ?? throw new InvalidOperationException("The event is undefined.");
            _polls.Add(poll);
            ev.PollIds.Add(poll.Id);
            _events.Update(ev);
            _notifier.RaisePollCreated(poll.Id);
        }

        public void AddQuestion(int pollId, PollQuestion question)
        {
            var poll = _polls.GetById(pollId) ?? throw new InvalidOperationException("The poll is undefined.");
            poll.Questions.Add(question);
            _polls.Update(poll);
        }

        public void SubmitAnswer(int pollId, int questionId, int participantId, string answer)
        {
            var poll = _polls.GetById(pollId) ?? throw new InvalidOperationException("The poll is undefined.");
            var question = poll.Questions.FirstOrDefault(q => q.Id == questionId)
                ?? throw new InvalidOperationException("The question is undefined.");

            question.SetAnswer(participantId, answer);
            _polls.Update(poll);
        }

        public (Poll poll, Dictionary<int, Dictionary<string, double>> resultsByQuestion) GetPollResults(int pollId)
        {
            var poll = _polls.GetById(pollId) ?? throw new InvalidOperationException("The poll is undefined.");

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
