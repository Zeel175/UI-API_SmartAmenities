using Application.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class HikvisionSyncService : IHikvisionSyncService
    {
        private readonly AppDbContext _db;

        public HikvisionSyncService(AppDbContext db)
        {
            _db = db;
        }

        public async Task SyncUserBiometricStatusAsync(int deviceId, string employeeNo)
        {
            // TODO: Replace with real device calls
            bool faceEnrolled = true;        // TEMP (replace)
            bool fingerprintEnrolled = true; // TEMP (replace)

            // 1) Try ResidentMaster first
            var resident = await _db.ResidentMasters
                .FirstOrDefaultAsync(x => x.Code == employeeNo);

            if (resident != null)
            {
                ApplyBiometricFlagsToResident(resident, faceEnrolled, fingerprintEnrolled);
                await _db.SaveChangesAsync();
                return;
            }

            // 2) If not found, try ResidentFamilyMember (change DbSet name as per your project)
            var family = await _db.ResidentFamilyMembers
                .FirstOrDefaultAsync(x => x.Code == employeeNo);

            if (family != null)
            {
                ApplyBiometricFlagsToFamily(family, faceEnrolled, fingerprintEnrolled);
                await _db.SaveChangesAsync();
            }
        }

        private static void ApplyBiometricFlagsToResident(dynamic r, bool faceEnrolled, bool fingerprintEnrolled)
        {
            // Only set if enrolled AND DB is empty
            if (faceEnrolled && string.IsNullOrWhiteSpace((string)r.FaceId))
                r.FaceId = "ENROLLED";

            if (fingerprintEnrolled && string.IsNullOrWhiteSpace((string)r.FingerId))
                r.FingerId = "ENROLLED";

            // If you have UpdatedOn/ModifiedOn column, set it here
            // r.UpdatedOn = DateTime.UtcNow;
        }

        private static void ApplyBiometricFlagsToFamily(dynamic f, bool faceEnrolled, bool fingerprintEnrolled)
        {
            if (faceEnrolled && string.IsNullOrWhiteSpace((string)f.FaceId))
                f.FaceId = "ENROLLED";

            if (fingerprintEnrolled && string.IsNullOrWhiteSpace((string)f.FingerId))
                f.FingerId = "ENROLLED";

            // If you have UpdatedOn/ModifiedOn column, set it here
            // f.UpdatedOn = DateTime.UtcNow;
        }
    }
}
