using Application.Helper;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Infrastructure.Context;
using Infrastructure.Integrations.Hikvision;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BuildingEntity = Domain.Entities.Building;

namespace Application.Services
{
    public class ResidentMasterService : IResidentMasterService
    {
        private readonly IResidentMasterRepository _residentMasterRepository;
        private readonly IAutoMapperGenericDataMapper _dataMapper;
        private readonly IClaimAccessorService _claimAccessorService;
        private readonly UserManager<User> _userManager;
        private const string DefaultResidentPassword = "User@123";
        private readonly AppDbContext _context;
        private readonly IResidentDocumentService _documentService;
        private readonly HikvisionClient _hikvisionClient;
        private readonly ISecretProtector _secretProtector;
        private readonly bool _qrEncryptionEnabled;
        private readonly byte[]? _qrEncryptionKey;
        private readonly string _webRootPath;

        public ResidentMasterService(
            IResidentMasterRepository residentMasterRepository,
            IAutoMapperGenericDataMapper dataMapper,
            IClaimAccessorService claimAccessorService,
            UserManager<User> userManager,
            AppDbContext context,
            IResidentDocumentService documentService,
            HikvisionClient hikvisionClient,
            ISecretProtector secretProtector,
            IConfiguration configuration)
        {
            _residentMasterRepository = residentMasterRepository;
            _dataMapper = dataMapper;
            _claimAccessorService = claimAccessorService;
            _userManager = userManager;
            _context = context;
            _documentService = documentService;
            _hikvisionClient = hikvisionClient;
            _secretProtector = secretProtector;
            _webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            _qrEncryptionEnabled = bool.TryParse(configuration["QrCodeEncryption:Enabled"], out var enabled) && enabled;
            _qrEncryptionKey = _qrEncryptionEnabled ? LoadQrEncryptionKey(configuration) : null;
        }

        private async Task<string> GenerateCodeAsync(CancellationToken ct = default)
        {
            // Must be called INSIDE an open transaction (we’ll do that below)
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();

            // ✅ UPDLOCK + HOLDLOCK prevents two requests generating same "next" code
            cmd.CommandText = @"
        SELECT ISNULL(MAX(TRY_CONVERT(INT, SUBSTRING([Code], 4, 20))), 0)
        FROM dbo.adm_ResidentMaster WITH (UPDLOCK, HOLDLOCK)
        WHERE [Code] LIKE 'RES%';";

            var currentTx = _context.Database.CurrentTransaction;
            if (currentTx != null)
                cmd.Transaction = currentTx.GetDbTransaction();

            var max = Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
            return $"RES{(max + 1):0000000}";
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
            if (string.IsNullOrWhiteSpace(qrValue)) return qrValue;

            // ✅ encryption disabled => store plain
            if (!_qrEncryptionEnabled) return qrValue;

            if (_qrEncryptionKey == null)
                throw new InvalidOperationException("QR encryption key is not initialized.");

            Span<byte> nonce = stackalloc byte[12];
            RandomNumberGenerator.Fill(nonce);

            var nonceBytes = nonce.ToArray();
            var plaintext = Encoding.UTF8.GetBytes(qrValue);
            var ciphertext = new byte[plaintext.Length];
            var tag = new byte[AesGcm.TagByteSizes.MaxSize];

            using (var aesGcm = new AesGcm(_qrEncryptionKey))
                aesGcm.Encrypt(nonceBytes, plaintext, ciphertext, tag);

            var encryptedPayload = new byte[nonceBytes.Length + tag.Length + ciphertext.Length];
            Buffer.BlockCopy(nonceBytes, 0, encryptedPayload, 0, nonceBytes.Length);
            Buffer.BlockCopy(tag, 0, encryptedPayload, nonceBytes.Length, tag.Length);
            Buffer.BlockCopy(ciphertext, 0, encryptedPayload, nonceBytes.Length + tag.Length, ciphertext.Length);

            return Convert.ToBase64String(encryptedPayload);
        }

        private string DecryptQrValue(string encryptedQrValue)
        {
            if (string.IsNullOrWhiteSpace(encryptedQrValue)) return encryptedQrValue;

            // ✅ encryption disabled => already plain
            if (!_qrEncryptionEnabled) return encryptedQrValue;

            if (_qrEncryptionKey == null)
                throw new InvalidOperationException("QR encryption key is not initialized.");

            var encryptedPayload = Convert.FromBase64String(encryptedQrValue);

            var minimumLength = 12 + AesGcm.TagByteSizes.MaxSize;
            if (encryptedPayload.Length < minimumLength)
                throw new InvalidOperationException("Encrypted QR value is too short.");

            var nonce = encryptedPayload.AsSpan(0, 12);
            var tag = encryptedPayload.AsSpan(12, AesGcm.TagByteSizes.MaxSize);
            var ciphertext = encryptedPayload.AsSpan(12 + AesGcm.TagByteSizes.MaxSize);

            var plaintext = new byte[ciphertext.Length];

            using var aesGcm = new AesGcm(_qrEncryptionKey);
            aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);

            return Encoding.UTF8.GetString(plaintext);
        }


        private string BuildResidentQrValue(ResidentMaster resident)
        {
            var unitIds = resident.ParentUnits?.Select(u => u.UnitId).Distinct().ToList() ?? new List<long>();
            var unitList = unitIds.Count == 0 ? "" : string.Join(",", unitIds);
            return $"{resident.QrId}";
        }

        private string BuildFamilyMemberQrValue(ResidentMaster resident, ResidentFamilyMember member)
        {
            var unitIds = member.MemberUnits?.Select(u => u.UnitId).Distinct().ToList() ?? new List<long>();
            var unitList = unitIds.Count == 0 ? "" : string.Join(",", unitIds);
            return $"{member.QrId}";
        }

