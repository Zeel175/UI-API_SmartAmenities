using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Application.Services
{
    public class AmenityUnitService : IAmenityUnitService
    {
        private readonly IAmenityUnitRepository _unitRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly IClaimAccessorService _claimAccessorService;

        public AmenityUnitService(
            IAmenityUnitRepository unitRepository,
            IAutoMapperGenericDataMapper dataMapper,
            IClaimAccessorService claimAccessorService)
        {
            _unitRepository = unitRepository;
            _dataMapper = dataMapper;
            _claimAccessorService = claimAccessorService;
        }

        public async Task<InsertResponseModel> CreateAmenityUnitAsync(AmenityUnitAddEdit unit)
        {
            try
            {
                long loggedInUserId = _claimAccessorService.GetUserId();
                var mappedModel = _dataMapper.Map<AmenityUnitAddEdit, AmenityUnit>(unit);
                mappedModel.CreatedBy = loggedInUserId;
                mappedModel.CreatedDate = DateTime.Now;
                mappedModel.ModifiedBy = loggedInUserId;
                mappedModel.ModifiedDate = DateTime.Now;
                if (string.IsNullOrWhiteSpace(mappedModel.Status))
                {
                    mappedModel.Status = "Active";
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

            return _dataMapper.Map<AmenityUnit, AmenityUnitAddEdit>(entity);
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
                var mappedModel = _dataMapper.Map(unit, entity);
                if (string.IsNullOrWhiteSpace(mappedModel.Status))
                {
                    mappedModel.Status = "Active";
                }

                entity.Features.Clear();
                foreach (var feature in unit.Features)
                {
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
    }
}
