using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO
{
    public class PollQuestion
    {
        public int Id { get; set; }
        public required string QuestionText { get; init; }
        public List<string> Options { get; set; } = new();
        public Dictionary<int, string> Answers { get; set; } = new();
    }
}
