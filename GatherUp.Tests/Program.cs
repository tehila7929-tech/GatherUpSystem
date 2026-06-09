using System;
using System.IO;
using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;
using GatherUp.Infrastructure.Data;
using GatherUp.Infrastructure.Memory;
using GatherUp.Infrastructure.XML;

namespace GatherUp.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== תחילת ריצת פרויקט הבדיקה GatherUpSystem ===");
            Console.WriteLine("---------------------------------------------");

            RunXmlEngine();

            Console.WriteLine("\n=== הבדיקה הסתיימה בהצלחה! לחצי Enter לסגירה ===");
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

        static void RunXmlEngine()
        {
            Console.WriteLine("[מנוע: XML] מריץ בדיקות כתיבה וקריאה מול הדיסק הקשיח...");

            string xmlFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataXML");
            string receiptsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UploadedReceipts");
            if (!Directory.Exists(xmlFolder)) Directory.CreateDirectory(xmlFolder);
            if (!Directory.Exists(receiptsFolder)) Directory.CreateDirectory(receiptsFolder);

            IRepository<Event> eventRepo = new XmlRepository<Event>();
            IRepository<Participant> participantRepo = new XmlRepository<Participant>();
            IRepository<EventManager> managerRepo = new XmlRepository<EventManager>();
            IRepository<EventHost> hostRepo = new XmlRepository<EventHost>();
            IRepository<VendorAllocation> vendorRepo = new XmlRepository<VendorAllocation>();
            IRepository<Poll> pollRepo = new XmlRepository<Poll>();

            ReceiptRepository receiptRepo = new ReceiptRepository();

            Initialize.DataInit(eventRepo, participantRepo, managerRepo, hostRepo, vendorRepo, pollRepo);
            Console.WriteLine("[V] נתוני SeedData ראשוניים נכתבו לתוך קובצי ה-XML בהצלחה.");

            Console.WriteLine("\n--- שלב 1: הוספת 3 משתתפים נוספים ל-XML ---");
            var p1 = new Participant { Id = 31, Name = "נתנאל מאיר", Email = "netanel@test.com", IsAttending = true, HasPaid = true, AmountContributed = 120 };
            var p2 = new Participant { Id = 32, Name = "אסתר לוי", Email = "esther@test.com", IsAttending = false, HasPaid = false, AmountContributed = 0 };
            var p3 = new Participant { Id = 33, Name = "גד עובדיה", Email = "gad@test.com", IsAttending = true, HasPaid = true, AmountContributed = 180 };

            participantRepo.Add(p1);
            participantRepo.Add(p2);
            participantRepo.Add(p3);

            Console.WriteLine("\n--- שלב 2: עדכון נתונים בסקר (Poll) ---");
            var existingPoll = pollRepo.GetById(1);
            if (existingPoll != null)
            {
                existingPoll.Name = "סקר שביעות רצון מעודכן - שלב ב'";
                pollRepo.Update(existingPoll);
                Console.WriteLine("[V] כותרת הסקר שונתה ועודכנה בהצלחה בתוך ה-XML.");
            }

            Console.WriteLine("\n--- שלב 3: הדפסת רשימת המשתתפים המלאה שנקראה מה-XML ---");
            var allXmlParticipants = participantRepo.GetAll();
            foreach (var participant in allXmlParticipants)
            {
                string attendanceStatus = participant.IsAttending == true ? "מגיע" : participant.IsAttending == false ? "לא מגיע" : "טרם השיב";
                Console.WriteLine($"- [ID: {participant.Id}] {participant.Name} ({participant.Email}) -> סטטוס: {attendanceStatus}");
            }

            Console.WriteLine("\n--- שלב 4: הוספת קבלה והעלאת קובץ פיזי ---");

            string sourceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "invoice_original.pdf");
            File.WriteAllText(sourceFile, "תוכן דמי מדמה של מסמך סרוק");

            var myReceipt = new ReceiptDetails("REC-998811", 550.00m, DateTime.Now);

            receiptRepo.Add(myReceipt, sourceFile);
            Console.WriteLine("[V] הקבלה נוספה ל-XML והקובץ הועלה בהצלחה לתיקיית הקבלות הייעודית.");

            var checkReceipt = receiptRepo.GetByNumber("REC-998811");
            if (checkReceipt != null)
            {
                string uploadedPath = receiptRepo.GetUploadedFilePath("REC-998811");
                Console.WriteLine($"[V] אימות שליפה הצליח! נשלפה קבלה {checkReceipt.ReceiptNumber} בסך {checkReceipt.Amount}₪. קובץ שמור ב: {uploadedPath}");
            }
        }
    }
}