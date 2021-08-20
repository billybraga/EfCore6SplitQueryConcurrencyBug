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
            var dbArgs = args.Where(x => !x.StartsWith("--")).ToArray();
            var conStringBuilder = new SqlConnectionStringBuilder
            {
                Password = dbArgs.ElementAtOrDefault(0) ?? throw new Exception("provide sa password"),
                UserID = dbArgs.ElementAtOrDefault(1) ?? "sa",
                DataSource = dbArgs.ElementAtOrDefault(2) ?? "127.0.0.1,1433",
                InitialCatalog = "ef-row-number-bug-test-db",
            };

            var useWorkaround = args.Any(x => x == "--workaround");
            return new AppDbContext(conStringBuilder.ConnectionString, useWorkaround);
        }
    }
}