        private async Task<string> GenerateResidentQrImageAsync(string encryptedQrValue, string folderName, CancellationToken ct = default)
        {
            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(encryptedQrValue, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(data);
            var qrBytes = qrCode.GetGraphic(20);

            var fileName = $"resident_qr_{Guid.NewGuid():N}.png";
            var folder = Path.Combine(_webRootPath, "uploads", folderName);
            Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(filePath, qrBytes, ct);

            return $"/uploads/{folderName}/{fileName}";
        }

        //private static string GenerateResidentQrId(ResidentMaster resident)
        //{
        //    var firstInitial = string.IsNullOrWhiteSpace(resident.ParentFirstName) ? "X" : resident.ParentFirstName.Trim()[0].ToString().ToUpper();
        //    var lastInitial = string.IsNullOrWhiteSpace(resident.ParentLastName) ? "X" : resident.ParentLastName.Trim()[0].ToString().ToUpper();
        //    return $"{firstInitial}{lastInitial}{resident.Code}{resident.Id:000000}";
        //}

        private static string GenerateFamilyMemberQrId(ResidentFamilyMember member, string residentCode)
        {
            var firstInitial = string.IsNullOrWhiteSpace(member.FirstName) ? "X" : member.FirstName.Trim()[0].ToString().ToUpper();
            var lastInitial = string.IsNullOrWhiteSpace(member.LastName) ? "X" : member.LastName.Trim()[0].ToString().ToUpper();
            var baseCode = string.IsNullOrWhiteSpace(member.Code) ? residentCode : member.Code;
            return $"{firstInitial}{lastInitial}{baseCode}{member.Id:000000}";
        }
        private static int GetNextFamilyMemberSequence(IEnumerable<ResidentFamilyMember> members, string residentCode)
        {
            var max = 0;
            var prefix = $"{residentCode}-FM-";

            foreach (var member in members)
            {
                if (string.IsNullOrWhiteSpace(member.Code))
                {
                    continue;
                }

                if (!member.Code.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var suffix = member.Code.Substring(prefix.Length);
                if (int.TryParse(suffix, out var parsed) && parsed > max)
                {
                    max = parsed;
                }
            }

            return max + 1;
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
        private string ResolvePlainPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            return _secretProtector.IsProtected(password)
                ? _secretProtector.Unprotect(password)
                : password;
        }
        private static bool TryNormalizeCardId(string cardId, out string normalized, out string errorMessage)
        {
            normalized = null;
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(cardId))
            {
                return true;
            }

            var trimmed = cardId.Trim();
            if (!long.TryParse(trimmed, out var parsed) || parsed <= 0)
            {
                errorMessage = "Invalid CardId. It must be a positive number.";
                return false;
            }

            normalized = trimmed;
            return true;
        }

        //private static string NormalizeQrId(string qrId)
        //{
        //    return string.IsNullOrWhiteSpace(qrId) ? null : qrId.Trim();
        //}

        private long GetMaxResidentCardId()
        {
            var residentMax = _context.Set<ResidentMaster>()
                .Select(r => r.CardId)
                .AsEnumerable()
                .Select(cardId => long.TryParse(cardId, out var value) ? value : 0)
                .DefaultIfEmpty(0)
                .Max();

            var familyMax = _context.Set<ResidentFamilyMember>()
                .Select(r => r.CardId)
                .AsEnumerable()
                .Select(cardId => long.TryParse(cardId, out var value) ? value : 0)
                .DefaultIfEmpty(0)
                .Max();

            return Math.Max(residentMax, familyMax);
        }

        private async Task<string?> ValidateResidentIdsAsync(
            ResidentMasterAddEdit resident,
            IReadOnlyList<ResidentFamilyMemberAddEdit> familyMembers,
            long? residentId = null)
        {
            if (string.IsNullOrWhiteSpace(resident.CardId) && !string.IsNullOrWhiteSpace(resident.QrId))
            {
                resident.CardId = resident.QrId;
            }

            if (!string.IsNullOrWhiteSpace(resident.CardId)
                && !string.IsNullOrWhiteSpace(resident.QrId)
                && !string.Equals(resident.CardId.Trim(), resident.QrId.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return "QrId must match CardId.";
            }
            if (!TryNormalizeCardId(resident.CardId, out var parentCardId, out var cardError))
            {
                return cardError;
            }

            resident.CardId = parentCardId;
            resident.QrId = parentCardId;


            var requestedCardIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            //var requestedQrIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(parentCardId))
            {
                requestedCardIds.Add(parentCardId);

                var cardExists = await _context.Set<ResidentMaster>()
                    .AnyAsync(x => x.Id != residentId
                        && (x.CardId == parentCardId || x.QrId == parentCardId));

                if (!cardExists)
                {
                    cardExists = await _context.Set<ResidentFamilyMember>()
                        .AnyAsync(x => x.CardId == parentCardId || x.QrId == parentCardId);
                }

                if (cardExists)
                {
                    return $"CardId '{parentCardId}' already exists.";
                }
            }

            foreach (var member in familyMembers)
            {
                if (string.IsNullOrWhiteSpace(member.CardId) && !string.IsNullOrWhiteSpace(member.QrId))
                {
                    member.CardId = member.QrId;
                }

                if (!string.IsNullOrWhiteSpace(member.CardId)
                    && !string.IsNullOrWhiteSpace(member.QrId)
                    && !string.Equals(member.CardId.Trim(), member.QrId.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return "QrId must match CardId.";
                }
            
                if (!TryNormalizeCardId(member.CardId, out var memberCardId, out var memberCardError))
                {
                    return memberCardError;
                }

                member.CardId = memberCardId;
                member.QrId = memberCardId;

                if (!string.IsNullOrWhiteSpace(memberCardId))
                {
                    if (!requestedCardIds.Add(memberCardId))
                    {
                        return $"CardId '{memberCardId}' already exists in the request.";
                    }

                    var cardExists = await _context.Set<ResidentMaster>()
                        .AnyAsync(x => x.CardId == memberCardId || x.QrId == memberCardId);

                    if (!cardExists)
                    {
                        cardExists = await _context.Set<ResidentFamilyMember>()
                            .AnyAsync(x => x.Id != member.Id
                                && (x.CardId == memberCardId || x.QrId == memberCardId));
                    }

                    if (cardExists)
                    {
                        return $"CardId '{memberCardId}' already exists.";
                    }
                }

            }

            return null;
        }
        private async Task<string?> ValidateFamilyMemberIdsAsync(ResidentFamilyMemberAddEdit member)
        {
            if (string.IsNullOrWhiteSpace(member.CardId) && !string.IsNullOrWhiteSpace(member.QrId))
            {
                member.CardId = member.QrId;
            }

            if (!string.IsNullOrWhiteSpace(member.CardId)
                && !string.IsNullOrWhiteSpace(member.QrId)
                && !string.Equals(member.CardId.Trim(), member.QrId.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return "QrId must match CardId.";
            }

            if (!TryNormalizeCardId(member.CardId, out var memberCardId, out var memberCardError))
            {
                return memberCardError;
            }

            member.CardId = memberCardId;
            member.QrId = memberCardId;

            if (!string.IsNullOrWhiteSpace(memberCardId))
            {
                var cardExists = await _context.Set<ResidentMaster>()
                    .AnyAsync(x => x.CardId == memberCardId || x.QrId == memberCardId);

                if (!cardExists)
                {
                    cardExists = await _context.Set<ResidentFamilyMember>()
                        .AnyAsync(x => x.Id != member.Id
                            && (x.CardId == memberCardId || x.QrId == memberCardId));
                }

                if (cardExists)
                {
                    return $"CardId '{memberCardId}' already exists.";
                }
            }

            return null;
        }

        private sealed class FamilyQrSnapshot
        {
            public string QrId { get; init; }
            public HashSet<long> UnitIds { get; init; }
            public string QrCodeValue { get; init; }
        }

        private static HashSet<long> GetUnitIds(IEnumerable<long> unitIds)
        {
            return new HashSet<long>(unitIds ?? Enumerable.Empty<long>());
        }

        private async Task EnsureResidentQrDataAsync(
            ResidentMaster entity,
            string oldParentQrId,
            HashSet<long> oldParentUnitIds,
            Dictionary<long, FamilyQrSnapshot> oldFamilySnapshots)
        {
            var nextCardId = GetMaxResidentCardId() + 1;

            if (string.IsNullOrWhiteSpace(entity.CardId))
            {
                entity.CardId = nextCardId.ToString();
                nextCardId++;
            }

            entity.QrId = entity.CardId;

            var currentParentUnitIds = GetUnitIds(entity.ParentUnits?.Select(u => u.UnitId));
            var parentChanged = !string.Equals(oldParentQrId, entity.QrId, StringComparison.OrdinalIgnoreCase)
                                || !oldParentUnitIds.SetEquals(currentParentUnitIds)
                                || string.IsNullOrWhiteSpace(entity.QrCodeValue);

            if (parentChanged)
            {
                var plain = BuildResidentQrValue(entity);
                var encrypted = EncryptQrValue(plain);
                entity.QrCodeValue = encrypted;
                entity.QrCodeImagePath = await GenerateResidentQrImageAsync(encrypted, "Residents-QRCode");
            }

            if (entity.FamilyMembers == null || entity.FamilyMembers.Count == 0)
            {
                return;
            }

            foreach (var member in entity.FamilyMembers)
            {
                if (string.IsNullOrWhiteSpace(member.CardId))
                {
                    member.CardId = nextCardId.ToString();
                    nextCardId++;
                }

                member.QrId = member.CardId;

                var currentMemberUnitIds = GetUnitIds(member.MemberUnits?.Select(u => u.UnitId));
                var hasSnapshot = oldFamilySnapshots.TryGetValue(member.Id, out var snapshot);
                var memberChanged = !hasSnapshot
                                    || !string.Equals(snapshot.QrId, member.QrId, StringComparison.OrdinalIgnoreCase)
                                    || !snapshot.UnitIds.SetEquals(currentMemberUnitIds)
                                    || string.IsNullOrWhiteSpace(member.QrCodeValue)
                                    || string.IsNullOrWhiteSpace(snapshot.QrCodeValue);

                if (memberChanged)
                {
                    var plain = BuildFamilyMemberQrValue(entity, member);
                    var encrypted = EncryptQrValue(plain);
                    member.QrCodeValue = encrypted;
                    member.QrCodeImagePath = await GenerateResidentQrImageAsync(encrypted, "ResidentFamily-QRCode");
                }
            }
        }
        private async Task<(User? User, string? Error)> EnsureOrUpdateResidentUserAsync(
    long? preferredUserId,   // pass mapped user id if available (best)
    string name,
    string? email,
    string? mobile,
    string fallbackUserName,
    long actorUserId,
    string? plainPassword,
    bool isResident,
    bool isActive)
        {
            // if not allowed to have user => just return null and let map deactivate
            if (!isResident || !isActive)
                return (null, null);

            var cleanEmail = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
            var cleanMobile = string.IsNullOrWhiteSpace(mobile) ? null : mobile.Trim();

            var desiredUserName = GetUserName(cleanEmail, cleanMobile, fallbackUserName);

            User? user = null;

            // 1) Prefer updating the already-mapped user (prevents new user creation on email change)
            if (preferredUserId.HasValue && preferredUserId.Value > 0)
                user = await _userManager.FindByIdAsync(preferredUserId.Value.ToString());

            // 2) fallback lookups
            if (user == null && cleanEmail != null)
                user = await _userManager.FindByEmailAsync(cleanEmail);

            if (user == null)
                user = await _userManager.FindByNameAsync(desiredUserName);

            // ✅ If user exists => UPDATE it
            if (user != null)
            {
                // email uniqueness inside Identity
                if (cleanEmail != null)
                {
                    var normalizedEmail = _userManager.NormalizeEmail(cleanEmail);
                    var emailTaken = await _userManager.Users
                        .AnyAsync(u => u.Id != user.Id && u.NormalizedEmail == normalizedEmail);

                    if (emailTaken) return (null, "Email is already taken in users.");
                }

                // phone uniqueness (you already validate, but keeping extra safety is ok)
                if (cleanMobile != null)
                {
                    var phoneTaken = await _userManager.Users
                        .AnyAsync(u => u.Id != user.Id && u.PhoneNumber == cleanMobile);

                    if (phoneTaken) return (null, "Phone number is already taken in users.");
                }

                user.Name = name;
                user.Email = cleanEmail;
                user.PhoneNumber = cleanMobile;
                user.UserName = desiredUserName;
                user.IsActive = true;

                // if your User entity has these fields, keep them; otherwise remove
                user.ModifiedBy = actorUserId;
                user.ModifiedDate = DateTime.Now;

                var upd = await _userManager.UpdateAsync(user);
                if (!upd.Succeeded)
                    return (null, string.Join(" | ", upd.Errors.Select(e => e.Description)));

                return (user, null);
            }

            // ✅ If not exists => CREATE it
            var newUser = new User
            {
                UserName = desiredUserName,
                Email = cleanEmail,
                Name = name,
                PhoneNumber = cleanMobile,
                IsActive = true,
                CreatedBy = actorUserId,
                CreatedDate = DateTime.Now,
                ModifiedBy = actorUserId,
                ModifiedDate = DateTime.Now
            };

            var pwd = string.IsNullOrWhiteSpace(plainPassword) ? DefaultResidentPassword : plainPassword;
            var create = await _userManager.CreateAsync(newUser, pwd);

            if (!create.Succeeded)
                return (null, string.Join(" | ", create.Errors.Select(e => e.Description)));

            return (newUser, null);
        }
        private async Task<long?> GetMappedUserIdAsync(long? residentMasterId, long? familyMemberId)
        {
            return await _context.ResidentUserMaps
                .AsNoTracking()
                .Where(m => m.IsActive
                    && m.ResidentMasterId == residentMasterId
                    && m.ResidentFamilyMemberId == familyMemberId)
                .Select(m => (long?)m.UserId)
                .FirstOrDefaultAsync();
        }

        private async Task<User> EnsureResidentUserAsync(
            string name,
            string email,
            string mobile,
            string fallbackUserName,
            long createdBy,
            string plainPassword,
            bool isResident,
            bool isActive)
        {
            if (!isResident || !isActive)
            {
                return null;
            }

            var userName = GetUserName(email, mobile, fallbackUserName);

            User existingUser = null;
            if (!string.IsNullOrWhiteSpace(email))
            {
                existingUser = await _userManager.FindByEmailAsync(email.Trim());
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
                Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                Name = name,
                PhoneNumber = string.IsNullOrWhiteSpace(mobile) ? null : mobile.Trim(),
                IsActive = true,
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now
            };

            var resolvedPassword = string.IsNullOrWhiteSpace(plainPassword)
                ? DefaultResidentPassword
                : plainPassword;
            var result = await _userManager.CreateAsync(user, resolvedPassword);
            return result.Succeeded ? user : null;
        }

        private static string BuildFamilyMemberFallback(ResidentMaster residentMaster, ResidentFamilyMember member)
        {
            if (!string.IsNullOrWhiteSpace(member.Code))
            {
                return member.Code;
            }

            var suffix = member.FaceId ?? member.FingerId ?? member.CardId ?? member.QrId ?? member.Id.ToString();
            return $"{residentMaster.Code}-FM-{suffix}";
        }

        private async Task SyncResidentUserMapAsync(
            User user,
            long? residentMasterId,
            long? residentFamilyMemberId,
            bool isResident,
            bool isActive,
            long actorUserId)
        {
            if (residentMasterId == null && residentFamilyMemberId == null)
            {
                return;
            }

            var existingMaps = await _context.ResidentUserMaps
                .Where(map => map.ResidentMasterId == residentMasterId
                    && map.ResidentFamilyMemberId == residentFamilyMemberId)
                .ToListAsync();

            if (!isResident || !isActive)
            {
                foreach (var map in existingMaps.Where(map => map.IsActive))
                {
                    map.IsActive = false;
                    map.ModifiedBy = actorUserId;
                    map.ModifiedDate = DateTime.Now;
                }

                return;
            }

            if (user == null)
            {
                return;
            }

            foreach (var map in existingMaps.Where(map => map.UserId != user.Id && map.IsActive))
            {
                map.IsActive = false;
                map.ModifiedBy = actorUserId;
                map.ModifiedDate = DateTime.Now;
            }

            var existing = existingMaps.FirstOrDefault(map => map.UserId == user.Id);
            if (existing == null)
            {
                _context.ResidentUserMaps.Add(new ResidentUserMap
                {
                    ResidentMasterId = residentMasterId,
                    ResidentFamilyMemberId = residentFamilyMemberId,
                    UserId = user.Id,
                    IsActive = true,
                    CreatedBy = actorUserId,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = actorUserId,
                    ModifiedDate = DateTime.Now
                });
            }
            else if (!existing.IsActive)
            {
                existing.IsActive = true;
                existing.ModifiedBy = actorUserId;
                existing.ModifiedDate = DateTime.Now;
            }
        }
        private sealed class ResidentUserCandidate
        {
            public string Email { get; init; }
            public string Mobile { get; init; }
            public string FallbackUserName { get; init; }
            public bool IsResident { get; init; }
        }

        private static bool IsSameUser(User existingUser, string email, string userName)
        {
            if (!string.IsNullOrWhiteSpace(existingUser.UserName)
                && string.Equals(existingUser.UserName, userName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(email)
                && !string.IsNullOrWhiteSpace(existingUser.Email)
                && string.Equals(existingUser.Email, email.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private async Task<string> ValidateResidentPhoneNumbersAsync(IEnumerable<ResidentUserCandidate> candidates)
        {
            var seenPhones = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var candidate in candidates)
            {
                if (!candidate.IsResident)
                {
                    continue;
                }

                var mobile = string.IsNullOrWhiteSpace(candidate.Mobile) ? null : candidate.Mobile.Trim();
                if (string.IsNullOrWhiteSpace(mobile))
                {
                    continue;
                }

                var userName = GetUserName(candidate.Email, mobile, candidate.FallbackUserName);

                if (seenPhones.TryGetValue(mobile, out var existingUserName))
                {
                    if (!string.Equals(existingUserName, userName, StringComparison.OrdinalIgnoreCase))
                    {
                        return "Phone number is already taken.";
                    }
                }
                else
                {
                    seenPhones[mobile] = userName;
                }

                var existingUser = await _userManager.Users.FirstOrDefaultAsync(user => user.PhoneNumber == mobile);
                if (existingUser != null && !IsSameUser(existingUser, candidate.Email, userName))
                {
                    return "Phone number is already taken.";
                }
            }

            return null;
        }
        private async Task<string> ValidateParentEmailAsync(string email, long? residentId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var normalized = email.Trim().ToLower();
            var existing = await _residentMasterRepository.Get(r => r.IsActive
                    && r.Email != null
                    && r.Id != residentId
                    && r.Email.ToLower() == normalized)
                .AnyAsync();

            return existing ? "Email is already taken." : null;
        }
        private static string ToEmployeeNo(string raw)
        {
            // safest: keep only letters+digits, and limit length
            if (string.IsNullOrWhiteSpace(raw)) return Guid.NewGuid().ToString("N");

            var cleaned = new string(raw.Where(char.IsLetterOrDigit).ToArray());

            // Hikvision commonly limits employeeNo length (often 32)
            return cleaned.Length <= 32 ? cleaned : cleaned.Substring(0, 32);
        }
        private async Task<(string? Error, string? IpAddress, int Port, string? UserName, string? Password, string? devIndex)> ResolveHikvisionCredentialsAsync(long unitId)
        {
            if (unitId <= 0)
            {
                return ("Unit not found for resident.", null, 0, null, null, null);
            }

            var unit = await _context.Set<Unit>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == unitId);

            if (unit == null)
            {
                return ("Unit not found for resident.", null, 0, null, null, null);
            }

            var building = await _context.Set<BuildingEntity>()
               .AsNoTracking()
               .Include(b => b.Device)
               .FirstOrDefaultAsync(b => b.Id == unit.BuildingId);

            if (building?.Device == null || string.IsNullOrWhiteSpace(building.Device.IpAddress))
            {
                return ("Hikvision device not found for building.", null, 0, null, null, null);
            }

            var userName = building.DeviceUserName?.Trim();
            var devicePassword = building.DevicePassword?.Trim();

            try
            {
                if (!string.IsNullOrWhiteSpace(devicePassword) &&
                    (devicePassword.StartsWith("CfDJ8", StringComparison.OrdinalIgnoreCase)
                     || _secretProtector.IsProtected(devicePassword)))
                {
                    devicePassword = _secretProtector.Unprotect(devicePassword)?.Trim();
                }
            }
            catch (Exception ex)
            {
                return ($"Device password cannot be decrypted (DataProtection key mismatch on IIS). Error: {ex.Message}", null, 0, null, null,null);
            }

            if (string.IsNullOrWhiteSpace(devicePassword))
            {
                return ("Device password resolved to empty after decrypt.", null, 0, null, null, null);
            }

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(devicePassword))
            {
                return ("Hikvision device credentials not found for building.", null, 0, null, null, null);
            }

            try
            {
                if (_secretProtector.IsProtected(devicePassword))
                {
                    devicePassword = _secretProtector.Unprotect(devicePassword);
                }

                devicePassword = devicePassword?.Trim();
            }
            catch (Exception ex)
            {
                return ($"Device password cannot be decrypted on IIS. Fix DataProtection keys/permissions. Error: {ex.Message}", null, 0, null, null, null);
            }
            var devIndex = building.Device?.DevIndex; // ✅ add
            var port = building.Device.PortNo ?? 80;
            return (null, building.Device.IpAddress, port, userName, devicePassword, devIndex);
        }
        private async Task<string?> TryUpdatePersonInHikvisionAsync(long unitId, string employeeNo, string name)
        {
            try
            {
                var (error, ip, port, user, pass, devIndex) = await ResolveHikvisionCredentialsAsync(unitId);
                if (!string.IsNullOrWhiteSpace(error)) return error;

                var check = await _hikvisionClient.CheckDeviceCredentialsAsync(ip!, port, user!, pass!, devIndex, ct: default);
                if (!check.IsAuthorized) return $"Hikvision credentials invalid... {check.ResponseBody}";

                await _hikvisionClient.UpdatePersonInDeviceAsync(
                    ip!, port, user!, pass!,
                    new HikvisionPersonInfo { EmployeeNo = employeeNo, Name = name },
                    devIndex: devIndex,
                    userType: "normal");

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private async Task<string?> TryAddResidentToHikvisionAsync(ResidentMasterAddEdit resident, ResidentMaster entity)
        {
            if (!resident.IsResident) return null;

            try
            {
                var unitId = resident.UnitIds?.FirstOrDefault() ?? 0;
                var (error, ipAddress, port, userName, devicePassword, devIndex) = await ResolveHikvisionCredentialsAsync(unitId);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return error;
                }

                var people = new List<(HikvisionPersonInfo Person, string CardId)>();

                // ✅ Parent
                people.Add((new HikvisionPersonInfo
                {
                    EmployeeNo = ToEmployeeNo(entity.Code),
                    Name = $"{entity.ParentFirstName} {entity.ParentLastName}".Trim()
                }, entity.CardId));

                // ✅ Family members
                if (entity.FamilyMembers != null && entity.FamilyMembers.Any())
                {
                    people.AddRange(entity.FamilyMembers
                        .Where(m => m.IsResident)
                        .Select(m =>
                        {
                            var employeeNo = ToEmployeeNo(!string.IsNullOrWhiteSpace(m.Code)
                                ? m.Code
                                : $"{entity.Code}FM{m.Id}");

                            return (new HikvisionPersonInfo
                            {
                                EmployeeNo = employeeNo,
                                Name = $"{m.FirstName} {m.LastName}".Trim()
                            }, m.CardId);
                        }));
                }

                // ✅ extra safety: duplicates
                var dup = people.GroupBy(p => p.Person.EmployeeNo).FirstOrDefault(g => g.Count() > 1);
                if (dup != null)
                    return $"Duplicate employeeNo generated in code: {dup.Key}";

                var check = await _hikvisionClient.CheckDeviceCredentialsAsync(
    ipAddress,
                    port,
                    userName,
                    devicePassword,
                    ct: default);

                if (!check.IsAuthorized)
                {
                    return
                        $"Hikvision credentials invalid for device {ipAddress}:{port}. " +
                        $"Status={check.StatusCode}. URL={check.RequestUrl}. WWW-Authenticate={check.WwwAuthenticate}. " +
                        $"Body={check.ResponseBody}";
                }

                foreach (var person in people)
                {
                    await _hikvisionClient.AddPersonToDeviceAsync(
                        ipAddress,
                        port,
                        userName,
                        devicePassword,
                        person.Person,
                        devIndex: devIndex,   // ✅ pass devIndex you already resolved
                        userType: "normal",
                        ct: default);

                    if (!string.IsNullOrWhiteSpace(person.CardId))
                    {
                        await _hikvisionClient.AddCardToDeviceAsync(
                            ipAddress,
                            port,
                            userName,
                            devicePassword,
                            new HikvisionCardInfo
                            {
                                EmployeeNo = person.Person.EmployeeNo,
                                CardNo = person.CardId.Trim(),
                                CardType = "normalCard"
                            },
                            devIndex: devIndex, // ✅ pass devIndex
                            ct: default);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        //private static string BuildEmployeeNo(long residentId, long? familyMemberId = null)

        //{
        //    if (familyMemberId == null) return residentId.ToString();
        //    return $"{residentId}{familyMemberId.Value:D6}"; // example: 12 + 000045 => "12000045"
        //}
        public async Task<InsertResponseModel> CreateResidentAsync(ResidentMasterAddEdit resident)
        {
            try
            {
                var userId = 1;

                var plainPassword = ResolvePlainPassword(resident.Password);

                if (!string.IsNullOrWhiteSpace(resident.Password)
                    && !_secretProtector.IsProtected(resident.Password))
                {
                    resident.Password = _secretProtector.Protect(resident.Password);
                }

                using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);


                var entity = _dataMapper.Map<ResidentMasterAddEdit, ResidentMaster>(resident);

                entity.Code = await GenerateCodeAsync();
                entity.CreatedBy = userId;
                entity.CreatedDate = DateTime.Now;
                entity.ModifiedBy = userId;
                entity.ModifiedDate = DateTime.Now;
                entity.IsActive = true;
                entity.IsResident = resident.IsResident;
                entity.ProfilePhoto = resident.ProfilePhoto;

                entity.ParentUnits = resident.UnitIds?.Distinct().Select(unitId => new ResidentMasterUnit
                {
                    UnitId = unitId,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = userId,
                    ModifiedDate = DateTime.Now
                }).ToList();

                var familyMembers = resident.FamilyMembers?
                    .Where(m => !m.IsEmpty())
                    .ToList() ?? new List<ResidentFamilyMemberAddEdit>();

                // generate member codes if needed
                var familySequence = 1;
                foreach (var m in familyMembers)
                {
                    if (string.IsNullOrWhiteSpace(m.Code))
                        m.Code = $"{entity.Code}FM{familySequence:0000}";
                    familySequence++;
                }

                // validations (your existing ones)
                var idConflict = await ValidateResidentIdsAsync(resident, familyMembers);
                if (!string.IsNullOrWhiteSpace(idConflict))
                    return new InsertResponseModel { Id = 0, Code = "409", Message = idConflict };

                var phoneConflict = await ValidateResidentPhoneNumbersAsync(
                    new[] {
                new ResidentUserCandidate {
                    Email = resident.Email,
                    Mobile = resident.Mobile,
                    FallbackUserName = entity.Code,
                    IsResident = resident.IsResident
                }
                    }
                    .Concat(familyMembers.Select(m => new ResidentUserCandidate
                    {
                        Email = m.Email,
                        Mobile = m.Mobile,
                        FallbackUserName = string.IsNullOrWhiteSpace(m.Code) ? $"{entity.Code}-FM-{Guid.NewGuid():N}" : m.Code,
                        IsResident = m.IsResident
                    }))
                );

                if (!string.IsNullOrWhiteSpace(phoneConflict))
                    return new InsertResponseModel { Id = 0, Code = "409", Message = phoneConflict };

                var emailConflict = await ValidateParentEmailAsync(resident.Email);
                if (!string.IsNullOrWhiteSpace(emailConflict))
                    return new InsertResponseModel { Id = 0, Code = "409", Message = emailConflict };

                // build entity family members
                if (familyMembers.Any())
                {
                    entity.FamilyMembers = familyMembers.Select(m => new ResidentFamilyMember
                    {
                        Code = m.Code,
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        Email = m.Email,
                        Mobile = m.Mobile,
                        FaceId = m.FaceId,
                        FaceUrl = m.FaceUrl,
                        FingerId = m.FingerId,
                        CardId = m.CardId,
                        QrId = m.QrId,
                        IsActive = true,
                        IsResident = m.IsResident,
                        ProfilePhoto = m.ProfilePhoto,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = userId,
                        ModifiedDate = DateTime.Now,
                        MemberUnits = m.UnitIds?.Distinct().Select(uid => new ResidentFamilyMemberUnit
                        {
                            UnitId = uid,
                            IsActive = true,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = userId,
                            ModifiedDate = DateTime.Now
                        }).ToList()
                    }).ToList();
                }

                await _residentMasterRepository.AddAsync(entity, userId.ToString(), "Insert");
                await _context.SaveChangesAsync(); // ✅ IDs generated here

                // ✅ QR + card auto-gen (your existing logic)
                var nextCardId = GetMaxResidentCardId() + 1;

                if (string.IsNullOrWhiteSpace(entity.CardId))
                    entity.CardId = (nextCardId++).ToString();

                entity.QrId = entity.CardId;

                var parentQrPlain = BuildResidentQrValue(entity);
                var parentQrEncrypted = EncryptQrValue(parentQrPlain);
                entity.QrCodeValue = parentQrEncrypted;
                entity.QrCodeImagePath = await GenerateResidentQrImageAsync(parentQrEncrypted, "Residents-QRCode");

                if (entity.FamilyMembers != null)
                {
                    foreach (var m in entity.FamilyMembers)
                    {
                        if (string.IsNullOrWhiteSpace(m.CardId))
                            m.CardId = (nextCardId++).ToString();

                        m.QrId = m.CardId;

                        var plain = BuildFamilyMemberQrValue(entity, m);
                        var enc = EncryptQrValue(plain);
                        m.QrCodeValue = enc;
                        m.QrCodeImagePath = await GenerateResidentQrImageAsync(enc, "ResidentFamily-QRCode");
                    }
                }

                await _context.SaveChangesAsync();

                // ✅ CREATE USERS (Resident + Family) WITH ERROR HANDLING
                var (parentUser, parentErr) = await EnsureOrUpdateResidentUserAsync(
                    preferredUserId: null,
                    name: $"{entity.ParentFirstName} {entity.ParentLastName}".Trim(),
                    email: entity.Email,
                    mobile: entity.Mobile,
                    fallbackUserName: entity.Code,
                    actorUserId: userId,
                    plainPassword: plainPassword,
                    isResident: entity.IsResident,
                    isActive: entity.IsActive
                );

                if (!string.IsNullOrWhiteSpace(parentErr))
                {
                    await tx.RollbackAsync();
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "400",
                        Message = $"Resident user create failed: {parentErr}"
                    };
                }

                await SyncResidentUserMapAsync(parentUser, entity.Id, null, entity.IsResident, entity.IsActive, userId);

                if (entity.FamilyMembers != null)
                {
                    foreach (var m in entity.FamilyMembers)
                    {
                        var (memberUser, memberErr) = await EnsureOrUpdateResidentUserAsync(
                            preferredUserId: null,
                            name: $"{m.FirstName} {m.LastName}".Trim(),
                            email: m.Email,
                            mobile: m.Mobile,
                            fallbackUserName: BuildFamilyMemberFallback(entity, m),
                            actorUserId: userId,
                            plainPassword: null, // keep default for family (or add password support if you want)
                            isResident: m.IsResident,
                            isActive: m.IsActive
                        );

                        if (!string.IsNullOrWhiteSpace(memberErr))
                        {
                            await tx.RollbackAsync();
                            return new InsertResponseModel
                            {
                                Id = 0,
                                Code = "400",
                                Message = $"Family member user create failed ({m.FirstName}): {memberErr}"
                            };
                        }

                        await SyncResidentUserMapAsync(memberUser, null, m.Id, m.IsResident, m.IsActive, userId);
                    }
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                // ✅ Hikvision after commit (so DB insert never depends on device)
                var response = new InsertResponseModel
                {
                    Id = entity.Id,
                    Code = entity.Code,
                    FirstName = entity.ParentFirstName,
                    LastName = entity.ParentLastName,
                    QrCodeValue = entity.QrCodeValue,
                    QrCodeImagePath = entity.QrCodeImagePath,
                    Message = "Insert successfully."
                };

                var hikvisionWarning = await TryAddResidentToHikvisionAsync(resident, entity);
                if (!string.IsNullOrWhiteSpace(hikvisionWarning))
                    response.Message = $"Insert successfully. Data not inserted in Hikvision. {hikvisionWarning}";

                return response;
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                return new InsertResponseModel
                {
                    Id = 0,
                    Code = "500",
                    Message = $"DB error: {inner}"
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

        public async Task DeleteResidentAsync(long id)
        {
            var entity = await _residentMasterRepository.Get(r => r.Id == id)
                .Include(r => r.ParentUnits)
                .Include(r => r.FamilyMembers)
                    .ThenInclude(f => f.MemberUnits)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return;
            }

            var userId = 1;
            entity.IsActive = false;
            entity.ModifiedBy = userId;
            entity.ModifiedDate = DateTime.Now;
            if (entity.ParentUnits != null)
            {
                foreach (var unit in entity.ParentUnits)
                {
                    unit.IsActive = false;
                    unit.ModifiedBy = userId;
                    unit.ModifiedDate = DateTime.Now;
                }
            }
            if (entity.FamilyMembers != null)
            {
                foreach (var member in entity.FamilyMembers)
                {
                    member.IsActive = false;
                    member.ModifiedBy = userId;
                    member.ModifiedDate = DateTime.Now;

                    if (member.MemberUnits != null)
                    {
                        foreach (var unit in member.MemberUnits)
                        {
                            unit.IsActive = false;
                            unit.ModifiedBy = userId;
                            unit.ModifiedDate = DateTime.Now;
                        }
                    }
                }
            }
            var familyMemberIds = entity.FamilyMembers?.Select(member => member.Id).ToList() ?? new List<long>();
            var residentUserMaps = await _context.ResidentUserMaps
                .Where(map => map.ResidentMasterId == entity.Id
                    || (map.ResidentFamilyMemberId != null && familyMemberIds.Contains(map.ResidentFamilyMemberId.Value)))
                .ToListAsync();

            foreach (var map in residentUserMaps)
            {
                map.IsActive = false;
                map.ModifiedBy = userId;
                map.ModifiedDate = DateTime.Now;
            }

            await _residentMasterRepository.UpdateAsync(entity, userId.ToString(), "Delete");
        }

        public async Task<ResidentMasterAddEdit> GetResidentByIdAsync(long id, bool includeFamilyMembers = false)
        {
            IQueryable<ResidentMaster> query = _residentMasterRepository.Get(r => r.Id == id)
                .Include(r => r.ParentUnits)
                    .ThenInclude(mu => mu.Unit);

            if (includeFamilyMembers)
            {
                query = query
                    .Include(r => r.FamilyMembers)
                        .ThenInclude(f => f.MemberUnits)
                            .ThenInclude(mu => mu.Unit);
            }

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null) return null;

            var model = _dataMapper.Map<ResidentMaster, ResidentMasterAddEdit>(entity);

            // ✅ unprotect password if you really want to return it
            if (!string.IsNullOrWhiteSpace(model.Password) && _secretProtector.IsProtected(model.Password))
                model.Password = _secretProtector.Unprotect(model.Password);

            // ✅ you said you don't need member details in Details API
            if (!includeFamilyMembers)
                model.FamilyMembers = new List<ResidentFamilyMemberAddEdit>(); // or null

            // ✅ MAIN FIX: attach documents
            var docs = await _documentService.GetDocumentsByResidentAsync(id);

            model.DocumentDetails = docs.Select(d => new ResidentDocumentDto
            {
                Id = d.Id,
                FileName = d.FileName,       // adjust if your column name differs
                FilePath = d.FilePath,
                ContentType = d.ContentType, // adjust if needed
                CreatedDate = d.CreatedDate
            }).ToList();

            return model;
        }

        public async Task<ResidentDetailResponse> GetResidentDetailsAsync(long? residentMasterId, long? residentFamilyMemberId)
        {
            if (residentMasterId.HasValue)
            {
                // IMPORTANT: includeFamilyMembers = false
                var resident = await GetResidentByIdAsync(residentMasterId.Value, includeFamilyMembers: false);
                if (resident == null) return null;

                await TryAttachFaceImageAsync(resident, CancellationToken.None);

                return new ResidentDetailResponse
                {
                    Type = "Resident",
                    Resident = resident,
                    FamilyMember = null
                };
            }

            if (residentFamilyMemberId.HasValue)
            {
                var memberEntity = await _context.Set<ResidentFamilyMember>()
                    .Include(m => m.MemberUnits)
                        .ThenInclude(mu => mu.Unit)
                    .FirstOrDefaultAsync(m => m.Id == residentFamilyMemberId.Value);

                if (memberEntity == null) return null;

                var member = _dataMapper.Map<ResidentFamilyMember, ResidentFamilyMemberAddEdit>(memberEntity);

                await TryAttachFaceImageAsync(memberEntity, member, CancellationToken.None);

                return new ResidentDetailResponse
                {
                    Type = "FamilyMember",
                    FamilyMember = member,
                    Resident = null
                };
            }

            return null;
        }

        private async Task TryAttachFaceImageAsync(ResidentMasterAddEdit resident, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(resident.FaceUrl) || resident.UnitIds == null || resident.UnitIds.Count == 0)
                return;

            var buildingId = await _context.Set<Unit>()
                .Where(unit => resident.UnitIds.Contains(unit.Id))
                .Select(unit => unit.BuildingId)
                .FirstOrDefaultAsync(ct);

            if (buildingId == 0)
                return;

            var result = await TryDownloadFaceImageAsync(buildingId, resident.FaceUrl, ct);
            if (result == null || result.Value.Data.Length == 0)
                return;

            resident.FaceImageBase64 = Convert.ToBase64String(result.Value.Data);
            resident.FaceImageContentType = result.Value.ContentType;
        }

        private async Task TryAttachFaceImageAsync(
            ResidentFamilyMember memberEntity,
            ResidentFamilyMemberAddEdit member,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(member.FaceUrl))
                return;

            var buildingId = memberEntity.MemberUnits?
                .Select(mu => mu.Unit?.BuildingId)
                .FirstOrDefault(id => id.HasValue);

            if (!buildingId.HasValue || buildingId.Value == 0)
                return;

            var result = await TryDownloadFaceImageAsync(buildingId.Value, member.FaceUrl, ct);
            if (result == null || result.Value.Data.Length == 0)
                return;

            member.FaceImageBase64 = Convert.ToBase64String(result.Value.Data);
            member.FaceImageContentType = result.Value.ContentType;
        }

        private async Task<(byte[] Data, string? ContentType)?> TryDownloadFaceImageAsync(
            long buildingId,
            string faceUrl,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(faceUrl))
                return null;

            var device = await (
                from b in _context.Buildings
                join d in _context.HikDevices on b.DeviceId equals d.Id
                where b.IsActive && b.Id == buildingId
                select new
                {
                    d.IpAddress,
                    d.PortNo,
                    d.DevIndex,
                    b.DeviceUserName,
                    b.DevicePassword
                }).FirstOrDefaultAsync(ct);

            if (device == null
                || string.IsNullOrWhiteSpace(device.IpAddress)
                || string.IsNullOrWhiteSpace(device.DeviceUserName)
                || string.IsNullOrWhiteSpace(device.DevicePassword))
            {
                return null;
            }

            var password = _secretProtector.Unprotect(device.DevicePassword);

            return await _hikvisionClient.DownloadFaceImageAsync(
                device.IpAddress,
                device.PortNo ?? 80,
                device.DeviceUserName,
                password,
                faceUrl,
                ct);
        }
        public async Task<PaginatedList<ResidentMasterList>> GetResidentsAsync(int pageIndex, int pageSize)
        {
            var query = _residentMasterRepository
                .Get(r => r.IsActive)
                .Include(r => r.ParentUnits)
                    .ThenInclude(mu => mu.Unit)
                .Include(r => r.FamilyMembers)
                    .ThenInclude(f => f.MemberUnits)
                        .ThenInclude(mu => mu.Unit);

            var totalCount = await query.CountAsync();
            var rows = await query
                .OrderBy(r => r.ParentFirstName)
                .ThenBy(r => r.ParentLastName)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = rows.Select(r => new ResidentMasterList
            {
                Id = r.Id,
                Code = r.Code,
                ParentFirstName = r.ParentFirstName,
                ParentLastName = r.ParentLastName,
                Email = r.Email,
                Mobile = r.Mobile,
                CountryCode = r.CountryCode,
                FaceId = r.FaceId,
                FaceUrl = r.FaceUrl,
                FingerId = r.FingerId,
                CardId = r.CardId,
                QrId = r.QrId,
                QrCodeValue = r.QrCodeValue,
                QrCodeImagePath = r.QrCodeImagePath,
                IsActive = r.IsActive,
                IsResident = r.IsResident,
                UnitIds = r.ParentUnits?.Select(mu => mu.UnitId).ToList(),
                FamilyMembers = r.FamilyMembers?.Select(f => new ResidentFamilyMemberList
                {
                    Id = f.Id,
                    ResidentMasterId = f.ResidentMasterId,
                    UnitIds = f.MemberUnits?.Select(mu => mu.UnitId).ToList(),
                    Units = f.MemberUnits?.Select(mu => new ResidentFamilyMemberUnitList
                    {
                        UnitId = mu.UnitId,
                        UnitCode = mu.Unit?.Code,
                        UnitName = mu.Unit?.UnitName
                    }).ToList(),
                    FirstName = f.FirstName,
                    Code = f.Code,
                    LastName = f.LastName,
                    Email = f.Email,
                    Mobile = f.Mobile,
                    FaceId = f.FaceId,
                    FaceUrl = f.FaceUrl,
                    FingerId = f.FingerId,
                    CardId = f.CardId,
                    QrId = f.QrId,
                    QrCodeValue = f.QrCodeValue,
                    QrCodeImagePath = f.QrCodeImagePath,
                    IsActive = f.IsActive,
                    IsResident = f.IsResident
                }).ToList()
            }).ToList();

            return new PaginatedList<ResidentMasterList>(mapped, totalCount, pageIndex, pageSize);
        }

        public async Task<InsertResponseModel> UpdateResidentAsync(ResidentMasterAddEdit resident)
        {
            try
            {
                var entity = await _residentMasterRepository.Get(r => r.Id == resident.Id)
                    .Include(r => r.ParentUnits)
                    .Include(r => r.FamilyMembers)
                        .ThenInclude(f => f.MemberUnits)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "404",
                        Message = "Resident not found."
                    };
                }

                var oldParentQrId = entity.QrId;
                var oldParentUnitIds = new HashSet<long>(entity.ParentUnits?.Select(mu => mu.UnitId) ?? Enumerable.Empty<long>());
                var oldFamilySnapshots = entity.FamilyMembers?.ToDictionary(
                    member => member.Id,
                    member => new FamilyQrSnapshot
                    {
                        QrId = member.QrId,
                        UnitIds = new HashSet<long>(member.MemberUnits?.Select(mu => mu.UnitId) ?? Enumerable.Empty<long>()),
                        QrCodeValue = member.QrCodeValue
                    }) ?? new Dictionary<long, FamilyQrSnapshot>();

                if (!string.IsNullOrWhiteSpace(resident.ProfilePhoto))
                    entity.ProfilePhoto = resident.ProfilePhoto;

                var userId = 1; // current actor
                var plainPassword = ResolvePlainPassword(resident.Password);

                // update parent basic info
                entity.ParentFirstName = resident.ParentFirstName;
                entity.ParentLastName = resident.ParentLastName;
                entity.Email = resident.Email;
                entity.Mobile = resident.Mobile;
                entity.CountryCode = resident.CountryCode;
                entity.FaceId = resident.FaceId;
                entity.FaceUrl = resident.FaceUrl;
                entity.FingerId = resident.FingerId;
                entity.IsActive = resident.IsActive;
                entity.IsResident = resident.IsResident;

                if (!string.IsNullOrWhiteSpace(resident.Password))
                {
                    entity.Password = _secretProtector.IsProtected(resident.Password)
                        ? resident.Password
                        : _secretProtector.Protect(resident.Password);
                }

                entity.ModifiedBy = userId;
                entity.ModifiedDate = DateTime.Now;

                // ---- Update parent units ----
                entity.ParentUnits ??= new List<ResidentMasterUnit>();
                var incomingParentUnitIds = resident.UnitIds?.Distinct().ToHashSet() ?? new HashSet<long>();

                // remove missing
                foreach (var link in entity.ParentUnits.ToList())
                    if (!incomingParentUnitIds.Contains(link.UnitId))
                        entity.ParentUnits.Remove(link);

                // add new
                var currentParentUnitIds = entity.ParentUnits.Select(mu => mu.UnitId).ToHashSet();
                foreach (var unitId in incomingParentUnitIds)
                    if (!currentParentUnitIds.Contains(unitId))
                        entity.ParentUnits.Add(new ResidentMasterUnit
                        {
                            UnitId = unitId,
                            IsActive = true,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = userId,
                            ModifiedDate = DateTime.Now
                        });

                // ---- Update family members ----
                var existingMembers = entity.FamilyMembers.ToList();
                var updatedMembers = resident.FamilyMembers?
                    .Where(member => !member.IsEmpty())
                    .ToList() ?? new List<ResidentFamilyMemberAddEdit>();

                var nextFamilySequence = GetNextFamilyMemberSequence(entity.FamilyMembers, entity.Code);
                foreach (var memberModel in updatedMembers)
                {
                    if (string.IsNullOrWhiteSpace(memberModel.Code))
                    {
                        memberModel.Code = $"{entity.Code}-FM-{nextFamilySequence:0000}";
                        nextFamilySequence++;
                    }

                    var existingMember = existingMembers.FirstOrDefault(f => f.Id == memberModel.Id && f.Id != 0);
                    if (existingMember != null)
                    {
                        existingMember.FirstName = memberModel.FirstName;
                        existingMember.LastName = memberModel.LastName;
                        existingMember.Email = memberModel.Email;
                        existingMember.Mobile = memberModel.Mobile;
                        existingMember.FaceId = memberModel.FaceId;
                        existingMember.FaceUrl = memberModel.FaceUrl;
                        existingMember.FingerId = memberModel.FingerId;
                        existingMember.CardId = memberModel.CardId;
                        existingMember.QrId = memberModel.QrId;
                        existingMember.IsActive = memberModel.IsActive;
                        existingMember.IsResident = memberModel.IsResident;
                        if (!string.IsNullOrWhiteSpace(memberModel.ProfilePhoto))
                            existingMember.ProfilePhoto = memberModel.ProfilePhoto;
                        existingMember.ModifiedBy = userId;
                        existingMember.ModifiedDate = DateTime.Now;

                        // update units
                        existingMember.MemberUnits ??= new List<ResidentFamilyMemberUnit>();
                        var incomingUnitIds = memberModel.UnitIds?.Distinct().ToHashSet() ?? new HashSet<long>();
                        foreach (var link in existingMember.MemberUnits.ToList())
                            if (!incomingUnitIds.Contains(link.UnitId))
                                existingMember.MemberUnits.Remove(link);

                        var currentUnitIds = existingMember.MemberUnits.Select(mu => mu.UnitId).ToHashSet();
                        foreach (var unitId in incomingUnitIds)
                            if (!currentUnitIds.Contains(unitId))
                                existingMember.MemberUnits.Add(new ResidentFamilyMemberUnit
                                {
                                    UnitId = unitId,
                                    IsActive = true,
                                    CreatedBy = userId,
                                    CreatedDate = DateTime.Now,
                                    ModifiedBy = userId,
                                    ModifiedDate = DateTime.Now
                                });
                    }
                    else
                    {
                        entity.FamilyMembers.Add(new ResidentFamilyMember
                        {
                            ResidentMasterId = entity.Id,
                            Code = memberModel.Code,
                            FirstName = memberModel.FirstName,
                            LastName = memberModel.LastName,
                            Email = memberModel.Email,
                            Mobile = memberModel.Mobile,
                            FaceId = memberModel.FaceId,
                            FaceUrl = memberModel.FaceUrl,
                            FingerId = memberModel.FingerId,
                            CardId = memberModel.CardId,
                            QrId = memberModel.QrId,
                            IsActive = true,
                            IsResident = memberModel.IsResident,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = userId,
                            ModifiedDate = DateTime.Now,
                            MemberUnits = memberModel.UnitIds?.Distinct().Select(unitId => new ResidentFamilyMemberUnit
                            {
                                UnitId = unitId,
                                IsActive = true,
                                CreatedBy = userId,
                                CreatedDate = DateTime.Now,
                                ModifiedBy = userId,
                                ModifiedDate = DateTime.Now
                            }).ToList()
                        });
                    }
                }

                // deactivate removed members
                var incomingIds = updatedMembers.Where(m => m.Id != 0).Select(m => m.Id).ToHashSet();
                foreach (var member in existingMembers)
                {
                    if (member.Id != 0 && !incomingIds.Contains(member.Id))
                    {
                        member.IsActive = false;
                        member.ModifiedBy = userId;
                        member.ModifiedDate = DateTime.Now;
                    }
                }

                await _residentMasterRepository.UpdateAsync(entity, userId.ToString(), "Update");

                // ---- ✅ User table upsert logic (Resident + Family) ----
                var mappedUserId = await GetMappedUserIdAsync(entity.Id, null);

                var (parentUser, parentUserErr) = await EnsureOrUpdateResidentUserAsync(
                    preferredUserId: mappedUserId,
                    name: $"{entity.ParentFirstName} {entity.ParentLastName}".Trim(),
                    email: entity.Email,
                    mobile: entity.Mobile,
                    fallbackUserName: entity.Code,
                    actorUserId: userId,
                    plainPassword: plainPassword,
                    isResident: entity.IsResident,
                    isActive: entity.IsActive
                );

                if (!string.IsNullOrWhiteSpace(parentUserErr))
                    return new InsertResponseModel { Id = 0, Code = "409", Message = parentUserErr };

                await SyncResidentUserMapAsync(
                    parentUser,
                    entity.Id,
                    null,
                    entity.IsResident,
                    entity.IsActive,
                    userId
                );

                foreach (var member in entity.FamilyMembers ?? new List<ResidentFamilyMember>())
                {
                    var memberMapUserId = await GetMappedUserIdAsync(null, member.Id);
                    var (memberUser, memberUserErr) = await EnsureOrUpdateResidentUserAsync(
                        preferredUserId: memberMapUserId,
                        name: $"{member.FirstName} {member.LastName}".Trim(),
                        email: member.Email,
                        mobile: member.Mobile,
                        fallbackUserName: BuildFamilyMemberFallback(entity, member),
                        actorUserId: userId,
                        plainPassword: null,
                        isResident: member.IsResident,
                        isActive: member.IsActive
                    );

                    if (!string.IsNullOrWhiteSpace(memberUserErr))
                        return new InsertResponseModel { Id = 0, Code = "409", Message = memberUserErr };

                    await SyncResidentUserMapAsync(
                        memberUser,
                        null,
                        member.Id,
                        member.IsResident,
                        member.IsActive,
                        userId
                    );
                }

                await _context.SaveChangesAsync();
                await EnsureResidentQrDataAsync(entity, oldParentQrId, oldParentUnitIds, oldFamilySnapshots);
                await _context.SaveChangesAsync();

                // ---- Hikvision sync ----
                var hikvisionWarnings = new List<string>();

                var residentShouldSync = entity.IsResident && entity.IsActive;
                if (residentShouldSync)
                {
                    var unitId = entity.ParentUnits?.FirstOrDefault()?.UnitId ?? 0;
                    if (unitId > 0)
                    {
                        var warning = await TryUpsertPersonAndCardInHikvisionAsync(
                            unitId,
                            ToEmployeeNo(entity.Code),
                            $"{entity.ParentFirstName} {entity.ParentLastName}".Trim(),
                            entity.CardId // pass cardId too
                        );

                        if (!string.IsNullOrWhiteSpace(warning))
                            hikvisionWarnings.Add($"Resident: {warning}");
                    }
                    else
                    {
                        hikvisionWarnings.Add("Resident: Unit not found for resident.");
                    }
                }

                foreach (var member in entity.FamilyMembers ?? new List<ResidentFamilyMember>())
                {
                    var memberShouldSync = member.IsResident && member.IsActive;
                    if (!memberShouldSync) continue;

                    var unitId = member.MemberUnits?.FirstOrDefault()?.UnitId
                        ?? entity.ParentUnits?.FirstOrDefault()?.UnitId
                        ?? 0;

                    if (unitId <= 0)
                    {
                        hikvisionWarnings.Add($"Family member {member.Id}: Unit not found.");
                        continue;
                    }

                    var employeeNo = ToEmployeeNo(!string.IsNullOrWhiteSpace(member.Code)
                        ? member.Code
                        : $"{entity.Code}FM{member.Id}");

                    var warning = await TryUpsertPersonAndCardInHikvisionAsync(
                        unitId,
                        employeeNo,
                        $"{member.FirstName} {member.LastName}".Trim(),
                        member.CardId
                    );

                    if (!string.IsNullOrWhiteSpace(warning))
                        hikvisionWarnings.Add($"Family member {member.Id}: {warning}");
                }

                var message = "Update successfully.";
                if (hikvisionWarnings.Any())
                    message = $"{message} Hikvision update warning: {string.Join(" | ", hikvisionWarnings)}";

                return new InsertResponseModel { Id = entity.Id, Code = entity.Code, Message = message };
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
        private async Task<string?> TryUpsertPersonAndCardInHikvisionAsync(
    long unitId,
    string employeeNo,
    string name,
    string? cardId)
        {
            try
            {
                var (error, ip, port, user, pass, devIndex) = await ResolveHikvisionCredentialsAsync(unitId);
                if (!string.IsNullOrWhiteSpace(error)) return error;

                var check = await _hikvisionClient.CheckDeviceCredentialsAsync(ip!, port, user!, pass!, devIndex, ct: default);
                if (!check.IsAuthorized)
                    return $"Hikvision credentials invalid. Status={check.StatusCode}. Body={check.ResponseBody}";

                // 1) try update first
                try
                {
                    await _hikvisionClient.UpdatePersonInDeviceAsync(
                        ip!, port, user!, pass!,
                        new HikvisionPersonInfo { EmployeeNo = employeeNo, Name = name },
                        devIndex: devIndex,
                        userType: "normal");
                }
                catch
                {
                    // 2) if update fails, try add
                    await _hikvisionClient.AddPersonToDeviceAsync(
                        ip!, port, user!, pass!,
                        new HikvisionPersonInfo { EmployeeNo = employeeNo, Name = name },
                        devIndex: devIndex,
                        userType: "normal",
                        ct: default);
                }

                // 3) card sync (optional but recommended)
                if (!string.IsNullOrWhiteSpace(cardId))
                {
                    try
                    {
                        await _hikvisionClient.AddCardToDeviceAsync(
                            ip!, port, user!, pass!,
                            new HikvisionCardInfo
                            {
                                EmployeeNo = employeeNo,
                                CardNo = cardId.Trim(),
                                CardType = "normalCard"
                            },
                            devIndex: devIndex,
                            ct: default);
                    }
                    catch (Exception ex)
                    {
                        // if card already exists, you may ignore; otherwise return warning
                        return $"Card sync warning: {ex.Message}";
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<InsertResponseModel> UpdateResidentDetailsAsync(ResidentDetailUpdateRequest_Profile request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Type))
                return new InsertResponseModel { Id = 0, Code = "400", Message = "Type is required." };
            // ✅ Determine target id from payload
            long? targetResidentId = null;
            long? targetFamilyMemberId = null;

            if (string.Equals(request.Type, "Resident", StringComparison.OrdinalIgnoreCase))
            {
                if (request.Resident == null)
                    return new InsertResponseModel { Id = 0, Code = "400", Message = "Resident payload required." };

                targetResidentId = request.Resident.Id;
            }
            else if (string.Equals(request.Type, "FamilyMember", StringComparison.OrdinalIgnoreCase))
            {
                if (request.FamilyMember == null)
                    return new InsertResponseModel { Id = 0, Code = "400", Message = "FamilyMember payload required." };

                targetFamilyMemberId = request.FamilyMember.Id;
            }
            else
            {
                return new InsertResponseModel { Id = 0, Code = "400", Message = "Invalid Type. Allowed: Resident / FamilyMember." };
            }

            // ✅ Resolve actor user id
            long actorUserId = request.ActorUserId.GetValueOrDefault();

            // If frontend didn't send actorUserId, derive from map using target ids (⚠️ insecure)
            if (actorUserId <= 0)
            {
                actorUserId = await _context.ResidentUserMaps
                    .AsNoTracking()
                    .Where(m => m.IsActive
                        && m.ResidentMasterId == targetResidentId
                        && m.ResidentFamilyMemberId == targetFamilyMemberId)
                    .Select(m => m.UserId)
                    .FirstOrDefaultAsync();

                if (actorUserId <= 0)
                    return new InsertResponseModel { Id = 0, Code = "404", Message = "User mapping not found for given Resident/FamilyMember Id." };
            }
            var map = await _context.ResidentUserMaps
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == actorUserId && x.IsActive);

            if (map == null)
                return new InsertResponseModel { Id = 0, Code = "404", Message = "User is not mapped in adm_ResidentUserMap." };

            if (map.ResidentMasterId.HasValue)
            {
                if (!string.Equals(request.Type, "Resident", StringComparison.OrdinalIgnoreCase))
                    return new InsertResponseModel { Id = 0, Code = "403", Message = "Logged-in user is Resident, only Resident update allowed." };

                if (request.Resident == null)
                    return new InsertResponseModel { Id = 0, Code = "400", Message = "Resident payload required." };

                if (request.Resident.Id != map.ResidentMasterId.Value)
                    return new InsertResponseModel { Id = 0, Code = "403", Message = "You can update only your own Resident profile." };

                return await UpdateResidentOnly_ProfilePatchAsync(request.Resident, actorUserId);
            }

            if (map.ResidentFamilyMemberId.HasValue)
            {
                if (!string.Equals(request.Type, "FamilyMember", StringComparison.OrdinalIgnoreCase))
                    return new InsertResponseModel { Id = 0, Code = "403", Message = "Logged-in user is FamilyMember, only FamilyMember update allowed." };

                if (request.FamilyMember == null)
                    return new InsertResponseModel { Id = 0, Code = "400", Message = "FamilyMember payload required." };

                if (request.FamilyMember.Id != map.ResidentFamilyMemberId.Value)
                    return new InsertResponseModel { Id = 0, Code = "403", Message = "You can update only your own FamilyMember profile." };

                return await UpdateFamilyMember_ProfilePatchAsync(request.FamilyMember, actorUserId);
            }

            return new InsertResponseModel { Id = 0, Code = "400", Message = "Invalid mapping found." };
        }
        private async Task<InsertResponseModel> UpdateFamilyMember_ProfilePatchAsync(
    FamilyMemberProfilePatchDto dto,
    long actorUserId)
        {
            try
            {
                var memberEntity = await _context.Set<ResidentFamilyMember>()
                    .Include(m => m.MemberUnits)
                    .FirstOrDefaultAsync(m => m.Id == dto.Id);

                if (memberEntity == null)
                    return new InsertResponseModel { Id = 0, Code = "404", Message = "Family member not found." };

                // Fetch resident once (needed for fallback username + QR build)
                var resident = await _context.Set<ResidentMaster>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == memberEntity.ResidentMasterId);

                if (resident == null)
                    return new InsertResponseModel { Id = 0, Code = "404", Message = "Resident not found for family member." };

                var oldQrId = memberEntity.QrId;
                var oldUnitIds = new HashSet<long>(
                    memberEntity.MemberUnits?.Select(x => x.UnitId) ?? Enumerable.Empty<long>());

                // ---------------- MERGE (NULL = keep existing) ----------------
                if (dto.FirstName != null) memberEntity.FirstName = dto.FirstName.Trim();
                if (dto.LastName != null) memberEntity.LastName = dto.LastName.Trim();

                if (dto.Email != null) memberEntity.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
                if (dto.Mobile != null) memberEntity.Mobile = string.IsNullOrWhiteSpace(dto.Mobile) ? null : dto.Mobile.Trim();

                if (dto.FaceId != null) memberEntity.FaceId = dto.FaceId;
                if (dto.FaceUrl != null) memberEntity.FaceUrl = dto.FaceUrl;
                if (dto.FingerId != null) memberEntity.FingerId = dto.FingerId;

                if (dto.ProfilePhoto != null && !string.IsNullOrWhiteSpace(dto.ProfilePhoto))
                    memberEntity.ProfilePhoto = dto.ProfilePhoto.Trim();

                if (dto.IsActive.HasValue) memberEntity.IsActive = dto.IsActive.Value;
                if (dto.IsResident.HasValue) memberEntity.IsResident = dto.IsResident.Value;

                // ---------------- CardId update ONLY if provided ----------------
                if (dto.CardId != null)
                {
                    var incomingCard = string.IsNullOrWhiteSpace(dto.CardId) ? null : dto.CardId.Trim();

                    // If you want to allow "clear card", handle that here.
                    // Current behavior: only update when a non-empty value is provided.
                    if (!string.IsNullOrWhiteSpace(incomingCard))
                    {
                        var temp = new ResidentFamilyMemberAddEdit
                        {
                            Id = memberEntity.Id,
                            CardId = incomingCard,
                            QrId = incomingCard,
                            Email = memberEntity.Email,
                            Mobile = memberEntity.Mobile,
                            IsResident = memberEntity.IsResident,
                            IsActive = memberEntity.IsActive,
                            UnitIds = dto.UnitIds ?? memberEntity.MemberUnits?.Select(x => x.UnitId).ToList() ?? new List<long>()
                        };

                        var idConflict = await ValidateFamilyMemberIdsAsync(temp);
                        if (!string.IsNullOrWhiteSpace(idConflict))
                            return new InsertResponseModel { Id = 0, Code = "409", Message = idConflict };

                        // temp.CardId is normalized inside ValidateFamilyMemberIdsAsync
                        memberEntity.CardId = temp.CardId;
                        memberEntity.QrId = temp.CardId;
                    }
                }

                // ---------------- Units update ONLY if provided ----------------
                if (dto.UnitIds != null)
                {
                    memberEntity.MemberUnits ??= new List<ResidentFamilyMemberUnit>();

                    var incoming = dto.UnitIds.Distinct().ToHashSet();

                    foreach (var link in memberEntity.MemberUnits.ToList())
                        if (!incoming.Contains(link.UnitId))
                            memberEntity.MemberUnits.Remove(link);

                    var current = memberEntity.MemberUnits.Select(x => x.UnitId).ToHashSet();
                    foreach (var unitId in incoming)
                        if (!current.Contains(unitId))
                            memberEntity.MemberUnits.Add(new ResidentFamilyMemberUnit
                            {
                                UnitId = unitId,
                                IsActive = true,
                                CreatedBy = actorUserId,
                                CreatedDate = DateTime.Now,
                                ModifiedBy = actorUserId,
                                ModifiedDate = DateTime.Now
                            });
                }

                // ---------------- VALIDATIONS using merged values ----------------
                var phoneConflict = await ValidateResidentPhoneNumbersAsync(new[]
                {
            new ResidentUserCandidate
            {
                Email = memberEntity.Email,
                Mobile = memberEntity.Mobile,
                FallbackUserName = string.IsNullOrWhiteSpace(memberEntity.Code) ? memberEntity.Id.ToString() : memberEntity.Code,
                IsResident = memberEntity.IsResident
            }
        });

                if (!string.IsNullOrWhiteSpace(phoneConflict))
                    return new InsertResponseModel { Id = 0, Code = "409", Message = phoneConflict };

                // ---------------- Save member ----------------
                memberEntity.ModifiedBy = actorUserId;
                memberEntity.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                // ---------------- User + Map upsert ----------------
                var mappedUserId = await GetMappedUserIdAsync(null, memberEntity.Id);

                var (user, userErr) = await EnsureOrUpdateResidentUserAsync(
                    preferredUserId: mappedUserId ?? actorUserId,
                    name: $"{memberEntity.FirstName} {memberEntity.LastName}".Trim(),
                    email: memberEntity.Email,
                    mobile: memberEntity.Mobile,
                    fallbackUserName: BuildFamilyMemberFallback(resident, memberEntity),
                    actorUserId: actorUserId,
                    plainPassword: null,
                    isResident: memberEntity.IsResident,
                    isActive: memberEntity.IsActive
                );

                if (!string.IsNullOrWhiteSpace(userErr))
                    return new InsertResponseModel { Id = 0, Code = "409", Message = userErr };

                await SyncResidentUserMapAsync(
                    user,
                    residentMasterId: null,
                    residentFamilyMemberId: memberEntity.Id,
                    isResident: memberEntity.IsResident,
                    isActive: memberEntity.IsActive,
                    actorUserId: actorUserId
                );

                await _context.SaveChangesAsync();

                // ---------------- QR regen ONLY if needed ----------------
                var newUnits = new HashSet<long>(memberEntity.MemberUnits.Select(x => x.UnitId));

                if (!string.Equals(oldQrId, memberEntity.QrId, StringComparison.OrdinalIgnoreCase) || !oldUnitIds.SetEquals(newUnits))
                {
                    var plain = BuildFamilyMemberQrValue(resident, memberEntity);
                    var encrypted = EncryptQrValue(plain);
                    memberEntity.QrCodeValue = encrypted;
                    memberEntity.QrCodeImagePath = await GenerateResidentQrImageAsync(encrypted, "ResidentFamily-QRCode");
                    await _context.SaveChangesAsync();
                }

                // ---------------- Hikvision update (optional) ----------------
                string? hikWarning = null;
                if (memberEntity.IsResident)
                {
                    var unitId = memberEntity.MemberUnits.FirstOrDefault()?.UnitId ?? 0;
                    if (unitId > 0)
                    {
                        var emp = ToEmployeeNo(!string.IsNullOrWhiteSpace(memberEntity.Code)
                            ? memberEntity.Code
                            : $"{resident.Code}FM{memberEntity.Id}");

                        hikWarning = await TryUpdatePersonInHikvisionAsync(
                            unitId,
                            emp,
                            $"{memberEntity.FirstName} {memberEntity.LastName}".Trim());
                    }
                }

                return new InsertResponseModel
                {
                    Id = memberEntity.Id,
                    Code = memberEntity.Code,
                    Message = string.IsNullOrWhiteSpace(hikWarning)
                        ? "Profile updated successfully."
                        : $"Profile updated successfully. Hikvision warning: {hikWarning}"
                };
            }
            catch (DbUpdateException ex)
            {
                return new InsertResponseModel
                {
                    Id = 0,
                    Code = "500",
                    Message = $"DB update error: {ex.InnerException?.Message ?? ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new InsertResponseModel { Id = 0, Code = "500", Message = ex.Message };
            }
        }

        private async Task<InsertResponseModel> UpdateResidentOnly_ProfilePatchAsync(ResidentProfilePatchDto dto, long actorUserId)
        {
            try
            {
                var entity = await _residentMasterRepository.Get(r => r.Id == dto.Id)
                    .Include(r => r.ParentUnits)
                    .FirstOrDefaultAsync();

                if (entity == null)
                    return new InsertResponseModel { Id = 0, Code = "404", Message = "Resident not found." };

                var oldQrId = entity.QrId;
                var oldUnits = new HashSet<long>(entity.ParentUnits?.Select(x => x.UnitId) ?? Enumerable.Empty<long>());

                // ---- Merge (NULL = keep existing) ----
                if (dto.ParentFirstName != null) entity.ParentFirstName = dto.ParentFirstName.Trim();
                if (dto.ParentLastName != null) entity.ParentLastName = dto.ParentLastName.Trim();

                if (dto.Email != null) entity.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
                if (dto.Mobile != null) entity.Mobile = string.IsNullOrWhiteSpace(dto.Mobile) ? null : dto.Mobile.Trim();

                if (dto.CountryCode != null) entity.CountryCode = dto.CountryCode;
                if (dto.FaceId != null) entity.FaceId = dto.FaceId;
                if (dto.FaceUrl != null) entity.FaceUrl = dto.FaceUrl;
                if (dto.FingerId != null) entity.FingerId = dto.FingerId;

                if (dto.ProfilePhoto != null && !string.IsNullOrWhiteSpace(dto.ProfilePhoto))
                    entity.ProfilePhoto = dto.ProfilePhoto.Trim();

                if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;
                if (dto.IsResident.HasValue) entity.IsResident = dto.IsResident.Value;

                // ---- CardId update ONLY if provided (not null) ----
                if (dto.CardId != null)
                {
                    var incomingCard = string.IsNullOrWhiteSpace(dto.CardId) ? null : dto.CardId.Trim();

                    if (!string.IsNullOrWhiteSpace(incomingCard))
                    {
                        // validate with your existing function
                        var temp = new ResidentMasterAddEdit
                        {
                            Id = entity.Id,
                            CardId = incomingCard,
                            QrId = incomingCard,
                            Email = entity.Email,
                            Mobile = entity.Mobile,
                            IsResident = entity.IsResident,
                            IsActive = entity.IsActive
                        };

                        var idConflict = await ValidateResidentIdsAsync(temp, new List<ResidentFamilyMemberAddEdit>(), entity.Id);
                        if (!string.IsNullOrWhiteSpace(idConflict))
                            return new InsertResponseModel { Id = 0, Code = "409", Message = idConflict };

                        entity.CardId = temp.CardId;
                        entity.QrId = temp.CardId;
                    }
                    // if dto.CardId == "" you can decide: ignore or clear. Currently: ignore clearing.
                }

                // ---- Units update ONLY if provided (not null) ----
                if (dto.UnitIds != null)
                {
                    entity.ParentUnits ??= new List<ResidentMasterUnit>();

                    var incoming = dto.UnitIds.Distinct().ToHashSet();

                    foreach (var link in entity.ParentUnits.ToList())
                        if (!incoming.Contains(link.UnitId))
                            entity.ParentUnits.Remove(link);

                    var current = entity.ParentUnits.Select(x => x.UnitId).ToHashSet();
                    foreach (var unitId in incoming)
                        if (!current.Contains(unitId))
                            entity.ParentUnits.Add(new ResidentMasterUnit
                            {
                                UnitId = unitId,
                                IsActive = true,
                                CreatedBy = actorUserId,
                                CreatedDate = DateTime.Now,
                                ModifiedBy = actorUserId,
                                ModifiedDate = DateTime.Now
                            });
                }

                // ---- VALIDATIONS using merged values ----
                var phoneConflict = await ValidateResidentPhoneNumbersAsync(new[]
                {
            new ResidentUserCandidate
            {
                Email = entity.Email,
                Mobile = entity.Mobile,
                FallbackUserName = entity.Code,
                IsResident = entity.IsResident
            }
        });

                if (!string.IsNullOrWhiteSpace(phoneConflict))
                    return new InsertResponseModel { Id = 0, Code = "409", Message = phoneConflict };

                var emailConflict = await ValidateParentEmailAsync(entity.Email, entity.Id);
                if (!string.IsNullOrWhiteSpace(emailConflict))
                    return new InsertResponseModel { Id = 0, Code = "409", Message = emailConflict };

                entity.ModifiedBy = actorUserId;
                entity.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                // QR regen if needed
                var newUnits = new HashSet<long>(entity.ParentUnits.Select(x => x.UnitId));
                if (!string.Equals(oldQrId, entity.QrId, StringComparison.OrdinalIgnoreCase) || !oldUnits.SetEquals(newUnits))
                {
                    var plain = BuildResidentQrValue(entity);
                    var encrypted = EncryptQrValue(plain);
                    entity.QrCodeValue = encrypted;
                    entity.QrCodeImagePath = await GenerateResidentQrImageAsync(encrypted, "Residents-QRCode");
                    await _context.SaveChangesAsync();
                }

                return new InsertResponseModel { Id = entity.Id, Code = entity.Code, Message = "Profile updated successfully." };
            }
            catch (DbUpdateException ex)
            {
                return new InsertResponseModel
                {
                    Id = 0,
                    Code = "500",
                    Message = $"DB update error: {ex.InnerException?.Message ?? ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new InsertResponseModel { Id = 0, Code = "500", Message = ex.Message };
            }
        }

        private async Task<InsertResponseModel> UpdateResidentOnly_ProfileAsync(ResidentMasterAddEdit resident, long actorUserId)
        {
            var entity = await _residentMasterRepository.Get(r => r.Id == resident.Id)
                .Include(r => r.ParentUnits)
                .FirstOrDefaultAsync();

            if (entity == null)
                return new InsertResponseModel { Id = 0, Code = "404", Message = "Resident not found." };

            var oldQrId = entity.QrId;
            var oldUnits = new HashSet<long>(entity.ParentUnits?.Select(x => x.UnitId) ?? Enumerable.Empty<long>());

            // validations
            var phoneConflict = await ValidateResidentPhoneNumbersAsync(new[]
            {
        new ResidentUserCandidate
        {
            Email = resident.Email,
            Mobile = resident.Mobile,
            FallbackUserName = entity.Code,
            IsResident = resident.IsResident
        }
    });

            if (!string.IsNullOrWhiteSpace(phoneConflict))
                return new InsertResponseModel { Id = 0, Code = "409", Message = phoneConflict };

            var idConflict = await ValidateResidentIdsAsync(resident, new List<ResidentFamilyMemberAddEdit>(), entity.Id);
            if (!string.IsNullOrWhiteSpace(idConflict))
                return new InsertResponseModel { Id = 0, Code = "409", Message = idConflict };

            var emailConflict = await ValidateParentEmailAsync(resident.Email, entity.Id);
            if (!string.IsNullOrWhiteSpace(emailConflict))
                return new InsertResponseModel { Id = 0, Code = "409", Message = emailConflict };

            // update fields
            entity.ParentFirstName = resident.ParentFirstName;
            entity.ParentLastName = resident.ParentLastName;
            entity.Email = resident.Email;
            entity.Mobile = resident.Mobile;
            entity.CountryCode = resident.CountryCode;
            entity.FaceId = resident.FaceId;
            entity.FaceUrl = resident.FaceUrl;
            entity.FingerId = resident.FingerId;
            var wantsCardUpdate = !string.IsNullOrWhiteSpace(resident.CardId);

            if (wantsCardUpdate)
            {
                // ✅ validate only if trying to change card
                var idConflicts = await ValidateResidentIdsAsync(resident, new List<ResidentFamilyMemberAddEdit>(), entity.Id);
                if (!string.IsNullOrWhiteSpace(idConflicts))
                    return new InsertResponseModel { Id = 0, Code = "409", Message = idConflicts };

                entity.CardId = resident.CardId;   // (already normalized inside ValidateResidentIdsAsync)
                entity.QrId = resident.CardId;   // ✅ keep qrId = cardId
            }
            entity.IsActive = resident.IsActive;
            entity.IsResident = resident.IsResident;
            if (!string.IsNullOrWhiteSpace(resident.ProfilePhoto))
                entity.ProfilePhoto = resident.ProfilePhoto;

            entity.ModifiedBy = actorUserId;
            entity.ModifiedDate = DateTime.Now;

            // update unit links
            entity.ParentUnits ??= new List<ResidentMasterUnit>();
            var incoming = resident.UnitIds?.Distinct().ToHashSet() ?? new HashSet<long>();

            foreach (var link in entity.ParentUnits.ToList())
                if (!incoming.Contains(link.UnitId)) entity.ParentUnits.Remove(link);

            var current = entity.ParentUnits.Select(x => x.UnitId).ToHashSet();
            foreach (var unitId in incoming)
                if (!current.Contains(unitId))
                    entity.ParentUnits.Add(new ResidentMasterUnit
                    {
                        UnitId = unitId,
                        IsActive = true,
                        CreatedBy = actorUserId,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = actorUserId,
                        ModifiedDate = DateTime.Now
                    });

            await _residentMasterRepository.UpdateAsync(entity, actorUserId.ToString(), "ProfileUpdate");
            await _context.SaveChangesAsync();

            var mappedUserId = await GetMappedUserIdAsync(entity.Id, null);

            var (user, userErr) = await EnsureOrUpdateResidentUserAsync(
                preferredUserId: mappedUserId ?? actorUserId,   // profile update should keep same user
                name: $"{entity.ParentFirstName} {entity.ParentLastName}".Trim(),
                email: entity.Email,
                mobile: entity.Mobile,
                fallbackUserName: entity.Code,
                actorUserId: actorUserId,
                plainPassword: null,
                isResident: entity.IsResident,
                isActive: entity.IsActive
            );

            if (!string.IsNullOrWhiteSpace(userErr))
                return new InsertResponseModel { Id = 0, Code = "409", Message = userErr };

            // keep map active / correct
            await SyncResidentUserMapAsync(
                user,
                residentMasterId: entity.Id,
                residentFamilyMemberId: null,
                isResident: entity.IsResident,
                isActive: entity.IsActive,
                actorUserId: actorUserId
            );

            await _context.SaveChangesAsync();

            // ✅ regenerate ONLY parent qr if needed
            var newUnits = new HashSet<long>(entity.ParentUnits.Select(x => x.UnitId));
            if (!string.Equals(oldQrId, entity.QrId, StringComparison.OrdinalIgnoreCase) || !oldUnits.SetEquals(newUnits))
            {
                var plain = BuildResidentQrValue(entity);
                var encrypted = EncryptQrValue(plain);
                entity.QrCodeValue = encrypted;
                entity.QrCodeImagePath = await GenerateResidentQrImageAsync(encrypted, "Residents-QRCode");
                await _context.SaveChangesAsync();
            }

            // ✅ Hikvision update (single person only)
            string? hikWarning = null;
            if (entity.IsResident)
            {
                var unitId = entity.ParentUnits.FirstOrDefault()?.UnitId ?? 0;
                if (unitId > 0)
                    hikWarning = await TryUpdatePersonInHikvisionAsync(unitId, ToEmployeeNo(entity.Code), $"{entity.ParentFirstName} {entity.ParentLastName}".Trim());
            }

            return new InsertResponseModel
            {
                Id = entity.Id,
                Code = entity.Code,
                Message = string.IsNullOrWhiteSpace(hikWarning)
                    ? "Profile updated successfully."
                    : $"Profile updated successfully. Hikvision warning: {hikWarning}"
            };
        }
        private async Task<InsertResponseModel> UpdateFamilyMember_ProfileAsync(ResidentFamilyMemberAddEdit member, long actorUserId)
        {
            var memberEntity = await _context.Set<ResidentFamilyMember>()
                .Include(m => m.MemberUnits)
                .FirstOrDefaultAsync(m => m.Id == member.Id);

            if (memberEntity == null)
                return new InsertResponseModel { Id = 0, Code = "404", Message = "Family member not found." };

            // validations
            var phoneConflict = await ValidateResidentPhoneNumbersAsync(new[]
            {
        new ResidentUserCandidate
        {
            Email = member.Email,
            Mobile = member.Mobile,
            FallbackUserName = memberEntity.Code ?? memberEntity.Id.ToString(),
            IsResident = member.IsResident
        }
    });

            if (!string.IsNullOrWhiteSpace(phoneConflict))
                return new InsertResponseModel { Id = 0, Code = "409", Message = phoneConflict };

            var idConflict = await ValidateFamilyMemberIdsAsync(member);
            if (!string.IsNullOrWhiteSpace(idConflict))
                return new InsertResponseModel { Id = 0, Code = "409", Message = idConflict };

            var oldQrId = memberEntity.QrId;
            var oldUnitIds = new HashSet<long>(memberEntity.MemberUnits?.Select(x => x.UnitId) ?? Enumerable.Empty<long>());

            // update fields
            memberEntity.FirstName = member.FirstName;
            memberEntity.LastName = member.LastName;
            memberEntity.Email = member.Email;
            memberEntity.Mobile = member.Mobile;
            memberEntity.FaceId = member.FaceId;
            memberEntity.FaceUrl = member.FaceUrl;
            memberEntity.FingerId = member.FingerId;
            var wantsCardUpdate = !string.IsNullOrWhiteSpace(member.CardId);

            if (wantsCardUpdate)
            {
                var idConflicts = await ValidateFamilyMemberIdsAsync(member);
                if (!string.IsNullOrWhiteSpace(idConflicts))
                    return new InsertResponseModel { Id = 0, Code = "409", Message = idConflicts };

                memberEntity.CardId = member.CardId;
                memberEntity.QrId = member.CardId;
            }
            memberEntity.IsActive = member.IsActive;
            memberEntity.IsResident = member.IsResident;
            if (!string.IsNullOrWhiteSpace(member.ProfilePhoto))
                memberEntity.ProfilePhoto = member.ProfilePhoto;

            memberEntity.ModifiedBy = actorUserId;
            memberEntity.ModifiedDate = DateTime.Now;

            // update unit links
            memberEntity.MemberUnits ??= new List<ResidentFamilyMemberUnit>();
            var incoming = member.UnitIds?.Distinct().ToHashSet() ?? new HashSet<long>();

            foreach (var link in memberEntity.MemberUnits.ToList())
                if (!incoming.Contains(link.UnitId)) memberEntity.MemberUnits.Remove(link);

            var current = memberEntity.MemberUnits.Select(x => x.UnitId).ToHashSet();
            foreach (var unitId in incoming)
                if (!current.Contains(unitId))
                    memberEntity.MemberUnits.Add(new ResidentFamilyMemberUnit
                    {
                        UnitId = unitId,
                        IsActive = true,
                        CreatedBy = actorUserId,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = actorUserId,
                        ModifiedDate = DateTime.Now
                    });

            await _context.SaveChangesAsync();
            var mappedUserId = await GetMappedUserIdAsync(null, memberEntity.Id);

            var (user, userErr) = await EnsureOrUpdateResidentUserAsync(
                preferredUserId: mappedUserId ?? actorUserId,
                name: $"{memberEntity.FirstName} {memberEntity.LastName}".Trim(),
                email: memberEntity.Email,
                mobile: memberEntity.Mobile,
                fallbackUserName: BuildFamilyMemberFallback(
                    await _context.Set<ResidentMaster>().AsNoTracking().FirstAsync(r => r.Id == memberEntity.ResidentMasterId),
                    memberEntity),
                actorUserId: actorUserId,
                plainPassword: null,
                isResident: memberEntity.IsResident,
                isActive: memberEntity.IsActive
            );

            if (!string.IsNullOrWhiteSpace(userErr))
                return new InsertResponseModel { Id = 0, Code = "409", Message = userErr };

            await SyncResidentUserMapAsync(
                user,
                residentMasterId: null,
                residentFamilyMemberId: memberEntity.Id,
                isResident: memberEntity.IsResident,
                isActive: memberEntity.IsActive,
                actorUserId: actorUserId
            );

            await _context.SaveChangesAsync();
            // ✅ regenerate ONLY member QR if changed
            var newUnits = new HashSet<long>(memberEntity.MemberUnits.Select(x => x.UnitId));
            if (!string.Equals(oldQrId, memberEntity.QrId, StringComparison.OrdinalIgnoreCase) || !oldUnitIds.SetEquals(newUnits))
            {
                var resident = await _context.Set<ResidentMaster>()
                    .AsNoTracking()
                    .FirstAsync(r => r.Id == memberEntity.ResidentMasterId);

                var plain = BuildFamilyMemberQrValue(resident, memberEntity);
                var encrypted = EncryptQrValue(plain);
                memberEntity.QrCodeValue = encrypted;
                memberEntity.QrCodeImagePath = await GenerateResidentQrImageAsync(encrypted, "ResidentFamily-QRCode");
                await _context.SaveChangesAsync();
            }

            // ✅ Hikvision update (single person only)
            string? hikWarning = null;
            if (memberEntity.IsResident)
            {
                var unitId = memberEntity.MemberUnits.FirstOrDefault()?.UnitId ?? 0;
                if (unitId > 0)
                {
                    var emp = ToEmployeeNo(!string.IsNullOrWhiteSpace(memberEntity.Code) ? memberEntity.Code : $"{memberEntity.ResidentMasterId}FM{memberEntity.Id}");
                    hikWarning = await TryUpdatePersonInHikvisionAsync(unitId, emp, $"{memberEntity.FirstName} {memberEntity.LastName}".Trim());
                }
            }

            return new InsertResponseModel
            {
                Id = memberEntity.Id,
                Code = memberEntity.Code,
                Message = string.IsNullOrWhiteSpace(hikWarning)
                    ? "Profile updated successfully."
                    : $"Profile updated successfully. Hikvision warning: {hikWarning}"
            };
        }

        private async Task<InsertResponseModel> UpdateResidentOnlyAsync(ResidentMasterAddEdit resident)
        {
            try
            {
                var entity = await _residentMasterRepository.Get(r => r.Id == resident.Id)
                    .Include(r => r.ParentUnits)
                    .Include(r => r.FamilyMembers)
                        .ThenInclude(f => f.MemberUnits)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "404",
                        Message = "Resident not found."
                    };
                }

                var oldParentQrId = entity.QrId;
                var oldParentUnitIds = new HashSet<long>(entity.ParentUnits?.Select(mu => mu.UnitId) ?? Enumerable.Empty<long>());
                var oldFamilySnapshots = entity.FamilyMembers?.ToDictionary(
                    member => member.Id,
                    member => new FamilyQrSnapshot
                    {
                        QrId = member.QrId,
                        UnitIds = new HashSet<long>(member.MemberUnits?.Select(mu => mu.UnitId) ?? Enumerable.Empty<long>()),
                        QrCodeValue = member.QrCodeValue
                    }) ?? new Dictionary<long, FamilyQrSnapshot>();

                if (!string.IsNullOrWhiteSpace(resident.ProfilePhoto))
                {
                    entity.ProfilePhoto = resident.ProfilePhoto;
                }

                var userId = 1;
                var plainPassword = ResolvePlainPassword(resident.Password);

                entity.ParentFirstName = resident.ParentFirstName;
                entity.ParentLastName = resident.ParentLastName;
                entity.Email = resident.Email;
                entity.Mobile = resident.Mobile;
                entity.CountryCode = resident.CountryCode;
                entity.FaceId = resident.FaceId;
                entity.FaceUrl = resident.FaceUrl;
                entity.FingerId = resident.FingerId;

                if (!string.IsNullOrWhiteSpace(resident.Password))
                {
                    entity.Password = _secretProtector.IsProtected(resident.Password)
                        ? resident.Password
                        : _secretProtector.Protect(resident.Password);
                }

                entity.IsActive = resident.IsActive;
                entity.IsResident = resident.IsResident;
                entity.ModifiedBy = userId;
                entity.ModifiedDate = DateTime.Now;

                var phoneConflict = await ValidateResidentPhoneNumbersAsync(
                    new[]
                    {
                        new ResidentUserCandidate
                        {
                            Email = resident.Email,
                            Mobile = resident.Mobile,
                            FallbackUserName = entity.Code,
                            IsResident = resident.IsResident
                        }
                    });

                if (!string.IsNullOrWhiteSpace(phoneConflict))
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "409",
                        Message = phoneConflict
                    };
                }

                var idConflict = await ValidateResidentIdsAsync(resident, new List<ResidentFamilyMemberAddEdit>(), entity.Id);
                if (!string.IsNullOrWhiteSpace(idConflict))
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "409",
                        Message = idConflict
                    };
                }

                entity.CardId = resident.CardId;
                entity.QrId = resident.QrId;

                var emailConflict = await ValidateParentEmailAsync(resident.Email, entity.Id);
                if (!string.IsNullOrWhiteSpace(emailConflict))
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "409",
                        Message = emailConflict
                    };
                }

                entity.ParentUnits ??= new List<ResidentMasterUnit>();
                var incomingParentUnitIds = resident.UnitIds?.Distinct().ToHashSet() ?? new HashSet<long>();
                var existingParentUnits = entity.ParentUnits.ToList();

                foreach (var link in existingParentUnits)
                {
                    if (!incomingParentUnitIds.Contains(link.UnitId))
                    {
                        entity.ParentUnits.Remove(link);
                    }
                }

                var currentParentUnitIds = entity.ParentUnits.Select(mu => mu.UnitId).ToHashSet();
                foreach (var unitId in incomingParentUnitIds)
                {
                    if (!currentParentUnitIds.Contains(unitId))
                    {
                        entity.ParentUnits.Add(new ResidentMasterUnit
                        {
                            UnitId = unitId,
                            IsActive = true,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = userId,
                            ModifiedDate = DateTime.Now
                        });
                    }
                }

                await _residentMasterRepository.UpdateAsync(entity, userId.ToString(), "Update");

                var updatedParentUser = await EnsureResidentUserAsync(
                    $"{resident.ParentFirstName} {resident.ParentLastName}".Trim(),
                    resident.Email,
                    resident.Mobile,
                    entity.Code,
                    userId,
                    plainPassword,
                    resident.IsResident,
                    resident.IsActive);

                await SyncResidentUserMapAsync(
                    updatedParentUser,
                    entity.Id,
                    null,
                    resident.IsResident,
                    resident.IsActive,
                    userId);

                await _context.SaveChangesAsync();
                await EnsureResidentQrDataAsync(entity, oldParentQrId, oldParentUnitIds, oldFamilySnapshots);
                await _context.SaveChangesAsync();

                var hikvisionWarnings = new List<string>();
                if (resident.IsResident)
                {
                    var unitId = resident.UnitIds?.FirstOrDefault() ?? entity.ParentUnits?.FirstOrDefault()?.UnitId ?? 0;
                    if (unitId > 0)
                    {
                        var warning = await TryUpdatePersonInHikvisionAsync(
                            unitId,
                            ToEmployeeNo(entity.Code),
                            $"{entity.ParentFirstName} {entity.ParentLastName}".Trim());

                        if (!string.IsNullOrWhiteSpace(warning))
                        {
                            hikvisionWarnings.Add($"Resident: {warning}");
                        }
                    }
                    else
                    {
                        hikvisionWarnings.Add("Resident: Unit not found for resident.");
                    }
                }

                var message = "Update successfully.";
                if (hikvisionWarnings.Any())
                {
                    message = $"{message} Hikvision update warning: {string.Join(" | ", hikvisionWarnings)}";
                }


                return new InsertResponseModel
                {
                    Id = entity.Id,
                    Code = entity.Code,
                    Message = message
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

        private async Task<InsertResponseModel> UpdateFamilyMemberAsync(ResidentFamilyMemberAddEdit member)
        {
            try
            {
                var entity = await _residentMasterRepository.Get(r => r.Id == member.ResidentMasterId)
                    .Include(r => r.ParentUnits)
                    .Include(r => r.FamilyMembers)
                        .ThenInclude(f => f.MemberUnits)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "404",
                        Message = "Resident not found."
                    };
                }

                var existingMember = entity.FamilyMembers?.FirstOrDefault(f => f.Id == member.Id);
                if (existingMember == null)
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "404",
                        Message = "Family member not found."
                    };
                }

                var oldParentQrId = entity.QrId;
                var oldParentUnitIds = new HashSet<long>(entity.ParentUnits?.Select(mu => mu.UnitId) ?? Enumerable.Empty<long>());
                var oldFamilySnapshots = entity.FamilyMembers?.ToDictionary(
                    m => m.Id,
                    m => new FamilyQrSnapshot
                    {
                        QrId = m.QrId,
                        UnitIds = new HashSet<long>(m.MemberUnits?.Select(mu => mu.UnitId) ?? Enumerable.Empty<long>()),
                        QrCodeValue = m.QrCodeValue
                    }) ?? new Dictionary<long, FamilyQrSnapshot>();

                var phoneConflict = await ValidateResidentPhoneNumbersAsync(
                    new[]
                    {
                        new ResidentUserCandidate
                        {
                            Email = member.Email,
                            Mobile = member.Mobile,
                            FallbackUserName = string.IsNullOrWhiteSpace(existingMember.Code)
                                ? $"{entity.Code}-FM-{existingMember.Id:0000}"
                                : existingMember.Code,
                            IsResident = member.IsResident
                        }
                    });

                if (!string.IsNullOrWhiteSpace(phoneConflict))
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "409",
                        Message = phoneConflict
                    };
                }

