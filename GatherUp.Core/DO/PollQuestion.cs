using System.Collections.Generic;
using System.Xml.Serialization;

namespace GatherUp.Core.DO
{
    // עזר לסריאליזציה של Dictionary ל-XML
    public class PollAnswer
    {
        [XmlAttribute]
        public int ParticipantId { get; set; }
        public string Answer { get; set; } = string.Empty;
    }

    public class PollQuestion
    {
        [XmlAttribute]
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public List<string> Options { get; set; } = new();

        // ParticipantId -> Answer, מומש כרשימה לצורך XML, מאפשר שינוי בחירה ומונע כפילויות
        public List<PollAnswer> Answers { get; set; } = new();

        public void SetAnswer(int participantId, string answer)
        {
            var existing = Answers.Find(a => a.ParticipantId == participantId);
            if (existing != null)
                existing.Answer = answer;
            else
                Answers.Add(new PollAnswer { ParticipantId = participantId, Answer = answer });
        }

        public PollQuestion() { }
    }
}
