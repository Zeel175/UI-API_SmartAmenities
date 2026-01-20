using Application.Helper;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Infrastructure.Context;
using Infrastructure.Integrations.Hikvision;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingEntity = Domain.Entities.Building;

namespace Application.Services
{
    public class BuildingService : IBuildingService
    {
        private readonly IBuildingRepository _buildingRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly AppDbContext _context;
        private readonly IClaimAccessorService _claimAccessorService;
        private readonly HikvisionClient _hikvisionClient;
        private readonly ISecretProtector _secretProtector;

        public BuildingService(
            IBuildingRepository buildingRepository,
            IAutoMapperGenericDataMapper dataMapper,
            AppDbContext context,
            IClaimAccessorService claimAccessorService,
            HikvisionClient hikvisionClient,
            ISecretProtector secretProtector)
        {
            _buildingRepository = buildingRepository;
            _dataMapper = dataMapper;
            _context = context;
            _claimAccessorService = claimAccessorService;
            _hikvisionClient = hikvisionClient;
            _secretProtector = secretProtector;
        }

        private async Task<string> GenerateCode()
        {
            // Similar to PropertyService → "PROP0000001"
            // Here: "BLD0000001"
            var count = _buildingRepository.Get().Select(b => b.Code).Distinct().Count();
            var code = "BLD" + (count + 1).ToString("0000000");
            return code.ToUpper();
        }

        public async Task<InsertResponseModel> CreateBuildingAsync(BuildingAddEdit building)
        {
            try
            {
                var validationFailure = await ValidateDeviceCredentialsAsync(building);
                if (validationFailure != null)
                {
                    return validationFailure;
                }
                // ✅ Encrypt before saving
                if (!string.IsNullOrWhiteSpace(building.DevicePassword))
                {
                    building.DevicePassword = _secretProtector.Protect(building.DevicePassword);
                }

                long userId = _claimAccessorService.GetUserId();

                var entity = _dataMapper.Map<BuildingAddEdit, BuildingEntity>(building);
                entity.CreatedBy = userId;
                entity.CreatedDate = DateTime.Now;
                entity.ModifiedBy = userId;
                entity.ModifiedDate = DateTime.Now;
                entity.IsActive = true;
                entity.Code = await GenerateCode();

                await _buildingRepository.AddAsync(entity, userId.ToString(), "Insert");
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

        public async Task DeleteBuildingAsync(long id)
        {
            var entity = await _buildingRepository.GetByIdAsync(id);
            if (entity == null) return;

            var userId = _claimAccessorService.GetUserId();
            entity.IsActive = false;
            entity.ModifiedBy = userId;
            entity.ModifiedDate = DateTime.Now;

            await _buildingRepository.UpdateAsync(entity, userId.ToString(), "Delete");
            await DeactivateBuildingChildrenAsync(entity.Id, userId);
        }

        public async Task<PaginatedList<BuildingList>> GetAllBuildingsAsync(int pageIndex, int pageSize)
        {
            var query = _buildingRepository.Get(b => b.IsActive)
                               .Include(b => b.Property);

            var totalCount = await query.CountAsync();

            var rows = await query
                .OrderBy(b => b.BuildingName)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _dataMapper.Project<Building, BuildingList>(rows.AsQueryable());
            return new PaginatedList<BuildingList>(mapped.ToList(), totalCount, pageIndex, pageSize);

        }

        public async Task<List<object>> GetAllBuildingBasicAsync()
        {
            var query = _buildingRepository.Get(m => m.IsActive == true);

            var result = await query
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.BuildingName
                })
                .ToListAsync<object>();

            return result;
        }

        public async Task<BuildingAddEdit> GetBuildingByIdAsync(long id)
        {
            var entity = await _buildingRepository.GetByIdAsync(id);
            if (entity == null) return null;
            var model = _dataMapper.Map<Building, BuildingAddEdit>(entity);
            if (!string.IsNullOrWhiteSpace(model.DevicePassword)
                && _secretProtector.IsProtected(model.DevicePassword))
            {
                model.DevicePassword = _secretProtector.Unprotect(model.DevicePassword);
            }
            return model;
        }

        public async Task<InsertResponseModel> UpdateBuildingAsync(BuildingAddEdit building)
        {
            try
            {
                var validationFailure = await ValidateDeviceCredentialsAsync(building);
                if (validationFailure != null)
                {
                    return validationFailure;
                }

                var entity = await _buildingRepository.GetByIdAsync(building.Id);
                if (entity == null)
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "404",
                        Message = "Building not found."
                    };
                }
                // ✅ Encrypt before mapping to entity
                if (!string.IsNullOrWhiteSpace(building.DevicePassword))
                {
                    building.DevicePassword = _secretProtector.Protect(building.DevicePassword);
                }
                // Preserve immutable fields
                var existingCode = entity.Code;
                var existingIsActive = entity.IsActive;

                var userId = _claimAccessorService.GetUserId();

                entity.ModifiedBy = userId;
                entity.ModifiedDate = DateTime.Now;

                // Map incoming fields onto tracked entity
                var updated = _dataMapper.Map<BuildingAddEdit, Building>(building, entity);
                updated.Code = existingCode;
                updated.IsActive = building.IsActive;

                await _buildingRepository.UpdateAsync(updated, userId.ToString(), "Update");
                if (existingIsActive && !updated.IsActive)
                {
                    await DeactivateBuildingChildrenAsync(updated.Id, userId);
                }
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
        private async Task<InsertResponseModel?> ValidateDeviceCredentialsAsync(BuildingAddEdit building)
        {
            if (!building.DeviceId.HasValue)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(building.DeviceUserName)
                || string.IsNullOrWhiteSpace(building.DevicePassword))
            {
                return new InsertResponseModel
                {
                    Id = 0,
                    Code = "400",
                    Message = "Device username and password are required."
                };
            }

            var device = await _context.HikDevices
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == building.DeviceId.Value);

            if (device == null || string.IsNullOrWhiteSpace(device.IpAddress))
            {
                return new InsertResponseModel
                {
                    Id = 0,
                    Code = "404",
                    Message = "Device IP not found."
                };
            }
            var port = device.PortNo ?? 80;
            var result = await _hikvisionClient.CheckDeviceCredentialsAsync(
                device.IpAddress,
                 port,
                building.DeviceUserName,
                building.DevicePassword);

            if (!result.IsAuthorized)
            {
                return new InsertResponseModel
                {
                    Id = 0,
                    Code = "401",
                    Message = "Device credentials are invalid."
                };
            }

            return null;
        }

        private async Task DeactivateBuildingChildrenAsync(long buildingId, long userId)
        {
            var now = DateTime.Now;

            await _context.Set<Floor>()
                .Where(f => f.BuildingId == buildingId && f.IsActive)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(f => f.IsActive, false)
                    .SetProperty(f => f.ModifiedBy, userId)
                    .SetProperty(f => f.ModifiedDate, now));

            await _context.Set<global::Unit>()
                .Where(u => u.BuildingId == buildingId && u.IsActive)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(u => u.IsActive, false)
                    .SetProperty(u => u.ModifiedBy, userId)
                    .SetProperty(u => u.ModifiedDate, now));
        }

    }
}
