using System.Collections.Generic;

namespace SamuraiApp.Domain
{
    public class Samurai
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Quote> Quotes { get; set; } = new();

        public List<Battle> Battles { get; set; } = new();

        public Horse Horse { get; set; }

        // use these properties when LL is enabled
        //public virtual List<Quote> Quotes { get; set; } = new List<Quote>();

        //public virtual List<Battle> Battles { get; set; } = new List<Battle>();

        //public virtual Horse Horse { get; set; }
    }
}
