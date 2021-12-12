using System.Linq;
using System.Threading.Tasks;
using Benchmark.Data;
using Benchmark.Entities;
using BenchmarkDotNet.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static Dapper.SqlMapper;

//|                   Method |     Mean |    Error |   StdDev |
//|------------------------- |---------:|---------:|---------:|
//|   EfMultipleRowsWithLINQ | 17.15 ms | 0.333 ms | 0.370 ms |
//| EfMultipleRowsWithRawSQL | 23.48 ms | 0.227 ms | 0.178 ms |
//|       DapperMultipleRows | 23.05 ms | 0.442 ms | 0.473 ms |
namespace Benchmark.Queries
{
    [MemoryDiagnoser]
    public class CatsDeleteQuery
    {
        //delete multiple rows
        // cats with age 4 = 1000 rows
        // cats with age 5 = 1000 rows
        // cats with age 7 = 1000 rows
        public async Task Execute()
        {
            await EfMultipleRowsWithLINQ();

            // EF Core Makes 2 Queries - One Read and One Delete
            using (var db = new CatsDbContext())
            {
                var cat = await db.Cats.FindAsync(1);

                db.Remove(cat);
                await db.SaveChangesAsync();
                
            }

            // EF Core Makes One Query - Only Delete
            using (var db = new CatsDbContext())
            {
                var cat = new Cat { Id = 2 };

                db.Remove(cat);

                await db.SaveChangesAsync();
            }
        }

        [Benchmark(Description = "EfMultipleRowsWithLINQ")]
        public async Task EfMultipleRowsWithLINQ()
        {
            using (var db = new CatsDbContext())
            {
                var catsToDelete = await db.Cats.Where(c => c.Age == 9)
                    .Select(c => c.Id)
                    .ToListAsync();

                db.RemoveRange(catsToDelete.Select(id => new Cat { Id = id }));

                await db.SaveChangesAsync();
            }

            // ...

            //DELETE FROM[Cats]
            //WHERE[Id] = @p33;
            //SELECT @@ROWCOUNT;

            //DELETE FROM[Cats]
            //WHERE[Id] = @p34;
            //SELECT @@ROWCOUNT;

            //DELETE FROM[Cats]
            //WHERE[Id] = @p35;
            //SELECT @@ROWCOUNT;

            // ...
        }

        [Benchmark(Description = "EfMultipleRowsWithRawSQL")]
        public async Task EfMultipleRowsWithRawSQL()
        {
            using (var db = new CatsDbContext())
            {
                await db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM Cats WHERE Age = {5}");
            }
        }

        [Benchmark(Description = "DapperMultipleRows")]
        public async Task DapperMultipleRows()
        {
            using var connection = new SqlConnection(ConnectionString.DefaultConnection);

            await connection.ExecuteAsync("DELETE FROM Cats WHERE Age = @Id", new { Id = 7 });
        }
    }
}
