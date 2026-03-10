using Domain.Entities;

namespace Application.Interfaces.Pictures;

public interface IPictureRepository
{
    Task<Picture?> GetByIdAsync(Guid id);

    Task<IEnumerable<Picture>> GetAllAsync();

    Task<Picture?> GetDefaultAsync();
}