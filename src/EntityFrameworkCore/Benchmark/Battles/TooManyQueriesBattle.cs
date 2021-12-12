using Benchmark.Data;
using Benchmark.Entities;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System;
using System.Linq;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Benchmark.Battles
{
    internal class TooManyQueriesBattle
    {
        public static void Fight()
        {
            Console.WriteLine("Too Many Queries Battle");

            // Problem - N + 1 query
            //EfNPlus1Problem1();
            //EfCoreNPlus1Problem2(lazyLoadingEnabled: false);
            //EfCoreNPlus1Problem3(lazyLoadingEnabled: false);

            // Solutions
            // N.B all queries are cached !!
            //EfUsingInclude();
            EfUsingFilteredInclude();
            //EfCoreUsingSelect();
            //EfCoreUsingJoin();
            //EFCoreRawSql();

            //DapperWithEfQuery();

            Console.WriteLine(new string('-', 50));
        }

        private static void EfUsingInclude()
        {
            #region cache query
            using (var db = new CatsDbContext())
            {
                var owners = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Include(o => o.Cats)
                    .ToList();

                var total = 0;

                foreach (var owner in owners)
                {
                    var cats = owner.Cats
                        .Where(c => c.Name.Contains("1"))
                        .ToList();

                    total += cats.Count;
                }
            }


            #endregion

            var stopWatch = Stopwatch.StartNew();

            using (var db = new CatsDbContext())
            {
                var owners = db.Owners.Where(o => o.Name.Contains("1")).Include(o => o.Cats).ToList();

                var total = 0;

                foreach (var owner in owners)
                {
                    var cats = owner.Cats.Where(c => c.Name.Contains("1")).ToList();

                    total += cats.Count;
                }

                Console.WriteLine($"EF Core Include: {stopWatch.Elapsed} - {total} Results");

                // EF Core Include: 00:00:00.5380735 - 34400 Results
            }
        }
        
        private static void EfUsingFilteredInclude()
        {
            #region cache query
            using (var db = new CatsDbContext())
            {
                var owners = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Include(o => o.Cats.Where(c => c.Name.Contains("1")))
                    .ToList();
            }


            #endregion

            var stopWatch = Stopwatch.StartNew();

            using (var db = new CatsDbContext())
            {
                var owners = db.Owners.Where(o => o.Name.Contains("1"))
                    .Include(o => o.Cats.Where(c => c.Name.Contains("1")))
                    .ToList();

                Console.WriteLine($"EF Core Filtered Include: {stopWatch.Elapsed} - {owners.SelectMany(o => o.Cats).Count()} Results");
            }
        }

        private static void EfCoreUsingSelect()
        {
            #region cache query
            using (var db = new CatsDbContext())
            {
                var owners = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Select(o => new
                    {
                        Cats = o.Cats.Where(c => c.Name.Contains("1"))
                    })
                    .ToList();
            }
            #endregion

            var stopWatch = Stopwatch.StartNew();

            using (var db = new CatsDbContext())
            {
                var owners = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Select(o => new
                    {
                        Cats = o.Cats.Where(c => c.Name.Contains("1"))
                    })
                    .ToList();

                Console.WriteLine(
                    $"EF Core Select: {stopWatch.Elapsed} - {owners.SelectMany(o => o.Cats).Count()} Results");
            }
        }

        private static void EfCoreUsingJoin()
        {
            #region cache query
            using (var db = new CatsDbContext())
            {
                var cats = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Join(db.Cats.Where(c => c.Name.Contains("1")),
                        o => o.Id,
                        c => c.OwnerId, (o, c) => c)
                    .ToList();
            }
            #endregion

            var stopWatch = Stopwatch.StartNew();

            // EF Core Using Join
            using (var db = new CatsDbContext())
            {
                var cats = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Join(db.Cats.Where(c => c.Name.Contains("1")),
                        o => o.Id,
                        c => c.OwnerId, (o, c) => c)
                    .ToList();

                Console.WriteLine($"EF Core Join: {stopWatch.Elapsed} - {cats.Count} Results");
            }
        }

        private static void EFCoreRawSql()
        {
            #region cache query
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .FromSqlRaw(
                        @"SELECT
                         [c_1].[Id],
                         [c_1].[Name],
                         [c_1].[Age],
                         [c_1].[BirthDate],
                         [c_1].[Color],
                         [c_1].[OwnerId]
                        FROM
                         [Owners] [o]
                          INNER JOIN [Cats] [c_1] ON [o].[Id] = [c_1].[OwnerId]
                        WHERE
                         [c_1].[Name] LIKE N'%1%' AND [o].[Name] LIKE N'%1%'")
                    .AsNoTracking()
                    .ToList();
            }

            #endregion

            var stopWatch = Stopwatch.StartNew();

            // EF Core Raw SQL Query
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .FromSqlRaw(
                        @"SELECT
                         [c_1].[Id],
                         [c_1].[Name],
                         [c_1].[Age],
                         [c_1].[BirthDate],
                         [c_1].[Color],
                         [c_1].[OwnerId]
                        FROM
                         [Owners] [o]
                          INNER JOIN [Cats] [c_1] ON [o].[Id] = [c_1].[OwnerId]
                        WHERE
                         [c_1].[Name] LIKE N'%1%' AND [o].[Name] LIKE N'%1%'")
                    .AsNoTracking()
                    .ToList();

                Console.WriteLine($"EF Core Raw SQL Query: {stopWatch.Elapsed} - {cats.Count} Results");
            }
        }

        private static void DapperWithEfQuery()
        {
            #region cached query
            using (var connection = new SqlConnection(ConnectionString.DefaultConnection))
            {
                var cats = connection.Query<Cat>(
                    @"SELECT [o].[Id], [t].[Id], [t].[Age], [t].[BirthDate], [t].[Color], [t].[Name], [t].[OwnerId]
                    FROM[Owners] AS[o]
                    LEFT JOIN(
                        SELECT[c].[Id], [c].[Age], [c].[BirthDate], [c].[Color], [c].[Name], [c].[OwnerId]
                    FROM[Cats] AS[c]
                    WHERE CHARINDEX(N'1', [c].[Name]) > 0
                        ) AS[t] ON[o].[Id] = [t].[OwnerId]
                    WHERE CHARINDEX(N'1', [o].[Name]) > 0
                    ORDER BY[o].[Id], [t].[Id]");

            }


            #endregion

            var stopWatch = Stopwatch.StartNew();

            // Dapper With EF Query
            using (var connection = new SqlConnection(ConnectionString.DefaultConnection))
            {
                var cats = connection.Query<Cat>(
                    @"SELECT [o].[Id], [t].[Id], [t].[Age], [t].[BirthDate], [t].[Color], [t].[Name], [t].[OwnerId]
                    FROM[Owners] AS[o]
                    LEFT JOIN(
                        SELECT[c].[Id], [c].[Age], [c].[BirthDate], [c].[Color], [c].[Name], [c].[OwnerId]
                    FROM[Cats] AS[c]
                    WHERE CHARINDEX(N'1', [c].[Name]) > 0
                        ) AS[t] ON[o].[Id] = [t].[OwnerId]
                    WHERE CHARINDEX(N'1', [o].[Name]) > 0
                    ORDER BY[o].[Id], [t].[Id]");

                Console.WriteLine($"Dapper (EF): {stopWatch.Elapsed} - {cats.Count()} Results");
            }
        }
    }

    public class OldCatResult
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public string Owner { get; set; }
    }
}

