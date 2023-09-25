using Microsoft.EntityFrameworkCore;

namespace YoutubeChannelAPI.Data;

public class YoutubeChannelContext : DbContext
{
    protected YoutubeChannelContext()
    {
    }

    public YoutubeChannelContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<YoutubeChannelEntity> YoutubeChannelEntities { get; set; }

    public virtual DbSet<YoutubeVideoEntity> YoutubeVideoEntities { get; set; }

    #region ModelCreate

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<YoutubeChannelEntity>(channel =>
        {
            channel.ToTable("Channels");
            channel.HasIndex(ch => ch.ChannelId).IsUnique();
            /*channel.Property(ch => ch.Id).ValueGeneratedOnAdd();
            channel.HasKey(ch => ch.Id);
            channel.HasMany<YoutubeVideoEntity>()
                .WithOne()
                .HasForeignKey(videoEntity => videoEntity.ChannelEntityId)
                .IsRequired();*/
        });
        modelBuilder.Entity<YoutubeVideoEntity>(video =>
        {
            video.ToTable("Videos");
            video.HasIndex(v => v.VideoId).IsUnique();
            /*video.Property(v => v.Id).ValueGeneratedOnAdd();
            video.HasKey(v => v.Id);*/
        });
    }

    #endregion
}