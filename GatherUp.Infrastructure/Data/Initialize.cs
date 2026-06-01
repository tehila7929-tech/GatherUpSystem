using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;
using GatherUp.Infrastructure.Memory; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Infrastructure.Data
{
    public static class Initialize
    {
        public static void DataInit(
            IRepository<Event> eventRepo,
            IRepository<Participant> participantRepo,
            IRepository<EventManager> managerRepo,
            IRepository<EventHost> hostRepo,
            IRepository<VendorAllocation> vendorRepo,
            IRepository<Poll> pollRepo)
        {
            managerRepo.Add(SeedData.Manager);
            hostRepo.Add(SeedData.Host);

            foreach (var participant in SeedData.Participants)
            {
                participantRepo.Add(participant);
            }

            vendorRepo.Add(SeedData.Vendor);

            foreach (var poll in SeedData.Polls)
            {
                pollRepo.Add(poll);
            }

            eventRepo.Add(SeedData.Event);
        }
    }
}