using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookPortal.Api.Data;
public class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
    public LibraryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();
        optionsBuilder.UseSqlite("Data Source=library.db");

        return new LibraryDbContext(optionsBuilder.Options);
    }
}

public sealed class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public DbSet<BookEntity> Books => Set<BookEntity>();
    public DbSet<LoanEntity> Loans => Set<LoanEntity>();

    public DbSet<User> Users => Set<User>();
    public DbSet<Purchase> Purchases => Set<Purchase>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookEntity>(entity =>
        {
            entity.ToTable("Books");
            entity.HasKey(book => book.Id);
            entity.Property(book => book.Title).HasMaxLength(240).IsRequired();
            entity.Property(book => book.Author).HasMaxLength(180).IsRequired();
            entity.Property(book => book.Category).HasMaxLength(120).IsRequired();
            entity.Property(book => book.Format).HasMaxLength(80).IsRequired();
            entity.Property(book => book.Language).HasMaxLength(80).IsRequired();
            entity.Property(book => book.TagsJson).HasColumnType("TEXT").IsRequired();
            entity.HasIndex(book => book.Category);
            entity.HasIndex(book => book.Title);
        });

        modelBuilder.Entity<LoanEntity>(entity =>
        {
            entity.ToTable("Loans");
            entity.HasKey(loan => loan.Id);
            entity.Property(l => l.UserId)
                .IsRequired();
            entity.Property(l => l.BookId)
                .IsRequired();
            entity.HasIndex(l => l.UserId);
            entity.HasOne<BookEntity>()
                .WithMany()
                .HasForeignKey(loan => loan.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Name).HasMaxLength(150).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(150).IsRequired();
            entity.Property(user => user.Password).HasMaxLength(200).IsRequired();

            entity.Property(user => user.Cpf)
                .HasMaxLength(11)
                .IsRequired();

            entity.HasIndex(user => user.Email).IsUnique();
            entity.HasIndex(user => user.Cpf).IsUnique();
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.ToTable("Purchases");
            entity.HasKey(p => p.Id);

            entity.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Book)
                .WithMany()
                .HasForeignKey(p => p.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

