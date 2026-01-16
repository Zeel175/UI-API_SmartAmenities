using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Application.Services
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly AppDbContext _context;
        private readonly IClaimAccessorService _claimAccessorService;

        public UnitService(
            IUnitRepository unitRepository,
            IAutoMapperGenericDataMapper dataMapper,
            AppDbContext context,
            IClaimAccessorService claimAccessorService)
        {
            _unitRepository = unitRepository;
            _dataMapper = dataMapper;
            _context = context;
            _claimAccessorService = claimAccessorService;
        }

        private async Task<string> GenerateCode()
        {
            // Similar to BuildingService → "BLD0000001"
            // Here: "UNT0000001"
            var count = _unitRepository.Get().Select(u => u.Code).Distinct().Count();
            var code = "UNT" + (count + 1).ToString("0000000");
            return code.ToUpper();
        }

        public async Task<InsertResponseModel> CreateUnitAsync(UnitAddEdit unit)
        {
            try
            {
                //long userId = _claimAccessorService.GetUserId();
                long userId = 1;


                var entity = _dataMapper.Map<UnitAddEdit, Unit>(unit);
                entity.CreatedBy = userId;
                entity.CreatedDate = DateTime.Now;
                entity.ModifiedBy = userId;
                entity.ModifiedDate = DateTime.Now;
                entity.IsActive = true;
                entity.Code = await GenerateCode();

                await _unitRepository.AddAsync(entity, userId.ToString(), "Insert");
                return new InsertResponseModel
                {
                    Id = entity.Id,
                    Code = entity.Code,
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

        public async Task DeleteUnitAsync(long id)
        {
            var entity = await _unitRepository.GetByIdAsync(id);
            if (entity == null) return;

            var userId = _claimAccessorService.GetUserId();
            entity.IsActive = false;
            entity.ModifiedBy = userId;
            entity.ModifiedDate = DateTime.Now;

            await _unitRepository.UpdateAsync(entity, userId.ToString(), "Delete");
        }

        public async Task<PaginatedList<UnitList>> GetAllUnitsAsync(int pageIndex, int pageSize)
        {
            var query = _unitRepository
                .Get(u => u.IsActive)
                .Include(u => u.Building)
                .Include(u => u.Floor)
                .Include(u => u.OccupancyStatus);

            var totalCount = await query.CountAsync();

            var rows = await query
                .OrderBy(u => u.UnitName)
                .ThenBy(u => u.Building.BuildingName)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _dataMapper.Project<Unit, UnitList>(rows.AsQueryable());
            return new PaginatedList<UnitList>(mapped.ToList(), totalCount, pageIndex, pageSize);
        }

        public async Task<List<object>> GetAllUnitBasicAsync()
        {
            var query = _unitRepository
                .Get(u => u.IsActive)
                .Include(u => u.Building);

            var result = await query
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.UnitName,
                    BuildingName = x.Building.BuildingName
                })
                .ToListAsync<object>();

            return result;
        }

        public async Task<UnitAddEdit> GetUnitByIdAsync(long id)
        {
            var entity = await _unitRepository
                .Get(u => u.Id == id)
                .Include(u => u.Building)
                .Include(u => u.Floor)
                .Include(u => u.OccupancyStatus)
                .FirstOrDefaultAsync();

            if (entity == null) return null;
            return _dataMapper.Map<Unit, UnitAddEdit>(entity);
        }

        public async Task<InsertResponseModel> UpdateUnitAsync(UnitAddEdit unit)
        {
            try
            {
                var entity = await _unitRepository.GetByIdAsync(unit.Id);
                if (entity == null)
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "404",
                        Message = "Unit not found."
                    };
                }

                // Preserve immutable fields
                var existingCode = entity.Code;
                var existingIsActive = entity.IsActive;

                var userId = _claimAccessorService.GetUserId();

                entity.ModifiedBy = userId;
                entity.ModifiedDate = DateTime.Now;

                // Map incoming fields onto tracked entity
                var updated = _dataMapper.Map<UnitAddEdit, Unit>(unit, entity);
                updated.Code = existingCode;
                updated.IsActive = existingIsActive;

                await _unitRepository.UpdateAsync(updated, userId.ToString(), "Update");
                return new InsertResponseModel
                {
                    Id = updated.Id,
                    Code = updated.Code,
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

        // (Optional) Handy filtered list for dropdowns on screens:
        public async Task<List<object>> GetUnitsByBuildingFloorAsync(long buildingId, long floorId)
        {
            var query = _unitRepository.Get(u => u.IsActive &&
                                                 u.BuildingId == buildingId &&
                                                 u.FloorId == floorId);

            var result = await query
                .OrderBy(u => u.UnitName)
                .Select(u => new
                {
                    u.Id,
                    u.Code,
                    u.UnitName
                })
                .ToListAsync<object>();

            return result;
        }
        public async Task<List<object>> GetUnitsByBuildingAsync(long buildingId)
        {
            var query = _unitRepository
                .Get(u => u.IsActive && u.BuildingId == buildingId);

            var result = await query
                .OrderBy(u => u.UnitName)
                .Select(u => new
                {
                    u.Id,
                    u.Code,
                    u.UnitName
                })
                .ToListAsync<object>();

            return result;
        }
    }
}
