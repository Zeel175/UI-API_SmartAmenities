using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class FloorRepository : GenericRepository<Floor>, IFloorRepository
    {
        public FloorRepository(AppDbContext context) : base(context)
        {
        }

        // Implement methods specific to Floor if needed
    }
}
