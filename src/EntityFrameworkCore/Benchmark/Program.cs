using Benchmark.Battles;

namespace Benchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // CatsDbContextSeeder.Seed();

            TooManyQueriesBattle.Fight();
        }
    }
}
