using GatherUp.Core.Interfaces;
using GatherUp.Infrastructure.Memory; 
using GatherUp.Core.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GatherUp.Infrastructure.Data
{
    //לשנות לאנשים אמיתיים עם מיילים זמינים!!
    public static class SeedData
    {
        public static EventManager Manager => new EventManager
        {
            Id = 1,
            Name = "Tiferet Aaroni",
            Email = "t7187325@gmail.com"
        };

        public static EventHost Host => new EventHost
        {
            Id = 2,
            Name = "Tehila Goldshmidt",
            Email = "tehila7929@gmail.com"
        };

        public static List<Participant> Participants => new List<Participant>
        {
            new Participant { Id = 3, Name = "אברהם לוי", Email = "avi@gmail.com", IsAttending = true, HasPaid = true, AmountContributed = 150 },
            new Participant { Id = 4, Name = "שרה רבקה", Email = "sara@gmail.com", IsAttending = null, HasPaid = false, AmountContributed = 0 }
        };

        public static VendorAllocation Vendor => new VendorAllocation
        {
            Id = 1,
            Name = "קייטרינג איכותי",
            AmountOwed = 4500,
            HasReceipt = false,
            Receipts = new List<ReceiptDetails>()
        };

        public static List<Poll> Polls => new List<Poll>
        {
            new Poll
            {
                Id = 1,
                Name = "סקר פרטים התחלתיים",
                Description = "בחירת תאריך ומיקום ראשוני",
                Questions = new List<PollQuestion>
                {
                    new PollQuestion { QuestionText = "מהו התאריך המועדף עליך?", Options = new List<string> { "01/06", "15/06" } },
                    new PollQuestion { QuestionText = "איזה מיקום עדיף?", Options = new List<string> { "ירושלים", "תל אביב" } }
                }
            },
            new Poll
            {
                Id = 2,
                Name = "סקר המשך - אוכל",
                Description = "בחירת תפריט מועדף לאירוע",
                Questions = new List<PollQuestion>
                {
                    new PollQuestion { QuestionText = "איזו מנה עיקרית תעדיפו?", Options = new List<string> { "בשר", "דג" } }
                }
            }
        };

        public static Event Event => new Event
        {
            Id = 1,
            Name = "אירוע גיבוש GatherUp",
            Description = "אירוע פתיחה חגיגי",
            ParticipantIds = new List<int> { 3, 4 },
            ManagerId = 1,
            HostId = 2,
            VendorIds = new List<int> { 1 },
            PollIds = new List<int> { 1, 2 }
        };
    }
}
