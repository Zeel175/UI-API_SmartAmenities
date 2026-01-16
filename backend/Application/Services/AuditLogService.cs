using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly AppDbContext _context;
        private readonly IClaimAccessorService _claimAccessorService;

        public AuditLogService(IAuditLogRepository auditLogRepository,
            IAutoMapperGenericDataMapper dataMapper, AppDbContext context, IClaimAccessorService claimAccessorService)
        {
            _auditLogRepository = auditLogRepository;
            _dataMapper = dataMapper;
            _context = context;
            _claimAccessorService = claimAccessorService;
        }

        public async Task<PaginatedList<AuditLogList>> GetAllAuditLogListAsync(int pageIndex, int pageSize)
        {
            var query = _auditLogRepository.Get();

            var totalCount = await query.CountAsync();
            var logs = await query
                .OrderByDescending(x => x.Id)   // ✅ descending
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map data using DataMapper (if needed)
            var mappedAuditLogs = _dataMapper.Project<AuditLog, AuditLogList>(logs.AsQueryable());

            return new PaginatedList<AuditLogList>(mappedAuditLogs.ToList(), totalCount, pageIndex, pageSize);
        }
    }
}
