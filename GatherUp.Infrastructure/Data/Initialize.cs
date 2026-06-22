using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;

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
            managerRepo.Add(SeedData.Manager1);
            managerRepo.Add(SeedData.Manager2);
            hostRepo.Add(SeedData.Host);

            foreach (var participant in SeedData.Participants)
                participantRepo.Add(participant);

            foreach (var vendor in SeedData.Vendors)
                vendorRepo.Add(vendor);

            foreach (var poll in SeedData.Polls)
                pollRepo.Add(poll);

            foreach (var ev in SeedData.Events)
                eventRepo.Add(ev);
        }
    }
}
