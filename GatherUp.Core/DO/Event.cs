using GatherUp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GatherUp.Core.DO
{
    public class Event : IEntity
    {
        [XmlAttribute]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public string Location { get; set; }

        public int HostId { get; set; }
        public int ManagerId { get; set; }

        public List<int> ParticipantIds { get; set; } = new();
        public List<int> VendorIds { get; set; } = new();
        public List<int> PollIds { get; set; } = new();

        public Event() { }
    }
}
