using System.Security.Cryptography.X509Certificates;
using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

public class AuctionDbContext : DbContext
{
    public AuctionDbContext(DbContextOptions options) : base(options)
    {
        
    }
    

    public DbSet<Auction> Auctions {get; set;}
}