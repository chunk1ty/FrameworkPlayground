using Benchmark.Battles;
using Benchmark.Data;
using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Benchmark.Problems
{
    internal class NPlus1Problem
    {
        public static void Execute()
        {
            // Problem - N + 1 query
            //EfNPlus1Problem1();
            //EfCoreNPlus1Problem2(lazyLoadingEnabled: false);
            //EfCoreNPlus1Problem3(lazyLoadingEnabled: false);
        }

        private static void EfNPlus1Problem1()
        {
            var stopWatch = Stopwatch.StartNew();

            using (var db = new CatsDbContext())
            {
                var owners = db.Owners.Where(o => o.Name.Contains("1")).ToList();

                // Solution
                // var owners = db.Owners.Where(o => o.Name.Contains("1")).Include(o => o.Cats).ToList();

                var total = 0;

                foreach (var owner in owners)
                {
                    var cats = db.Cats.Where(c => c.OwnerId == owner.Id && c.Name.Contains("1")).ToList();

                    total += cats.Count;
                }

                Console.WriteLine($"EF Core N+1: {stopWatch.Elapsed} - {total} Results");

                // EF Core N+1: 00:00:08.5868066 - 34400 Results
            }

            // Solution 
        }

        private static void EfCoreNPlus1Problem2(bool lazyLoadingEnabled)
        {
            var stopWatch = Stopwatch.StartNew();

            using (var db = new CatsDbContext(lazyLoadingEnabled))
            {
                var oldCats = db.Cats.Where(o => o.Name.Contains("1")).ToList();

                foreach (var oldCat in oldCats)
                {
                    // In case of lazy loading enabled
                    Console.WriteLine($"{oldCat.Name} - {oldCat.Age} - {oldCat.Owner.Name}");
                }

                Console.WriteLine($"EF Core N+1 (problem 2): {stopWatch.Elapsed}.");
                // LL = true  EF Core N+1 (problem 2): 00:00:02.9167553.
                // LL = false EF Core N+1 (problem 2): 00:00:01.6231336.
            }
        }

        private static void EfCoreNPlus1Problem3(bool lazyLoadingEnabled)
        {
            var stopWatch = Stopwatch.StartNew();

            using var db = new CatsDbContext(lazyLoadingEnabled);

            var oldCats = db.Cats.Where(o => o.Name.Contains("1"))
                .Select(c => new OldCatResult
                {
                    Name = c.Name,
                    Age = c.Age,
                    Owner = c.Owner.Name
                })
                .ToList();

            foreach (var oldCat in oldCats)
            {
                // In case of lazy loading enabled
                Console.WriteLine($"{oldCat.Name} - {oldCat.Age} - {oldCat.Owner}");
            }
            Console.WriteLine($"EF Core N+1 (problem 3): {stopWatch.Elapsed}.");
            // LL = true  EF Core N+1 (problem 3): 00:00:07.1100336.
            // LL = false EF Core N+1 (problem 3): 00:00:07.0764866.
        }
    }
}
