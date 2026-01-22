using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class AmenitySlotTemplateRepository : GenericRepository<AmenitySlotTemplate>, IAmenitySlotTemplateRepository
    {
        public AmenitySlotTemplateRepository(AppDbContext context) : base(context)
        {
        }
    }
}
