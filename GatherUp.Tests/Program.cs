using System;
using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;
using GatherUp.Infrastructure.Data;
using GatherUp.Infrastructure.Memory;


namespace GatherUp.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            // הגדרה המאפשרת ל-Console להציג עברית בצורה תקינה בלי סימני שאלה
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== תחילת הרצת פרויקט הבדיקה GatherUpSystem ===");
            Console.WriteLine("---------------------------------------------");

            // 1. יצירת מופעים של ה-MemoryRepository (הזרקת תלויות ידנית עבור ה-Core)
            IRepository<Event> eventRepo = new MemoryRepository<Event>();
            IRepository<Participant> participantRepo = new MemoryRepository<Participant>();
            IRepository<EventManager> managerRepo = new MemoryRepository<EventManager>();
            IRepository<EventHost> hostRepo = new MemoryRepository<EventHost>();
            IRepository<VendorAllocation> vendorRepo = new MemoryRepository<VendorAllocation>();
            IRepository<Poll> pollRepo = new MemoryRepository<Poll>();

            // 2. קריאה לפונקציית האתחול עם ה-Repositories שיצרנו
            // פעולה זו תטען למערכת את הנתונים מתוך SeedData (מנהל, בעל אירוע, 2 משתתפים, ספק וסקרים)
            Initialize.DataInit(eventRepo, participantRepo, managerRepo, hostRepo, vendorRepo, pollRepo);
            Console.WriteLine("[V] הנתונים הראשוניים מ-SeedData הוטענו בהצלחה לזיכרון.");
            Console.WriteLine("---------------------------------------------\n");


            // 3. בדיקת דרישות המערכת בשטח:

            // --- בדיקה 1: הוספת 3 משתתפים חדשים למערכת ---
            Console.WriteLine("--- שלב א': הוספת 3 משתתפים חדשים ---");

            var p1 = new Participant { Id = 5, Name = "דניאל גולד", Email = "daniel@example.com", IsAttending = true, HasPaid = false, AmountContributed = 0 };
            var p2 = new Participant { Id = 6, Name = "מיכל כץ", Email = "michal@example.com", IsAttending = false, HasPaid = false, AmountContributed = 0 };
            var p3 = new Participant { Id = 7, Name = "יונתן רז", Email = "yonatan@example.com", IsAttending = null, HasPaid = false, AmountContributed = 0 };

            participantRepo.Add(p1);
            participantRepo.Add(p2);
            participantRepo.Add(p3);

            Console.WriteLine("3 משתתפים נוספו בהצלחה דינמית ל-Repository.");
            Console.WriteLine("---------------------------------------------\n");


            // --- בדיקה 2: שליפת אחד המשתתפים לפי ה-Id שלו ---
            int idToFind = 6;
            Console.WriteLine($"--- שלב ב': שליפת משתתף לפי מזהה {idToFind} ---");

            var searchedParticipant = participantRepo.GetById(idToFind);

            if (searchedParticipant != null)
            {
                //Console.WriteLine($"נמצאה רשומה! שם: {searchedParticipant.Name} | מייל: {searchedParticipant.Email}");
            }
            else
            {
                Console.WriteLine($"שגיאה: לא נמצא משתתף עם מזהה {idToFind}");
            }
            Console.WriteLine("---------------------------------------------\n");


            // --- בדיקה 3: הדפסת רשימת כל המשתתפים למסך ---
            Console.WriteLine("--- שלב ג': הדפסת רשימת כל המשתתפים במערכת ---");

            var allParticipants = participantRepo.GetAll();

            foreach (var participant in allParticipants)
            {
                // המרת סטטוס ה-bool? למחרוזת ברורה בעברית
                string attendanceStatus = participant.IsAttending == true ? "מגיע" :
                                         participant.IsAttending == false ? "לא מגיע" : "טרם השיב";

                Console.WriteLine($"- [ID: {participant.Id}] {participant.Name} ({participant.Email}) -> סטטוס: {attendanceStatus}");
            }

            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("=== הבדיקה הסתיימה בהצלחה! לחצי Enter לסגירה ===");
            Console.ReadLine();
        }
    }
}
