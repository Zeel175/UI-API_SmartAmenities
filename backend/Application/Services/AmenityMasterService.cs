using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class AmenityMasterService : IAmenityMasterService
    {
        private readonly IAmenityMasterRepository _amenityRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly IClaimAccessorService _claimAccessorService;

        public AmenityMasterService(
            IAmenityMasterRepository amenityRepository,
            IAutoMapperGenericDataMapper dataMapper,
            IClaimAccessorService claimAccessorService)
        {
            _amenityRepository = amenityRepository;
            _dataMapper = dataMapper;
            _claimAccessorService = claimAccessorService;
        }

        public async Task<InsertResponseModel> CreateAmenityAsync(AmenityMasterAddEdit amenity)
        {
            try
            {
                long loggedinUserId = _claimAccessorService.GetUserId();
                var mappedModel = _dataMapper.Map<AmenityMasterAddEdit, AmenityMaster>(amenity);
                mappedModel.CreatedBy = loggedinUserId;
                mappedModel.CreatedDate = DateTime.Now;
                mappedModel.ModifiedBy = loggedinUserId;
                mappedModel.ModifiedDate = DateTime.Now;
                if (string.IsNullOrWhiteSpace(mappedModel.Status))
                {
                    mappedModel.Status = "Active";
                }

                await _amenityRepository.AddAsync(mappedModel, loggedinUserId.ToString(), "Insert");
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

        public async Task DeleteAmenityAsync(long id)
        {
            var entity = await _amenityRepository.GetByIdAsync(id);
            if (entity == null)
                return;

            long loggedinUserId = _claimAccessorService.GetUserId();
            entity.ModifiedBy = loggedinUserId;
            entity.ModifiedDate = DateTime.Now;
            entity.Status = "Inactive";

            await _amenityRepository.UpdateAsync(entity, loggedinUserId.ToString(), "Delete");
        }

        public async Task<AmenityMasterAddEdit?> GetAmenityByIdAsync(long id)
        {
            var entity = await _amenityRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return null;
            }
            return _dataMapper.Map<AmenityMaster, AmenityMasterAddEdit>(entity);
        }

        public async Task<PaginatedList<AmenityMasterList>> GetAmenitiesAsync(int pageIndex, int pageSize)
        {
            var query = _amenityRepository.Get(includeProperties: "Building,Floor");

            var totalCount = await query.CountAsync();
            var rows = await query
                .OrderByDescending(a => a.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _dataMapper.Project<AmenityMaster, AmenityMasterList>(rows.AsQueryable());
            return new PaginatedList<AmenityMasterList>(mapped.ToList(), totalCount, pageIndex, pageSize);
        }

        public async Task<InsertResponseModel> UpdateAmenityAsync(AmenityMasterAddEdit amenity)
        {
            try
            {
                var entity = await _amenityRepository.GetByIdAsync(amenity.Id);
                if (entity == null)
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "404",
                        Message = "Amenity not found."
                    };
                }

                long loggedInUserId = _claimAccessorService.GetUserId();
                entity.ModifiedBy = loggedInUserId;
                entity.ModifiedDate = DateTime.Now;

                var mappedModel = _dataMapper.Map(amenity, entity);
                if (string.IsNullOrWhiteSpace(mappedModel.Status))
                {
                    mappedModel.Status = "Active";
                }

                await _amenityRepository.UpdateAsync(mappedModel, loggedInUserId.ToString(), "Update");

                return new InsertResponseModel
                {
                    Id = entity.Id,
                    Code = mappedModel.Id.ToString(),
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
