using Application.Interfaces;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WebAPI.Controller
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ResidentMasterController : ControllerBase
    {
        private readonly IResidentMasterService _residentMasterService;
        private readonly IWebHostEnvironment _env;
        private readonly IResidentDocumentService _documentService;
        private readonly IResidentProfilePhotoService _profilePhotoService;

        public ResidentMasterController(IResidentMasterService residentMasterService, IWebHostEnvironment env,
             IResidentDocumentService documentService,
             IResidentProfilePhotoService profilePhotoService)
        {
            _residentMasterService = residentMasterService;
            _env = env;
            _documentService = documentService;
            _profilePhotoService = profilePhotoService;

        }

        //[HttpPost("Create")]
        //public async Task<IActionResult> CreateAsync(ResidentMasterAddEdit resident)
        //{
        //    var response = await _residentMasterService.CreateResidentAsync(resident);
        //    return Ok(response);
        //}
        [HttpPost("Create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateFormAsync([FromForm] ResidentMasterAddEdit resident)
        {
            resident.IsActive = true;

            if (resident.ProfilePhotoFile != null)
            {
                resident.ProfilePhoto = await SaveProfilePhotoAsync(
                    resident.ProfilePhotoFile,
                    $"RES_{Guid.NewGuid()}"
                );
            }

            if (resident.FamilyMembers != null)
            {
                foreach (var member in resident.FamilyMembers)
                {
                    if (member.IsEmpty())
                    {
                        continue;
                    }

                    member.IsActive = true;

                    if (member.ProfilePhotoFile != null)
                    {
                        member.ProfilePhoto = await SaveFamilyMemberPhotoAsync(
                            member.ProfilePhotoFile,
                            $"FM_{Guid.NewGuid()}"
                        );
                    }
                }
            }

            var response = await _residentMasterService.CreateResidentAsync(resident);

            if (response.Id <= 0)
                return StatusCode(500, response);

            if (resident.Documents != null && resident.Documents.Any())
            {
                var userId = 1;
                var documents = await _documentService.SaveDocumentsAsync(
                    response.Id,
                    resident.Documents,
                    userId,
                    _env.WebRootPath
                );
            }

            return Ok(response);
        }
        [HttpPost("CreateResident")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateAsync([FromBody] ResidentMasterCreateRequestJson request)
        {
            // Map JSON DTO -> your existing service model
            var resident = new ResidentMasterAddEdit
            {
                ParentFirstName = request.ParentFirstName,
                ParentLastName = request.ParentLastName,
                Email = request.Email,
                Mobile = request.Mobile,
                CountryCode = request.CountryCode,
                FaceId = request.FaceId,
                FingerId = request.FingerId,
                CardId = request.CardId,
                QrId = request.QrId,
                Password = request.Password,
                IsResident = request.IsResident,
                IsActive = true,
                UnitIds = request.UnitIds ?? new List<long>(),

                // ✅ Parent Profile Photo path (ONLY Parent)
                ProfilePhoto = string.IsNullOrWhiteSpace(request.ProfilePhoto) ? null : request.ProfilePhoto.Trim(),

                // keep as-is (no file upload here)
                Documents = new List<IFormFile>(),

                FamilyMembers = request.FamilyMembers?.Select(fm => new ResidentFamilyMemberAddEdit
                {
                    FirstName = fm.FirstName,
                    LastName = fm.LastName,
                    Email = fm.Email,
                    Mobile = fm.Mobile,
                    FaceId = fm.FaceId,
                    FingerId = fm.FingerId,
                    CardId = fm.CardId,
                    QrId = fm.QrId,
                    IsResident = fm.IsResident,
                    IsActive = true,
                    UnitIds = fm.UnitIds ?? new List<long>(),

                    // ❌ not doing family member photo here (as per your requirement)
                    ProfilePhoto = null
                }).ToList() ?? new List<ResidentFamilyMemberAddEdit>()
            };

            var response = await _residentMasterService.CreateResidentAsync(resident);

            //var response = await _residentMasterService.CreateResidentAsync(resident);

            if (response.Id <= 0)
            {
                if (int.TryParse(response.Code, out var statusCode) && statusCode >= 400 && statusCode < 600)
                    return StatusCode(statusCode, response);

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }


            // ✅ Save document PATHS (Parent only)
            if (request.DocumentFilePaths != null && request.DocumentFilePaths.Any())
            {
                var userId = 1;
                var linkResult = await _documentService.SaveDocumentPathsAsync(response.Id, request.DocumentFilePaths, userId);

                if (linkResult.Conflicts.Any() || linkResult.NotFound.Any())
                {
                    var parts = new List<string>();

                    if (linkResult.Conflicts.Any())
                        parts.Add($"Some documents already linked to another resident: {string.Join(", ", linkResult.Conflicts)}");

                    if (linkResult.NotFound.Any())
                        parts.Add($"Some documents not found in DB: {string.Join(", ", linkResult.NotFound)}");

                    response.Message = $"{response.Message} | {string.Join(" | ", parts)}";
                }
            }


            return Ok(response);
        }
        [HttpPost("{residentId:long}/ProfilePhoto")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadResidentProfilePhoto(
     long residentId,
     [FromForm] UploadResidentProfilePhotoRequest request)
        {
            if (request?.File == null || request.File.Length == 0)
                return BadRequest("File is required.");

            var photoPath = await SaveProfilePhotoAsync(request.File, $"RES_{residentId}");

            var userId = 1;
            var ok = await _residentMasterService.UpdateResidentProfilePhotoAsync(residentId, photoPath, userId);
            if (!ok) return NotFound("Resident not found.");

            return Ok(new { residentId, profilePhoto = photoPath });
        }

        [HttpPost("{residentId:long}/Documents")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadResidentDocuments(long residentId, [FromForm] UploadResidentDocumentsRequest request)
        {
            if (request.Files == null || !request.Files.Any())
                return BadRequest("At least one file is required.");

            var userId = 1;

            await _documentService.SaveDocumentsAsync(
                residentId,
                request.Files,
                userId,
                _env.WebRootPath
            );

            return Ok(new { residentId, message = "Documents uploaded successfully." });
        }

        [HttpPost("Documents")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadResidentDocuments([FromForm] UploadResidentDocumentsRequest request)
        {
            if (request.Files == null || !request.Files.Any())
                return BadRequest("At least one file is required.");

            var userId = 1;

            // ✅ CAPTURE the returned saved documents
            var documents = await _documentService.SaveDocumentsAsync(
                null,
                request.Files,
                userId,
                _env.WebRootPath
            );

            return Ok(new
            {
                message = "Documents uploaded successfully.",
                filePaths = documents.Select(d => d.FilePath).ToList()
            });
        }


        [HttpPost("ProfilePhoto")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadResidentProfilePhoto([FromForm] UploadResidentProfilePhotoRequest request)
        {
            if (request?.File == null || request.File.Length == 0)
                return BadRequest("File is required.");

            var photoPath = await SaveProfilePhotoAsync(request.File, "RES_NOID");
            var userId = 1;

            var record = await _profilePhotoService.SaveProfilePhotoAsync(
                request.File.FileName,
                photoPath,
                request.File.ContentType,
                userId);

            return Ok(new { profilePhotoId = record.Id, filePath = record.FilePath });
        }
        [HttpPost("FamilyMembers/{familyMemberId:long}/ProfilePhoto")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFamilyMemberProfilePhoto(
    long familyMemberId,
    [FromForm] UploadFamilyMemberProfilePhotoRequest request)
        {
            if (request?.File == null || request.File.Length == 0)
                return BadRequest("File is required.");

            var photoPath = await SaveFamilyMemberPhotoAsync(request.File, $"FM_{familyMemberId}");

            var userId = 1;
            var ok = await _residentMasterService.UpdateFamilyMemberProfilePhotoAsync(familyMemberId, photoPath, userId);
            if (!ok) return NotFound("Family member not found.");

            return Ok(new { familyMemberId, profilePhoto = photoPath });
        }



        [HttpGet("List/paged")]
        public async Task<IActionResult> GetPagedAsync(int pageIndex, int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                return BadRequest("Page index and page size must be greater than zero.");
            }

            var residents = await _residentMasterService.GetResidentsAsync(pageIndex, pageSize);
            return Ok(residents);
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> GetByIdAsync(long id, bool includeFamilyMembers = true)
        {
            var resident = await _residentMasterService.GetResidentByIdAsync(id, includeFamilyMembers);
            if (resident == null) return NotFound();
            return Ok(resident);
        }
        //[HttpGet("GetById")]
        //public async Task<IActionResult> GetByIdAsync(long id)
        //{
        //    var resident = await _residentMasterService.GetResidentByIdAsync(id);
        //    if (resident == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(resident);
        //}
        [HttpGet("Details")]
        public async Task<IActionResult> GetDetailsAsync(long? residentId, long? residentFamilyMemberId)
        {
            if (residentId.HasValue == residentFamilyMemberId.HasValue)
            {
                return BadRequest("Provide either residentId or residentFamilyMemberId.");
            }

            var details = await _residentMasterService.GetResidentDetailsAsync(residentId, residentFamilyMemberId);
            if (details == null)
            {
                return NotFound();
            }

            return Ok(details);
        }
        [HttpPut("Details")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateDetailsAsync([FromBody] ResidentDetailUpdateRequest_Profile request)
        {
            if (request == null) return BadRequest("Request body is required.");

            var response = await _residentMasterService.UpdateResidentDetailsAsync(request);
            if (response.Id <= 0) return BadRequest(response);

            return Ok(response);
        }
        [HttpGet("{residentMasterId:long}/FamilyMembers")]
        public async Task<IActionResult> GetFamilyMembersByResidentId(long residentMasterId)
        {
            var members = await _residentMasterService.GetFamilyMembersByResidentIdAsync(residentMasterId);
            if (members == null)
            {
                return NotFound();
            }

            return Ok(members);
        }
        //[HttpPost("Update")]
        //public async Task<IActionResult> UpdateAsync(ResidentMasterAddEdit resident)
        //{
        //    var response = await _residentMasterService.UpdateResidentAsync(resident);
        //    return Ok(response);
        //}
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateAsync([FromForm] ResidentMasterAddEdit resident)
        {
            if (resident.ProfilePhotoFile != null)
            {
                resident.ProfilePhoto = await SaveProfilePhotoAsync(
                    resident.ProfilePhotoFile,
                    resident.Code
                );
            }
            if (resident.FamilyMembers != null)
            {
                foreach (var member in resident.FamilyMembers)
                {
                    if (member.ProfilePhotoFile != null)
                    {
                        member.ProfilePhoto = await SaveFamilyMemberPhotoAsync(
                            member.ProfilePhotoFile,
                            $"FM_{member.Id}"
                        );
                    }
                }
            }


            var response = await _residentMasterService.UpdateResidentAsync(resident);
            var userId = 1;

            if (resident.Documents != null && resident.Documents.Any())
            {
                await _documentService.SaveDocumentsAsync(
                    resident.Id,
                    resident.Documents,
                    userId,
                    _env.WebRootPath
                );
            }
            return Ok(response);
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            await _residentMasterService.DeleteResidentAsync(id);
            return NoContent();
        }
        private async Task<string> SaveProfilePhotoAsync(IFormFile file, string residentCode)
        {
            if (file == null || file.Length == 0)
                return null;

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var uploadPath = Path.Combine(
                webRoot,
                "uploads",
                "residents",
                "profile"
            );

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{residentCode}_{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/residents/profile/{fileName}";
        }

        [HttpGet("Documents")]
        public async Task<IActionResult> GetDocuments(long residentId)
        {
            var docs = await _documentService.GetDocumentsByResidentAsync(residentId);
            return Ok(docs);
        }

        // ------------------------------------------------------------------
        // DELETE SINGLE DOCUMENT (Soft Delete)
        // ------------------------------------------------------------------
        [HttpDelete("Documents/Delete")]
        public async Task<IActionResult> DeleteDocument(long documentId)
        {
            var userId = 1;
            await _documentService.DeleteDocumentAsync(documentId, userId);
            return NoContent();
        }
        private async Task<string> SaveFamilyMemberPhotoAsync(
    IFormFile file,
    string familyCode)
        {
            if (file == null || file.Length == 0)
                return null;

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var uploadPath = Path.Combine(
                webRoot,
                "uploads",
                "residents",
                "family"
            );

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{familyCode}_{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadPath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/residents/family/{fileName}";
        }
        [HttpGet("GetUsersByUnit")]
        public async Task<IActionResult> GetUsersByUnit(long unitId)
        {
            var users = await _residentMasterService.GetResidentUsersByUnitAsync(unitId);
            return Ok(users);
        }
    }
}
