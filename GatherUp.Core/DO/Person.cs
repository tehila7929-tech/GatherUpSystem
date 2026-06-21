using GatherUp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GatherUp.Core.DO
{
    public class Person : IEntity
    {
        [XmlAttribute]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordId { get; set; }
        public Person() { }
    }
}
