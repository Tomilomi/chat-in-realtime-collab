using Domain.Entities;

namespace Application.Interfaces;

public interface IPictureRepository
{
    Task<Picture?> GetByIdAsync(Guid id);
    Task<Picture?> GetDefaultAsync();
}