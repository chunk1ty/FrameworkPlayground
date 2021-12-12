using Benchmark.Battles;
using Benchmark.Problems;
using BenchmarkDotNet.Running;

namespace Benchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // CatsDbContextSeeder.Seed();

            // NPlus1Problem.Execute();

            BenchmarkRunner.Run(typeof(Program).Assembly);

            //var catsCountQuery = new CatsCountQuery();
            //catsCountQuery.Execute().GetAwaiter().GetResult();
        }
    }
}
