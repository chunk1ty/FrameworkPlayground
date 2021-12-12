using System.Collections.Generic;

namespace Benchmark.Entities
{
    public class Owner
    {
        public int Id { get; set; }

        public string Name { get; set; }

        // public byte[] ProfilePicture { get; set; }

        // virtual because of Lazy loading
        public ICollection<Cat> Cats { get; set; } = new HashSet<Cat>();
    }
}