                var idConflict = await ValidateFamilyMemberIdsAsync(member);
                if (!string.IsNullOrWhiteSpace(idConflict))
                {
                    return new InsertResponseModel
                    {
                        Id = 0,
                        Code = "409",
                        Message = idConflict
                    };
                }

                existingMember.FirstName = member.FirstName;
                existingMember.LastName = member.LastName;
                existingMember.Email = member.Email;
                existingMember.Mobile = member.Mobile;
                existingMember.FaceId = member.FaceId;
                existingMember.FaceUrl = member.FaceUrl;
                existingMember.FingerId = member.FingerId;
                existingMember.CardId = member.CardId;
                existingMember.QrId = member.QrId;
                existingMember.IsActive = member.IsActive;
                existingMember.IsResident = member.IsResident;

                if (!string.IsNullOrWhiteSpace(member.Code))
                {
                    existingMember.Code = member.Code;
                }

                if (!string.IsNullOrWhiteSpace(member.ProfilePhoto))
                {
                    existingMember.ProfilePhoto = member.ProfilePhoto;
                }

                var userId = 1;
                existingMember.ModifiedBy = userId;
                existingMember.ModifiedDate = DateTime.Now;

                existingMember.MemberUnits ??= new List<ResidentFamilyMemberUnit>();
                var incomingUnitIds = member.UnitIds?.Distinct().ToHashSet() ?? new HashSet<long>();
                var existingUnitLinks = existingMember.MemberUnits.ToList();

