using Domain.Entities;

namespace Application.Interfaces.Picture;

public interface IPictureService
{
    Task<IEnumerable<Picture>> GetAllAsync();
}