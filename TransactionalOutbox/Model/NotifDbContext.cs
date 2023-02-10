using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TransactionalOutbox.Model;

public class NotifDbContext : DbContext
{
    public NotifDbContext(DbContextOptions options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notif>().HasIndex(x => x.CreatedAt).IsClustered(false);
        base.OnModelCreating(modelBuilder);
    }

    public async Task<List<Notif>> TopUnsentNotifications(int batchSize)
    {
        var result = await Notifications.FromSqlRaw($"""
                        SELECT TOP {batchSize} * 
                        FROM notif 
                        WITH (READPAST, UPDLOCK) 
                        WHERE [Sent]= 0
                        ORDER BY ID DESC
                """)
                .ToListAsync();

        return result;
    }

    public async Task<int> MarkNotificationsAsSent(List<int> notificationIds)
    {
        return await Database.ExecuteSqlRawAsync($"""
                        UPDATE notif SET [Sent]=1 
                        WHERE id IN 
                        ({string.Join(",", notificationIds)})
                """);
    }

    public DbSet<Notif> Notifications { set; get; }
    public DbSet<Item> Items { set; get; }
}

[Table("Item")]
public class Item
{
    [Key]
    public int Id { set; get; }

    public string Value { set; get; }

    //
    // Summary:
    //  Stringifies the object as JSON
    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}

[Table("Notif")]
public class Notif
{
    [Key]
    public int Id { set; get; }

    public DateTime CreatedAt { set; get; }

    public bool Sent { set; get; }

    public string Event { set; get; }
}