                foreach (var link in existingUnitLinks)
                {
                    if (!incomingUnitIds.Contains(link.UnitId))
                    {
                        existingMember.MemberUnits.Remove(link);
                    }
                }

                var currentUnitIds = existingMember.MemberUnits.Select(mu => mu.UnitId).ToHashSet();
                foreach (var unitId in incomingUnitIds)
                {
                    if (!currentUnitIds.Contains(unitId))
                    {
                        existingMember.MemberUnits.Add(new ResidentFamilyMemberUnit
                        {
                            UnitId = unitId,
                            IsActive = true,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = userId,
                            ModifiedDate = DateTime.Now
                        });
                    }
                }

                await _residentMasterRepository.UpdateAsync(entity, userId.ToString(), "UpdateFamilyMember");

                var memberUser = await EnsureResidentUserAsync(
                    $"{existingMember.FirstName} {existingMember.LastName}".Trim(),
                    existingMember.Email,
                    existingMember.Mobile,
                    BuildFamilyMemberFallback(entity, existingMember),
                    userId,
                    null,
                    existingMember.IsResident,
                    existingMember.IsActive);

                await SyncResidentUserMapAsync(
                    memberUser,
                    null,
                    existingMember.Id,
                    existingMember.IsResident,
                    existingMember.IsActive,
                    userId);

