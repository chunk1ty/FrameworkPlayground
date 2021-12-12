using System;
using System.Linq;
using System.Threading.Tasks;
using Benchmark.Data;
using Benchmark.Entities;
using BenchmarkDotNet.Attributes;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

//|                          Method |     Mean |   Error |   StdDev |      Gen 0 |     Gen 1 |     Gen 2 | Allocated |
//|-------------------------------- |---------:|--------:|---------:|-----------:|----------:|----------:|----------:|
//| EfUsingIncludeWithoutNoTracking | 536.3 ms | 8.81 ms |  7.81 ms | 12000.0000 | 4000.0000 | 1000.0000 |     71 MB |
//|                  EfUsingInclude | 324.7 ms | 6.42 ms |  8.99 ms |  8000.0000 | 2000.0000 |         - |     51 MB |
//|          EfUsingFilteredInclude | 437.8 ms | 7.15 ms |  5.97 ms |  8000.0000 | 2000.0000 |         - |     51 MB |
//|               EfCoreUsingSelect | 350.2 ms | 6.39 ms |  7.84 ms |  3000.0000 | 1000.0000 |         - |     23 MB |
//|                 EfCoreUsingJoin | 282.3 ms | 4.99 ms | 13.14 ms |  3000.0000 | 1000.0000 |         - |     23 MB |
//|                    EFCoreRawSql | 278.9 ms | 5.42 ms |  8.59 ms |  4000.0000 | 1000.0000 |         - |     23 MB |
//|              DapperWithLeftJoin | 272.2 ms | 2.35 ms |  1.96 ms |  2000.0000 |  500.0000 |         - |     13 MB |
//|             DapperWithInnerJoin | 263.9 ms | 5.25 ms |  9.47 ms |  2000.0000 |  500.0000 |         - |     13 MB |
namespace Benchmark.Queries
{
    [MemoryDiagnoser]
    public class CatsCountQuery
    {
        public async Task Execute()
        {
            //return count of cats which name contains "1" and cat's owner name contains "1"

            // cats rows - 100000
            // owners rows - 10000

            await EfUsingIncludeWithoutNoTracking();
            await EfUsingInclude();
            await EfUsingFilteredInclude();
            await EfCoreUsingSelect();
            await EfCoreUsingJoin();
            await EFCoreRawSql();
            await DapperWithLeftJoin();
            await DapperWithInnerJoin();
        }

        [Benchmark(Description = "EfUsingIncludeWithoutNoTracking")]
        public async Task EfUsingIncludeWithoutNoTracking()
        {
            using (var db = new CatsDbContext())
            {
                var owners = await db.Owners.Where(o => o.Name.Contains("1"))
                                            .Include(o => o.Cats)
                                            .ToListAsync();

                var total = 0;

                foreach (var owner in owners)
                {
                    total += owner.Cats.Count(c => c.Name.Contains("1"));
                }
                Console.WriteLine($"EF Core Include: {total} Results");
            }

            //SELECT[o].[Id], [o].[Name], [c].[Id], [c].[Age], [c].[BirthDate], [c].[Color], [c].[Name], [c].[OwnerId]
            //FROM[Owners] AS[o]
            //LEFT JOIN[Cats] AS[c] ON[o].[Id] = [c].[OwnerId]
            //WHERE[o].[Name] LIKE N'%1%'
            //ORDER BY[o].[Id], [c].[Id]
        }

        [Benchmark(Description = "EfUsingInclude")]
        public async Task EfUsingInclude()
        {
            using (var db = new CatsDbContext())
            {
                var owners = await db.Owners.Where(o => o.Name.Contains("1"))
                    .Include(o => o.Cats)
                    .AsNoTracking()
                    .ToListAsync();

                var total = 0;

                foreach (var owner in owners)
                {
                    total += owner.Cats.Count(c => c.Name.Contains("1"));
                }

                Console.WriteLine($"EF Core Include: {total} Results");
            }

            //SELECT[o].[Id], [o].[Name], [c].[Id], [c].[Age], [c].[BirthDate], [c].[Color], [c].[Name], [c].[OwnerId]
            //FROM[Owners] AS[o]
            //LEFT JOIN[Cats] AS[c] ON[o].[Id] = [c].[OwnerId]
            //WHERE[o].[Name] LIKE N'%1%'
            //ORDER BY[o].[Id], [c].[Id]
        }

        [Benchmark(Description = "EfUsingFilteredInclude")]
        public async Task EfUsingFilteredInclude()
        {
            using (var db = new CatsDbContext())
            {
                var owners = await db.Owners.Where(o => o.Name.Contains("1"))
                    .Include(o => o.Cats.Where(c => c.Name.Contains("1")))
                    .AsNoTracking()
                    .ToListAsync();

                Console.WriteLine($"EfUsingFilteredInclude: {owners.SelectMany(o => o.Cats).Count()} Results");
            }

            //SELECT[o].[Id], [o].[Name], [t].[Id], [t].[Age], [t].[BirthDate], [t].[Color], [t].[Name], [t].[OwnerId]
            //FROM[Owners] AS[o]
            //LEFT JOIN(
            //    SELECT[c].[Id], [c].[Age], [c].[BirthDate], [c].[Color], [c].[Name], [c].[OwnerId]
            //    FROM [Cats] AS [c]
            //    WHERE [c].[Name] LIKE N'%1%') AS[t] ON[o].[Id] = [t].[OwnerId]
            //WHERE[o].[Name] LIKE N'%1%'
            //ORDER BY[o].[Id], [t].[Id]
        }

