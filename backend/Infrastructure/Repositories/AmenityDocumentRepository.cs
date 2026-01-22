using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class AmenityDocumentRepository
        : GenericRepository<AmenityDocument>, IAmenityDocumentRepository
    {
        public AmenityDocumentRepository(AppDbContext context)
            : base(context)
        {
        }
    }
}
