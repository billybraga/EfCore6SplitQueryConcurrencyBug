using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Design;

namespace EfCore6SplitQueryConcurrencyBug
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            return Create(args);
        }
        
        public static AppDbContext Create(string[] args)
        {
            var conStringBuilder = new SqlConnectionStringBuilder
            {
                Password = args.ElementAtOrDefault(0) ?? throw new Exception("provide sa password"),
                UserID = args.ElementAtOrDefault(1) ?? "sa",
                DataSource = args.ElementAtOrDefault(2) ?? "127.0.0.1,1433",
                InitialCatalog = "ef-row-number-bug-test-db",
            };

            var useWorkaround = args.Length > 1
                && args[1].Equals("true", StringComparison.OrdinalIgnoreCase);
            return new AppDbContext(conStringBuilder.ConnectionString, useWorkaround);
        }
    }
}