        [Benchmark(Description = "EfCoreUsingSelect")]
        public async Task EfCoreUsingSelect()
        {
            using (var db = new CatsDbContext())
            {
                var owners = await db.Owners.Where(o => o.Name.Contains("1"))
                    .Select(o => new
                    {
                        Cats = o.Cats.Where(c => c.Name.Contains("1"))
                    })
                    .AsNoTracking()
                    .ToListAsync();

                Console.WriteLine($"EF Core Select: {owners.SelectMany(o => o.Cats).Count()} Results");
            }

            //SELECT[o].[Id], [t].[Id], [t].[Age], [t].[BirthDate], [t].[Color], [t].[Name], [t].[OwnerId]
            //FROM[Owners] AS[o]
            //LEFT JOIN(
            //    SELECT[c].[Id], [c].[Age], [c].[BirthDate], [c].[Color], [c].[Name], [c].[OwnerId]
            //    FROM [Cats] AS [c]
            //    WHERE [c].[Name] LIKE N'%1%') AS[t] ON[o].[Id] = [t].[OwnerId]
            //WHERE[o].[Name] LIKE N'%1%'
            //ORDER BY[o].[Id], [t].[Id]
        }

        [Benchmark(Description = "EfCoreUsingJoin")]
        public async Task EfCoreUsingJoin()
        {
            using (var db = new CatsDbContext())
            {
                var cats = await db.Owners.Where(o => o.Name.Contains("1"))
                                          .Join(db.Cats.Where(c => c.Name.Contains("1")),
                                              o => o.Id,
                                              c => c.OwnerId, (o, c) => c)
                                          .AsNoTracking()
                                          .ToListAsync();

                Console.WriteLine($"EF Core Join:  {cats.Count} Results");
            }

            //SELECT[t].[Id], [t].[Age], [t].[BirthDate], [t].[Color], [t].[Name], [t].[OwnerId]
            //FROM[Owners] AS[o]
            //INNER JOIN(
            //    SELECT[c].[Id], [c].[Age], [c].[BirthDate], [c].[Color], [c].[Name], [c].[OwnerId]
            //    FROM [Cats] AS [c]
            //    WHERE [c].[Name] LIKE N'%1%') AS[t] ON[o].[Id] = [t].[OwnerId]
            //WHERE[o].[Name] LIKE N'%1%'
        }

        [Benchmark(Description = "EFCoreRawSql")]
        public async Task EFCoreRawSql()
        {
            using (var db = new CatsDbContext())
            {
                var cats = await db.Cats
                    .FromSqlRaw(
                        @"SELECT
                         [c_1].[Id],
                         [c_1].[Name],
                         [c_1].[Age],
                         [c_1].[BirthDate],
                         [c_1].[Color],
                         [c_1].[OwnerId]
                        FROM [Owners] [o]
                        INNER JOIN [Cats] [c_1] ON [o].[Id] = [c_1].[OwnerId]
                        WHERE [c_1].[Name] LIKE N'%1%' AND [o].[Name] LIKE N'%1%'")
                    .AsNoTracking()
                    .ToListAsync();

                Console.WriteLine($"EF Core Raw SQL Query: - {cats.Count} Results");
            }
        }

        [Benchmark(Description = "DapperWithLeftJoin")]
        public async Task DapperWithLeftJoin()
        {
            using (var connection = new SqlConnection(ConnectionString.DefaultConnection))
            {
                var cats = await connection.QueryAsync<Cat>(
                    @"SELECT [o].[Id], [t].[Id], [t].[Age], [t].[BirthDate], [t].[Color], [t].[Name], [t].[OwnerId]
                    FROM[Owners] AS[o]
                    LEFT JOIN(
                        SELECT[c].[Id], [c].[Age], [c].[BirthDate], [c].[Color], [c].[Name], [c].[OwnerId]
                        FROM[Cats] AS[c]
                        WHERE CHARINDEX(N'1', [c].[Name]) > 0) AS[t] ON[o].[Id] = [t].[OwnerId]
                    WHERE CHARINDEX(N'1', [o].[Name]) > 0
                    ORDER BY[o].[Id], [t].[Id]");

                Console.WriteLine($"Dapper (EF Left Join): {cats.Count()} Results");
            }
        }

        [Benchmark(Description = "DapperWithInnerJoin")]
        public async Task DapperWithInnerJoin()
        {
            using (var connection = new SqlConnection(ConnectionString.DefaultConnection))
            {
                var cats = await connection.QueryAsync<Cat>(
                    @"SELECT[t].[Id], [t].[Age], [t].[BirthDate], [t].[Color], [t].[Name], [t].[OwnerId]
                    FROM[Owners] AS[o]
                    INNER JOIN(
                        SELECT[c].[Id], [c].[Age], [c].[BirthDate], [c].[Color], [c].[Name], [c].[OwnerId]
                        FROM[Cats] AS[c]
                        WHERE[c].[Name] LIKE N'%1%') AS[t] ON[o].[Id] = [t].[OwnerId]
                    WHERE[o].[Name] LIKE N'%1%'");

                Console.WriteLine($"Dapper (EF Inner Join): {cats.Count()} Results");
            }
        }
    }
}

