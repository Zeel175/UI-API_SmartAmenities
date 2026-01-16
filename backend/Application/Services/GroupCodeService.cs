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
    public class GroupCodeService : IGroupCodeService
    {
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly IGroupCodeRepository _groupCodeRepository;
        private readonly IClaimAccessorService _claimAccessorService;
        //private readonly IUnitOfWork _unitOfWork;

        public GroupCodeService(IAutoMapperGenericDataMapper dataMapper, IGroupCodeRepository groupCodeRepository, IClaimAccessorService claimAccessorService)//, IUnitOfWork unitOfWork
        {
            _dataMapper = dataMapper;
            _groupCodeRepository = groupCodeRepository;
            _claimAccessorService = claimAccessorService;
            //_unitOfWork = unitOfWork;
        }
        private async Task<string> GenerateCode()
        {
            string code = "";
            var ct = _groupCodeRepository.Get().Select(a => a.Code).Distinct().ToList().Count;

            code = $"GC" + (ct + 1).ToString("0000000");

            return code.ToUpper();
        }

        public async Task<List<object>> GetAllGroupCodeBasicAsync()
        {
            var query = _groupCodeRepository.Get(m => m.IsActive == true);

            var result = await query
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.GroupName,
                    x.Name
                })
                .ToListAsync<object>();

            return result;
        }

        public async Task<IList<SelectListModel>> GetGroupCodeByGroupName(string GroupName)
        {
            var model = _groupCodeRepository.Get(m => m.GroupName == GroupName && m.IsActive == true).OrderBy(m => m.Name);
            var mappedModel = _dataMapper.Project<GroupCode, SelectListModel>(model);

            return await Task.Run(() => mappedModel.ToList());
        }
        public async Task<InsertResponseModel> CreateGroupCodeAsync(GroupCodeAddEdit groupCode)
        {
            try
            {
                long loggedinUserId = _claimAccessorService.GetUserId();
                //_claimAccessorService.GetUserId();
                var mappedModel = _dataMapper.Map<GroupCodeAddEdit, GroupCode>(groupCode);
                mappedModel.CreatedBy = loggedinUserId;
                mappedModel.CreatedDate = DateTime.Now;
                mappedModel.ModifiedBy = loggedinUserId;
                mappedModel.ModifiedDate = DateTime.Now;
                mappedModel.IsActive = true;
                mappedModel.Code = await GenerateCode();

                await _groupCodeRepository.AddAsync(mappedModel, loggedinUserId.ToString(), "Insert");
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
        public async Task<PaginatedList<GroupCodeList>> GetAllGroupCodesAsync(int pageIndex, int pageSize)
        {
            var query = _groupCodeRepository.Get().AsNoTracking();

            var totalCount = await query.CountAsync();
            var pageQuery = query
            .OrderBy(x => x.Id) // or CreatedDate, Code — pick a stable key
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);

            // Map data using DataMapper (if needed)
            var items = await _dataMapper
                .Project<GroupCode, GroupCodeList>(pageQuery)
                .ToListAsync();

            return new PaginatedList<GroupCodeList>(items, totalCount, pageIndex, pageSize);
        }
        public async Task<ResponseModel> Activate(long id, bool isActive)
        {
            try
            {
                long loggedinUserId = _claimAccessorService.GetUserId();
                var entity = await _groupCodeRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        Message = "Group code not found."
                    };
                }

                entity.IsActive = isActive;
                entity.ModifiedBy = loggedinUserId;
                entity.ModifiedDate = DateTime.Now;

                await _groupCodeRepository.UpdateAsync(entity, loggedinUserId.ToString(), "Update");
                bool status;
                if (entity.Id <= 0)
                {
                    status = false;
                }
                else
                {
                    status = true;
                }

                if (status)
                {
                    return new ResponseModel
                    {
                        IsSuccess = true,
                        Message = "Group code updated successfully."
                    };
                }

                return new ResponseModel
                {
                    IsSuccess = false,
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<IList<GroupCodeList>> GetAll()
        {
            var entities = await _groupCodeRepository.GetAllAsync();
            var models = _dataMapper.Project<GroupCode, GroupCodeList>(entities.AsQueryable().OrderByDescending(o => o.Id));
            return models.ToList();
        }

        public async Task<GroupCodeAddEdit> GetGroupCodeByIdAsync(long id)
        {
            var entity = await _groupCodeRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return null;
            }
            var result = _dataMapper.Map<GroupCode, GroupCodeAddEdit>(entity);
            return result;
        }

        public async Task<InsertResponseModel> UpdateGroupCodeAsync(GroupCodeAddEdit groupCode)
        {
            try
            {
                var entity = await _groupCodeRepository.GetByIdAsync(groupCode.Id);
                if (entity == null)
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "404",
                        Message = "Group code not found."
                    };
                }
                string code = entity.Code;
                bool isActive = entity.IsActive;
                long loggedInUserId = _claimAccessorService.GetUserId();
                //_claimAccessorService.GetUserId();

                entity.ModifiedBy = loggedInUserId;
                entity.ModifiedDate = DateTime.Now;
                var mappedModel = _dataMapper.Map<GroupCodeAddEdit, GroupCode>(groupCode, entity);
                mappedModel.IsActive = isActive;
                mappedModel.Code = code;

                await _groupCodeRepository.UpdateAsync(mappedModel, loggedInUserId.ToString(), "Update");

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
