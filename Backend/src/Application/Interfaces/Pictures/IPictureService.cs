using Domain.Entities;

namespace Application.Interfaces.Pictures;

public interface IPictureService
{
    Task<IEnumerable<Picture>> GetAllAsync();
}