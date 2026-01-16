using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using System.Linq;

namespace Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;

        public PermissionService(IPermissionRepository permissionRepository,
                                 IAutoMapperGenericDataMapper dataMapper)
        {
            _permissionRepository = permissionRepository;
            _dataMapper = dataMapper;
        }

        public IQueryable<PermissionList> GetAllPermissionsAsync()
        {
            var entities = _permissionRepository.GetAllPermission();
            return _dataMapper.Project<Permission, PermissionList>(entities);
        }
    }
}
