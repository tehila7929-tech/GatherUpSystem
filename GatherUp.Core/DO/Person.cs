using GatherUp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO
{
    public class Person : IEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; init; }
    }
}
