using Microsoft.EntityFrameworkCore;
using Shoplio.ConsoleApp.Data;
using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;

namespace Shoplio.ConsoleApp.Repositories;

public sealed class SqlReviewRepository : IReviewRepository
{
    private readonly ShoplioDbContext _context;

    public SqlReviewRepository(ShoplioDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Add(Review review)
    {
        if (review is null)
        {
            throw new ArgumentNullException(nameof(review));
        }

        _context.Reviews.Add(review);
        _context.SaveChanges();
    }

    public IReadOnlyList<Review> GetByProductId(int productId)
    {
        return _context.Reviews
            .AsNoTracking()
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }
}
