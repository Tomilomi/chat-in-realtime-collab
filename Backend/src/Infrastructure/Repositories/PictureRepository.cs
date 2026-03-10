using Application.Interfaces.Picture;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PictureRepository : IPictureRepository
{
    private readonly AppDbContext _context;
    
    public PictureRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Picture?> GetByIdAsync(Guid id)
    {
        return await _context.Pictures.FindAsync(id);
    }

    public async Task<Picture?> GetDefaultAsync()
    {
        return await _context.Pictures.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Picture>> GetAllAsync()
    {
        return await _context.Pictures.ToListAsync();
    }
}