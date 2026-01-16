using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGuestMasterService
    {
        Task<InsertResponseModel> CreateGuestAsync(GuestMasterAddEdit guest);
        Task<InsertResponseModel> UpdateGuestAsync(GuestMasterAddEdit guest);
        Task DeleteGuestAsync(long id);
        Task<IEnumerable<GuestMasterAddEdit>> GetAllGuestsAsync();
        Task<PaginatedList<GuestMasterList>> GetGuestsAsync(int pageIndex, int pageSize);
        Task<GuestMasterAddEdit> GetGuestByIdAsync(long id);
        Task<QrCodeDecryptionResponse> DecryptGuestQrCodeValueAsync(
        long guestId,
        CancellationToken ct = default);
        Task<IReadOnlyList<GuestMasterList>> GetGuestsByResidentAsync(long? residentMasterId, long? residentFamilyMemberId);
        Task<GuestMasterAddEdit?> GetGuestByQrIdAsync(string qrId, CancellationToken ct = default);
    }
}
