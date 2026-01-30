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
    public class AmenityMasterService : IAmenityMasterService
    {
        private readonly IAmenityMasterRepository _amenityRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly IClaimAccessorService _claimAccessorService;
        private readonly IAmenityDocumentService _amenityDocumentService;
        private readonly ISecretProtector _secretProtector;

        public AmenityMasterService(
            IAmenityMasterRepository amenityRepository,
            IAutoMapperGenericDataMapper dataMapper,
            IClaimAccessorService claimAccessorService,
            IAmenityDocumentService amenityDocumentService,
            ISecretProtector secretProtector)
        {
            _amenityRepository = amenityRepository;
            _dataMapper = dataMapper;
            _claimAccessorService = claimAccessorService;
            _amenityDocumentService = amenityDocumentService;
            _secretProtector = secretProtector;
        }

        private async Task<string> GenerateCode()
        {
            var count = await _amenityRepository
                .Get()
                .Select(a => a.Code)
                .Distinct()
                .CountAsync();
            return ("AMN" + (count + 1).ToString("0000000")).ToUpper();
        }

        public async Task<InsertResponseModel> CreateAmenityAsync(AmenityMasterAddEdit amenity)
        {
            try
            {
                long loggedinUserId = _claimAccessorService.GetUserId();
                if (!string.IsNullOrWhiteSpace(amenity.DevicePassword))
                {
                    amenity.DevicePassword = _secretProtector.Protect(amenity.DevicePassword);
                }
                var mappedModel = _dataMapper.Map<AmenityMasterAddEdit, AmenityMaster>(amenity);
                mappedModel.CreatedBy = loggedinUserId;
                mappedModel.CreatedDate = DateTime.Now;
                mappedModel.ModifiedBy = loggedinUserId;
                mappedModel.ModifiedDate = DateTime.Now;
                if (string.IsNullOrWhiteSpace(mappedModel.Status))
                {
                    mappedModel.Status = "Active";
                }
                if (string.IsNullOrWhiteSpace(mappedModel.Code) || mappedModel.Code.Equals("string", StringComparison.OrdinalIgnoreCase))
                {
                    mappedModel.Code = await GenerateCode();
                }

                await _amenityRepository.AddAsync(mappedModel, loggedinUserId.ToString(), "Insert");
                return new InsertResponseModel
                {
                    Id = mappedModel.Id,
                    Code = mappedModel.Code ?? mappedModel.Id.ToString(),
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
            var model = _dataMapper.Map<AmenityMaster, AmenityMasterAddEdit>(entity);
            if (!string.IsNullOrWhiteSpace(model.DevicePassword)
                && _secretProtector.IsProtected(model.DevicePassword))
            {
                model.DevicePassword = _secretProtector.Unprotect(model.DevicePassword);
            }
            var documents = await _amenityDocumentService.GetDocumentsByAmenityAsync(id);
            model.DocumentDetails = documents.Select(d => new AmenityDocumentDto
            {
                Id = d.Id,
                FileName = d.FileName,
                FilePath = d.FilePath,
                ContentType = d.ContentType
            }).ToList();
            return model;
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

            var mapped = _dataMapper.Map<List<AmenityMaster>, List<AmenityMasterList>>(rows);
            return new PaginatedList<AmenityMasterList>(mapped, totalCount, pageIndex, pageSize);
        }

        public async Task<IReadOnlyList<AmenityMasterList>> GetAmenitiesAsync()
        {
            var rows = await _amenityRepository
                .Get(includeProperties: "Building,Floor")
                .OrderByDescending(a => a.Id)
                .ToListAsync();

            var mapped = _dataMapper.Map<List<AmenityMaster>, List<AmenityMasterList>>(rows);
            return mapped;
        }

        public async Task<List<object>> GetAmenityBasicAsync()
        {
            var query = _amenityRepository.Get(a => a.Status == "Active");

            var result = await query
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.BookingSlotRequired,
                    x.AllowMultipleUnits
                })
                .ToListAsync<object>();

            return result;
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

                if (!string.IsNullOrWhiteSpace(amenity.DevicePassword))
                {
                    amenity.DevicePassword = _secretProtector.Protect(amenity.DevicePassword);
                }
                var mappedModel = _dataMapper.Map(amenity, entity);
                mappedModel.AllowMultipleUnits = amenity.AllowMultipleUnits;
                if (string.IsNullOrWhiteSpace(mappedModel.Status))
                {
                    mappedModel.Status = "Active";
                }
                if (string.IsNullOrWhiteSpace(mappedModel.Code) || mappedModel.Code.Equals("string", StringComparison.OrdinalIgnoreCase))
                {
                    mappedModel.Code = entity.Code;
                }

                await _amenityRepository.UpdateAsync(mappedModel, loggedInUserId.ToString(), "Update");

                return new InsertResponseModel
                {
                    Id = entity.Id,
                    Code = mappedModel.Code ?? mappedModel.Id.ToString(),
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
