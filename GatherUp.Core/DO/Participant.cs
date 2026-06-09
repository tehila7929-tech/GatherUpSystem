using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO
{
    public enum ContactPreference
    {
        None=0,
        Email=1,
        SMS=2,
        WhatsApp=4
    }

    public class Participant : Person
    {
        public bool? IsAttending { get; set; }
        public bool HasPaid { get; set; }
        public decimal AmountContributed { get; set; }
        public ContactPreference ContactPreference { get; set; }
        public Participant() { }
    }
}
