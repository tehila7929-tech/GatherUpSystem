using GatherUp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO
{
    public class Poll : IEntity
    {
        public int Id {  get; set; }
        public required  string Name { get; init; }
        public required  string Description { get; init; }
        public List<PollQuestion> Questions { get; set; } = new();

    }
}
