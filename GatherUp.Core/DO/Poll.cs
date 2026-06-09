using GatherUp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GatherUp.Core.DO
{
    public class Poll : IEntity
    {
        [XmlAttribute]
        public int Id {  get; set; }
        public required  string Name { get; set; }
        public required  string Description { get; set; }
        public List<PollQuestion> Questions { get; set; } = new();

    }
}
