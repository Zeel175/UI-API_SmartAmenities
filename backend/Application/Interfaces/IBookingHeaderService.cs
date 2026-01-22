using Domain.ViewModels;

namespace Application.Interfaces
{
    public interface IBookingHeaderService
    {
        Task<InsertResponseModel> CreateBookingAsync(BookingHeaderAddEdit booking);
        Task<InsertResponseModel> UpdateBookingAsync(BookingHeaderAddEdit booking);
        Task DeleteBookingAsync(long id);
        Task<BookingHeaderAddEdit?> GetBookingByIdAsync(long id);
        Task<PaginatedList<BookingHeaderList>> GetBookingsAsync(int pageIndex, int pageSize);
    }
}
