using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class AmenityUnitRepository : GenericRepository<AmenityUnit>, IAmenityUnitRepository
    {
        public AmenityUnitRepository(AppDbContext context) : base(context)
        {
        }
    }
}
