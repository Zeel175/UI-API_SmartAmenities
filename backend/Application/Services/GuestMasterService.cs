using Application.Helper;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Infrastructure.Context;
using Infrastructure.Integrations.Hikvision;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BuildingEntity = Domain.Entities.Building;

namespace Application.Services
{
    public class GuestMasterService : IGuestMasterService
    {
        private readonly IGuestMasterRepository _guestMasterRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly IClaimAccessorService _claimAccessorService;
        private readonly AppDbContext _context;
        private readonly HikvisionClient _hikvisionClient;
        private readonly ISecretProtector _secretProtector;
        private readonly bool _qrEncryptionEnabled;
        private readonly byte[]? _qrEncryptionKey;
        private readonly string _webRootPath;
        private readonly string _filesBaseUrl;
        private readonly UserManager<User> _userManager;
        private const string DefaultGuestPassword = "User@123";
        private const int CardIdWidth = 7; // 0000001 (7 digits)

        public GuestMasterService(
            IGuestMasterRepository guestMasterRepository,
            IAutoMapperGenericDataMapper dataMapper,
            IClaimAccessorService claimAccessorService,
            AppDbContext context,
            HikvisionClient hikvisionClient,
            ISecretProtector secretProtector,
            IConfiguration configuration,
            UserManager<User> userManager)
        {
            _guestMasterRepository = guestMasterRepository;
            _dataMapper = dataMapper;
            _claimAccessorService = claimAccessorService;
            _context = context;
            _hikvisionClient = hikvisionClient;
            _secretProtector = secretProtector;
            _userManager = userManager;

            _webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            _filesBaseUrl =
                configuration["Files:BaseUrl"]?.TrimEnd('/')
                ?? configuration["Jwt:Issuer"]?.TrimEnd('/')
                ?? string.Empty;

            _qrEncryptionEnabled = bool.TryParse(configuration["QrCodeEncryption:Enabled"], out var enabled) && enabled;
            _qrEncryptionKey = _qrEncryptionEnabled ? LoadQrEncryptionKey(configuration) : null;


        }
        private static string GetUserName(string email, string mobile, string fallback)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                return email.Trim();
            }

            if (!string.IsNullOrWhiteSpace(mobile))
            {
                return mobile.Trim();
            }

            return string.IsNullOrWhiteSpace(fallback) ? Guid.NewGuid().ToString("N") : fallback.Trim();
        }

        private async Task<User?> EnsureGuestUserAsync(GuestMaster entity, long createdBy)
        {
            if (!entity.IsActive)
            {
                return null;
            }

            var userName = GetUserName(entity.Email, entity.Mobile, entity.Code);

            User existingUser = null;
            if (!string.IsNullOrWhiteSpace(entity.Email))
            {
                existingUser = await _userManager.FindByEmailAsync(entity.Email.Trim());
            }

            if (existingUser == null)
            {
                existingUser = await _userManager.FindByNameAsync(userName);
            }

            if (existingUser != null)
            {
                return existingUser;
            }

            var user = new User
            {
                UserName = userName,
                Email = string.IsNullOrWhiteSpace(entity.Email) ? null : entity.Email.Trim(),
                Name = $"{entity.FirstName} {entity.LastName}".Trim(),
                PhoneNumber = string.IsNullOrWhiteSpace(entity.Mobile) ? null : entity.Mobile.Trim(),
                IsActive = true,
                IsGuest = true,
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, DefaultGuestPassword);
            return result.Succeeded ? user : null;
        }
        private static byte[] LoadQrEncryptionKey(IConfiguration configuration)
        {
            var base64Key = configuration["QrCodeEncryption:Key"];
            if (string.IsNullOrWhiteSpace(base64Key))
                throw new InvalidOperationException("QR code encryption key is not configured.");

            byte[] key;
            try { key = Convert.FromBase64String(base64Key); }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("QR code encryption key must be a valid Base64 string.", ex);
            }

            if (key.Length != 32)
                throw new InvalidOperationException("QR code encryption key must be 256 bits (32 bytes).");

            return key;
        }
        private string EncryptQrValue(string qrValue)
{
    // ✅ no encryption (store plain value)
    return qrValue;
}

