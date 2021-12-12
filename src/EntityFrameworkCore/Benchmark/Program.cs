using Benchmark.Data;
using Benchmark.Problems;
using BenchmarkDotNet.Running;

namespace Benchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // CatsDbContextSeeder.Seed();
            // CatsDbContextSeeder.DeleteAndSeed();

            // BenchmarkRunner.Run(typeof(Program).Assembly);

            //var catsCountQuery = new CatsCountQuery();
            //catsCountQuery.Execute().GetAwaiter().GetResult();
            //var catsDeleteQuery = new CatsDeleteQuery();
            //catsDeleteQuery.Execute().GetAwaiter().GetResult();

            // NPlus1Problem.Execute();
        }
    }
}
