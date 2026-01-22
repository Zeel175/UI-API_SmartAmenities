using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class BookingHeaderRepository : GenericRepository<BookingHeader>, IBookingHeaderRepository
    {
        public BookingHeaderRepository(AppDbContext context) : base(context)
        {
        }
    }
}
