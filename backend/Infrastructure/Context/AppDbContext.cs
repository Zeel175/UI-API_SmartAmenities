using Domain.Entities;
using Domain.Entities.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Context
{
    public class AppDbContext : IdentityDbContext<User, Role, long>
    {
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermissionMap> RolePermissionMaps { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<GroupCode> GroupCodes { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<ResidentMaster> ResidentMasters { get; set; }
        public DbSet<GuestMaster> GuestMasters { get; set; }
        public DbSet<AmenityMaster> AmenityMasters { get; set; }
        public DbSet<AmenityUnit> AmenityUnits { get; set; }
        public DbSet<AmenityUnitFeature> AmenityUnitFeatures { get; set; }
        public DbSet<AmenitySlotTemplate> AmenitySlotTemplates { get; set; }
        public DbSet<AmenitySlotTemplateTime> AmenitySlotTemplateTimes { get; set; }
        public DbSet<BookingHeader> BookingHeaders { get; set; }
        public DbSet<BookingUnit> BookingUnits { get; set; }
        public DbSet<BookingSlot> BookingSlots { get; set; }
        public DbSet<ResidentFamilyMember> ResidentFamilyMembers { get; set; }
        public DbSet<ResidentFamilyMemberUnit> ResidentFamilyMemberUnits { get; set; }
        public DbSet<ResidentUserMap> ResidentUserMaps { get; set; }
        public DbSet<ResidentMasterUnit> ResidentMasterUnits { get; set; }
        public DbSet<HikDevice> HikDevices { get; set; }
        public DbSet<ResidentDocument> ResidentDocuments { get; set; }
        public DbSet<AmenityDocument> AmenityDocuments { get; set; }
        public DbSet<ResidentProfilePhoto> ResidentProfilePhotos { get; set; }
        public DbSet<OtpRequest> OtpRequests { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Permission>()
               .HasKey(sm => sm.Id);

            modelBuilder.Entity<Permission>()
                .ToTable("adm_Permission");

            modelBuilder.Entity<RolePermissionMap>()
                .HasOne(mg => mg.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(mg => mg.RoleId)
                .IsRequired();

            modelBuilder.Entity<RolePermissionMap>()
                .HasOne(mg => mg.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(mg => mg.PermissionId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Role>()
                .HasKey(r => r.Id);
            // Configure entity relationships and constraints here


            // Configure RolePermissionMap
            modelBuilder.Entity<RolePermissionMap>()
                .HasKey(rp => rp.Id);

            modelBuilder.Entity<RolePermissionMap>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermissionMap>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);


            modelBuilder.Entity<RolePermissionMap>()
                .ToTable("adm_RolePermissionMap");


            modelBuilder.Entity<Property>()
               .HasKey(pm => pm.Id);

            modelBuilder.Entity<Property>()
                .ToTable("adm_Property");


            modelBuilder.Entity<AuditLog>()
               .HasKey(pm => pm.Id);

            modelBuilder.Entity<AuditLog>()
                .ToTable("AuditLog");

            modelBuilder.Entity<GroupCode>()
                .HasKey(gc => gc.Id);

            modelBuilder.Entity<GroupCode>()
                .ToTable("adm_GroupCodes");

            // ---------- Building ----------
            modelBuilder.Entity<Building>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Building>()
                .ToTable("adm_Building");

            // Optional but recommended constraints
            modelBuilder.Entity<Building>()
                .Property(b => b.Code)
                .IsRequired();

            modelBuilder.Entity<Building>()
                .Property(b => b.BuildingName)
                .IsRequired();

            // Unique within a Property
            modelBuilder.Entity<Building>()
                .HasIndex(b => new { b.PropertyId, b.Code })
                .IsUnique();

            modelBuilder.Entity<Building>()
                .HasIndex(b => new { b.PropertyId, b.BuildingName })
                .IsUnique();

            // FK: Building → Property
            modelBuilder.Entity<Building>()
                .HasOne(b => b.Property)
                .WithMany(p=>p.Buildings)                // if you later add ICollection<Building> Buildings on Property, change to .WithMany(p => p.Buildings)
                .HasForeignKey(b => b.PropertyId)
                .OnDelete(DeleteBehavior.NoAction); // avoid accidental cascade deletes

            modelBuilder.Entity<Building>()
                .HasOne(b => b.Device)
                .WithMany()
                .HasForeignKey(b => b.DeviceId)
                .OnDelete(DeleteBehavior.NoAction);
            // ---------- Zone ----------
            modelBuilder.Entity<Zone>()
                .HasKey(z => z.Id);

            modelBuilder.Entity<Zone>()
                .ToTable("adm_Zone");

            modelBuilder.Entity<Zone>()
                .Property(z => z.Code)
                .IsRequired();

            modelBuilder.Entity<Zone>()
                .Property(z => z.ZoneName)
                .IsRequired();

            modelBuilder.Entity<Zone>()
                .HasIndex(z => new { z.BuildingId, z.Code })
                .IsUnique();

            modelBuilder.Entity<Zone>()
                .HasIndex(z => new { z.BuildingId, z.ZoneName })
                .IsUnique();

            modelBuilder.Entity<Zone>()
                .HasOne(z => z.Building)
                .WithMany(b => b.Zones)
                .HasForeignKey(z => z.BuildingId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
            // ---------- Floor ----------
            modelBuilder.Entity<Floor>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<Floor>()
                .ToTable("adm_Floor");

            modelBuilder.Entity<Floor>()
                .Property(f => f.Code)
                .IsRequired();

            modelBuilder.Entity<Floor>()
                .Property(f => f.FloorName)
                .IsRequired();

            // Unique within a Building
            modelBuilder.Entity<Floor>()
                .HasIndex(f => new { f.BuildingId, f.Code })
                .IsUnique();

            modelBuilder.Entity<Floor>()
                .HasIndex(f => new { f.BuildingId, f.FloorName })
                .IsUnique();

            // FK: Floor → Building
            modelBuilder.Entity<Floor>()
                .HasOne(f => f.Building)
                .WithMany(b => b.Floors)   // Building has ICollection<Floor> Floors
                .HasForeignKey(f => f.BuildingId)
                .OnDelete(DeleteBehavior.NoAction); // avoid cascading deletes down the chain
            // ---------- Amenity Master ----------
            modelBuilder.Entity<AmenityMaster>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<AmenityMaster>()
                .ToTable("adm_AmenityMaster");

            modelBuilder.Entity<AmenityMaster>()
                .Property(a => a.Name)
                .IsRequired();

            modelBuilder.Entity<AmenityMaster>()
                .Property(a => a.Code)
                .HasMaxLength(50);

            modelBuilder.Entity<AmenityMaster>()
                .Property(a => a.Type)
                .IsRequired();

            modelBuilder.Entity<AmenityMaster>()
                .Property(a => a.Status)
                .IsRequired();

            modelBuilder.Entity<AmenityMaster>()
                .HasOne(a => a.Building)
                .WithMany()
                .HasForeignKey(a => a.BuildingId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AmenityMaster>()
                .HasOne(a => a.Floor)
                .WithMany()
                .HasForeignKey(a => a.FloorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AmenityMaster>()
                .HasOne(a => a.Device)
                .WithMany()
                .HasForeignKey(a => a.DeviceId)
                .OnDelete(DeleteBehavior.NoAction);

            // ---------- Amenity Unit ----------
            modelBuilder.Entity<AmenityUnit>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<AmenityUnit>()
                .ToTable("adm_AmenityUnitMaster");

            modelBuilder.Entity<AmenityUnit>()
                .Property(u => u.UnitName)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<AmenityUnit>()
                .Property(u => u.UnitCode)
                .HasMaxLength(50);

            modelBuilder.Entity<AmenityUnit>()
                .Property(u => u.Status)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<AmenityUnit>()
                .Property(u => u.ShortDescription)
                .HasMaxLength(200);

            modelBuilder.Entity<AmenityUnit>()
                .HasOne(u => u.AmenityMaster)
                .WithMany()
                .HasForeignKey(u => u.AmenityId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AmenityUnit>()
                .HasIndex(u => new { u.AmenityId, u.UnitCode })
                .IsUnique();

            modelBuilder.Entity<AmenityUnitFeature>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<AmenityUnitFeature>()
                .ToTable("adm_AmenityUnitFeature");

            modelBuilder.Entity<AmenityUnitFeature>()
                .Property(f => f.FeatureName)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<AmenityUnitFeature>()
                .HasOne(f => f.AmenityUnit)
                .WithMany(u => u.Features)
                .HasForeignKey(f => f.AmenityUnitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AmenityDocument>()
                .ToTable("adm_AmenityDocument");

            modelBuilder.Entity<AmenityDocument>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<AmenityDocument>()
                .HasOne(d => d.AmenityMaster)
                .WithMany(a => a.Documents)
                .HasForeignKey(d => d.AmenityMasterId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            modelBuilder.Entity<AmenitySlotTemplate>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<AmenitySlotTemplate>()
                .ToTable("adm_AmenitySlotTemplate");

            modelBuilder.Entity<AmenitySlotTemplate>()
                .Property(a => a.DayOfWeek)
                .IsRequired()
                .HasMaxLength(10);

            modelBuilder.Entity<AmenitySlotTemplate>()
                .Property(a => a.SlotDurationMinutes)
                .IsRequired();

            modelBuilder.Entity<AmenitySlotTemplate>()
                .HasOne(a => a.AmenityMaster)
                .WithMany()
                .HasForeignKey(a => a.AmenityId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AmenitySlotTemplate>()
                .HasOne(a => a.AmenityUnit)
                .WithMany()
                .HasForeignKey(a => a.AmenityUnitId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            modelBuilder.Entity<AmenitySlotTemplateTime>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<AmenitySlotTemplateTime>()
                .ToTable("adm_AmenitySlotTemplateTime");

            modelBuilder.Entity<AmenitySlotTemplateTime>()
                .Property(t => t.StartTime)
                .IsRequired();

            modelBuilder.Entity<AmenitySlotTemplateTime>()
                .Property(t => t.EndTime)
                .IsRequired();

            modelBuilder.Entity<AmenitySlotTemplateTime>()
                .HasOne(t => t.AmenitySlotTemplate)
                .WithMany(t => t.SlotTimes)
                .HasForeignKey(t => t.SlotTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------- Booking Header ----------
            modelBuilder.Entity<BookingHeader>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<BookingHeader>()
                .ToTable("adm_BookingHeader");

            modelBuilder.Entity<BookingHeader>()
                .Property(b => b.BookingNo)
                .IsRequired();

            modelBuilder.Entity<BookingHeader>()
                .Property(b => b.BookingDate)
                .IsRequired();

            modelBuilder.Entity<BookingHeader>()
                .Property(b => b.Status)
                .IsRequired();

            modelBuilder.Entity<BookingHeader>()
                .HasOne(b => b.AmenityMaster)
                .WithMany()
                .HasForeignKey(b => b.AmenityId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BookingHeader>()
                .HasOne(b => b.Society)
                .WithMany()
                .HasForeignKey(b => b.SocietyId)
                .OnDelete(DeleteBehavior.NoAction);

            // ---------- Booking Unit ----------
            modelBuilder.Entity<BookingUnit>()
                .HasKey(bu => bu.Id);

            modelBuilder.Entity<BookingUnit>()
                .ToTable("adm_BookingUnit");

            modelBuilder.Entity<BookingUnit>()
                .Property(bu => bu.UnitNameSnapshot)
                .HasMaxLength(200);

            modelBuilder.Entity<BookingUnit>()
                .HasOne(bu => bu.BookingHeader)
                .WithMany(b => b.BookingUnits)
                .HasForeignKey(bu => bu.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingUnit>()
                .HasOne(bu => bu.AmenityUnit)
                .WithMany()
                .HasForeignKey(bu => bu.AmenityUnitId)
                .OnDelete(DeleteBehavior.NoAction);

            // ---------- Booking Slot ----------
            modelBuilder.Entity<BookingSlot>()
                .HasKey(bs => bs.Id);

            modelBuilder.Entity<BookingSlot>()
                .ToTable("adm_BookingSlot");

            modelBuilder.Entity<BookingSlot>()
                .Property(bs => bs.SlotStartDateTime)
                .IsRequired();

            modelBuilder.Entity<BookingSlot>()
                .Property(bs => bs.SlotEndDateTime)
                .IsRequired();

            modelBuilder.Entity<BookingSlot>()
                .Property(bs => bs.SlotStatus)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<BookingSlot>()
                .HasOne(bs => bs.BookingHeader)
                .WithMany(b => b.BookingSlots)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BookingSlot>()
                .HasOne(bs => bs.BookingUnit)
                .WithMany(bu => bu.BookingSlots)
                .HasForeignKey(bs => bs.BookingUnitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingSlot>()
                .HasOne(bs => bs.AmenityMaster)
                .WithMany()
                .HasForeignKey(bs => bs.AmenityId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BookingSlot>()
                .HasOne(bs => bs.AmenityUnit)
                .WithMany()
                .HasForeignKey(bs => bs.AmenityUnitId)
                .OnDelete(DeleteBehavior.NoAction);
                                                    // ---------- Unit ----------
            modelBuilder.Entity<Unit>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Unit>()
                .ToTable("adm_Unit");

            modelBuilder.Entity<Unit>()
                .Property(u => u.Code)
                .IsRequired();

            modelBuilder.Entity<Unit>()
                .Property(u => u.UnitName)
                .IsRequired();

            // (Optional) length constraints to keep data tidy
            modelBuilder.Entity<Unit>()
                .Property(u => u.Code)
                .HasMaxLength(50);

            modelBuilder.Entity<Unit>()
                .Property(u => u.UnitName)
                .HasMaxLength(200);

            // Uniqueness per parent: Unit code/name unique within a Floor
            modelBuilder.Entity<Unit>()
                .HasIndex(u => new { u.FloorId, u.Code })
                .IsUnique();

            modelBuilder.Entity<Unit>()
                .HasIndex(u => new { u.FloorId, u.UnitName })
                .IsUnique();

            // FK: Unit → Building
            modelBuilder.Entity<Unit>()
                .HasOne(u => u.Building)
                .WithMany(b =>b.Units) // or .WithMany(b => b.Units) if you add ICollection<Unit> Units in Building
                .HasForeignKey(u => u.BuildingId)
                .OnDelete(DeleteBehavior.NoAction);

            // FK: Unit → Floor
            modelBuilder.Entity<Unit>()
                .HasOne(u => u.Floor)
                .WithMany(f => f.Units) // Floor already has ICollection<Unit> Units
                .HasForeignKey(u => u.FloorId)
                .OnDelete(DeleteBehavior.NoAction);

            // FK: Unit → GroupCode (OccupancyStatus)
            modelBuilder.Entity<Unit>()
                .HasOne(u => u.OccupancyStatus)
                .WithMany(u=>u.Units)
                .HasForeignKey(u => u.OccupancyStatusId)
                .OnDelete(DeleteBehavior.NoAction);

            // (Nice to have) Ensure GroupCode has a unique (GroupName, Code) for sanity
            modelBuilder.Entity<GroupCode>()
                .HasIndex(g => new { g.GroupName, g.Code })
                .IsUnique();
            // ---------- Resident Master ----------
            modelBuilder.Entity<ResidentMaster>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<ResidentMaster>()
                .ToTable("adm_ResidentMaster");

            modelBuilder.Entity<ResidentMaster>()
                .Property(r => r.Code)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<ResidentMaster>()
                .HasIndex(r => r.Code)
                .IsUnique();

            modelBuilder.Entity<ResidentMaster>()
                .Property(r => r.ParentFirstName)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<ResidentMaster>()
                .Property(r => r.ParentLastName)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<ResidentMaster>()
                .Property(r => r.CountryCode)
                .HasMaxLength(10);
            // ---------- Guest Master ----------
            modelBuilder.Entity<GuestMaster>()
                .HasKey(g => g.Id);

            modelBuilder.Entity<GuestMaster>()
                .ToTable("adm_GuestMaster");

            modelBuilder.Entity<GuestMaster>()
                .Property(g => g.Code)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<GuestMaster>()
                .HasIndex(g => g.Code)
                .IsUnique();

            modelBuilder.Entity<GuestMaster>()
                .Property(g => g.CardId)
                .HasMaxLength(20);

            modelBuilder.Entity<GuestMaster>()
                .Property(g => g.FirstName)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<GuestMaster>()
                .Property(g => g.LastName)
                .HasMaxLength(200)
                .IsRequired(false);   // ✅ allow null

            modelBuilder.Entity<GuestMaster>()
                .Property(g => g.Mobile)
                .HasMaxLength(50)
                .IsRequired();        // ✅ NOT NULL

            modelBuilder.Entity<GuestMaster>()
                .Property(g => g.CountryCode)
                .HasMaxLength(10);

            modelBuilder.Entity<GuestMaster>()
                .HasOne(g => g.Unit)
                .WithMany()
                .HasForeignKey(g => g.UnitId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ResidentMasterUnit>()
               .HasKey(rmu => rmu.Id);

            modelBuilder.Entity<ResidentMasterUnit>()
                .ToTable("adm_ResidentMasterUnit");

            modelBuilder.Entity<ResidentMasterUnit>()
                .HasOne(rmu => rmu.ResidentMaster)
                .WithMany(r => r.ParentUnits)
                .HasForeignKey(rmu => rmu.ResidentMasterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResidentMasterUnit>()
                .HasOne(rmu => rmu.Unit)
                .WithMany()
                .HasForeignKey(rmu => rmu.UnitId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ResidentFamilyMember>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<ResidentFamilyMember>()
                .ToTable("adm_ResidentFamilyMember");

            modelBuilder.Entity<ResidentFamilyMember>()
                .Property(f => f.FirstName)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<ResidentFamilyMember>()
                .Property(f => f.Code)
                .HasMaxLength(50);

            modelBuilder.Entity<ResidentFamilyMember>()
                .Property(f => f.LastName)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<ResidentFamilyMember>()
                .HasIndex(f => f.Code)
                .IsUnique();
            modelBuilder.Entity<ResidentFamilyMember>()
                .HasOne(f => f.ResidentMaster)
                .WithMany(r => r.FamilyMembers)
                .HasForeignKey(f => f.ResidentMasterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResidentFamilyMemberUnit>()
                .HasKey(fmu => fmu.Id);

            modelBuilder.Entity<ResidentFamilyMemberUnit>()
                .ToTable("adm_ResidentFamilyMemberUnit");

            modelBuilder.Entity<ResidentFamilyMemberUnit>()
                .HasOne(fmu => fmu.ResidentFamilyMember)
                .WithMany(f => f.MemberUnits)
                .HasForeignKey(fmu => fmu.ResidentFamilyMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResidentFamilyMemberUnit>()
                .HasOne(fmu => fmu.Unit)
                .WithMany()
                .HasForeignKey(fmu => fmu.UnitId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ResidentUserMap>()
                .HasKey(map => map.Id);

            modelBuilder.Entity<ResidentUserMap>()
                .ToTable("adm_ResidentUserMap");

            modelBuilder.Entity<ResidentUserMap>()
                .HasOne(map => map.ResidentMaster)
                .WithMany()
                .HasForeignKey(map => map.ResidentMasterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResidentUserMap>()
                .HasOne(map => map.ResidentFamilyMember)
                .WithMany()
                .HasForeignKey(map => map.ResidentFamilyMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResidentUserMap>()
                .HasOne(map => map.User)
                .WithMany()
                .HasForeignKey(map => map.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ResidentUserMap>()
                .HasIndex(map => new { map.ResidentMasterId, map.UserId })
                .IsUnique();

            modelBuilder.Entity<ResidentUserMap>()
                .HasIndex(map => new { map.ResidentFamilyMemberId, map.UserId })
                .IsUnique();
            // ---------- HikDevice ----------
            modelBuilder.Entity<HikDevice>()
                .ToTable("hik_Devices");

            modelBuilder.Entity<HikDevice>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<HikDevice>()
                .Property(d => d.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<HikDevice>()
                .Property(d => d.DevIndex)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<HikDevice>()
                .HasIndex(d => d.DevIndex)
                .IsUnique();

            modelBuilder.Entity<HikDevice>()
                .Property(d => d.DevName)
                .HasMaxLength(200);

            modelBuilder.Entity<HikDevice>()
                .Property(d => d.IpAddress)
                .HasMaxLength(50);

            modelBuilder.Entity<HikDevice>()
                .Property(d => d.DevMode)
                .HasMaxLength(100);

            modelBuilder.Entity<HikDevice>()
                .Property(d => d.DevType)
                .HasMaxLength(50);

            modelBuilder.Entity<HikDevice>()
                .Property(d => d.DevStatus)
                .HasMaxLength(50);

            modelBuilder.Entity<HikDevice>()
                .Property(d => d.DevVersion)
                .HasMaxLength(200);

            modelBuilder.Entity<HikDevice>()
                .Property(d => d.ProtocolType)
                .HasMaxLength(50);

            modelBuilder.Entity<HikDevice>()
                .Property(d => d.RawJson)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<HikDevice>()
                .Property(d => d.LastSyncedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");
            modelBuilder.Entity<ResidentDocument>()
    .ToTable("adm_ResidentDocument");

            modelBuilder.Entity<ResidentDocument>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<ResidentDocument>()
                .HasOne(d => d.ResidentMaster)
                .WithMany(r => r.Documents)
                .HasForeignKey(d => d.ResidentMasterId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
            modelBuilder.Entity<ResidentProfilePhoto>()
               .ToTable("adm_ResidentProfilePhoto");

            modelBuilder.Entity<ResidentProfilePhoto>()
                .HasKey(p => p.Id);
        }
    }
}
