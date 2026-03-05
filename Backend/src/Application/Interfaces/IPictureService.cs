using Domain.Entities;

namespace Application.Interfaces;

public interface IPictureService
{
    Task<IEnumerable<Picture>> GetAllAsync();
}