using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services
{
    public class BookingHeaderService : IBookingHeaderService
    {
        private readonly IBookingHeaderRepository _bookingRepository;
        private readonly IBookingUnitRepository _bookingUnitRepository;
        private readonly IBookingSlotRepository _bookingSlotRepository;
        private readonly IAmenitySlotTemplateRepository _slotTemplateRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly IClaimAccessorService _claimAccessorService;

        public BookingHeaderService(
            IBookingHeaderRepository bookingRepository,
            IBookingUnitRepository bookingUnitRepository,
            IBookingSlotRepository bookingSlotRepository,
            IAmenitySlotTemplateRepository slotTemplateRepository,
            IAutoMapperGenericDataMapper dataMapper,
            IClaimAccessorService claimAccessorService)
        {
            _bookingRepository = bookingRepository;
            _bookingUnitRepository = bookingUnitRepository;
            _bookingSlotRepository = bookingSlotRepository;
            _slotTemplateRepository = slotTemplateRepository;
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

                if (booking.BookingUnits != null && booking.BookingUnits.Any())
                {
                    foreach (var unit in booking.BookingUnits)
                    {
                        var unitEntity = _dataMapper.Map<BookingUnitAddEdit, BookingUnit>(unit);
                        unitEntity.BookingId = mappedModel.Id;
                        unitEntity.CreatedBy = loggedInUserId;
                        unitEntity.CreatedDate = DateTime.Now;
                        unitEntity.ModifiedBy = loggedInUserId;
                        unitEntity.ModifiedDate = DateTime.Now;
                        await _bookingUnitRepository.AddAsync(unitEntity, loggedInUserId.ToString(), "Insert");

                        if (unit.BookingSlot != null && unit.BookingSlot.SlotStartDateTime != default && unit.BookingSlot.SlotEndDateTime != default)
                        {
                            var slotEntity = _dataMapper.Map<BookingSlotAddEdit, BookingSlot>(unit.BookingSlot);
                            slotEntity.BookingId = mappedModel.Id;
                            slotEntity.BookingUnitId = unitEntity.Id;
                            slotEntity.AmenityId = mappedModel.AmenityId;
                            slotEntity.AmenityUnitId = unitEntity.AmenityUnitId;
                            slotEntity.SlotStatus = string.IsNullOrWhiteSpace(slotEntity.SlotStatus) ? "Reserved" : slotEntity.SlotStatus;
                            slotEntity.CreatedBy = loggedInUserId;
                            slotEntity.CreatedDate = DateTime.Now;
                            slotEntity.ModifiedBy = loggedInUserId;
                            slotEntity.ModifiedDate = DateTime.Now;
                            await _bookingSlotRepository.AddAsync(slotEntity, loggedInUserId.ToString(), "Insert");
                        }
                    }
                }

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
            var entity = await _bookingRepository
                .Get(filter: booking => booking.Id == id, includeProperties: "BookingUnits,BookingUnits.BookingSlots")
                .FirstOrDefaultAsync();
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

        public async Task<IReadOnlyList<BookingHeaderList>> GetBookingsAsync()
        {
            var rows = await _bookingRepository
                .Get(includeProperties: "AmenityMaster,Society")
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            var mapped = _dataMapper.Project<BookingHeader, BookingHeaderList>(rows.AsQueryable());
            return mapped.ToList();
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

                var existingUnits = await _bookingUnitRepository
                    .Get(filter: unit => unit.BookingId == entity.Id)
                    .ToListAsync();

                foreach (var unit in existingUnits)
                {
                    await _bookingUnitRepository.DeleteAsync(unit.Id, loggedInUserId.ToString(), "Delete");
                }

                if (booking.BookingUnits != null && booking.BookingUnits.Any())
                {
                    foreach (var unit in booking.BookingUnits)
                    {
                        var unitEntity = _dataMapper.Map<BookingUnitAddEdit, BookingUnit>(unit);
                        unitEntity.BookingId = entity.Id;
                        unitEntity.CreatedBy = loggedInUserId;
                        unitEntity.CreatedDate = DateTime.Now;
                        unitEntity.ModifiedBy = loggedInUserId;
                        unitEntity.ModifiedDate = DateTime.Now;
                        await _bookingUnitRepository.AddAsync(unitEntity, loggedInUserId.ToString(), "Insert");

                        if (unit.BookingSlot != null && unit.BookingSlot.SlotStartDateTime != default && unit.BookingSlot.SlotEndDateTime != default)
                        {
                            var slotEntity = _dataMapper.Map<BookingSlotAddEdit, BookingSlot>(unit.BookingSlot);
                            slotEntity.BookingId = entity.Id;
                            slotEntity.BookingUnitId = unitEntity.Id;
                            slotEntity.AmenityId = entity.AmenityId;
                            slotEntity.AmenityUnitId = unitEntity.AmenityUnitId;
                            slotEntity.SlotStatus = string.IsNullOrWhiteSpace(slotEntity.SlotStatus) ? "Reserved" : slotEntity.SlotStatus;
                            slotEntity.CreatedBy = loggedInUserId;
                            slotEntity.CreatedDate = DateTime.Now;
                            slotEntity.ModifiedBy = loggedInUserId;
                            slotEntity.ModifiedDate = DateTime.Now;
                            await _bookingSlotRepository.AddAsync(slotEntity, loggedInUserId.ToString(), "Insert");
                        }
                    }
                }

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

        public async Task<IReadOnlyList<BookingSlotAvailability>> GetAvailableSlotsAsync(long amenityId, long amenityUnitId, DateTime bookingDate, long? bookingId)
        {
            var dayOfWeek = bookingDate.DayOfWeek.ToString();
            var templates = await _slotTemplateRepository
                .Get(filter: template => template.AmenityId == amenityId
                    && template.DayOfWeek == dayOfWeek
                    && template.IsActive
                    && (template.AmenityUnitId == null || template.AmenityUnitId == amenityUnitId),
                    includeProperties: "SlotTimes")
                .ToListAsync();

            if (!templates.Any())
            {
                return Array.Empty<BookingSlotAvailability>();
            }

            var unitSpecificTemplates = templates.Where(template => template.AmenityUnitId == amenityUnitId).ToList();
            if (unitSpecificTemplates.Any())
            {
                templates = unitSpecificTemplates;
            }
            else
            {
                templates = templates.Where(template => template.AmenityUnitId == null).ToList();
            }

            var slots = new List<BookingSlotAvailability>();
            foreach (var template in templates)
            {
                foreach (var slotTime in template.SlotTimes.Where(slot => slot.IsActive))
                {
                    var slotStart = bookingDate.Date.Add(slotTime.StartTime);
                    var slotEnd = bookingDate.Date.Add(slotTime.EndTime);
                    var capacity = slotTime.CapacityPerSlot ?? template.CapacityPerSlot ?? 1;

                    var reservedQuery = _bookingSlotRepository.Get(filter: slot =>
                        slot.AmenityId == amenityId
                        && slot.AmenityUnitId == amenityUnitId
                        && slot.SlotStatus == "Reserved"
                        && slot.SlotStartDateTime == slotStart
                        && slot.SlotEndDateTime == slotEnd);

                    if (bookingId.HasValue)
                    {
                        reservedQuery = reservedQuery.Where(slot => slot.BookingId != bookingId.Value);
                    }

                    var reservedCapacity = await reservedQuery
                        .Select(slot => (int?)(slot.BookingUnit.CapacityReserved ?? 1))
                        .SumAsync() ?? 0;
                    var availableCapacity = Math.Max(0, capacity - reservedCapacity);
                    var isReserved = availableCapacity == 0;

                    slots.Add(new BookingSlotAvailability
                    {
                        SlotStartDateTime = slotStart,
                        SlotEndDateTime = slotEnd,
                        CapacityPerSlot = capacity,
                        AvailableCapacity = availableCapacity,
                        IsReserved = isReserved,
                        SlotCharge = slotTime.SlotCharge,
                        IsChargeable = slotTime.IsChargeable,
                        ChargeType = slotTime.ChargeType,
                        BaseRate = slotTime.BaseRate,
                        SecurityDeposit = slotTime.SecurityDeposit,
                        RefundableDeposit = slotTime.RefundableDeposit,
                        TaxApplicable = slotTime.TaxApplicable,
                        TaxCodeId = slotTime.TaxCodeId,
                        TaxPercentage = slotTime.TaxPercentage
                    });
                }
            }

            return slots
                .OrderBy(slot => slot.SlotStartDateTime)
                .ToList();
        }
    }
}
