using Domain.Entities.Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ResidentFamilyMemberRepository : GenericRepository<ResidentFamilyMember>, IResidentFamilyMemberRepository
    {
        public ResidentFamilyMemberRepository(AppDbContext context) : base(context)
        {
        }
    }
}
