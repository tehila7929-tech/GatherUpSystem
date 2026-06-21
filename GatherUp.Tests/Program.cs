using System;
using System.IO;
using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;
using GatherUp.Infrastructure;
using GatherUp.Infrastructure.Data;
using GatherUp.Infrastructure.Memory;
using GatherUp.Infrastructure.XML;
using GatherUp.BL.Services;

namespace GatherUp.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== GatherUpSystem Test Project Started ===");
            Console.WriteLine("---------------------------------------------");

            RunXmlEngine();
            RunBLTests();

            Console.WriteLine("\n=== Tests completed successfully! Press Enter to exit ===");
            Console.ReadLine();
        }

        static void RunMemoryEngine()
        {
            IRepository<Event> eventRepo = new MemoryRepository<Event>();
            IRepository<Participant> participantRepo = new MemoryRepository<Participant>();
            IRepository<EventManager> managerRepo = new MemoryRepository<EventManager>();
            IRepository<EventHost> hostRepo = new MemoryRepository<EventHost>();
            IRepository<VendorAllocation> vendorRepo = new MemoryRepository<VendorAllocation>();
            IRepository<Poll> pollRepo = new MemoryRepository<Poll>();

            Initialize.DataInit(eventRepo, participantRepo, managerRepo, hostRepo, vendorRepo, pollRepo);
        }

        static void RunBLTests()
        {
            Console.WriteLine("\n=== BL Layer Tests ===");
            string xmlFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataXML");

            // Initialize Infrastructure
            IRepository<Event> eventRepo = new XmlRepository<Event>(xmlFolder);
            IRepository<Participant> participantRepo = new XmlRepository<Participant>(xmlFolder);
            IRepository<Poll> pollRepo = new XmlRepository<Poll>(xmlFolder);
            IRepository<VendorAllocation> vendorRepo = new XmlRepository<VendorAllocation>(xmlFolder);
            IEmailService emailService = new EmailService(xmlFolder);
            IEventNotifier notifier = new EventNotifier();

            // Inject dependencies into BL services
            var participantService = new ParticipantService(participantRepo, eventRepo, notifier);
            var financeService = new FinanceService(vendorRepo, participantRepo, eventRepo);
            var pollService = new PollService(pollRepo, eventRepo, notifier);
            var eventService = new EventService(eventRepo, notifier);

            // Register notification listener
            var notifications = new NotificationSubscription(notifier, participantRepo, emailService);

            // === Simulation: Main screen — click 'Create New Event' ===
            Console.WriteLine("\n[Main Screen] Clicked 'Create New Event'");
            var newEvent = new Event { Id = 100, Name = "Birthday Party", Location = "Tel Aviv", ManagerId = 1, HostId = 2 };
            eventService.CreateEvent(newEvent);
            Console.WriteLine("[V] Event created.");

            // === Simulation: Event screen — click 'Add Participant' ===
            Console.WriteLine("\n[Event Screen] Clicked 'Add Participant'");
            var p = new Participant { Id = 50, Name = "Rachel Cohen", Email = "rachel@test.com", ContactPreference = ContactPreference.Email };
            participantService.AddParticipantToEvent(p, 100);
            Console.WriteLine("[V] Participant added.");

            // === Simulation: Participant screen — click 'Confirm Attendance' → fires event → email ===
            Console.WriteLine("\n[Participant Screen] Clicked 'Confirm Attendance'");
            participantService.ConfirmAttendance(50, true);
            Console.WriteLine("[V] Attendance confirmed — email written to file.");

            // === Simulation: Management screen — click 'Confirm Payment' ===
            Console.WriteLine("\n[Management Screen] Clicked 'Confirm Payment'");
            participantService.ConfirmPayment(50, 200);
            Console.WriteLine("[V] Payment confirmed — email written to file.");

            // === Simulation: Management screen — click 'Create Poll' ===
            Console.WriteLine("\n[Management Screen] Clicked 'Create Poll'");
            var poll = new Poll { Id = 10, Name = "Event Location" };
            poll.Questions.Add(new PollQuestion { Id = 1, QuestionText = "Where?", Options = new() { "Tel Aviv", "Jerusalem" } });
            pollService.CreatePoll(poll, 100);
            Console.WriteLine("[V] Poll created — emails written to file.");

            // === Simulation: Edit screen — click 'Save Changes' ===
            Console.WriteLine("\n[Edit Screen] Clicked 'Save Changes'");
            newEvent.Location = "Haifa";
            eventService.UpdateEventDetails(newEvent);
            Console.WriteLine("[V] Event updated — emails written to file.");

            Console.WriteLine($"\n[V] All emails written to: {Path.Combine(xmlFolder, "emails.txt")}");
        }

        static void RunXmlEngine()
        {
            Console.WriteLine("[XML Engine] Running read/write tests against disk...");

            string xmlFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataXML");

            IRepository<Event> eventRepo = new XmlRepository<Event>(xmlFolder);
            IRepository<Participant> participantRepo = new XmlRepository<Participant>(xmlFolder);
            IRepository<EventManager> managerRepo = new XmlRepository<EventManager>(xmlFolder);
            IRepository<EventHost> hostRepo = new XmlRepository<EventHost>(xmlFolder);
            IRepository<VendorAllocation> vendorRepo = new XmlRepository<VendorAllocation>(xmlFolder);
            IRepository<Poll> pollRepo = new XmlRepository<Poll>(xmlFolder);
            ReceiptRepository receiptRepo = new ReceiptRepository(xmlFolder);

            // Clear XML files before seeding to avoid duplicates on re-run
            foreach (var file in Directory.GetFiles(xmlFolder, "*.xml"))
                File.Delete(file);

            Initialize.DataInit(eventRepo, participantRepo, managerRepo, hostRepo, vendorRepo, pollRepo);
            Console.WriteLine("[V] Initial SeedData written to XML files successfully.");

            Console.WriteLine("\n--- Step 1: Adding 3 more participants to XML ---");
            var p1 = new Participant { Id = 31, Name = "Netanel Meir", Email = "netanel@test.com", IsAttending = true, HasPaid = true, AmountContributed = 120 };
            var p2 = new Participant { Id = 32, Name = "Esther Levi", Email = "esther@test.com", IsAttending = false, HasPaid = false, AmountContributed = 0 };
            var p3 = new Participant { Id = 33, Name = "Gad Ovadia", Email = "gad@test.com", IsAttending = true, HasPaid = true, AmountContributed = 180 };
            participantRepo.Add(p1);
            participantRepo.Add(p2);
            participantRepo.Add(p3);
            Console.WriteLine("[V] 3 participants added successfully.");

            Console.WriteLine("\n--- Step 2: Adding a new question to the poll ---");
            var existingPoll = pollRepo.GetById(1);
            if (existingPoll != null)
            {
                existingPoll.Questions.Add(new PollQuestion
                {
                    Id = 10,
                    QuestionText = "Will you need a ride to the event?",
                    Options = new System.Collections.Generic.List<string> { "Yes", "No" }
                });
                pollRepo.Update(existingPoll);
                Console.WriteLine("[V] New question added to poll and updated in XML.");
            }

            Console.WriteLine("\n--- Step 3: Changing a participant's answer in the poll ---");
            var pollToUpdate = pollRepo.GetById(1);
            if (pollToUpdate != null && pollToUpdate.Questions.Count > 0)
            {
                pollToUpdate.Questions[0].SetAnswer(3, "15/06");
                pollToUpdate.Questions[0].SetAnswer(3, "01/06"); // changed answer - prevents duplicates
                pollRepo.Update(pollToUpdate);
                Console.WriteLine("[V] Participant 3 answer updated successfully in poll.");
            }

            Console.WriteLine("\n--- Step 4: Printing full participant list read from XML ---");
            foreach (var p in participantRepo.GetAll())
            {
                string status = p.IsAttending == true ? "Attending" : p.IsAttending == false ? "Not attending" : "No response";
                Console.WriteLine($"- [ID: {p.Id}] {p.Name} ({p.Email}) -> Status: {status}");
            }

            Console.WriteLine("\n--- Step 5: Adding receipt and uploading physical file ---");
            string sourceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "invoice_original.pdf");
            File.WriteAllText(sourceFile, "Simulated scanned document content");

            var myReceipt = new ReceiptDetails("REC-998811", 550.00m, DateTime.Now);
            receiptRepo.AddReceipt(myReceipt, sourceFile);
            Console.WriteLine("[V] Receipt added to XML and file uploaded to receipts folder.");

            var checkReceipt = receiptRepo.GetReceiptByNumber("REC-998811");
            if (checkReceipt != null)
            {
                string uploadedPath = receiptRepo.GetUploadedFilePath("REC-998811");
                Console.WriteLine($"[V] Receipt {checkReceipt.ReceiptNumber} for {checkReceipt.Amount}. File: {uploadedPath}");
            }
        }
    }
}
