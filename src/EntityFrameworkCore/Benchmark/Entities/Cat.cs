using System;

namespace Benchmark.Entities
{
    public class Cat
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public DateTime BirthDate { get; set; }

        public string Color { get; set; }

        public int OwnerId { get; set; }

        // virtual because of Lazy loading
        public Owner Owner { get; set; }
    }
}
