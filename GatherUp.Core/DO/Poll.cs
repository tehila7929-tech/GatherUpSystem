using GatherUp.Core.Interfaces;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GatherUp.Core.DO
{
    public class Poll : IEntity
    {
        [XmlAttribute]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PollQuestion> Questions { get; set; } = new();
        public Poll() { }
    }
}
