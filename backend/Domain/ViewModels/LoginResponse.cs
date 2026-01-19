using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Runtime;

public class LoginResponse
{
    public LoginResponse()
    {
        this.Permissions = new List<string>();
        //this.Roles = new List<IdentityRole>();
        this.RolePermissions = new List<RolePermission>();
        this.UnitIds = new List<long>();
        this.Units = new List<UnitInfo>();
        this.Documents = new List<ResidentDocument>();
    }

    public long? Id { get; set; }
    public long? ResidentMasterId { get; set; }
    public long? ResidentFamilyMemberId { get; set; }
    public bool HasFace { get; set; }
    public bool HasFingerprint { get; set; }
    public DateTime? LastBiometricSyncUtc { get; set; }
    public List<long> UnitIds { get; set; }
    public List<UnitInfo> Units { get; set; }
    public string Token { get; set; }
    public string DisplayName { get; set; }
    public DateTime ExpireAt { get; set; }
    public string UserImageFilePath { get; set; }
    public string? ProfilePhoto { get; set; }
    public bool IsAdmin { get; set; }
    public string SignaturePath { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    //public List<IdentityRole> Roles { get; set; }
    public List<IdentityRole<long>> Role { get; set; }
    public List<string> Permissions { get; set; }
    public List<RolePermission> RolePermissions { get; set; }
    public List<ResidentDocument> Documents { get; set; }
    public User User { get; set; }
}
public class UnitInfo
{
    public long UnitId { get; set; }
    public string UnitName { get; set; }
}
public class RolePermission
{
    public long Id { get; set; }
    
    public long RoleId { get; set; }
    public long PermissionId { get; set; }
    public bool HasMasterAccess { get; set; }
    public string ActionName { get; set; }
}
