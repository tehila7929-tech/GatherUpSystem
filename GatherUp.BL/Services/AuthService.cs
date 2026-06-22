using GatherUp.Core.DO;
using GatherUp.Core.Exceptions;
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
            Person? user = _participants.GetAll().FirstOrDefault(p => p.Email == email && p.PasswordId == passwordId)
                       ?? _managers.GetAll().FirstOrDefault(p => p.Email == email && p.PasswordId == passwordId)
                       ?? (Person?)_hosts.GetAll().FirstOrDefault(p => p.Email == email && p.PasswordId == passwordId);

            if (user == null)
                throw new ForbiddenException("Invalid email or ID.");

            return user;
        }

        public Person Register(string name, string email, string passwordId)
        {
            bool exists = _participants.GetAll().Any(p => p.Email == email)
                       || _managers.GetAll().Any(p => p.Email == email)
                       || _hosts.GetAll().Any(p => p.Email == email);

            if (exists)
                throw new AlreadyExistsException("A user with this email already exists.");

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

        public Person GetUserById(int userId)
        {
            return (Person?)_participants.GetById(userId)
                ?? _managers.GetById(userId)
                ?? (Person?)_hosts.GetById(userId)
                ?? throw new NotFoundException("User not found.");
        }

        public void UpdateUser(int userId, string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
                throw new InvalidInputException("Name and email are required.");

            var participant = _participants.GetById(userId);
            if (participant != null)
            {
                participant.Name = name;
                participant.Email = email;
                _participants.Update(participant);
                return;
            }

            var manager = _managers.GetById(userId);
            if (manager != null)
            {
                manager.Name = name;
                manager.Email = email;
                _managers.Update(manager);
                return;
            }

            var host = _hosts.GetById(userId);
            if (host != null)
            {
                host.Name = name;
                host.Email = email;
                _hosts.Update(host);
                return;
            }

            throw new NotFoundException("User not found.");
        }
    }
}
