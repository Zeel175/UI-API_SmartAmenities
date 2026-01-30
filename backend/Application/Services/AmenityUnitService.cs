using Application.Helper;
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
    public class AmenityUnitService : IAmenityUnitService
    {
        private readonly IAmenityUnitRepository _unitRepository;
        private readonly IAmenitySlotTemplateRepository _slotTemplateRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly IClaimAccessorService _claimAccessorService;
        private readonly ISecretProtector _secretProtector;

        public AmenityUnitService(
            IAmenityUnitRepository unitRepository,
            IAmenitySlotTemplateRepository slotTemplateRepository,
            IAutoMapperGenericDataMapper dataMapper,
            IClaimAccessorService claimAccessorService,
            ISecretProtector secretProtector)
        {
            _unitRepository = unitRepository;
            _slotTemplateRepository = slotTemplateRepository;
            _dataMapper = dataMapper;
            _claimAccessorService = claimAccessorService;
            _secretProtector = secretProtector;
        }

        private async Task<string> GenerateCode(long amenityId)
        {
            var count = await _unitRepository
                .Get(u => u.AmenityId == amenityId && !string.IsNullOrWhiteSpace(u.UnitCode))
                .Select(u => u.UnitCode)
                .Distinct()
                .CountAsync();
            return ("AMU" + (count + 1).ToString("0000000")).ToUpper();
        }

        public async Task<InsertResponseModel> CreateAmenityUnitAsync(AmenityUnitAddEdit unit)
        {
            try
            {
                long loggedInUserId = _claimAccessorService.GetUserId();
                if (!unit.DeviceId.HasValue)
                {
                    unit.DeviceUserName = null;
                    unit.DevicePassword = null;
                }
                else if (!string.IsNullOrWhiteSpace(unit.DevicePassword))
                {
                    unit.DevicePassword = _secretProtector.Protect(unit.DevicePassword);
                }
                var mappedModel = _dataMapper.Map<AmenityUnitAddEdit, AmenityUnit>(unit);
                mappedModel.CreatedBy = loggedInUserId;
                mappedModel.CreatedDate = DateTime.Now;
                mappedModel.ModifiedBy = loggedInUserId;
                mappedModel.ModifiedDate = DateTime.Now;
                if (string.IsNullOrWhiteSpace(mappedModel.Status))
                {
                    mappedModel.Status = "Active";
                }
                if (string.IsNullOrWhiteSpace(mappedModel.UnitCode)
                    || mappedModel.UnitCode.Equals("string", StringComparison.OrdinalIgnoreCase))
                {
                    mappedModel.UnitCode = await GenerateCode(mappedModel.AmenityId);
                }
                else
                {
                    var unitCodeExists = await _unitRepository
                        .Get(u => u.AmenityId == mappedModel.AmenityId
                            && u.UnitCode == mappedModel.UnitCode)
                        .AnyAsync();

                    if (unitCodeExists)
                    {
                        return new InsertResponseModel
                        {
                            Id = 0,
                            Code = "409",
                            Message = "Unit code already exists for the selected amenity."
                        };
                    }
                }

                mappedModel.Features = unit.Features.Select(feature => new AmenityUnitFeature
                {
                    FeatureId = feature.FeatureId,
                    FeatureName = feature.FeatureName,
                    IsActive = feature.IsActive,
                    CreatedBy = loggedInUserId,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = loggedInUserId,
                    ModifiedDate = DateTime.Now
                }).ToList();

                await _unitRepository.AddAsync(mappedModel, loggedInUserId.ToString(), "Insert");

                return new InsertResponseModel
                {
                    Id = mappedModel.Id,
                    Code = mappedModel.UnitCode ?? mappedModel.Id.ToString(),
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

        public async Task DeleteAmenityUnitAsync(long id)
        {
            var entity = await _unitRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return;
            }

            long loggedInUserId = _claimAccessorService.GetUserId();
            entity.ModifiedBy = loggedInUserId;
            entity.ModifiedDate = DateTime.Now;
            entity.Status = "Inactive";

            await _unitRepository.UpdateAsync(entity, loggedInUserId.ToString(), "Delete");
        }

        public async Task<AmenityUnitAddEdit?> GetAmenityUnitByIdAsync(long id)
        {
            var entity = await _unitRepository
                .Get(unit => unit.Id == id, includeProperties: "Features")
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                return null;
            }
            var model = _dataMapper.Map<AmenityUnit, AmenityUnitAddEdit>(entity);
            if (!string.IsNullOrWhiteSpace(model.DevicePassword)
                && _secretProtector.IsProtected(model.DevicePassword))
            {
                model.DevicePassword = _secretProtector.Unprotect(model.DevicePassword);
            }
            return model;
        }

        public async Task<PaginatedList<AmenityUnitList>> GetAmenityUnitsAsync(int pageIndex, int pageSize)
        {
            var query = _unitRepository
                .Get(includeProperties: "AmenityMaster,Features");

            var totalCount = await query.CountAsync();
            var rows = await query
                .OrderByDescending(a => a.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _dataMapper.Project<AmenityUnit, AmenityUnitList>(rows.AsQueryable());
            return new PaginatedList<AmenityUnitList>(mapped.ToList(), totalCount, pageIndex, pageSize);
        }

        public async Task<IReadOnlyList<AmenityUnitList>> GetAmenityUnitsAsync()
        {
            var rows = await _unitRepository
                .Get(includeProperties: "AmenityMaster,Features")
                .OrderByDescending(a => a.Id)
                .ToListAsync();

            var mapped = _dataMapper.Project<AmenityUnit, AmenityUnitList>(rows.AsQueryable());
            return mapped.ToList();
        }

        public async Task<IReadOnlyList<AmenityUnitList>> GetAmenityUnitsByAmenityIdAsync(long amenityId)
        {
            var rows = await _unitRepository
                .Get(unit => unit.AmenityId == amenityId, includeProperties: "AmenityMaster,Features")
                .OrderByDescending(unit => unit.Id)
                .ToListAsync();

            var mapped = _dataMapper.Project<AmenityUnit, AmenityUnitList>(rows.AsQueryable());
            return mapped.ToList();
        }

        public async Task<IReadOnlyList<AmenityUnitWithSlotsList>> GetAmenityUnitsWithSlotsByAmenityIdAsync(long amenityId)
        {
            var units = await _unitRepository
                .Get(unit => unit.AmenityId == amenityId, includeProperties: "AmenityMaster,Features")
                .OrderByDescending(unit => unit.Id)
                .ToListAsync();

            if (!units.Any())
            {
                return new List<AmenityUnitWithSlotsList>();
            }

            var unitIds = units.Select(unit => unit.Id).ToList();
            var slotTemplates = await _slotTemplateRepository
                .Get(slot => slot.AmenityId == amenityId
                    && slot.AmenityUnitId.HasValue
                    && unitIds.Contains(slot.AmenityUnitId.Value),
                    includeProperties: "AmenityMaster,SlotTimes")
                .ToListAsync();

            var mappedUnits = _dataMapper.Project<AmenityUnit, AmenityUnitWithSlotsList>(units.AsQueryable())
                .ToList();
            var mappedSlots = _dataMapper.Project<AmenitySlotTemplate, AmenitySlotTemplateList>(slotTemplates.AsQueryable())
                .ToList();

            var slotLookup = mappedSlots
                .GroupBy(slot => slot.AmenityUnitId ?? 0)
                .ToDictionary(group => group.Key, group => group.ToList());

            foreach (var unit in mappedUnits)
            {
                if (slotLookup.TryGetValue(unit.Id, out var slots))
                {
                    unit.Slots = slots;
                }
            }

            return mappedUnits;
        }

        public async Task<InsertResponseModel> UpdateAmenityUnitAsync(AmenityUnitAddEdit unit)
        {
            try
            {
                var entity = await _unitRepository
                    .Get(u => u.Id == unit.Id, includeProperties: "Features")
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "404",
                        Message = "Amenity unit not found."
                    };
                }

                long loggedInUserId = _claimAccessorService.GetUserId();
                entity.ModifiedBy = loggedInUserId;
                entity.ModifiedDate = DateTime.Now;
                if (!unit.DeviceId.HasValue)
                {
                    unit.DeviceUserName = null;
                    unit.DevicePassword = null;
                }
                else if (string.IsNullOrWhiteSpace(unit.DevicePassword))
                {
                    unit.DevicePassword = entity.DevicePassword;
                }
                else if (!string.IsNullOrWhiteSpace(unit.DevicePassword))
                {
                    unit.DevicePassword = _secretProtector.Protect(unit.DevicePassword);
                }
                var existingUnitCode = entity.UnitCode;
                var existingAmenityId = entity.AmenityId;
                var requestedAmenityId = unit.AmenityId > 0 ? unit.AmenityId : existingAmenityId;
                var mappedModel = _dataMapper.Map(unit, entity);
                mappedModel.AmenityId = requestedAmenityId;
                if (string.IsNullOrWhiteSpace(mappedModel.Status))
                {
                    mappedModel.Status = "Active";
                }
                var amenityChanged = requestedAmenityId != existingAmenityId;
                if (string.IsNullOrWhiteSpace(mappedModel.UnitCode)
                    || mappedModel.UnitCode.Equals("string", StringComparison.OrdinalIgnoreCase))
                {
                    mappedModel.UnitCode = !amenityChanged && !string.IsNullOrWhiteSpace(existingUnitCode)
                        ? existingUnitCode
                        : await GenerateCode(mappedModel.AmenityId);
                }

                if (!string.IsNullOrWhiteSpace(mappedModel.UnitCode))
                {
                    var unitCodeExists = await _unitRepository
                        .Get(u => u.AmenityId == mappedModel.AmenityId
                            && u.UnitCode == mappedModel.UnitCode
                            && u.Id != entity.Id)
                        .AnyAsync();

                    if (unitCodeExists)
                    {
                        return new InsertResponseModel
                        {
                            Id = 0,
                            Code = "409",
                            Message = "Unit code already exists for the selected amenity."
                        };
                    }
                }

                entity.Features.Clear();
                var features = unit.Features ?? new List<AmenityUnitFeatureAddEdit>();
                foreach (var feature in features)
                {
                    if (string.IsNullOrWhiteSpace(feature.FeatureName))
                    {
                        continue;
                    }
                    entity.Features.Add(new AmenityUnitFeature
                    {
                        FeatureId = feature.FeatureId,
                        FeatureName = feature.FeatureName,
                        IsActive = feature.IsActive,
                        CreatedBy = loggedInUserId,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = loggedInUserId,
                        ModifiedDate = DateTime.Now
                    });
                }

                await _unitRepository.UpdateAsync(mappedModel, loggedInUserId.ToString(), "Update");

                return new InsertResponseModel
                {
                    Id = entity.Id,
                    Code = mappedModel.UnitCode ?? entity.Id.ToString(),
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
