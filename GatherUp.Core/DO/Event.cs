using GatherUp.Core.Interfaces;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GatherUp.Core.DO
{
    public class Event : IEntity
    {
        [XmlAttribute]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public int HostId { get; set; }
        public int ManagerId { get; set; }

        public List<int> ParticipantIds { get; set; } = new();
        public List<int> VendorIds { get; set; } = new();
        public List<int> PollIds { get; set; } = new();

        public Event() { }
    }
}
