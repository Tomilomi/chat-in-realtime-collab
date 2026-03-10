using Application.Interfaces.Pictures;
using Domain.Entities;

namespace Application.Services;

public class PictureService : IPictureService
{
    private readonly IPictureRepository _pictureRepository;

    public PictureService(IPictureRepository pictureRepository)
    {
        _pictureRepository = pictureRepository;
    }

    public async Task<IEnumerable<Picture>> GetAllAsync()
    {
        return await _pictureRepository.GetAllAsync();
    }
}