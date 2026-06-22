using GatherUp.Core.DO;

namespace GatherUp.Infrastructure.Data
{
    /// <summary>
    /// Static seed data for initial system state.
    /// Note: use real email addresses before submission.
    ///
    /// User roles per event:
    ///   Tiferet (Id=1) - Manager of Event 1, Participant in Event 2
    ///   Rivka   (Id=5) - Manager of Event 2, Participant in Event 1
    ///   Tehila  (Id=2) - Host of both events
    ///   Avi     (Id=3) - Participant in Event 1
    ///   Sara    (Id=4) - Participant in Event 1
    ///   Yossi   (Id=6) - Participant in Event 2
    /// </summary>
    public static class SeedData
    {
        public static EventManager Manager1 => new EventManager
        {
            Id = 1,
            Name = "Tiferet Aaroni",
            Email = "t7187325@gmail.com",
            PasswordId = "123456789"
        };

        public static EventManager Manager2 => new EventManager
        {
            Id = 5,
            Name = "Rivka Cohen",
            Email = "rivka.cohen@gmail.com",
            PasswordId = "555555555"
        };

        public static EventHost Host => new EventHost
        {
            Id = 2,
            Name = "Tehila Goldshmidt",
            Email = "tehila7929@gmail.com",
            PasswordId = "987654321"
        };

        public static List<Participant> Participants => new List<Participant>
        {
            new Participant
            {
                Id = 3,
                Name = "Avi Levi",
                Email = "avi@gmail.com",
                PasswordId = "111111111",
                IsAttending = true,
                HasPaid = true,
                AmountContributed = 150,
                ContactPreference = ContactPreference.Email
            },
            new Participant
            {
                Id = 4,
                Name = "Sara Rivka",
                Email = "sara@gmail.com",
                PasswordId = "222222222",
                IsAttending = null,
                HasPaid = false,
                AmountContributed = 0,
                ContactPreference = ContactPreference.None
            },
            // Tiferet is Manager of Event1 but also a Participant in Event2
            new Participant
            {
                Id = 10,
                Name = "Tiferet Aaroni",
                Email = "t7187325@gmail.com",
                PasswordId = "123456789",
                IsAttending = true,
                HasPaid = true,
                AmountContributed = 200,
                ContactPreference = ContactPreference.Email
            },
            // Rivka is Manager of Event2 but also a Participant in Event1
            new Participant
            {
                Id = 11,
                Name = "Rivka Cohen",
                Email = "rivka.cohen@gmail.com",
                PasswordId = "555555555",
                IsAttending = true,
                HasPaid = false,
                AmountContributed = 0,
                ContactPreference = ContactPreference.Email
            },
            new Participant
            {
                Id = 6,
                Name = "Yossi Mizrahi",
                Email = "yossi@gmail.com",
                PasswordId = "666666666",
                IsAttending = null,
                HasPaid = false,
                AmountContributed = 0,
                ContactPreference = ContactPreference.Email
            }
        };

        public static List<VendorAllocation> Vendors => new List<VendorAllocation>
        {
            new VendorAllocation
            {
                Id = 1,
                Name = "Quality Catering",
                AmountOwed = 4500,
                HasReceipt = false,
                Receipts = new List<ReceiptDetails>()
            },
            new VendorAllocation
            {
                Id = 2,
                Name = "Event Photographer",
                AmountOwed = 2000,
                HasReceipt = false,
                Receipts = new List<ReceiptDetails>()
            }
        };

        public static List<Poll> Polls => new List<Poll>
        {
            // Event 1 polls
            new Poll
            {
                Id = 1,
                Name = "Initial Details Poll",
                Description = "Choose preferred date and location for Event 1",
                Questions = new List<PollQuestion>
                {
                    new PollQuestion
                    {
                        Id = 1,
                        QuestionText = "Which date do you prefer?",
                        Options = new List<string> { "June 1", "June 15", "June 29" },
                        Answers = new List<PollAnswer>
                        {
                            new PollAnswer { ParticipantId = 3, Answer = "June 15" }
                        }
                    },
                    new PollQuestion
                    {
                        Id = 2,
                        QuestionText = "Which location do you prefer?",
                        Options = new List<string> { "Jerusalem", "Tel Aviv", "Haifa" },
                        Answers = new List<PollAnswer>
                        {
                            new PollAnswer { ParticipantId = 3, Answer = "Tel Aviv" }
                        }
                    }
                }
            },
            new Poll
            {
                Id = 2,
                Name = "Food Preferences Poll",
                Description = "Choose the menu style for Event 1",
                Questions = new List<PollQuestion>
                {
                    new PollQuestion
                    {
                        Id = 3,
                        QuestionText = "Which main course do you prefer?",
                        Options = new List<string> { "Meat", "Fish", "Vegetarian" },
                        Answers = new List<PollAnswer>()
                    }
                }
            },
            // Event 2 polls
            new Poll
            {
                Id = 3,
                Name = "Event 2 – Venue Poll",
                Description = "Choose the venue for the team gathering",
                Questions = new List<PollQuestion>
                {
                    new PollQuestion
                    {
                        Id = 4,
                        QuestionText = "Indoor or outdoor?",
                        Options = new List<string> { "Indoor hall", "Open garden", "Rooftop" },
                        Answers = new List<PollAnswer>
                        {
                            new PollAnswer { ParticipantId = 10, Answer = "Open garden" }
                        }
                    }
                }
            },
            new Poll
            {
                Id = 4,
                Name = "Event 2 – Activity Poll",
                Description = "Choose the main activity for the evening",
                Questions = new List<PollQuestion>
                {
                    new PollQuestion
                    {
                        Id = 5,
                        QuestionText = "What activity do you prefer?",
                        Options = new List<string> { "Escape room", "Bowling", "Karaoke" },
                        Answers = new List<PollAnswer>()
                    }
                }
            }
        };

        public static List<Event> Events => new List<Event>
        {
            new Event
            {
                Id = 1,
                Name = "GatherUp Opening Celebration",
                Description = "A festive group gathering for the whole team.",
                Location = "Tel Aviv",
                ManagerId = 1,   // Tiferet is manager
                HostId = 2,      // Tehila is host
                ParticipantIds = new List<int> { 3, 4, 11 },  // Avi, Sara, Rivka(participant)
                VendorIds = new List<int> { 1 },
                PollIds = new List<int> { 1, 2 }
            },
            new Event
            {
                Id = 2,
                Name = "Team End-of-Year Party",
                Description = "An end-of-year celebration for the whole class.",
                Location = "Jerusalem",
                ManagerId = 5,   // Rivka is manager
                HostId = 2,      // Tehila is host
                ParticipantIds = new List<int> { 6, 10 },  // Yossi, Tiferet(participant)
                VendorIds = new List<int> { 2 },
                PollIds = new List<int> { 3, 4 }
            }
        };
    }
}
