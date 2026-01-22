using Application.Helper;
using Application.Interfaces;
using Application.Services;
using Domain;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Infrastructure.Context;
using Infrastructure.Integrations.Hikvision;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// ? Add this
builder.Services.AddDataProtection()
    .SetApplicationName("SmartAmenities")
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\SmartAmenities\DataProtectionKeys"));

// Configure database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity with long as ID type for Role
builder.Services.AddIdentity<User, Role>(options =>
{
    // Configure identity options if needed
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomClaimsPrincipalFactory>();

// Configure JWT Bearer authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var key = jwtSettings["SecretKey"];
    var issuer = jwtSettings["Issuer"];
    var audience = jwtSettings["Audience"];

    if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
    {
        throw new ArgumentNullException("JWT settings are not properly configured.");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

//                                   Don't Uncomment this
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
//        options.JsonSerializerOptions.MaxDepth = 64; // Increase if needed
//    });


// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register application services
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IClaimAccessorService, ClaimAccessorService>();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddTransient<IAutoMapperGenericDataMapper, AutoMapperGenericDataMapper>();

builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IGroupCodeRepository, GroupCodeRepository>();
builder.Services.AddScoped<IGroupCodeService, GroupCodeService>();
builder.Services.AddScoped<IBuildingRepository, BuildingRepository>();
builder.Services.AddScoped<IBuildingService, BuildingService>();
builder.Services.AddScoped<IFloorRepository, FloorRepository>();
builder.Services.AddScoped<IFloorService, FloorService>();
builder.Services.AddScoped<IAmenityMasterRepository, AmenityMasterRepository>();
builder.Services.AddScoped<IAmenityMasterService, AmenityMasterService>();
builder.Services.AddScoped<IAmenitySlotTemplateRepository, AmenitySlotTemplateRepository>();
builder.Services.AddScoped<IAmenitySlotTemplateService, AmenitySlotTemplateService>();
builder.Services.AddScoped<IBookingHeaderRepository, BookingHeaderRepository>();
builder.Services.AddScoped<IBookingHeaderService, BookingHeaderService>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IZoneRepository, ZoneRepository>();
builder.Services.AddScoped<IZoneService, ZoneService>();
builder.Services.AddScoped<IResidentMasterRepository, ResidentMasterRepository>();
builder.Services.AddScoped<IResidentFamilyMemberRepository, ResidentFamilyMemberRepository>();
builder.Services.AddScoped<IResidentMasterService, ResidentMasterService>();
builder.Services.AddScoped<IResidentDocumentRepository, ResidentDocumentRepository>();
builder.Services.AddScoped<IResidentDocumentService, ResidentDocumentService>();
builder.Services.AddScoped<IAmenityDocumentRepository, AmenityDocumentRepository>();
builder.Services.AddScoped<IAmenityDocumentService, AmenityDocumentService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IGuestMasterRepository, GuestMasterRepository>();
builder.Services.AddScoped<IGuestMasterService, GuestMasterService>();
builder.Services.AddScoped<DeviceSyncService>();
builder.Services.Configure<DeviceSyncOptions>(
    builder.Configuration.GetSection("DeviceSync"));
builder.Services.AddHostedService<DeviceSyncHostedService>();
builder.Services.Configure<BiometricSyncOptions>(
    builder.Configuration.GetSection("BiometricSync"));
builder.Services.AddHostedService<HikvisionBiometricSyncHostedService>();
builder.Services.AddSingleton<ISecretProtector, SecretProtector>();
builder.Services.AddScoped<IResidentProfilePhotoRepository, ResidentProfilePhotoRepository>();
builder.Services.AddScoped<IResidentProfilePhotoService, ResidentProfilePhotoService>();
builder.Services.AddScoped<ResidentMasterService>();
builder.Services.AddScoped<GuestMasterService>();
builder.Services.AddScoped<IHikvisionLogsService, HikvisionLogsService>();
builder.Services.AddScoped<IHikvisionSyncService, HikvisionSyncService>();
// Register generic and specific repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//builder.Services.AddHostedService<HikvisionAlertStreamWorker>();

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

    // ✅ THIS LINE IS THE MAIN THING:
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});


// Add controllers
builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartAmenities API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your_token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// Hikvision options
builder.Services.Configure<HikvisionOptions>(
    builder.Configuration.GetSection("Hikvision"));

// Hikvision Digest HttpClient
builder.Services.AddHttpClient<HikvisionClient>((sp, client) =>
{
    var opt = sp.GetRequiredService<IOptions<HikvisionOptions>>().Value;

    var baseUrl = opt.BaseUrl.EndsWith("/") ? opt.BaseUrl : opt.BaseUrl + "/";
    client.BaseAddress = new Uri(baseUrl);

    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
})
.ConfigurePrimaryHttpMessageHandler(sp =>
{
    var opt = sp.GetRequiredService<IOptions<HikvisionOptions>>().Value;

    var handler = new HttpClientHandler
    {
        PreAuthenticate = true,
        UseCookies = false
    };

    var cache = new CredentialCache
    {
        { new Uri(opt.BaseUrl), "Digest", new NetworkCredential(opt.Username, opt.Password) }
    };

    handler.Credentials = cache;
    return handler;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
// Use CORS
app.UseCors();

// Use authentication and authorization middleware
app.UseAuthentication(); // Ensure this is before UseAuthorization
app.UseAuthorization();

// Configure HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{

//}

app.MapControllers().RequireAuthorization();

// Seed data if needed
await SeedData(app);

app.Run();

// Method to seed data
async Task SeedData(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    // var permissionManager = scope.ServiceProvider.GetRequiredService<IPermissionRepository>();
    // var rolePermissionManager = scope.ServiceProvider.GetRequiredService<IRolePermissionRepository>();

    //var propertyRepository = scope.ServiceProvider.GetRequiredService<IPropertyRepository>();

    // Seed roles
    string[] roles = { "Admin", "User", "Manager" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var newRole = new Role
            {
                Name = role,
                IsActive = true
            };
            await roleManager.CreateAsync(newRole);
        }
    }

    // Seed users
    var adminEmail = "admin@nutem.com";
    var adminPassword = "Admin@123";
    var adminName = "steve";

    if (userManager.Users.All(u => u.UserName != adminEmail))
    {
        var adminUser = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            Name = adminName
        };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    var user1Email = "user1@nutem.com";
    var user2Email = "user2@nutem.com";
    var userPassword = "User@123";
    var user1Name = "adam";
    var user2Name = "alpha";

    if (userManager.Users.All(u => u.UserName != user1Email))
    {
        var user1 = new User
        {
            UserName = user1Email,
            Email = user1Email,
            Name = user1Name
        };
        var result = await userManager.CreateAsync(user1, userPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user1, "User");
        }
    }

    if (userManager.Users.All(u => u.UserName != user2Email))
    {
        var user2 = new User
        {
            UserName = user2Email,
            Email = user2Email,
            Name = user2Name
        };
        var result = await userManager.CreateAsync(user2, userPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user2, "User");
        }
    }
}
