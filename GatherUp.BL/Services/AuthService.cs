using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;
using System.Linq;

namespace GatherUp.BL.Services
{
    public class AuthService
    {
        private readonly IRepository<Participant> _participants;
        private readonly IRepository<EventManager> _managers;
        private readonly IRepository<EventHost> _hosts;

        public AuthService(IRepository<Participant> participants, IRepository<EventManager> managers, IRepository<EventHost> hosts)
        {
            _participants = participants;
            _managers = managers;
            _hosts = hosts;
        }

        public Person Login(string email, string passwordId)
        {
            Person user = _participants.GetAll().FirstOrDefault(p => p.Email == email && p.PasswordId == passwordId)
                       ?? _managers.GetAll().FirstOrDefault(p => p.Email == email && p.PasswordId == passwordId)
                       ?? (Person)_hosts.GetAll().FirstOrDefault(p => p.Email == email && p.PasswordId == passwordId);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or ID.");

            return user;
        }

        public Person Register(string name, string email, string passwordId)
        {
            bool exists = _participants.GetAll().Any(p => p.Email == email)
                       || _managers.GetAll().Any(p => p.Email == email)
                       || _hosts.GetAll().Any(p => p.Email == email);

            if (exists)
                throw new InvalidOperationException("A user with this email already exists.");

            int newId = _participants.GetAll().Any()
                ? _participants.GetAll().Max(p => p.Id) + 1
                : 1;

            var newParticipant = new Participant
            {
                Id = newId,
                Name = name,
                Email = email,
                PasswordId = passwordId
            };

            _participants.Add(newParticipant);
            return newParticipant;
        }
    }
}
