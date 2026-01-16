using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class FloorService : IFloorService
    {
        private readonly IFloorRepository _floorRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly IClaimAccessorService _claimAccessorService;

        public FloorService(IFloorRepository floorRepository,
            IAutoMapperGenericDataMapper dataMapper,
            IClaimAccessorService claimAccessorService)
        {
            _floorRepository = floorRepository;
            _dataMapper = dataMapper;
            _claimAccessorService = claimAccessorService;
        }

        private async Task<string> GenerateCode(long buildingId)
        {
            var count = _floorRepository.Get(f => f.BuildingId == buildingId)
                                        .Select(f => f.Code)
                                        .Distinct()
                                        .ToList()
                                        .Count;
            return $"FLR" + (count + 1).ToString("0000000");
        }

        public async Task<List<object>> GetAllFloorBasicAsync()
        {
            var query = _floorRepository.Get(m => m.IsActive == true);

            var result = await query
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.FloorName
                })
                .ToListAsync<object>();

            return result;
        }

        public async Task<InsertResponseModel> CreateFloorAsync(FloorAddEdit floor)
        {
            try
            {
                //long loggedinUserId = _claimAccessorService.GetUserId();
                long loggedinUserId = 1;
                var mappedModel = _dataMapper.Map<FloorAddEdit, Floor>(floor);
                mappedModel.CreatedBy = loggedinUserId;
                mappedModel.CreatedDate = DateTime.Now;
                mappedModel.ModifiedBy = loggedinUserId;
                mappedModel.ModifiedDate = DateTime.Now;
                mappedModel.IsActive = true;
                mappedModel.Code = await GenerateCode(floor.BuildingId);

                await _floorRepository.AddAsync(mappedModel, loggedinUserId.ToString(), "Insert");
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

        public async Task DeleteFloorAsync(long id)
        {
            var entity = await _floorRepository.GetByIdAsync(id);
            if (entity == null)
                return;

            long loggedinUserId = _claimAccessorService.GetUserId();
            entity.ModifiedBy = loggedinUserId;
            entity.ModifiedDate = DateTime.Now;
            entity.IsActive = false;

            await _floorRepository.UpdateAsync(entity, loggedinUserId.ToString(), "Delete");
        }

        public async Task<PaginatedList<FloorList>> GetAllFloorsAsync(int pageIndex, int pageSize)
        {
            var query = _floorRepository.Get(m => m.IsActive == true, includeProperties: "Building");

            var totalCount = await query.CountAsync();
            var floors = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedFloors = _dataMapper.Project<Floor, FloorList>(floors.AsQueryable());

            return new PaginatedList<FloorList>(mappedFloors.ToList(), totalCount, pageIndex, pageSize);
        }

        public async Task<FloorAddEdit> GetFloorByIdAsync(long id)
        {
            var entity = await _floorRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return null;
            }
            var result = _dataMapper.Map<Floor, FloorAddEdit>(entity);
            return result;
        }

        public async Task<InsertResponseModel> UpdateFloorAsync(FloorAddEdit floor)
        {
            try
            {
                var entity = await _floorRepository.GetByIdAsync(floor.Id);
                if (entity == null)
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "404",
                        Message = "Floor not found."
                    };
                }
                string code = entity.Code;
                bool isActive = entity.IsActive;
                long loggedInUserId = _claimAccessorService.GetUserId();

                entity.ModifiedBy = loggedInUserId;
                entity.ModifiedDate = DateTime.Now;
                var mappedModel = _dataMapper.Map<FloorAddEdit, Floor>(floor, entity);
                mappedModel.IsActive = isActive;
                mappedModel.Code = code;

                await _floorRepository.UpdateAsync(mappedModel, loggedInUserId.ToString(), "Update");

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
