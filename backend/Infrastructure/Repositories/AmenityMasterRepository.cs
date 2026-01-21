using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class AmenityMasterRepository : GenericRepository<AmenityMaster>, IAmenityMasterRepository
    {
        public AmenityMasterRepository(AppDbContext context) : base(context)
        {
        }
    }
}