private string DecryptQrValue(string encryptedQrValue)
{
    // ✅ no decryption (already plain value)
    return encryptedQrValue;
}
        //private string EncryptQrValue(string qrValue)
        //{
        //    if (string.IsNullOrWhiteSpace(qrValue)) return qrValue;

        //    Span<byte> nonce = stackalloc byte[12];
        //    RandomNumberGenerator.Fill(nonce);

        //    var nonceBytes = nonce.ToArray();
        //    var plaintext = Encoding.UTF8.GetBytes(qrValue);
        //    var ciphertext = new byte[plaintext.Length];
        //    var tag = new byte[AesGcm.TagByteSizes.MaxSize];

        //    using (var aesGcm = new AesGcm(_qrEncryptionKey))
        //        aesGcm.Encrypt(nonceBytes, plaintext, ciphertext, tag);

        //    var encryptedPayload = new byte[nonceBytes.Length + tag.Length + ciphertext.Length];
        //    Buffer.BlockCopy(nonceBytes, 0, encryptedPayload, 0, nonceBytes.Length);
        //    Buffer.BlockCopy(tag, 0, encryptedPayload, nonceBytes.Length, tag.Length);
        //    Buffer.BlockCopy(ciphertext, 0, encryptedPayload, nonceBytes.Length + tag.Length, ciphertext.Length);

        //    return Convert.ToBase64String(encryptedPayload);
        //}

        //private string DecryptQrValue(string encryptedQrValue)
        //{
        //    if (string.IsNullOrWhiteSpace(encryptedQrValue)) return encryptedQrValue;

        //    var encryptedPayload = Convert.FromBase64String(encryptedQrValue);

        //    var minimumLength = 12 + AesGcm.TagByteSizes.MaxSize;
        //    if (encryptedPayload.Length < minimumLength)
        //        throw new InvalidOperationException("Encrypted QR value is too short to contain required data.");

        //    var nonce = encryptedPayload.AsSpan(0, 12);
        //    var tag = encryptedPayload.AsSpan(12, AesGcm.TagByteSizes.MaxSize);
        //    var ciphertext = encryptedPayload.AsSpan(12 + AesGcm.TagByteSizes.MaxSize);

        //    var plaintext = new byte[ciphertext.Length];

        //    using (var aesGcm = new AesGcm(_qrEncryptionKey))
        //        aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);

        //    return Encoding.UTF8.GetString(plaintext);
        //}

        private string BuildGuestQrValue(GuestMaster g)
        {
            // ✅ Keep it simple + stable (pipe separated like attendee)
            // Code|GuestId|UnitId|From|To
            var from = g.FromDateTime?.ToString("yyyy-MM-ddTHH:mm:ss") ?? "";
            var to = g.ToDateTime?.ToString("yyyy-MM-ddTHH:mm:ss") ?? "";
            return $"{g.QrId}";
        }
        //private string GenerateQrIdFromEntity(GuestMaster e)
        //{
        //    var firstInitial = string.IsNullOrWhiteSpace(e.FirstName) ? "X" : e.FirstName.Trim()[0].ToString().ToUpper();
        //    var lastInitial = string.IsNullOrWhiteSpace(e.LastName) ? "X" : e.LastName.Trim()[0].ToString().ToUpper();

        //    // Example: MR + GST0000001 + 000123
        //    return $"{firstInitial}{lastInitial}{e.Code}{e.Id:000000}";
        //}
        private async Task<string> GenerateGuestQrImageAsync(string encryptedQrValue, CancellationToken ct = default)
        {
            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(encryptedQrValue, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(data);
            var qrBytes = qrCode.GetGraphic(20);

            var fileName = $"guest_qr_{Guid.NewGuid():N}.png";
            var folder = Path.Combine(_webRootPath, "uploads", "Guests-QRCode");
            Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(filePath, qrBytes, ct);

            return $"/uploads/Guests-QRCode/{fileName}";
        }

        private string GenerateCode()
        {
            var count = _guestMasterRepository.Get().Select(g => g.Code).Distinct().Count();
            var code = "GST" + (count + 1).ToString("0000000");
            return code.ToUpper();
        }
        private string GenerateCardId()
        {
            var count = _guestMasterRepository.Get().Select(g => g.Code).Distinct().Count();
            var code = "GCARD" + (count + 1).ToString("0000000");
            return code.ToUpper();
        }
        //private string GenerateCardId()
        //{
        //    var maxCardId = _guestMasterRepository.Get()
        //        .Select(g => g.CardId)
        //        .AsEnumerable()
        //        .Select(cardId => long.TryParse(cardId, out var value) ? value : 0)
        //        .DefaultIfEmpty(0)
        //        .Max();

        //    return (maxCardId + 1).ToString();
        //}
        private async Task<string> GenerateCardIdAsync(CancellationToken ct = default)
        {
            await using var conn = _context.Database.GetDbConnection();

            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT NEXT VALUE FOR dbo.GlobalCardNoSeq;";

            var result = await cmd.ExecuteScalarAsync(ct);
            var next = Convert.ToInt64(result);

            return next.ToString($"D{CardIdWidth}"); // ✅ 0000001, 0000002...
        }



        //private string GenerateQrId(string code, string firstName, string lastName)
        //{
        //    var sequence = _guestMasterRepository.Get().Select(g => g.QrId).Count() + 1;
        //    var firstInitial = string.IsNullOrWhiteSpace(firstName) ? "X" : firstName.Trim()[0].ToString().ToUpper();
        //    var lastInitial = string.IsNullOrWhiteSpace(lastName) ? "X" : lastName.Trim()[0].ToString().ToUpper();

        //    return $"{firstInitial}{lastInitial}{code}{sequence:0000}";
        //}

        //private async Task<string> SaveQrCodeAsync(string qrValue, string guestCode)
        //{
        //    using var generator = new QRCodeGenerator();
        //    using var qrData = generator.CreateQrCode(qrValue, QRCodeGenerator.ECCLevel.Q);
        //    var qrCode = new PngByteQRCode(qrData);
        //    var bytes = qrCode.GetGraphic(20);

        //    return await _fileStorage.SaveQrPngAsync(bytes, guestCode);
        //}

        private async Task<string?> TryAddGuestToHikvisionAsync(GuestMaster entity)
        {
            try
            {
                var unit = await _context.Set<Unit>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == entity.UnitId);

                if (unit == null)
                {
                    return "Unit not found for guest.";
                }

                var building = await _context.Set<BuildingEntity>()
                    .AsNoTracking()
                    .Include(b => b.Device)
                    .FirstOrDefaultAsync(b => b.Id == unit.BuildingId);

                if (building?.Device == null || string.IsNullOrWhiteSpace(building.Device.IpAddress))
                {
                    return "Hikvision device not found for building.";
                }

                if (string.IsNullOrWhiteSpace(building.DeviceUserName)
                    || string.IsNullOrWhiteSpace(building.DevicePassword))
                {
                    return "Hikvision device credentials not found for building.";
                }

                var devicePassword = building.DevicePassword;
                if (_secretProtector.IsProtected(devicePassword))
                {
                    devicePassword = _secretProtector.Unprotect(devicePassword);
                }

                var port = building.Device.PortNo ?? 80;

                var person = new HikvisionPersonInfo
                {
                    EmployeeNo = entity.Code,
                    Name = $"{entity.FirstName} {entity.LastName}".Trim()
                };

                await _hikvisionClient.AddPersonToDeviceAsync(
                    building.Device.IpAddress,
                    port,
                    building.DeviceUserName,
                    devicePassword,
                    person,
                    building.Device.DevIndex,
                    userType: "visitor");

                await _hikvisionClient.AddCardToDeviceAsync(
                    building.Device.IpAddress,
                    port,
                    building.DeviceUserName,
                    devicePassword,
                    new HikvisionCardInfo
                    {
                        EmployeeNo = entity.Code,
                        CardNo = entity.CardId,
                        CardType = "normalCard"
                    });

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private static string NormalizeCardId(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            var trimmed = value.Trim();

            if (!long.TryParse(trimmed, out var n) || n <= 0)
                throw new ValidationException("CardId must be a positive number.");

            return n.ToString($"D{CardIdWidth}"); // ✅ always padded
        }

        public async Task<InsertResponseModel> CreateGuestAsync(GuestMasterAddEdit guest)
        {
            try
            {
                var userId = 1;
                //if (!string.IsNullOrWhiteSpace(guest.Email))
                //{
                //    var trimmedEmail = guest.Email.Trim();
                //    var emailValidator = new EmailAddressAttribute();

                //    if (!emailValidator.IsValid(trimmedEmail))
                //    {
                //        return new InsertResponseModel
                //        {
                //            Id = 0,
                //            Code = "",
                //            Message = "Invalid email address format."
                //        };
                //    }

                //    var normalizedEmail = trimmedEmail.ToLowerInvariant();
                //    var emailExists = await _context.Set<GuestMaster>()
                //        .AnyAsync(x => x.Email != null && x.Email.ToLower() == normalizedEmail);

                //    if (emailExists)
                //    {
                //        return new InsertResponseModel
                //        {
                //            Id = 0,
                //            Code = "",
                //            Message = $"Email '{trimmedEmail}' already exists."
                //        };
                //    }

                //    guest.Email = trimmedEmail;
                //}

                //if (!string.IsNullOrWhiteSpace(guest.Mobile))
                //{
                //    var trimmedMobile = guest.Mobile.Trim();
                //    var phoneRegex = new Regex(@"^\+?[0-9]{7,15}$");

                //    if (!phoneRegex.IsMatch(trimmedMobile))
                //    {
                //        return new InsertResponseModel
                //        {
                //            Id = 0,
                //            Code = "",
                //            Message = "Invalid mobile number format. Use 7-15 digits, optional leading +."
                //        };
                //    }

                //    var mobileExists = await _context.Set<GuestMaster>()
                //        .AnyAsync(x => x.Mobile == trimmedMobile);

                //    if (mobileExists)
                //    {
                //        return new InsertResponseModel
                //        {
                //            Id = 0,
                //            Code = "",
                //            Message = $"Mobile '{trimmedMobile}' already exists."
                //        };
                //    }

                //    guest.Mobile = trimmedMobile;
                //}
                if (string.IsNullOrWhiteSpace(guest.CardId) && !string.IsNullOrWhiteSpace(guest.QrId))
                {
                    guest.CardId = guest.QrId;
                }

                if (!string.IsNullOrWhiteSpace(guest.CardId)
                    && !string.IsNullOrWhiteSpace(guest.QrId)
                    && !string.Equals(guest.CardId.Trim(), guest.QrId.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "",
                        Message = "QrId must match CardId."
                    };
                }
                if (!string.IsNullOrWhiteSpace(guest.CardId))
                {
                    var trimmed = guest.CardId.Trim();

                    // optional: numeric validation
                    if (!long.TryParse(trimmed, out var num) || num <= 0)
                    {
                        return new InsertResponseModel
                        {
                            Id = 0,
                            Code = "",
                            Message = "Invalid CardId. It must be a positive number."
                        };
                    }

                    var exists = await _context.Set<GuestMaster>()
                         .AnyAsync(x => x.CardId == trimmed || x.QrId == trimmed);

                    if (exists)
                    {
                        return new InsertResponseModel
                        {
                            Id = 0,
                            Code = "",
                            Message = $"CardId '{trimmed}' already exists."
                        };
                    }

                    guest.CardId = trimmed; // normalize
                }

                var entity = _dataMapper.Map<GuestMasterAddEdit, GuestMaster>(guest);
                entity.CreatedBy = userId;
                entity.CreatedDate = DateTime.Now;
                entity.ModifiedBy = userId;
                entity.ModifiedDate = DateTime.Now;
                entity.IsActive = guest.IsActive;
                entity.Code = string.IsNullOrWhiteSpace(guest.Code) ? GenerateCode() : guest.Code.Trim();
                entity.CardId = string.IsNullOrWhiteSpace(guest.CardId)
    ? GenerateCardId()
    : NormalizeCardId(guest.CardId);

                entity.QrId = entity.CardId; // keep same

                // 1) Insert guest first (so Id is created)
                await _guestMasterRepository.AddAsync(entity, userId.ToString(), "Insert");
                await _context.SaveChangesAsync(); // ensure entity.Id exists

                // 3) Build QR -> Encrypt -> Generate image (based on QrId)
                var plainQr = BuildGuestQrValue(entity);      // now returns QrId
                var encryptedQr = EncryptQrValue(plainQr);

                entity.QrCodeValue = encryptedQr;
                entity.QrCodeImagePath = await GenerateGuestQrImageAsync(encryptedQr);

                await _context.SaveChangesAsync();
                await EnsureGuestUserAsync(entity, userId);

                var response = new InsertResponseModel
                {
                    Id = entity.Id,
                    Code = entity.Code,

                    // ✅ ADD THESE
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,

                    QrCodeValue = entity.QrCodeValue,
                    QrCodeImagePath = entity.QrCodeImagePath,
                    Message = "Insert successfully."
                };

                // 3) Hikvision (your existing logic) - even if it fails, guest + QR remain saved
                var hikvisionWarning = await TryAddGuestToHikvisionAsync(entity);
                if (!string.IsNullOrWhiteSpace(hikvisionWarning))
                {
                    response.Message = $"Data not inserted in Hikvision. {hikvisionWarning}";
                }

                return response;
            }
            catch (Exception ex)
            {
                return new InsertResponseModel
                {
                    Id = 0,
                    Code = ex.HResult.ToString(),
                    Message = ex.Message
                };
            }
        }
        public async Task<InsertResponseModel> UpdateGuestAsync(GuestMasterAddEdit guest)
        {
            try
            {
                var entity = await _guestMasterRepository
                    .Get(g => g.Id == guest.Id)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "404",
                        Message = "Guest not found."
                    };
                }

                if (!string.IsNullOrWhiteSpace(guest.Email))
                {
                    var trimmedEmail = guest.Email.Trim();
                    var emailValidator = new EmailAddressAttribute();

                    if (!emailValidator.IsValid(trimmedEmail))
                    {
                        return new InsertResponseModel
                        {
                            Id = 0,
                            Code = "",
                            Message = "Invalid email address format."
                        };
                    }

                    var normalizedEmail = trimmedEmail.ToLowerInvariant();
                    var emailExists = await _context.Set<GuestMaster>()
                        .AnyAsync(x => x.Id != entity.Id && x.Email != null && x.Email.ToLower() == normalizedEmail);

                    if (emailExists)
                    {
                        return new InsertResponseModel
                        {
                            Id = 0,
                            Code = "",
                            Message = $"Email '{trimmedEmail}' already exists."
                        };
                    }

                    guest.Email = trimmedEmail;
                }

                if (!string.IsNullOrWhiteSpace(guest.Mobile))
                {
                    var trimmedMobile = guest.Mobile.Trim();
                    var phoneRegex = new Regex(@"^\+?[0-9]{7,15}$");

                    if (!phoneRegex.IsMatch(trimmedMobile))
                    {
                        return new InsertResponseModel
                        {
                            Id = 0,
                            Code = "",
                            Message = "Invalid mobile number format. Use 7-15 digits, optional leading +."
                        };
                    }

                    var mobileExists = await _context.Set<GuestMaster>()
                        .AnyAsync(x => x.Id != entity.Id && x.Mobile == trimmedMobile);

                    if (mobileExists)
                    {
                        return new InsertResponseModel
                        {
                            Id = 0,
                            Code = "",
                            Message = $"Mobile '{trimmedMobile}' already exists."
                        };
                    }

                    guest.Mobile = trimmedMobile;
                }
                if (string.IsNullOrWhiteSpace(guest.CardId) && !string.IsNullOrWhiteSpace(guest.QrId))
                {
                    guest.CardId = guest.QrId;
                }

                if (!string.IsNullOrWhiteSpace(guest.CardId)
                    && !string.IsNullOrWhiteSpace(guest.QrId)
                    && !string.Equals(guest.CardId.Trim(), guest.QrId.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "",
                        Message = "QrId must match CardId."
                    };
                }
                // If CardId not sent but QrId sent, treat that as CardId
                if (string.IsNullOrWhiteSpace(guest.CardId) && !string.IsNullOrWhiteSpace(guest.QrId))
                {
                    guest.CardId = guest.QrId;
                }

                // Normalize + validate CardId (supports "3" OR "GCARD0000001")
                if (!TryNormalizeGuestCardId(guest.CardId, out var normalizedCardId, out var cardErr))
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "",
                        Message = cardErr
                    };
                }

                // If QrId is provided, it must match the normalized CardId
                if (!string.IsNullOrWhiteSpace(guest.QrId) && !string.IsNullOrWhiteSpace(normalizedCardId)
                    && !string.Equals(guest.QrId.Trim(), normalizedCardId, StringComparison.OrdinalIgnoreCase))
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "",
                        Message = "QrId must match CardId."
                    };
                }

                // Uniqueness check ONLY when CardId is actually provided
                if (!string.IsNullOrWhiteSpace(normalizedCardId))
                {
                    var exists = await _context.Set<GuestMaster>()
                        .AnyAsync(x => x.Id != entity.Id && (x.CardId == normalizedCardId || x.QrId == normalizedCardId));

                    if (exists)
                    {
                        return new InsertResponseModel
                        {
                            Id = 0,
                            Code = "",
                            Message = $"CardId '{normalizedCardId}' already exists."
                        };
                    }

                    guest.CardId = normalizedCardId;
                    guest.QrId = normalizedCardId;
                }

                //if (!string.IsNullOrWhiteSpace(guest.CardId))
                //{
                //    var trimmed = guest.CardId.Trim();

                //    if (!long.TryParse(trimmed, out var num) || num <= 0)
                //    {
                //        return new InsertResponseModel
                //        {
                //            Id = 0,
                //            Code = "",
                //            Message = "Invalid CardId. It must be a positive number."
                //        };
                //    }

                //    var exists = await _context.Set<GuestMaster>()
                //        .AnyAsync(x => x.Id != entity.Id
                //            && (x.CardId == trimmed || x.QrId == trimmed));

                //    if (exists)
                //    {
                //        return new InsertResponseModel
                //        {
                //            Id = 0,
                //            Code = "",
                //            Message = $"CardId '{trimmed}' already exists."
                //        };
                //    }

                //    guest.CardId = trimmed;
                //}

                var userId = _claimAccessorService.GetUserId();
                var oldQrId = entity.QrId;
                var oldUnitId = entity.UnitId;
                var oldFrom = entity.FromDateTime;
                var oldTo = entity.ToDateTime;

                entity.FirstName = guest.FirstName;
                entity.LastName = guest.LastName;
                entity.Email = string.IsNullOrWhiteSpace(guest.Email) ? null : guest.Email.Trim();
                entity.Mobile = string.IsNullOrWhiteSpace(guest.Mobile) ? null : guest.Mobile.Trim();
                entity.CountryCode = guest.CountryCode;
                entity.UnitId = guest.UnitId;
                entity.CardId = string.IsNullOrWhiteSpace(guest.CardId) ? entity.CardId : guest.CardId.Trim();
                entity.QrId = entity.CardId;
                entity.IsActive = guest.IsActive;
                entity.FromDateTime = guest.FromDateTime;
                entity.ToDateTime = guest.ToDateTime;
                entity.ModifiedBy = userId;
                entity.ModifiedDate = DateTime.Now;


                var qrChanged = oldQrId != entity.QrId
                                || oldUnitId != entity.UnitId
                                || oldFrom != entity.FromDateTime
                                || oldTo != entity.ToDateTime
                                || string.IsNullOrWhiteSpace(entity.QrCodeValue);

                if (qrChanged)
                {
                    var plainQr = BuildGuestQrValue(entity);
                    var encryptedQr = EncryptQrValue(plainQr);
                    entity.QrCodeValue = encryptedQr;
                    entity.QrCodeImagePath = await GenerateGuestQrImageAsync(encryptedQr);
                }

                await _guestMasterRepository.UpdateAsync(entity, userId.ToString(), "Update");

                return new InsertResponseModel
                {
                    Id = entity.Id,
                    Code = entity.Code,
                    Message = "Update successfully."
                };
            }
            catch (Exception ex)
            {
                return new InsertResponseModel
                {
                    Id = 0,
                    Code = ex.HResult.ToString(),
                    Message = ex.Message
                };
            }
        }
        private static bool TryNormalizeGuestCardId(string? cardId, out string? normalized, out string? errorMessage)
        {
            normalized = null;
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(cardId))
                return true;

            var trimmed = cardId.Trim();

            // Case 1: numeric only
            if (Regex.IsMatch(trimmed, @"^\d+$"))
            {
                if (!long.TryParse(trimmed, out var n) || n <= 0)
                {
                    errorMessage = "Invalid CardId. It must be a positive number.";
                    return false;
                }

                normalized = trimmed;
                return true;
            }

            // Case 2: GCARD + digits (GCARD0000001)
            var m = Regex.Match(trimmed, @"^(?i)GCARD(\d+)$");
            if (m.Success)
            {
                var digits = m.Groups[1].Value; // keeps leading zeros

                // validate digits part is > 0
                if (!long.TryParse(digits, out var n) || n <= 0)
                {
                    errorMessage = "Invalid CardId. 'GCARD' must be followed by a positive number.";
                    return false;
                }

                normalized = "GCARD" + digits; // normalize prefix casing
                return true;
            }

            errorMessage = "Invalid CardId. Allowed formats: numeric (e.g., 1) OR GCARD + digits (e.g., GCARD0000001).";
            return false;
        }

        public async Task DeleteGuestAsync(long id)
        {
            var entity = await _guestMasterRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return;
            }

            var userId = _claimAccessorService.GetUserId();
            entity.IsActive = false;
            entity.ModifiedBy = userId;
            entity.ModifiedDate = DateTime.Now;

            await _guestMasterRepository.UpdateAsync(entity, userId.ToString(), "Delete");
        }
        public async Task<QrCodeDecryptionResponse> DecryptGuestQrCodeValueAsync(
    long guestId,
    CancellationToken ct = default)
        {
            var guest = await _context.Set<GuestMaster>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == guestId && x.IsActive, ct);

            if (guest == null)
            {
                return new QrCodeDecryptionResponse
                {
                    IsSuccess = false,
                    Message = "Guest not found."
                };
            }

            if (string.IsNullOrWhiteSpace(guest.QrCodeValue))
            {
                return new QrCodeDecryptionResponse
                {
                    IsSuccess = false,
                    Message = "Guest does not have an encrypted QR code value."
                };
            }

            try
            {
                var plain = DecryptQrValue(guest.QrCodeValue); // your AES-GCM decrypt helper
                return new QrCodeDecryptionResponse
                {
                    IsSuccess = true,
                    QrCodeValue = plain,
                    Message = "QR code value decrypted successfully."
                };
            }
            catch
            {
                return new QrCodeDecryptionResponse
                {
                    IsSuccess = false,
                    Message = "Failed to decrypt QR code value."
                };
            }
        }

        //public async Task<InsertResponseModel> CreateGuestAsync(GuestMasterAddEdit guest)
        //{
        //    try
        //    {
        //        var userId = 1;

        //        var entity = _dataMapper.Map<GuestMasterAddEdit, GuestMaster>(guest);
        //        entity.CreatedBy = userId;
        //        entity.CreatedDate = DateTime.Now;
        //        entity.ModifiedBy = userId;
        //        entity.ModifiedDate = DateTime.Now;
        //        entity.IsActive = guest.IsActive;
        //        entity.Code = string.IsNullOrWhiteSpace(guest.Code) ? GenerateCode() : guest.Code.Trim();
        //        entity.CardId = string.IsNullOrWhiteSpace(guest.CardId) ? GenerateCardId() : guest.CardId.Trim();
        //        //entity.QrId = GenerateQrId(entity.Code, entity.FirstName, entity.LastName);
        //        //entity.QrCode = await SaveQrCodeAsync(entity.QrId, entity.Code);

        //        await _guestMasterRepository.AddAsync(entity, userId.ToString(), "Insert");

        //        var response = new InsertResponseModel
        //        {
        //            Id = entity.Id,
        //            Code = entity.Code,
        //            Message = "Insert successfully."
        //        };
        //        var hikvisionWarning = await TryAddGuestToHikvisionAsync(entity);
        //        if (!string.IsNullOrWhiteSpace(hikvisionWarning))
        //        {
        //            response.Message = string.IsNullOrWhiteSpace(hikvisionWarning)
        //                ? "Data not inserted in Hikvision"
        //                : $"Data not inserted in Hikvision. {hikvisionWarning}";
        //        }

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new InsertResponseModel
        //        {
        //            Id = 0,
        //            Code = ex.HResult.ToString(),
        //            Message = ex.Message
        //        };
        //    }
        //}
        public async Task<IEnumerable<GuestMasterAddEdit>> GetAllGuestsAsync()
        {
            var query = _guestMasterRepository
                .Get()
                .AsNoTracking();

            var mapped = _dataMapper.Project<GuestMaster, GuestMasterAddEdit>(query);
            return await mapped.ToListAsync();
        }

        public async Task<GuestMasterAddEdit> GetGuestByIdAsync(long id)
        {
            var entity = await _guestMasterRepository
                .Get(g => g.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return null;
            }

            return _dataMapper.Map<GuestMaster, GuestMasterAddEdit>(entity);
        }
        public async Task<PaginatedList<GuestMasterList>> GetGuestsAsync(int pageIndex, int pageSize)
        {
            var query = _guestMasterRepository
                .Get(r => r.IsActive);

            var totalCount = await query.CountAsync();
            var rows = await query
                .OrderBy(r => r.FirstName)
                .ThenBy(r => r.LastName)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = rows.Select(r => new GuestMasterList
            {
                Id = r.Id,
                Code = r.Code,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Email = r.Email,
                Mobile = r.Mobile,
                CountryCode = r.CountryCode,
                CardId = r.CardId,
                IsActive = r.IsActive,
            }).ToList();

            return new PaginatedList<GuestMasterList>(mapped, totalCount, pageIndex, pageSize);
        }
        public async Task<IReadOnlyList<GuestMasterList>> GetGuestsByResidentAsync(
            long? residentMasterId,
            long? residentFamilyMemberId)
        {
            if (residentMasterId == null && residentFamilyMemberId == null)
            {
                return Array.Empty<GuestMasterList>();
            }

            var unitIdsQuery = _context.Set<ResidentMasterUnit>()
                .AsNoTracking()
                .Where(unit => unit.IsActive);

            IQueryable<long> residentUnitIds = unitIdsQuery
                .Where(unit => residentMasterId != null && unit.ResidentMasterId == residentMasterId)
                .Select(unit => unit.UnitId);

            IQueryable<long> familyUnitIds = _context.Set<ResidentFamilyMemberUnit>()
                .AsNoTracking()
                .Where(unit => unit.IsActive)
                .Where(unit => residentFamilyMemberId != null && unit.ResidentFamilyMemberId == residentFamilyMemberId)
                .Select(unit => unit.UnitId);

            var unitIds = await residentUnitIds
                .Concat(familyUnitIds)
                .Distinct()
                .ToListAsync();

            if (unitIds.Count == 0)
            {
                return Array.Empty<GuestMasterList>();
            }

            var guests = await _guestMasterRepository
                .Get(guest => guest.IsActive && unitIds.Contains(guest.UnitId))
                .AsNoTracking()
                .OrderBy(guest => guest.FirstName)
                .ThenBy(guest => guest.LastName)
                .ToListAsync();

            return guests.Select(guest => new GuestMasterList
            {
                Id = guest.Id,
                Code = guest.Code,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                Email = guest.Email,
                Mobile = guest.Mobile,
                CountryCode = guest.CountryCode,
                UnitId = guest.UnitId,
                CardId = guest.CardId,
                IsActive = guest.IsActive,
                FromDateTime = guest.FromDateTime,
                ToDateTime = guest.ToDateTime,
                QrCodeValue = guest.QrCodeValue,
                QrCodeImagePath = guest.QrCodeImagePath
            }).ToList();
        }
        public async Task<GuestMasterAddEdit?> GetGuestByQrIdAsync(string qrId, CancellationToken ct = default)
        {
            qrId = qrId?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(qrId)) return null;

            var entity = await _context.Set<GuestMaster>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.QrId == qrId && x.IsActive, ct);

            return entity == null ? null : _dataMapper.Map<GuestMaster, GuestMasterAddEdit>(entity);
        }
    }
}