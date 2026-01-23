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
    public class AmenitySlotTemplateService : IAmenitySlotTemplateService
    {
        private readonly IAmenitySlotTemplateRepository _slotTemplateRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly IClaimAccessorService _claimAccessorService;

        public AmenitySlotTemplateService(
            IAmenitySlotTemplateRepository slotTemplateRepository,
            IAutoMapperGenericDataMapper dataMapper,
            IClaimAccessorService claimAccessorService)
        {
            _slotTemplateRepository = slotTemplateRepository;
            _dataMapper = dataMapper;
            _claimAccessorService = claimAccessorService;
        }

        public async Task<InsertResponseModel> CreateSlotTemplateAsync(AmenitySlotTemplateAddEdit template)
        {
            try
            {
                long loggedInUserId = _claimAccessorService.GetUserId();
                var mappedModel = BuildSlotTemplateEntity(template, loggedInUserId);

                await _slotTemplateRepository.AddAsync(mappedModel, loggedInUserId.ToString(), "Insert");
                return new InsertResponseModel
                {
                    Id = mappedModel.Id,
                    Code = mappedModel.Id.ToString(),
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

        public async Task<IReadOnlyList<InsertResponseModel>> CreateSlotTemplatesAsync(IReadOnlyList<AmenitySlotTemplateAddEdit> templates)
        {
            var responses = new List<InsertResponseModel>();
            if (templates == null || templates.Count == 0)
            {
                responses.Add(new InsertResponseModel
                {
                    Id = 0,
                    Code = "400",
                    Message = "Slot templates payload is required."
                });
                return responses;
            }

            long loggedInUserId = _claimAccessorService.GetUserId();
            foreach (var template in templates)
            {
                try
                {
                    var mappedModel = BuildSlotTemplateEntity(template, loggedInUserId);
                    await _slotTemplateRepository.AddAsync(mappedModel, loggedInUserId.ToString(), "Insert");
                    responses.Add(new InsertResponseModel
                    {
                        Id = mappedModel.Id,
                        Code = mappedModel.Id.ToString(),
                        Message = "Insert successfully."
                    });
                }
                catch (Exception ex)
                {
                    responses.Add(new InsertResponseModel
                    {
                        Id = 0,
                        Code = ex.HResult.ToString(),
                        Message = ex.Message
                    });
                }
            }

            return responses;
        }

        public async Task<IReadOnlyList<InsertResponseModel>> UpsertSlotTemplatesAsync(IReadOnlyList<AmenitySlotTemplateAddEdit> templates)
        {
            var responses = new List<InsertResponseModel>();
            if (templates == null || templates.Count == 0)
            {
                responses.Add(new InsertResponseModel
                {
                    Id = 0,
                    Code = "400",
                    Message = "Slot templates payload is required."
                });
                return responses;
            }

            foreach (var template in templates)
            {
                if (template.Id > 0)
                {
                    responses.Add(await UpdateSlotTemplateAsync(template));
                    continue;
                }

                responses.Add(await CreateSlotTemplateAsync(template));
            }

            return responses;
        }

        public async Task DeleteSlotTemplateAsync(long id)
        {
            var entity = await _slotTemplateRepository.GetByIdAsync(id);
            if (entity == null)
                return;

            long loggedInUserId = _claimAccessorService.GetUserId();
            entity.ModifiedBy = loggedInUserId;
            entity.ModifiedDate = DateTime.Now;
            entity.IsActive = false;

            await _slotTemplateRepository.UpdateAsync(entity, loggedInUserId.ToString(), "Delete");
        }

        public async Task<AmenitySlotTemplateAddEdit?> GetSlotTemplateByIdAsync(long id)
        {
            var entity = await _slotTemplateRepository
                .Get(slot => slot.Id == id, includeProperties: "SlotTimes")
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                return null;
            }
            return _dataMapper.Map<AmenitySlotTemplate, AmenitySlotTemplateAddEdit>(entity);
        }

        public async Task<PaginatedList<AmenitySlotTemplateList>> GetSlotTemplatesAsync(int pageIndex, int pageSize)
        {
            var query = _slotTemplateRepository
                .Get(includeProperties: "AmenityMaster,SlotTimes")
                .Where(slot => slot.IsActive);

            var totalCount = await query.CountAsync();
            var rows = await query
                .OrderByDescending(a => a.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _dataMapper.Project<AmenitySlotTemplate, AmenitySlotTemplateList>(rows.AsQueryable());
            return new PaginatedList<AmenitySlotTemplateList>(mapped.ToList(), totalCount, pageIndex, pageSize);
        }

        public async Task<InsertResponseModel> UpdateSlotTemplateAsync(AmenitySlotTemplateAddEdit template)
        {
            try
            {
                var entity = await _slotTemplateRepository
                    .Get(slot => slot.Id == template.Id, includeProperties: "SlotTimes")
                    .FirstOrDefaultAsync();
                if (entity == null)
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "404",
                        Message = "Slot template not found."
                    };
                }

                long loggedInUserId = _claimAccessorService.GetUserId();
                entity.ModifiedBy = loggedInUserId;
                entity.ModifiedDate = DateTime.Now;
                bool isActive = entity.IsActive;
                entity.AmenityId = template.AmenityId;
                entity.DayOfWeek = template.DayOfWeek;
                entity.SlotDurationMinutes = template.SlotDurationMinutes;
                entity.BufferTimeMinutes = template.BufferTimeMinutes;
                entity.IsActive = isActive;
                entity.AmenityUnitId = template.AmenityUnitId;

                var primarySlot = template.SlotTimes?.FirstOrDefault();
                entity.StartTime = primarySlot?.StartTime;
                entity.EndTime = primarySlot?.EndTime;
                entity.CapacityPerSlot = primarySlot?.CapacityPerSlot;

                entity.SlotTimes.Clear();
                foreach (var slotTime in template.SlotTimes ?? new List<AmenitySlotTemplateTimeAddEdit>())
                {
                    entity.SlotTimes.Add(new AmenitySlotTemplateTime
                    {
                        StartTime = slotTime.StartTime,
                        EndTime = slotTime.EndTime,
                        CapacityPerSlot = slotTime.CapacityPerSlot,
                        IsActive = slotTime.IsActive,
                        CreatedBy = loggedInUserId,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = loggedInUserId,
                        ModifiedDate = DateTime.Now
                    });
                }

                await _slotTemplateRepository.UpdateAsync(entity, loggedInUserId.ToString(), "Update");

                return new InsertResponseModel
                {
                    Id = entity.Id,
                    Code = entity.Id.ToString(),
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

        private AmenitySlotTemplate BuildSlotTemplateEntity(AmenitySlotTemplateAddEdit template, long loggedInUserId)
        {
            var mappedModel = _dataMapper.Map<AmenitySlotTemplateAddEdit, AmenitySlotTemplate>(template);
            mappedModel.CreatedBy = loggedInUserId;
            mappedModel.CreatedDate = DateTime.Now;
            mappedModel.ModifiedBy = loggedInUserId;
            mappedModel.ModifiedDate = DateTime.Now;
            mappedModel.IsActive = true;
            var primarySlot = template.SlotTimes?.FirstOrDefault();
            mappedModel.StartTime = primarySlot?.StartTime;
            mappedModel.EndTime = primarySlot?.EndTime;
            mappedModel.CapacityPerSlot = primarySlot?.CapacityPerSlot;
            mappedModel.SlotTimes.Clear();
            foreach (var slotTime in template.SlotTimes ?? new List<AmenitySlotTemplateTimeAddEdit>())
            {
                mappedModel.SlotTimes.Add(new AmenitySlotTemplateTime
                {
                    StartTime = slotTime.StartTime,
                    EndTime = slotTime.EndTime,
                    CapacityPerSlot = slotTime.CapacityPerSlot,
                    IsActive = slotTime.IsActive,
                    CreatedBy = loggedInUserId,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = loggedInUserId,
                    ModifiedDate = DateTime.Now
                });
            }
            return mappedModel;
        }
    }
}
