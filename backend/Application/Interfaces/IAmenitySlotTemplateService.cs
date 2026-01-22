using Domain.ViewModels;

namespace Application.Interfaces
{
    public interface IAmenitySlotTemplateService
    {
        Task<InsertResponseModel> CreateSlotTemplateAsync(AmenitySlotTemplateAddEdit template);
        Task<InsertResponseModel> UpdateSlotTemplateAsync(AmenitySlotTemplateAddEdit template);
        Task DeleteSlotTemplateAsync(long id);
        Task<AmenitySlotTemplateAddEdit?> GetSlotTemplateByIdAsync(long id);
        Task<PaginatedList<AmenitySlotTemplateList>> GetSlotTemplatesAsync(int pageIndex, int pageSize);
    }
}
