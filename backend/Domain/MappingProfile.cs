using AutoMapper;
using Domain.Entities;
using Domain.Entities.Domain.Entities;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Permission, PermissionList>();
            CreateMap<Permission, PermissionAddEdit>();

            CreateMap<RolePermissionMap, RolePermissionList>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                    .ForMember(dest => dest.PermissionCode, opt => opt.MapFrom(src => src.Permission.Code));
            //.ForMember(dest => dest.PermissionName, opt => opt.MapFrom(src => src.Permission.Name));

            CreateMap<RolePermissionMap, RolePermissionAddEdit>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.PermissionId, opt => opt.MapFrom(src => src.Permission.Code));

            CreateMap<RolePermissionAddEdit, RolePermissionMap>()
                .ForMember(dest => dest.Role, opt => opt.Ignore()) // Adjust if you need to map Role or Permission explicitly
                .ForMember(dest => dest.Permission, opt => opt.Ignore());

            // Ensure there is only one correct mapping for RoleAddEdit to Role and vice versa
            CreateMap<RoleAddEdit, Role>()
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore()); // Ignore if RolePermissions should not be mapped

            CreateMap<Role, RoleAddEdit>()
                .ForMember(dest => dest.RolePermissions, opt => opt.MapFrom(src => src.RolePermissions.Select(rp => new RolePermissionAddEdit
                {
                    Id = rp.Id,
                    RoleId = rp.RoleId,
                    PermissionId = rp.PermissionId,
                    HasMasterAccess = rp.HasMasterAccess,

                })));

            CreateMap<Role, RoleList>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));



            CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>))
               .ConvertUsing(typeof(PaginatedListConverter<,>));

            CreateMap<Property, PropertyList>().ReverseMap();
            CreateMap<Property, PropertyAddEdit>();
            CreateMap<PropertyAddEdit, Property>().ReverseMap();

            CreateMap<Floor, FloorList>()
               .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.Building.BuildingName));
            CreateMap<Floor, FloorAddEdit>().ReverseMap();

            CreateMap<AuditLog, AuditLogList>().ReverseMap();

            #region GroupCode
            CreateMap<GroupCodeAddEdit, GroupCode>().ReverseMap();
            CreateMap<GroupCode, GroupCodeList>().ReverseMap();
            CreateMap<GroupCode, SelectListModel>();
            CreateMap<SelectList, SelectListModel>();
            #endregion
            #region Building
            // Building
            // AutoMapper profile
            CreateMap<Building, BuildingList>()
                .ForMember(d => d.PropertyName, o => o.MapFrom(s => s.Property.PropertyName));
            CreateMap<Building, BuildingAddEdit>();
            CreateMap<BuildingAddEdit, Building>().ReverseMap();
            #endregion
            #region Unit
            // Unit
            CreateMap<Unit, UnitList>()
                 .ForMember(d => d.BuildingName, o => o.MapFrom(s => s.Building.BuildingName))
                 .ForMember(d => d.FloorName, o => o.MapFrom(s => s.Floor.FloorName))
                 .ForMember(d => d.OccupancyStatusName, o => o.MapFrom(s => s.OccupancyStatus.Name));

            CreateMap<Unit, UnitAddEdit>().ReverseMap();
            CreateMap<UnitAddEdit, Unit>().ReverseMap();
            #endregion
            #region Zone
            CreateMap<Zone, ZoneList>()
                .ForMember(d => d.BuildingName, o => o.MapFrom(s => s.Building != null ? s.Building.BuildingName : null));
            CreateMap<Zone, ZoneAddEdit>().ReverseMap();
            CreateMap<ZoneAddEdit, Zone>().ReverseMap();
            #endregion
            #region ResidentMaster
            //CreateMap<ResidentMaster, ResidentMasterList>()
            //   .ForMember(dest => dest.UnitIds, opt => opt.MapFrom(src => src.ParentUnits != null
            //       ? src.ParentUnits.Select(mu => mu.UnitId)
            //       : Enumerable.Empty<long>()))
            //   .ReverseMap();
            //CreateMap<ResidentMaster, ResidentMasterAddEdit>()
            //    .ForMember(dest => dest.UnitIds, opt => opt.MapFrom(src => src.ParentUnits != null
            //        ? src.ParentUnits.Select(mu => mu.UnitId)
            //        : Enumerable.Empty<long>()))
            //    .ReverseMap();
            CreateMap<ResidentMaster, ResidentMasterAddEdit>()
    .ForMember(d => d.UnitIds,
        o => o.MapFrom(s => s.ParentUnits != null
            ? s.ParentUnits.Select(x => x.UnitId)
            : new List<long>()))
    .ForMember(d => d.Documents, o => o.Ignore())
    .ForMember(d => d.ProfilePhotoFile, o => o.Ignore());

            CreateMap<ResidentMasterAddEdit, ResidentMaster>()
    .ForMember(d => d.ParentUnits, o => o.Ignore())
    .ForMember(d => d.FamilyMembers, o => o.Ignore())
    .ForMember(d => d.ProfilePhoto, o => o.MapFrom(s => s.ProfilePhoto))
    .ForMember(d => d.Id, o => o.Ignore())
    .ForMember(d => d.Code, o => o.Ignore())
    .ForMember(d => d.Documents, o => o.Ignore());



            //CreateMap<ResidentFamilyMember, ResidentFamilyMemberList>()
            //    .ForMember(dest => dest.UnitIds, opt => opt.MapFrom(src => src.MemberUnits != null
            //        ? src.MemberUnits.Select(mu => mu.UnitId)
            //        : Enumerable.Empty<long>()))
            //    .ForMember(dest => dest.Units, opt => opt.MapFrom(src => src.MemberUnits != null
            //        ? src.MemberUnits.Select(mu => new ResidentFamilyMemberUnitList
            //        {
            //            UnitId = mu.UnitId,
            //            UnitCode = mu.Unit != null ? mu.Unit.Code : null,
            //            UnitName = mu.Unit != null ? mu.Unit.UnitName : null
            //        })
            //        : Enumerable.Empty<ResidentFamilyMemberUnitList>()));

            //CreateMap<ResidentFamilyMember, ResidentFamilyMemberAddEdit>()
            //    .ForMember(dest => dest.UnitIds, opt => opt.MapFrom(src => src.MemberUnits != null
            //        ? src.MemberUnits.Select(mu => mu.UnitId)
            //        : Enumerable.Empty<long>()))
            //    .ReverseMap();
            //#endregion
            // ENTITY ➜ VIEWMODEL
            CreateMap<ResidentFamilyMember, ResidentFamilyMemberAddEdit>()
                .ForMember(d => d.UnitIds,
                    o => o.MapFrom(s => s.MemberUnits != null
                        ? s.MemberUnits.Select(mu => mu.UnitId)
                        : new List<long>()))
                .ForMember(d => d.ProfilePhotoFile, o => o.Ignore()); // 👈 VERY IMPORTANT
                                                                      // VIEWMODEL ➜ ENTITY
            CreateMap<ResidentFamilyMemberAddEdit, ResidentFamilyMember>()
                .ForMember(d => d.MemberUnits, o => o.Ignore())      // handled in service
                .ForMember(d => d.ProfilePhoto, o => o.MapFrom(s => s.ProfilePhoto))
                 .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Code, o => o.Ignore());
            #endregion
            #region GuestMaster
            CreateMap<GuestMaster, GuestMasterAddEdit>();
            CreateMap<GuestMasterAddEdit, GuestMaster>();
            #endregion

            #region AmenityMaster
            CreateMap<AmenityMaster, AmenityMasterAddEdit>();
            CreateMap<AmenityMasterAddEdit, AmenityMaster>()
                .ForMember(dest => dest.Documents, opt => opt.Ignore());
            CreateMap<AmenityMaster, AmenityMasterList>()
                .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.Building.BuildingName))
                .ForMember(dest => dest.FloorName, opt => opt.MapFrom(src => src.Floor.FloorName));
            #endregion

            #region AmenityUnit
            CreateMap<AmenityUnitFeature, AmenityUnitFeatureAddEdit>().ReverseMap();
            CreateMap<AmenityUnit, AmenityUnitAddEdit>()
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features));
            CreateMap<AmenityUnitAddEdit, AmenityUnit>()
                .ForMember(dest => dest.Features, opt => opt.Ignore());
            CreateMap<AmenityUnit, AmenityUnitList>()
                .ForMember(dest => dest.AmenityName, opt => opt.MapFrom(src => src.AmenityMaster.Name))
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features));
            #endregion

            #region AmenitySlotTemplate
            CreateMap<AmenitySlotTemplateTime, AmenitySlotTemplateTimeAddEdit>().ReverseMap();
            CreateMap<AmenitySlotTemplateTime, AmenitySlotTemplateTimeList>();
            CreateMap<AmenitySlotTemplate, AmenitySlotTemplateAddEdit>()
                .ForMember(dest => dest.SlotTimes, opt => opt.MapFrom(src => src.SlotTimes));
            CreateMap<AmenitySlotTemplateAddEdit, AmenitySlotTemplate>()
                .ForMember(dest => dest.SlotTimes, opt => opt.Ignore());
            CreateMap<AmenitySlotTemplate, AmenitySlotTemplateList>()
                .ForMember(dest => dest.AmenityName, opt => opt.MapFrom(src => src.AmenityMaster.Name))
                .ForMember(dest => dest.SlotTimes, opt => opt.MapFrom(src => src.SlotTimes));
            #endregion

            #region BookingHeader
            CreateMap<BookingHeader, BookingHeaderAddEdit>();
            CreateMap<BookingHeaderAddEdit, BookingHeader>();
            CreateMap<BookingHeader, BookingHeaderList>()
                .ForMember(dest => dest.AmenityName, opt => opt.MapFrom(src => src.AmenityMaster.Name))
                .ForMember(dest => dest.SocietyName, opt => opt.MapFrom(src => src.Society.PropertyName));
            #endregion
        }

        public class PaginatedListConverter<TSource, TDestination> : ITypeConverter<PaginatedList<TSource>, PaginatedList<TDestination>>
        {
            private readonly IMapper _mapper;

            public PaginatedListConverter(IMapper mapper)
            {
                _mapper = mapper;
            }

            public PaginatedList<TDestination> Convert(PaginatedList<TSource> source, PaginatedList<TDestination> destination, ResolutionContext context)
            {
                var mappedItems = _mapper.Map<List<TDestination>>(source.Items);
                return new PaginatedList<TDestination>(mappedItems, source.TotalCount, source.PageNumber, source.PageSize);
            }
        }
    }
}
