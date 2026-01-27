using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class BookingUnitRepository : GenericRepository<BookingUnit>, IBookingUnitRepository
    {
        public BookingUnitRepository(AppDbContext context) : base(context)
        {
        }
    }
}
