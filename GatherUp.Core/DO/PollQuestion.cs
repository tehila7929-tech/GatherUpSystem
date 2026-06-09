using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO
{
    public class PollAnswer
    {
        public int ParticipantId { get; set; }
        public string Answer { get; set; } = string.Empty;
    }

    public class PollQuestion
    {
        public int Id { get; set; }
        public required string QuestionText { get; set; }
        public List<string> Options { get; set; } = new();
        public List<PollAnswer> Answers { get; set; } = new();
        public PollQuestion() { }
    }
}