                await _context.SaveChangesAsync();
                await EnsureResidentQrDataAsync(entity, oldParentQrId, oldParentUnitIds, oldFamilySnapshots);
                await _context.SaveChangesAsync();

                var hikvisionWarnings = new List<string>();
                if (existingMember.IsResident)
                {
                    var unitId = member.UnitIds?.FirstOrDefault()
                        ?? existingMember.MemberUnits?.FirstOrDefault()?.UnitId
                        ?? entity.ParentUnits?.FirstOrDefault()?.UnitId
                        ?? 0;

                    if (unitId > 0)
                    {
                        var employeeNo = ToEmployeeNo(!string.IsNullOrWhiteSpace(existingMember.Code)
                            ? existingMember.Code
                            : $"{entity.Code}FM{existingMember.Id}");

                        var warning = await TryUpdatePersonInHikvisionAsync(
                            unitId,
                            employeeNo,
                            $"{existingMember.FirstName} {existingMember.LastName}".Trim());

                        if (!string.IsNullOrWhiteSpace(warning))
                        {
                            hikvisionWarnings.Add(warning);
                        }
                    }
                    else
                    {
                        hikvisionWarnings.Add("Unit not found for resident.");
                    }
                }

                var message = "Update successfully.";
                if (hikvisionWarnings.Any())
                {
                    message = $"{message} Hikvision update warning: {string.Join(" | ", hikvisionWarnings)}";
                }

