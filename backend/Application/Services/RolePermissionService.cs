using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Infrastructure.Context;
using Infrastructure.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly IClaimAccessorService _claimAccessorService;
        private readonly AppDbContext _context;

        public RolePermissionService(IRolePermissionRepository rolePermissionRepository,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IAutoMapperGenericDataMapper dataMapper,
            IClaimAccessorService claimAccessorService,
            AppDbContext context
            )
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _dataMapper = dataMapper;
            _claimAccessorService = claimAccessorService;
            _context = context;
        }

        public  IQueryable<RoleList> GetAllRolePermissionsAsync()
        {
            var entity = _roleRepository.GetAllRole();
            return _dataMapper.Project<Role, RoleList>(entity);

            //var list = await Task.FromResult(_dataMapper.Project<RolePermissionMap, RolePermissionList>(entity).ToList());
            //return list;
        }

        public async Task<RoleAddEdit> GetRolePermissionByIdAsync(long id)
        {
            var entity = await _roleRepository.GetByIdAsync(id);
            if (entity == null)
                return null;
            var mappedData = _dataMapper.Map<Role, RoleAddEdit>(entity);
            //mappedData.Permissions = entity.RolePermissions.ToList();
            return mappedData;
        }

        public async Task<long> AddRolePermissionAsync(RoleAddEdit model, long userId)
        {
            try
            {
                long loggedinuserId = _claimAccessorService.GetUserId();
                var mappedModel = _dataMapper.Map<RoleAddEdit, Role>(model);
                mappedModel.CreatedBy = loggedinuserId;
               // mappedModel.CreatedDate = DateTime.Now;
               // mappedModel.ModifiedBy = userId;
                // mappedModel.ModifiedDate = DateTime.Now;

              //  var listPermission1 = _permissionRepository.GetSingle(m => m.Code == "PER_DASHBOARD" && (long)m.PermissionTypeId == PermissionTypeStruct.PermissionTypeConstant.List);
                

                    foreach (var permission in model.Permissions)
                    {
                    // List permission
                    if (permission.IsList)
                    {
                        Permission listPermission = _permissionRepository.Get(m => m.Code == permission.Code && m.PermissionTypeId == PermissionTypeStruct.PermissionTypeConstant.List).FirstOrDefault();
                        if (listPermission != null)
                        {
                            mappedModel.RolePermissions.Add(new RolePermissionMap
                            {
                                PermissionId = listPermission.Id
                            });
                        }
                    }

                    // Add Permission
                    if (permission.IsAdd)
                    {
                        Permission addPermission = _permissionRepository.Get(m => m.Code == permission.Code && m.PermissionTypeId == PermissionTypeStruct.PermissionTypeConstant.Add).FirstOrDefault();
                        if (addPermission != null)
                        {
                            mappedModel.RolePermissions.Add(new RolePermissionMap
                            {
                                PermissionId = addPermission.Id
                            });
                        }
                    }

                    // Edit Permission
                    if (permission.IsEdit)
                    {
                        Permission editPermission = _permissionRepository.Get(m => m.Code == permission.Code && m.PermissionTypeId == PermissionTypeStruct.PermissionTypeConstant.Edit).FirstOrDefault();
                        if (editPermission != null)
                        {
                            mappedModel.RolePermissions.Add(new RolePermissionMap
                            {
                                PermissionId = editPermission.Id
                            });
                        }
                    }

                    if (permission.IsDelete)
                    {
                        Permission editPermission = _permissionRepository.Get(m => m.Code == permission.Code && m.PermissionTypeId == PermissionTypeStruct.PermissionTypeConstant.Delete).FirstOrDefault();
                        if (editPermission != null)
                        {
                            mappedModel.RolePermissions.Add(new RolePermissionMap
                            {
                                PermissionId = editPermission.Id
                            });
                        }
                    }

                    // Export Permission
                    //if (permission.IsExport)
                    //{
                    //    PermissionEntity exportPermission = _permissionRepository.Get(m => m.Code == permission.Code && m.PermissionTypeId == Constants.PermissionTypeConstant.Export).FirstOrDefault();
                    //    if (exportPermission != null)
                    //    {
                    //        entity.RolePermissions.Add(new RolePermissionEntity
                    //        {
                    //            PermissionId = exportPermission.Id
                    //        });
                    //    }
                    //}
                }

                mappedModel.IsActive = true;


                await _roleRepository.AddAsync(mappedModel, loggedinuserId.ToString(), "Insert");

                // Audit Logging
                //var auditLogs = AuditHelper.GenerateAuditLogs(null, mappedModel, "RolePermission", loggedinuserId.ToString(), "Insert");
                //await _context.AuditLog.AddRangeAsync(auditLogs);
                //await _context.SaveChangesAsync();

                return mappedModel.Id;
            }
            catch (Exception ex)
            {
                // Log exception
                return await Task.FromResult(0);
            }
        }

        public async Task<long> UpdateRolePermissionAsync(RoleAddEdit model, long userId)
        {
            try
            {
                long loggedinuserId = _claimAccessorService.GetUserId();
                var entity = _dataMapper.Map<RoleAddEdit, Role>(model);

              var existingRole = await _roleRepository.GetByIdAsync(model.Id);
                var oldData = _dataMapper.Map<Role, RoleAddEdit>(existingRole);

                var oldPermissions = existingRole.RolePermissions.ToList();

                if (model.Id != 0)
                {
                    var mapEntity = await _roleRepository.GetByIdAsync(model.Id);

                    if (mapEntity != null)
                    {
                        entity = _dataMapper.Map(model, mapEntity);

                        var permissionsToRemove = entity.RolePermissions.ToList();
                        foreach (var permission in permissionsToRemove)
                        {
                            _rolePermissionRepository.DeleteEntity(permission);
                            entity.RolePermissions.Remove(permission);
                        }
                    }
                }

                foreach (var permission in model.Permissions)
                {
                    // List permission
                    if (permission.IsList)
                    {
                        Permission listPermission = _permissionRepository.Get(m => m.Code == permission.Code && m.PermissionTypeId == PermissionTypeStruct.PermissionTypeConstant.List).FirstOrDefault();
                        if (listPermission != null)
                        {
                            entity.RolePermissions.Add(new RolePermissionMap
                            {
                                PermissionId = listPermission.Id
                            });
                        }
                    }

                    // Add Permission
                    if (permission.IsAdd)
                    {
                        Permission addPermission = _permissionRepository.Get(m => m.Code == permission.Code && m.PermissionTypeId == PermissionTypeStruct.PermissionTypeConstant.Add).FirstOrDefault();
                        if (addPermission != null)
                        {
                            entity.RolePermissions.Add(new RolePermissionMap
                            {
                                PermissionId = addPermission.Id
                            });
                        }
                    }

                    // Edit Permission
                    if (permission.IsEdit)
                    {
                        Permission editPermission = _permissionRepository.Get(m => m.Code == permission.Code && m.PermissionTypeId == PermissionTypeStruct.PermissionTypeConstant.Edit).FirstOrDefault();
                        if (editPermission != null)
                        {
                            entity.RolePermissions.Add(new RolePermissionMap
                            {
                                PermissionId = editPermission.Id
                            });
                        }
                    }

                    if (permission.IsDelete)
                    {
                        Permission editPermission = _permissionRepository.Get(m => m.Code == permission.Code && m.PermissionTypeId == PermissionTypeStruct.PermissionTypeConstant.Delete).FirstOrDefault();
                        if (editPermission != null)
                        {
                            entity.RolePermissions.Add(new RolePermissionMap
                            {
                                PermissionId = editPermission.Id
                            });
                        }
                    }

                    // Export Permission
                    //if (permission.IsExport)
                    //{
                    //    PermissionEntity exportPermission = _permissionRepository.Get(m => m.Code == permission.Code && m.PermissionTypeId == Constants.PermissionTypeConstant.Export).FirstOrDefault();
                    //    if (exportPermission != null)
                    //    {
                    //        entity.RolePermissions.Add(new RolePermissionEntity
                    //        {
                    //            PermissionId = exportPermission.Id
                    //        });
                    //    }
                    //}
                }

                entity.IsActive = true;

                entity.ModifiedBy = loggedinuserId;
                // entity.ModifiedDate = DateTime.Now;
                await _roleRepository.UpdateAsync(entity, loggedinuserId.ToString(), "Update");

                // Audit Logging Role
                var auditLogs = AuditHelper.GenerateAuditLogs(existingRole, entity, "Role", loggedinuserId.ToString(), "Update");
                await _context.AuditLog.AddRangeAsync(auditLogs);
                await _context.SaveChangesAsync();

                // Audit Logging Permission
                var auditLogsPermission = AuditHelper.GenerateAuditLogs(oldPermissions, entity.RolePermissions, "RolePermission", loggedinuserId.ToString(), "Update");
                await _context.AuditLog.AddRangeAsync(auditLogsPermission);
                await _context.SaveChangesAsync();

                return entity.Id;

            }
            catch (Exception ex)
            {
                // Log exception
                return await Task.FromResult(0);
            }
        }


        public async Task DeleteRolePermissionAsync(long Id, long userId)
        {
            long loggedinuserId = _claimAccessorService.GetUserId();
            var entity = await _roleRepository.GetByIdAsync(Id);
            entity.ModifiedBy = loggedinuserId;
            //entity.ModifiedDate = DateTime.Now;
            entity.IsActive = false;
            await _roleRepository.UpdateAsync(entity, loggedinuserId.ToString(), "Delete");

            // Audit Logging
            //var auditLogs = AuditHelper.GenerateAuditLogs(entity, null, "RolePermission", loggedinuserId.ToString(), "Delete");
            //await _context.AuditLog.AddRangeAsync(auditLogs);
            //await _context.SaveChangesAsync();

            //  return new ResponseModel { IsSuccess = true, Message = "Deleted Successfully." };
        }
    }
}
