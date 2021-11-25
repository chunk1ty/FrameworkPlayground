namespace SamuraiApp.Domain
{
    public class Quote
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public Samurai Samurai { get; set; }
        public int SamuraiId { get; set; }

        // use this property when LL is enabled
        // public virtual Samurai Samurai { get; set; }
    }
}