                return new InsertResponseModel
                {
                    Id = existingMember.Id,
                    Code = existingMember.Code,
                    Message = message
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
        public async Task<bool> UpdateResidentProfilePhotoAsync(long residentId, string photoPath, long userId)
        {
            var entity = await _residentMasterRepository.Get(r => r.Id == residentId).FirstOrDefaultAsync();
            if (entity == null) return false;

            entity.ProfilePhoto = photoPath;
            entity.ModifiedBy = userId;
            entity.ModifiedDate = DateTime.Now;

            await _residentMasterRepository.UpdateAsync(entity, userId.ToString(), "UpdateProfilePhoto");
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateFamilyMemberProfilePhotoAsync(long familyMemberId, string photoPath, long userId)
        {
            var member = await _context.Set<ResidentFamilyMember>().FirstOrDefaultAsync(f => f.Id == familyMemberId);
            if (member == null) return false;

            member.ProfilePhoto = photoPath;
            member.ModifiedBy = userId;
            member.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IReadOnlyList<ResidentFamilyMemberList>> GetFamilyMembersByResidentIdAsync(long residentMasterId)
        {
            var residentExists = await _residentMasterRepository.Get(r => r.Id == residentMasterId).AnyAsync();
            if (!residentExists)
            {
                return null;
            }

            var members = await _context.Set<ResidentFamilyMember>()
                .Where(f => f.ResidentMasterId == residentMasterId && f.IsActive)
                .Include(f => f.MemberUnits)
                    .ThenInclude(mu => mu.Unit)
                .OrderBy(f => f.FirstName)
                .ThenBy(f => f.LastName)
                .ToListAsync();

            return members.Select(f => new ResidentFamilyMemberList
            {
                Id = f.Id,
                ResidentMasterId = f.ResidentMasterId,
                UnitIds = f.MemberUnits?.Select(mu => mu.UnitId).ToList(),
                Units = f.MemberUnits?.Select(mu => new ResidentFamilyMemberUnitList
                {
                    UnitId = mu.UnitId,
                    UnitCode = mu.Unit?.Code,
                    UnitName = mu.Unit?.UnitName
                }).ToList(),
                FirstName = f.FirstName,
                Code = f.Code,
                LastName = f.LastName,
                Email = f.Email,
                Mobile = f.Mobile,
                FaceId = f.FaceId,
                FaceUrl = f.FaceUrl,
                FingerId = f.FingerId,
                CardId = f.CardId,
                QrId = f.QrId,
                QrCodeValue = f.QrCodeValue,
                QrCodeImagePath = f.QrCodeImagePath,
                HasFace = f.HasFace,
                HasFingerprint = f.HasFingerprint,
                LastBiometricSyncUtc = f.LastBiometricSyncUtc,
                IsActive = f.IsActive,
                IsResident = f.IsResident
            }).ToList();
        }
        private static (string begin, string end) BuildValidPeriod10Years()
        {
            var beginDt = DateTime.Now;

            var endDt = beginDt.AddYears(10);
            var maxEnd = new DateTime(2037, 12, 31, 23, 59, 59);

            if (endDt > maxEnd) endDt = maxEnd;

            // format like your gateway example (no timezone)
            var begin = beginDt.ToString("yyyy-MM-ddTHH:mm:ss");
            var end = endDt.ToString("yyyy-MM-ddTHH:mm:ss");

            return (begin, end);
        }
        public async Task<IReadOnlyList<ResidentUserDropdownItem>> GetResidentUsersByUnitAsync(long unitId)
        {
            var residentUnits = _context.Set<ResidentMasterUnit>()
                .AsNoTracking()
                .Where(u => u.UnitId == unitId && u.IsActive);

            var residentItems = await _context.Set<ResidentMaster>()
                .AsNoTracking()
                .Where(r => r.IsActive)
                .Join(
                    residentUnits,
                    resident => resident.Id,
                    unit => unit.ResidentMasterId,
                    (resident, unit) => new ResidentUserDropdownItem
                    {
                        Id = resident.Id,
                        Name = $"{resident.ParentFirstName} {resident.ParentLastName}".Trim(),
                        Type = "Resident",
                        ResidentMasterId = resident.Id,
                        ResidentFamilyMemberId = null
                    })
                .ToListAsync();

            var memberUnits = _context.Set<ResidentFamilyMemberUnit>()
                .AsNoTracking()
                .Where(u => u.UnitId == unitId && u.IsActive);

            var memberItems = await _context.Set<ResidentFamilyMember>()
                .AsNoTracking()
                .Where(m => m.IsActive)
                .Join(
                    memberUnits,
                    member => member.Id,
                    unit => unit.ResidentFamilyMemberId,
                    (member, unit) => new ResidentUserDropdownItem
                    {
                        Id = member.Id,
                        Name = $"{member.FirstName} {member.LastName}".Trim(),
                        Type = "FamilyMember",
                        ResidentMasterId = member.ResidentMasterId,
                        ResidentFamilyMemberId = member.Id
                    })
                .ToListAsync();

            var combined = residentItems
                .Concat(memberItems)
                .GroupBy(item => new { item.Type, item.Id })
                .Select(group => group.First())
                .OrderBy(item => item.Name)
                .ToList();

            return combined;
        }
    }
}
