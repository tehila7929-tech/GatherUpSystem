using GatherUp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO
{
    public class Event : IEntity
    {
        public int Id { get; set; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public DateTime Date { get; set; }
        public string Location { get; set; }

        public int HostId { get; set; }
        public int ManagerId { get; set; }

        public List<int> ParticipantIds { get; set; } = new();
        public List<int> VendorIds { get; set; } = new();
        public List<int> PollIds { get; set; } = new();
    }
}
