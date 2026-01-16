using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using PropertyEntity = Domain.Entities.Property;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly AppDbContext _context;
        private readonly IClaimAccessorService _claimAccessorService;

        public PropertyService(IPropertyRepository propertyRepository,
            IAutoMapperGenericDataMapper dataMapper, AppDbContext context, IClaimAccessorService claimAccessorService)
        {
            _propertyRepository = propertyRepository;
            _dataMapper = dataMapper;
            _context = context;
            _claimAccessorService = claimAccessorService;
        }
        private async Task<string> GenerateCode()
        {
            string code = "";
            var ct = _propertyRepository.Get().Select(a => a.Code).Distinct().ToList().Count;

            code = $"PROP" + (ct + 1).ToString("0000000");

            return code.ToUpper();
        }
        public async Task<InsertResponseModel> CreatePropertyAsync(PropertyAddEdit property)
        {
            try
            {
                long loggedinUserId = _claimAccessorService.GetUserId();
                var mappedModel = _dataMapper.Map<PropertyAddEdit, PropertyEntity>(property);
                mappedModel.CreatedBy = loggedinUserId;
                mappedModel.CreatedDate = DateTime.Now;
                mappedModel.ModifiedBy = loggedinUserId;
                mappedModel.ModifiedDate = DateTime.Now;
                mappedModel.IsActive = true;
                mappedModel.Code = await GenerateCode();

                await _propertyRepository.AddAsync(mappedModel, loggedinUserId.ToString(), "Insert");
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

        public async Task DeletePropertyAsync(long id)
        {
            var entity = await _propertyRepository.GetByIdAsync(id);

            long loggedinUserId = _claimAccessorService.GetUserId();
            entity.ModifiedBy = loggedinUserId;
            entity.ModifiedDate = DateTime.Now;
            entity.IsActive = false;

            await _propertyRepository.UpdateAsync(entity, loggedinUserId.ToString(), "Delete"); ;
        }
        //public IQueryable<CompanyMasterList> GetAllCompanyMasterAsync()
        //{
        //    var entity = _companyMasterRepository.Get(m => m.IsActive == true);
        //    return _dataMapper.Project<CompanyMaster, CompanyMasterList>(entity);
        //}


        public async Task<PaginatedList<PropertyList>> GetAllPropertiesAsync(int pageIndex, int pageSize)
        {
            var query = _propertyRepository.Get(m => m.IsActive == true);

            var totalCount = await query.CountAsync();
            var properties = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedProperties = _dataMapper.Project<Property, PropertyList>(properties.AsQueryable());

            return new PaginatedList<PropertyList>(mappedProperties.ToList(), totalCount, pageIndex, pageSize);
        }

        public async Task<PropertyAddEdit> GetPropertyByIdAsync(long id)
        {
            var entity = await _propertyRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return null;
            }
            var result = _dataMapper.Map<Property, PropertyAddEdit>(entity);
            return result;
        }

        public async Task<InsertResponseModel> UpdatePropertyAsync(PropertyAddEdit property)
        {
            try
            {
                var entity = await _propertyRepository.GetByIdAsync(property.Id);
                if (entity == null)
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "404",
                        Message = "Property not found."
                    };
                }
                string code = entity.Code;
                bool isActive = entity.IsActive;
                long loggedInUserId = _claimAccessorService.GetUserId();

                entity.ModifiedBy = loggedInUserId;
                entity.ModifiedDate = DateTime.Now;
                var mappedModel = _dataMapper.Map<PropertyAddEdit, Property>(property, entity);
                mappedModel.IsActive = isActive;
                mappedModel.Code = code;

                await _propertyRepository.UpdateAsync(mappedModel, loggedInUserId.ToString(), "Update");

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
        public async Task<List<object>> GetAllPropertiesBasicAsync()
        {
            var query = _propertyRepository.Get(m => m.IsActive == true);

            var result = await query
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.PropertyName
                })
                .ToListAsync<object>();

            return result;
        }

    }
}
