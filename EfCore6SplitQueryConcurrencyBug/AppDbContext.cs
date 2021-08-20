using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCore6SplitQueryConcurrencyBug
{
    public class AppDbContext : DbContext
    {
        private static readonly MemoryCacheExceptCommandCacheKey Cache = new();
        
        private readonly string connectionString;
        private readonly bool useConditionalMemoryCacheWorkaround;

        public DbSet<Parent> Parents { get; set; } = null!;
        public DbSet<Child> Children { get; set; } = null!;

        public AppDbContext(string connectionString, bool useConditionalMemoryCacheWorkaround)
            : base(new DbContextOptions<AppDbContext>())
        {
            this.connectionString = connectionString;
            this.useConditionalMemoryCacheWorkaround = useConditionalMemoryCacheWorkaround;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(
                    this.connectionString,
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                )
                .LogTo(
                    message =>
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(message);
                        Console.ResetColor();
                    },
                    LogLevel.Information
                );
            
            if (this.useConditionalMemoryCacheWorkaround)
            {
                optionsBuilder = optionsBuilder.UseMemoryCache(Cache);
            }
            
            base.OnConfiguring(optionsBuilder);
        }
    }
}