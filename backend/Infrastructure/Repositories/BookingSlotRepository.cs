using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class BookingSlotRepository : GenericRepository<BookingSlot>, IBookingSlotRepository
    {
        public BookingSlotRepository(AppDbContext context) : base(context)
        {
        }
    }
}
