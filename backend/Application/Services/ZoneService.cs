using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ZoneService : IZoneService
    {
        private readonly IZoneRepository _zoneRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly IClaimAccessorService _claimAccessorService;

        public ZoneService(
            IZoneRepository zoneRepository,
            IAutoMapperGenericDataMapper dataMapper,
            IClaimAccessorService claimAccessorService)
        {
            _zoneRepository = zoneRepository;
            _dataMapper = dataMapper;
            _claimAccessorService = claimAccessorService;
        }

        //private async Task<string> GenerateCode(long? buildingId)
        //{
        //    var count = _zoneRepository.Get(z => z.BuildingId == buildingId)
        //                               .Select(z => z.Code)
        //                               .Distinct()
        //                               .Count();
        //    return "ZN" + (count + 1).ToString("0000000");
        //}
        private async Task<string> GenerateCode()
        {
            // Similar to PropertyService → "PROP0000001"
            // Here: "BLD0000001"
            var count = _zoneRepository.Get().Select(b => b.Code).Distinct().Count();
            var code = "ZN" + (count + 1).ToString("0000000");
            return code.ToUpper();
        }
        public async Task<List<object>> GetAllZoneBasicAsync()
        {
            var query = _zoneRepository.Get(m => m.IsActive);

            var result = await query
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.ZoneName
                })
                .ToListAsync<object>();

            return result;
        }

        public async Task<InsertResponseModel> CreateZoneAsync(ZoneAddEdit zone)
        {
            try
            {
                if (zone.BuildingId.HasValue && zone.BuildingId.Value <= 0)
                    zone.BuildingId = null;
               
                long loggedInUserId = _claimAccessorService.GetUserId();
                var mappedModel = _dataMapper.Map<ZoneAddEdit, Zone>(zone);
                mappedModel.CreatedBy = loggedInUserId;
                mappedModel.CreatedDate = DateTime.Now;
                mappedModel.ModifiedBy = loggedInUserId;
                mappedModel.ModifiedDate = DateTime.Now;
                mappedModel.IsActive = true;
                mappedModel.Code = await GenerateCode();

                await _zoneRepository.AddAsync(mappedModel, loggedInUserId.ToString(), "Insert");
                return new InsertResponseModel
                {
                    Id = mappedModel.Id,
                    Code = mappedModel.Code,
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

        public async Task DeleteZoneAsync(long id)
        {
            var entity = await _zoneRepository.GetByIdAsync(id);
            if (entity == null)
                return;

            long loggedInUserId = _claimAccessorService.GetUserId();
            entity.ModifiedBy = loggedInUserId;
            entity.ModifiedDate = DateTime.Now;
            entity.IsActive = false;

            await _zoneRepository.UpdateAsync(entity, loggedInUserId.ToString(), "Delete");
        }

        public async Task<PaginatedList<ZoneList>> GetAllZonesAsync(int pageIndex, int pageSize)
        {
            var query = _zoneRepository.Get(m => m.IsActive, includeProperties: "Building");

            var totalCount = await query.CountAsync();
            var rows = await query
                .OrderBy(z => z.ZoneName)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _dataMapper.Project<Zone, ZoneList>(rows.AsQueryable());

            return new PaginatedList<ZoneList>(mapped.ToList(), totalCount, pageIndex, pageSize);
        }

        public async Task<ZoneAddEdit> GetZoneByIdAsync(long id)
        {
            var entity = await _zoneRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return null;
            }
            var result = _dataMapper.Map<Zone, ZoneAddEdit>(entity);
            return result;
        }

        public async Task<InsertResponseModel> UpdateZoneAsync(ZoneAddEdit zone)
        {
            try
            {
                var entity = await _zoneRepository.GetByIdAsync(zone.Id);
                if (entity == null)
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "404",
                        Message = "Zone not found."
                    };
                }
                if (zone.BuildingId.HasValue && zone.BuildingId.Value <= 0)
                    zone.BuildingId = null;
                
                string code = entity.Code;
                bool isActive = entity.IsActive;
                long loggedInUserId = _claimAccessorService.GetUserId();

                entity.ModifiedBy = loggedInUserId;
                entity.ModifiedDate = DateTime.Now;
                var mappedModel = _dataMapper.Map<ZoneAddEdit, Zone>(zone, entity);
                mappedModel.IsActive = isActive;
                mappedModel.Code = code;
                var duplicateName = await _zoneRepository
                    .Get(z => z.Id != zone.Id
                           && z.IsActive
                           && z.BuildingId == zone.BuildingId
                           && z.ZoneName == zone.ZoneName)
                    .AnyAsync();

                if (duplicateName)
                    throw new ArgumentException("Zone name already exists for this building.");

                await _zoneRepository.UpdateAsync(mappedModel, loggedInUserId.ToString(), "Update");

                return new InsertResponseModel
                {
                    Id = entity.Id,
                    Code = mappedModel.Code,
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
