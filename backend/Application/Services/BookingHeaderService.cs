using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Application.Services
{
    public class BookingHeaderService : IBookingHeaderService
    {
        private readonly IBookingHeaderRepository _bookingRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly IClaimAccessorService _claimAccessorService;

        public BookingHeaderService(
            IBookingHeaderRepository bookingRepository,
            IAutoMapperGenericDataMapper dataMapper,
            IClaimAccessorService claimAccessorService)
        {
            _bookingRepository = bookingRepository;
            _dataMapper = dataMapper;
            _claimAccessorService = claimAccessorService;
        }

        private async Task<string> GenerateBookingNo()
        {
            var count = await _bookingRepository
                .Get()
                .Select(b => b.BookingNo)
                .Distinct()
                .CountAsync();
            return ("BKG" + (count + 1).ToString("0000000")).ToUpper();
        }

        public async Task<InsertResponseModel> CreateBookingAsync(BookingHeaderAddEdit booking)
        {
            try
            {
                long loggedInUserId = _claimAccessorService.GetUserId();
                var mappedModel = _dataMapper.Map<BookingHeaderAddEdit, BookingHeader>(booking);
                mappedModel.CreatedBy = loggedInUserId;
                mappedModel.CreatedDate = DateTime.Now;
                mappedModel.ModifiedBy = loggedInUserId;
                mappedModel.ModifiedDate = DateTime.Now;
                if (string.IsNullOrWhiteSpace(mappedModel.Status))
                {
                    mappedModel.Status = "Draft";
                }
                if (string.IsNullOrWhiteSpace(mappedModel.BookingNo) || mappedModel.BookingNo.Equals("string", StringComparison.OrdinalIgnoreCase))
                {
                    mappedModel.BookingNo = await GenerateBookingNo();
                }

                await _bookingRepository.AddAsync(mappedModel, loggedInUserId.ToString(), "Insert");
                return new InsertResponseModel
                {
                    Id = mappedModel.Id,
                    Code = mappedModel.BookingNo,
                    Message = "Insert successfully."
                };
            }
            catch (Exception ex)
            {
                return new InsertResponseModel
                {
                    Id = 0,
                    Code = ex.HResult.ToString(),
                    Message = ex.Message
                };
            }
        }

        public async Task DeleteBookingAsync(long id)
        {
            var entity = await _bookingRepository.GetByIdAsync(id);
            if (entity == null)
                return;

            long loggedInUserId = _claimAccessorService.GetUserId();
            entity.ModifiedBy = loggedInUserId;
            entity.ModifiedDate = DateTime.Now;
            entity.Status = "Cancelled";

            await _bookingRepository.UpdateAsync(entity, loggedInUserId.ToString(), "Delete");
        }

        public async Task<BookingHeaderAddEdit?> GetBookingByIdAsync(long id)
        {
            var entity = await _bookingRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return null;
            }
            return _dataMapper.Map<BookingHeader, BookingHeaderAddEdit>(entity);
        }

        public async Task<PaginatedList<BookingHeaderList>> GetBookingsAsync(int pageIndex, int pageSize)
        {
            var query = _bookingRepository.Get(includeProperties: "AmenityMaster,Society");

            var totalCount = await query.CountAsync();
            var rows = await query
                .OrderByDescending(b => b.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _dataMapper.Project<BookingHeader, BookingHeaderList>(rows.AsQueryable());
            return new PaginatedList<BookingHeaderList>(mapped.ToList(), totalCount, pageIndex, pageSize);
        }

        public async Task<InsertResponseModel> UpdateBookingAsync(BookingHeaderAddEdit booking)
        {
            try
            {
                var entity = await _bookingRepository.GetByIdAsync(booking.Id);
                if (entity == null)
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "404",
                        Message = "Booking not found."
                    };
                }

                long loggedInUserId = _claimAccessorService.GetUserId();
                entity.ModifiedBy = loggedInUserId;
                entity.ModifiedDate = DateTime.Now;

                var mappedModel = _dataMapper.Map(booking, entity);
                if (string.IsNullOrWhiteSpace(mappedModel.Status))
                {
                    mappedModel.Status = "Draft";
                }
                if (string.IsNullOrWhiteSpace(mappedModel.BookingNo) || mappedModel.BookingNo.Equals("string", StringComparison.OrdinalIgnoreCase))
                {
                    mappedModel.BookingNo = entity.BookingNo;
                }

                await _bookingRepository.UpdateAsync(mappedModel, loggedInUserId.ToString(), "Update");

                return new InsertResponseModel
                {
                    Id = entity.Id,
                    Code = mappedModel.BookingNo,
                    Message = "Update successfully."
                };
            }
            catch (Exception ex)
            {
                return new InsertResponseModel
                {
                    Id = 0,
                    Code = ex.HResult.ToString(),
                    Message = ex.Message
                };
            }
        }
    }
}
