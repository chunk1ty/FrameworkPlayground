using System;
using System.Collections.Generic;

namespace SamuraiApp.Domain
{
    public class Battle
    {
        public int BattleId { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<Samurai> Samurais { get; set; } = new();

        // use this property when LL is enabled
        // public virtual List<Samurai> Samurais { get; set; } = new List<Samurai>();
    }
}
