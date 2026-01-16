using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GroupCodeRepository : GenericRepository<GroupCode>, IGroupCodeRepository
    {
        private readonly AppDbContext _dataContext;
        public GroupCodeRepository(AppDbContext dataContext)
            : base(dataContext)
        {
            _dataContext = dataContext;
        }

        //public IQueryable<SelectListModel> GetGroups()
        //{
        //    return _dataContext.Set<GroupCode>()
        //        .OrderBy(o => o.GroupName)
        //        .Select(s => new SelectListEntity { Id = 0, Code = string.Empty, Name = s.GroupName })
        //        .Distinct();
        //}
        //public IQueryable<GroupCodeEntity> GetDataByGroup(string groupName)
        //{
        //    return _dataContext.Set<GroupCodeEntity>()
        //        .Where(w => w.GroupName == groupName)
        //        .OrderBy(o => o.Name);
        //}
    }
}
