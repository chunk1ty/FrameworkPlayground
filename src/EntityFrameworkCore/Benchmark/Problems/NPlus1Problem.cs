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
            //EfCoreNPlus1Problem2();
            //EfCoreNPlus1Problem3();
        }

        private static void EfNPlus1Problem1()
        {
            var stopWatch = Stopwatch.StartNew();

            using (var db = new CatsDbContext(enableLazyLoading: true))
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
            }
        }

        private static void EfCoreNPlus1Problem2()
        {
            var stopWatch = Stopwatch.StartNew();

            using (var db = new CatsDbContext(enableLazyLoading: true))
            {
                var oldCats = db.Cats.Where(o => o.Name.Contains("1")).ToList();

                // Solution
                //var oldCats = db.Cats.Include(c => c.Owner).Where(o => o.Name.Contains("1")).ToList();

                foreach (var oldCat in oldCats)
                {
                    // In case of lazy loading enabled
                    Console.WriteLine($"{oldCat.Name} - {oldCat.Age} - {oldCat.Owner.Name}");
                }

                Console.WriteLine($"EF Core N+1 (problem 2): {stopWatch.Elapsed}.");
            }
        }

        private static void EfCoreNPlus1Problem3()
        {
            var stopWatch = Stopwatch.StartNew();

            using var db = new CatsDbContext(enableLazyLoading: true);

            var oldCats = db.Cats.Include(c => c.Owner).Where(o => o.Name.Contains("1"))
                .Select(c => new
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
        }
    }
}
