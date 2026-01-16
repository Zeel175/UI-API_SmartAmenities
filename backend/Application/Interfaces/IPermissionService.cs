using Domain.ViewModels;
using System.Linq;

namespace Application.Interfaces
{
    public interface IPermissionService
    {
        IQueryable<PermissionList> GetAllPermissionsAsync();

    }
}
