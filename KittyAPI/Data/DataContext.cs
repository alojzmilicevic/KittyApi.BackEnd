namespace KittyAPI.Data;

using KittyAPI.Models;
using Microsoft.EntityFrameworkCore;


public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Stream> Streams { get; set; }
    public DbSet<StreamUser> StreamUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StreamUser>().HasKey(e => new { e.UserId, e.StreamId });

        modelBuilder.Entity<StreamUser>()
            .HasOne<User>(sc => sc.User)
            .WithMany(s => s.Streams)
            .HasForeignKey(sc => sc.UserId);


        modelBuilder.Entity<StreamUser>()
            .HasOne<Stream>(sc => sc.Stream)
            .WithMany(s => s.Participants)
            .HasForeignKey(sc => sc.StreamId);
    }
}