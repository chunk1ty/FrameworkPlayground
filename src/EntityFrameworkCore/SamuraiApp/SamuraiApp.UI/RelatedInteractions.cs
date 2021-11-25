using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System.Collections.Generic;
using System.Linq;

namespace SamuraiApp.UI
{
    internal static class RelatedInteractions
    {
        private static SamuraiContext _context = new();

        public static void Interact()
        {
            //InsertNewSamuraiWithAQuote();
            //InsertNewSamuraiWithManyQuotes();
            //AddQuoteToExistingSamuraiWhileTracked();
            //AddQuoteToExistingSamuraiNotTracked(17);
            //Simpler_AddQuoteToExistingSamuraiNotTracked(17);
            //EagerLoadSamuraiWithQuotes();
            //ProjectSomeProperties();
            //ProjectSamuraisWithQuotes();
            //ExplicitLoadQuotes();
            //LazyLoadQuotes();
            //FiteringWithRelatedData();
            //ModifyingRelatedDataWhenTracked();
            //ModifyingRelatedDataWhenNotTracked();
            //AddingNewSamuraiToAnExistingBattle();
            //ReturnBattleWithSamurais();
            //ReturnAllBattlesWithSamurais();
            //AddAllSamuraisToAllBattles();
            //RemoveSamuraiFromABattle();
            //RemoveSamuraiFromABattleExplicit();
            //AddNewSamuraiWithHorse();
            //AddNewHorseToSamuraiUsingId();
            //AddNewHorseToSamuraiObject();
            //AddNewHorseToDisconnectedSamuraiObject();
            //ReplaceAHorse();
        }

        private static void InsertNewSamuraiWithAQuote()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote> { new Quote { Text = "I've come to save you" } }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();

            //Executed DbCommand(48ms) [Parameters=[@p0 = 'Kambei Shimada'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Samurais] ([Name])
            //VALUES(@p0);
            //SELECT[Id]
            //FROM[Samurais]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();

            //Executed DbCommand(5ms) [Parameters=[@p1 = '31', @p2 = 'I've come to save you' (Size = 4000)], CommandType='Text', CommandTimeout='30']
            //SET NOCOUNT ON;
            //INSERT INTO[Quotes] ([SamuraiId], [Text])
            //VALUES(@p1, @p2);
            //SELECT[Id]
            //FROM[Quotes]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();
        }

        private static void InsertNewSamuraiWithManyQuotes()
        {
            var samurai = new Samurai
            {
                Name = "Kyūzō",
                Quotes = new List<Quote>
                {
                    new Quote {Text = "Watch out for my sharp sword!"},
                    new Quote {Text="I told you to watch out for the sharp sword! Oh well!" }
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();

            //Executed DbCommand(37ms) [Parameters=[@p0 = 'Kyuzo'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Samurais] ([Name])
            //VALUES(@p0);
            //SELECT[Id]
            //FROM[Samurais]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();

            //Executed DbCommand(6ms) [Parameters=[@p0 = '33', @p1 = 'Watch out for my sharp sword!'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Quotes] ([SamuraiId], [Text])
            //VALUES(@p0, @p1);
            //SELECT[Id]
            //FROM[Quotes]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();

            //Executed DbCommand(2ms) [Parameters=[@p0 = '33', @p1 = 'I told you to watch out for the sharp sword! Oh well!'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Quotes] ([SamuraiId], [Text])
            //VALUES(@p0, @p1);
            //SELECT[Id]
            //FROM[Quotes]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();
        }

        private static void AddQuoteToExistingSamuraiWhileTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Quotes.Add(new Quote
            {
                Text = "I bet you're happy that I've saved you!"
            });
            _context.SaveChanges();

            //Executed DbCommand(27ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT TOP(1) [s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]

            //Executed DbCommand(40ms) [Parameters=[@p0 = '17', @p1 = 'I bet you're happy that I've saved you!'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Quotes] ([SamuraiId], [Text])
            //VALUES(@p0, @p1);
            //SELECT[Id]
            //FROM[Quotes]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();
        }

        private static void AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            samurai.Quotes.Add(new Quote
            {
                Text = "Now that I saved you, will you feed me dinner?"
            });
            using (var newContext = new SamuraiContext())
            {
                newContext.Samurais.Attach(samurai);
                newContext.SaveChanges();
            }

            //Executed DbCommand(42ms) [Parameters=[@__p_0 = '17'], CommandType = 'Text', CommandTimeout = '30']
            //SELECT TOP(1) [s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]
            //WHERE[s].[Id] = @__p_0

            //Executed DbCommand(3ms) [Parameters=[@p0 = '17', @p1 = 'Now that I saved you, will you feed me dinner?'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Quotes] ([SamuraiId], [Text])
            //VALUES(@p0, @p1);
            //SELECT[Id]
            //FROM[Quotes]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();
        }

        private static void Simpler_AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var quote = new Quote { Text = "Thanks for dinner!", SamuraiId = samuraiId };
            using var newContext = new SamuraiContext();
            newContext.Quotes.Add(quote);
            newContext.SaveChanges();

            //Executed DbCommand(75ms) [Parameters=[@p0 = '17', @p1 = 'Thanks for dinner!'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Quotes] ([SamuraiId], [Text])
            //VALUES(@p0, @p1);
            //SELECT[Id]
            //FROM[Quotes]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();
        }

        private static void EagerLoadSamuraiWithQuotes()
        {
            //var samuraiWithQuotes = _context.Samurais.Include(s => s.Quotes).ToList();

            //Executed DbCommand(34ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[s].[Id], [s].[Name], [q].[Id], [q].[SamuraiId], [q].[Text]
            //FROM[Samurais] AS[s]
            //LEFT JOIN[Quotes] AS[q] ON[s].[Id] = [q].[SamuraiId]
            //ORDER BY[s].[Id], [q].[Id]

            //var splitQuery = _context.Samurais.AsSplitQuery().Include(s => s.Quotes).ToList();

            //Executed DbCommand(28ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]
            //ORDER BY[s].[Id]

            //Executed DbCommand(5ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[q].[Id], [q].[SamuraiId], [q].[Text], [s].[Id]
            //FROM[Samurais] AS[s]
            //INNER JOIN[Quotes] AS[q] ON[s].[Id] = [q].[SamuraiId]
            //ORDER BY[s].[Id]

            //var filteredInclude = _context.Samurais.Include(s => s.Quotes.Where(q => q.Text.Contains("Thanks"))).ToList();

            //Executed DbCommand(71ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[s].[Id], [s].[Name], [t].[Id], [t].[SamuraiId], [t].[Text]
            //FROM[Samurais] AS[s]
            //LEFT JOIN (
            //    SELECT[q].[Id], [q].[SamuraiId], [q].[Text]
            //    FROM [Quotes] AS [q]
            //    WHERE [q].[Text] LIKE N'%Thanks%'
            //) AS[t] ON[s].[Id] = [t].[SamuraiId]
            //ORDER BY[s].[Id], [t].[Id]

            var filterPrimaryEntityWithInclude = _context.Samurais.Where(s => s.Name.Contains("Sampson"))
                                                                  .Include(s => s.Quotes)
                                                                  .FirstOrDefault();

            //SELECT[t].[Id], [t].[Name], [q].[Id], [q].[SamuraiId], [q].[Text]
            //FROM(
            //    SELECT TOP(1)[s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]
            //WHERE[s].[Name] LIKE N'%Sampson%'
            //    ) AS[t]
            //LEFT JOIN[Quotes] AS[q] ON[t].[Id] = [q].[SamuraiId]
            //ORDER BY[t].[Id], [q].[Id]
        }

        private static void ProjectSomeProperties()
        {
            var someProperties = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();

            //Executed DbCommand(52ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]

            var idAndNames = _context.Samurais.Select(s => new IdAndName(s.Id, s.Name)).ToList();

            //Executed DbCommand(6ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]
        }

        public struct IdAndName
        {
            public IdAndName(int id, string name)
            {
                Id = id;
                Name = name;
            }
            public int Id;
            public string Name;

        }

        private static void ProjectSamuraisWithQuotes()
        {
            // var somePropsWithQuotes = _context.Samurais.Select(s => new { s.Id, s.Name, NumberOfQuotes = s.Quotes.Count }).ToList();

            //Executed DbCommand(54ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[s].[Id], [s].[Name], (
            //    SELECT COUNT(*)
            //FROM[Quotes] AS[q]
            //WHERE[s].[Id] = [q].[SamuraiId]) AS[NumberOfQuotes]
            //FROM[Samurais] AS[s]

            //var somePropsWithQuotes = _context.Samurais.Select(s => new
            //{
            //    s.Id,
            //    s.Name,
            //    HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))
            //}).ToList();

            //Executed DbCommand(47ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[s].[Id], [s].[Name], [t].[Id], [t].[SamuraiId], [t].[Text]
            //FROM[Samurais] AS[s]
            //LEFT JOIN (
            //    SELECT[q].[Id], [q].[SamuraiId], [q].[Text]
            //    FROM [Quotes] AS [q]
            //    WHERE [q].[Text] LIKE N'%happy%'
            //) AS[t] ON[s].[Id] = [t].[SamuraiId]
            //ORDER BY[s].[Id], [t].[Id]

            var samuraisAndQuotes = _context.Samurais
            .Select(s => new
            {
                Samurai = s,
                HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))
            }).ToList();

            //Executed DbCommand(36ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[s].[Id], [s].[Name], [t].[Id], [t].[SamuraiId], [t].[Text]
            //FROM[Samurais] AS[s]
            //LEFT JOIN (
            //    SELECT[q].[Id], [q].[SamuraiId], [q].[Text]
            //    FROM [Quotes] AS [q]
            //    WHERE [q].[Text] LIKE N'%happy%'
            //) AS[t] ON[s].[Id] = [t].[SamuraiId]
            //ORDER BY[s].[Id], [t].[Id]
        }

        private static void ExplicitLoadQuotes()
        {
            //make sure there's a horse in the DB, then clear the context's change tracker
            _context.Set<Horse>().Add(new Horse { SamuraiId = 17, Name = "Mr. Ed" });
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            //-------------------
            var samurai = _context.Samurais.Find(17);
            _context.Entry(samurai).Collection(s => s.Quotes).Load();
            _context.Entry(samurai).Reference(s => s.Horse).Load();

            //Executed DbCommand(27ms) [Parameters=[@p0 = 'Mr. Ed'(Size = 4000), @p1 = '17'], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Horses] ([Name], [SamuraiId])
            //VALUES(@p0, @p1);
            //SELECT[Id]
            //FROM[Horses]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();

            //Executed DbCommand(3ms) [Parameters=[@__p_0 = '17'], CommandType = 'Text', CommandTimeout = '30']
            //SELECT TOP(1) [s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]
            //WHERE[s].[Id] = @__p_0

            //Executed DbCommand(3ms) [Parameters=[@__p_0 = '17'], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[q].[Id], [q].[SamuraiId], [q].[Text]
            //FROM[Quotes] AS[q]
            //WHERE[q].[SamuraiId] = @__p_0

            //Executed DbCommand(3ms) [Parameters=[@__p_0 = '17'], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[h].[Id], [h].[Name], [h].[SamuraiId]
            //FROM[Horses] AS[h]
            //WHERE[h].[SamuraiId] = @__p_0
        }

        private static void LazyLoadQuotes()
        {
            var samurai = _context.Samurais.Find(17);
            var quoteCount = samurai.Quotes.Count(); //won't work without LL setup

            //Executed DbCommand(39ms) [Parameters=[@__p_0 = '17'], CommandType = 'Text', CommandTimeout = '30']
            //SELECT TOP(1) [s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]
            //WHERE[s].[Id] = @__p_0

            //Executed DbCommand(4ms) [Parameters=[@__p_0 = '17'], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[q].[Id], [q].[SamuraiId], [q].[Text]
            //FROM[Quotes] AS[q]
            //WHERE[q].[SamuraiId] = @__p_0
        }

        private static void FiteringWithRelatedData()
        {
            _context.Samurais.Where(s => s.Quotes.Any(q => q.Text.Contains("happy"))).ToList();

            //Executed DbCommand(34ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]
            //WHERE EXISTS(
            //    SELECT 1
            //FROM[Quotes] AS[q]
            //WHERE([s].[Id] = [q].[SamuraiId]) AND([q].[Text] LIKE N'%happy%'))
        }

        private static void ModifyingRelatedDataWhenTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 17);
            // first deletes existing one and then insert new one
            samurai.Quotes[0].Text = "Did you hear that?";
            _context.Quotes.Remove(samurai.Quotes[2]);
            _context.SaveChanges();

            //Executed DbCommand(25ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[t].[Id], [t].[Name], [q].[Id], [q].[SamuraiId], [q].[Text]
            //FROM(
            //    SELECT TOP(1)[s].[Id], [s].[Name]
            //    FROM[Samurais] AS[s]
            //    WHERE[s].[Id] = 17
            //) AS[t]
            //LEFT JOIN[Quotes] AS[q] ON[t].[Id] = [q].[SamuraiId]
            //ORDER BY[t].[Id], [q].[Id]

            //Executed DbCommand(23ms) [Parameters=[@p0 = '9'], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //DELETE FROM[Quotes]
            //WHERE[Id] = @p0;
            //SELECT @@ROWCOUNT;

            //Executed DbCommand(5ms) [Parameters=[@p1 = '7', @p0 = 'Did you hear that?'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //UPDATE[Quotes] SET[Text] = @p0
            //WHERE[Id] = @p1;
            //SELECT @@ROWCOUNT;
        }

        private static void ModifyingRelatedDataWhenNotTracked()
        {
            //var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 17);
            //var quote = samurai.Quotes[0];
            //quote.Text += "Did you hear that again?";

            //using var newContext = new SamuraiContext();
            //// Note that Update will update quote text of all existing samurai quotes
            //// In order to avoid such behavior check below code.
            //newContext.Quotes.Update(quote);
            //newContext.SaveChanges();

            //Executed DbCommand(23ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[t].[Id], [t].[Name], [q].[Id], [q].[SamuraiId], [q].[Text]
            //FROM(
            //    SELECT TOP(1)[s].[Id], [s].[Name]
            //    FROM[Samurais] AS[s]
            //    WHERE[s].[Id] = 17
            //) AS[t]
            //LEFT JOIN[Quotes] AS[q] ON[t].[Id] = [q].[SamuraiId]
            //ORDER BY[t].[Id], [q].[Id]

            //Executed DbCommand(22ms) [Parameters=[@p2 = '7', @p0 = '17', @p1 = 'Did you hear that?Did you hear that again?'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //UPDATE[Quotes] SET[SamuraiId] = @p0, [Text] = @p1
            //WHERE[Id] = @p2;
            //SELECT @@ROWCOUNT;

            //Executed DbCommand(2ms) [Parameters=[@p2 = '8', @p0 = '17', @p1 = 'Now that I saved you, will you feed me dinner?'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //UPDATE[Quotes] SET[SamuraiId] = @p0, [Text] = @p1
            //WHERE[Id] = @p2;
            //SELECT @@ROWCOUNT;

            //Executed DbCommand(3ms) [Parameters=[@p1 = '17', @p0 = 'ShimadaSanSanSan'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //UPDATE[Samurais] SET[Name] = @p0
            //WHERE[Id] = @p1;
            //SELECT @@ROWCOUNT;


            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 17);
            var quote = samurai.Quotes[0];
            quote.Text += "Did you hear that again?";

            using var newContext = new SamuraiContext();
            newContext.Entry(quote).State = EntityState.Modified;
            newContext.SaveChanges();

            //Executed DbCommand(45ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[t].[Id], [t].[Name], [q].[Id], [q].[SamuraiId], [q].[Text]
            //FROM(
            //    SELECT TOP(1)[s].[Id], [s].[Name]
            //    FROM[Samurais] AS[s]
            //    WHERE[s].[Id] = 17
            //) AS[t]
            //LEFT JOIN[Quotes] AS[q] ON[t].[Id] = [q].[SamuraiId]
            //ORDER BY[t].[Id], [q].[Id]

            //Executed DbCommand(58ms) [Parameters=[@p2 = '7', @p0 = '17', @p1 = 'Did you hear that?Did you hear that again?Did you hear that again?'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //UPDATE[Quotes] SET[SamuraiId] = @p0, [Text] = @p1
            //WHERE[Id] = @p2;
            //SELECT @@ROWCOUNT;
        }

        private static void AddingNewSamuraiToAnExistingBattle()
        {
            var battle = _context.Battles.FirstOrDefault();
            battle.Samurais.Add(new Samurai { Name = "Takeda Shingen" });
            _context.SaveChanges();

            //Executed DbCommand(24ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT TOP(1) [b].[BattleId], [b].[EndDate], [b].[Name], [b].[StartDate]
            //FROM[Battles] AS[b]

            //Executed DbCommand(26ms) [Parameters=[@p0 = 'Takeda Shingen'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Samurais] ([Name])
            //VALUES(@p0);
            //SELECT[Id]
            //FROM[Samurais]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();

            //Executed DbCommand(10ms) [Parameters=[@p1 = '1', @p2 = '34'], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[BattleSamurai] ([BattleId], [SamuraiId])
            //VALUES(@p1, @p2);
            //SELECT[DateJoined]
            //FROM[BattleSamurai]
            //WHERE @@ROWCOUNT = 1 AND[BattleId] = @p1 AND[SamuraiId] = @p2;
        }

        private static void ReturnBattleWithSamurais()
        {
            _context.Battles.Include(b => b.Samurais).FirstOrDefault();

            //Executed DbCommand(43ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[t].[BattleId], [t].[EndDate], [t].[Name], [t].[StartDate], [t0].[BattleId], [t0].[SamuraiId], [t0].[DateJoined], [t0].[Id], [t0].[Name]
            //FROM(
            //    SELECT TOP(1)[b].[BattleId], [b].[EndDate], [b].[Name], [b].[StartDate]
            //    FROM[Battles] AS[b]
            //) AS[t]
            //LEFT JOIN(
            //    SELECT[b0].[BattleId], [b0].[SamuraiId], [b0].[DateJoined], [s].[Id], [s].[Name]
            //    FROM [BattleSamurai] AS [b0]
            //    INNER JOIN [Samurais] AS[s] ON [b0].[SamuraiId] = [s].[Id]
            //) AS[t0] ON[t].[BattleId] = [t0].[BattleId]
            //ORDER BY[t].[BattleId], [t0].[BattleId], [t0].[SamuraiId], [t0].[Id]
        }

        private static void ReturnAllBattlesWithSamurais()
        {
            _context.Battles.Include(b => b.Samurais).ToList();

            //Executed DbCommand(33ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[b].[BattleId], [b].[EndDate], [b].[Name], [b].[StartDate], [t].[BattleId], [t].[SamuraiId], [t].[DateJoined], [t].[Id], [t].[Name]
            //FROM[Battles] AS[b]
            //LEFT JOIN(
            //    SELECT[b0].[BattleId], [b0].[SamuraiId], [b0].[DateJoined], [s].[Id], [s].[Name]
            //    FROM [BattleSamurai] AS [b0]
            //    INNER JOIN [Samurais] AS[s] ON [b0].[SamuraiId] = [s].[Id]
            //) AS[t] ON[b].[BattleId] = [t].[BattleId]
            //ORDER BY[b].[BattleId], [t].[BattleId], [t].[SamuraiId], [t].[Id]
        }

        private static void AddAllSamuraisToAllBattles()
        {
            var allbattles = _context.Battles.Include(b => b.Samurais).ToList();
            var allSamurais = _context.Samurais.ToList();
            foreach (var battle in allbattles)
            {
                battle.Samurais.AddRange(allSamurais);
            }
            _context.SaveChanges();

            // Executed DbCommand(35ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //      SELECT[b].[BattleId], [b].[EndDate], [b].[Name], [b].[StartDate], [t].[BattleId], [t].[SamuraiId], [t].[DateJoined], [t].[Id], [t].[Name]
            //      FROM[Battles] AS[b]
            //      LEFT JOIN(
            //          SELECT[b0].[BattleId], [b0].[SamuraiId], [b0].[DateJoined], [s].[Id], [s].[Name]
            //          FROM [BattleSamurai] AS [b0]
            //          INNER JOIN [Samurais] AS[s] ON [b0].[SamuraiId] = [s].[Id]
            //      ) AS[t] ON[b].[BattleId] = [t].[BattleId]
            //      ORDER BY[b].[BattleId], [t].[BattleId], [t].[SamuraiId], [t].[Id]

            //      Executed DbCommand(5ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //      SELECT[s].[Id], [s].[Name]
            //      FROM[Samurais] AS[s]

            //      Executed DbCommand(66ms) [Parameters=[@p0 = '1', @p1 = '17', @p2 = '2', @p3 = '32', @p4 = '2', @p5 = '31', @p6 = '2', @p7 = '30', @p8 = '2', @p9 = '29', @p10 = '2', @p11 = '28', @p12 = '2', @p13 = '27', @p14 = '2', @p15 = '26', @p16 = '2', @p17 = '25', @p18 = '2', @p19 = '24', @p20 = '2', @p21 = '23', @p22 = '2', @p23 = '22', @p24 = '2', @p25 = '21', @p26 = '2', @p27 = '20', @p28 = '2', @p29 = '19', @p30 = '2', @p31 = '33', @p32 = '2', @p33 = '17', @p34 = '1', @p35 = '32', @p36 = '1', @p37 = '31', @p38 = '1', @p39 = '30', @p40 = '1', @p41 = '29', @p42 = '1', @p43 = '28', @p44 = '1', @p45 = '27', @p46 = '1', @p47 = '26', @p48 = '1', @p49 = '25', @p50 = '1', @p51 = '24', @p52 = '1', @p53 = '23', @p54 = '1', @p55 = '22', @p56 = '1', @p57 = '21', @p58 = '1', @p59 = '20', @p60 = '1', @p61 = '19', @p62 = '1', @p63 = '33', @p64 = '2', @p65 = '34'], CommandType = 'Text', CommandTimeout = '30']
            //      SET NOCOUNT ON;
            //            DECLARE @inserted0 TABLE([BattleId] int, [SamuraiId] int, [_Position] [int]);
            //            MERGE[BattleSamurai] USING(
            //           VALUES(@p0, @p1, 0),
            //           (@p2, @p3, 1),
            //           (@p4, @p5, 2),
            //           (@p6, @p7, 3),
            //           (@p8, @p9, 4),
            //           (@p10, @p11, 5),
            //           (@p12, @p13, 6),
            //           (@p14, @p15, 7),
            //           (@p16, @p17, 8),
            //           (@p18, @p19, 9),
            //           (@p20, @p21, 10),
            //           (@p22, @p23, 11),
            //           (@p24, @p25, 12),
            //           (@p26, @p27, 13),
            //           (@p28, @p29, 14),
            //           (@p30, @p31, 15),
            //           (@p32, @p33, 16),
            //           (@p34, @p35, 17),
            //           (@p36, @p37, 18),
            //           (@p38, @p39, 19),
            //           (@p40, @p41, 20),
            //           (@p42, @p43, 21),
            //           (@p44, @p45, 22),
            //           (@p46, @p47, 23),
            //           (@p48, @p49, 24),
            //           (@p50, @p51, 25),
            //           (@p52, @p53, 26),
            //           (@p54, @p55, 27),
            //           (@p56, @p57, 28),
            //           (@p58, @p59, 29),
            //           (@p60, @p61, 30),
            //           (@p62, @p63, 31),
            //           (@p64, @p65, 32)) AS i([BattleId], [SamuraiId], _Position) ON 1 = 0
            //      WHEN NOT MATCHED THEN
            //      INSERT([BattleId], [SamuraiId])
            //      VALUES(i.[BattleId], i.[SamuraiId])
            //      OUTPUT INSERTED.[BattleId], INSERTED.[SamuraiId], i._Position
            //      INTO @inserted0;

            //            SELECT[t].[DateJoined] FROM[BattleSamurai] t
            //          INNER JOIN @inserted0 i ON([t].[BattleId] = [i].[BattleId]) AND([t].[SamuraiId] = [i].[SamuraiId])
            //      ORDER BY[i].[_Position];
        }

        private static void RemoveSamuraiFromABattle()
        {
            var battleWithSamurai = _context.Battles.Include(b => b.Samurais.Where(s => s.Id == 17))
                                                    .Single(s => s.BattleId == 1);
            var samurai = battleWithSamurai.Samurais[0];
            battleWithSamurai.Samurais.Remove(samurai);
            _context.SaveChanges();

            //Executed DbCommand(24ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[t].[BattleId], [t].[EndDate], [t].[Name], [t].[StartDate], [t0].[BattleId], [t0].[SamuraiId], [t0].[DateJoined], [t0].[Id], [t0].[Name]
            //FROM(
            //    SELECT TOP(2)[b].[BattleId], [b].[EndDate], [b].[Name], [b].[StartDate]
            //    FROM[Battles] AS[b]
            //    WHERE[b].[BattleId] = 1
            //) AS[t]
            //LEFT JOIN(
            //    SELECT[b0].[BattleId], [b0].[SamuraiId], [b0].[DateJoined], [s].[Id], [s].[Name]
            //    FROM [BattleSamurai] AS [b0]
            //    INNER JOIN [Samurais] AS[s] ON [b0].[SamuraiId] = [s].[Id]
            //    WHERE[s].[Id] = 17
            //) AS[t0] ON[t].[BattleId] = [t0].[BattleId]
            //ORDER BY[t].[BattleId], [t0].[BattleId], [t0].[SamuraiId], [t0].[Id]

            //Executed DbCommand(20ms) [Parameters=[@p0 = '1', @p1 = '17'], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //DELETE FROM[BattleSamurai]
            //WHERE[BattleId] = @p0 AND[SamuraiId] = @p1;
            //SELECT @@ROWCOUNT;
        }

        private static void WillNotRemoveSamuraiFromABattle()
        {
            var battle = _context.Battles.Find(1);
            var samurai = _context.Samurais.Find(12);
            battle.Samurais.Remove(samurai);
            _context.SaveChanges(); //the relationship is not being tracked
        }

        private static void RemoveSamuraiFromABattleExplicit()
        {
            var b_s = _context.Set<BattleSamurai>().SingleOrDefault(bs => bs.BattleId == 1 && bs.SamuraiId == 19);
            if (b_s != null)
            {
                _context.Remove(b_s); //_context.Set<BattleSamurai>().Remove works, too
                _context.SaveChanges();
            }

            //Executed DbCommand(32ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT TOP(2) [b].[BattleId], [b].[SamuraiId], [b].[DateJoined]
            //FROM[BattleSamurai] AS[b]
            //WHERE([b].[BattleId] = 1) AND([b].[SamuraiId] = 19)

            //Executed DbCommand(26ms) [Parameters=[@p0 = '1', @p1 = '19'], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //DELETE FROM[BattleSamurai]
            //WHERE[BattleId] = @p0 AND[SamuraiId] = @p1;
            //SELECT @@ROWCOUNT;
        }

        private static void AddNewSamuraiWithHorse()
        {
            var samurai = new Samurai { Name = "Jina Ujichika" };
            samurai.Horse = new Horse { Name = "Silver" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();

            //Executed DbCommand(29ms) [Parameters=[@p0 = 'Jina Ujichika'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Samurais] ([Name])
            //VALUES(@p0);

            //SELECT[Id]
            //FROM[Samurais]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();

            //Executed DbCommand(5ms) [Parameters=[@p1 = 'Silver'(Size = 4000), @p2 = '35'], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Horses] ([Name], [SamuraiId])
            //VALUES(@p1, @p2);
            
            //SELECT[Id]
            //FROM[Horses]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();
        }

        private static void AddNewHorseToSamuraiUsingId()
        {
            var horse = new Horse { Name = "Scout", SamuraiId = 19 };
            _context.Add(horse);
            _context.SaveChanges();

            //Executed DbCommand(40ms) [Parameters=[@p0 = 'Scout'(Size = 4000), @p1 = '19'], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Horses] ([Name], [SamuraiId])
            //VALUES(@p0, @p1);
            //SELECT[Id]
            //FROM[Horses]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();
        }

        private static void AddNewHorseToSamuraiObject()
        {
            var samurai = _context.Samurais.Find(20);
            samurai.Horse = new Horse { Name = "Black Beauty" };
            _context.SaveChanges();

            //Executed DbCommand(43ms) [Parameters=[@__p_0 = '20'], CommandType = 'Text', CommandTimeout = '30']
            //SELECT TOP(1) [s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]
            //WHERE[s].[Id] = @__p_0

            //Executed DbCommand(3ms) [Parameters=[@p0 = 'Black Beauty'(Size = 4000), @p1 = '20'], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Horses] ([Name], [SamuraiId])
            //VALUES(@p0, @p1);
            
            //SELECT[Id]
            //FROM[Horses]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();
        }

        private static void AddNewHorseToDisconnectedSamuraiObject()
        {
            var samurai = _context.Samurais.AsNoTracking().FirstOrDefault(s => s.Id == 22);
            samurai.Horse = new Horse { Name = "Mr. Ed" };

            using var newContext = new SamuraiContext();
            newContext.Samurais.Attach(samurai);
            newContext.SaveChanges();

            //Executed DbCommand(27ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT TOP(1) [s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]
            //WHERE[s].[Id] = 22

            //Executed DbCommand(22ms) [Parameters=[@p0 = 'Mr. Ed'(Size = 4000), @p1 = '22'], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Horses] ([Name], [SamuraiId])
            //VALUES(@p0, @p1);
            
            //SELECT[Id]
            //FROM[Horses]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();
        }

        private static void ReplaceAHorse()
        {
            //var samurai = _context.Samurais.Include(s => s.Horse).FirstOrDefault(s => s.Id == 17);
            //samurai.Horse = new Horse { Name = "Trigger" };
            //_context.SaveChanges();

            //Executed DbCommand(74ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT TOP(1) [s].[Id], [s].[Name], [h].[Id], [h].[Name], [h].[SamuraiId]
            //FROM[Samurais] AS[s]
            //LEFT JOIN[Horses] AS[h] ON[s].[Id] = [h].[SamuraiId]
            //WHERE[s].[Id] = 17

            //Executed DbCommand(29ms) [Parameters=[@p0 = '3'], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //DELETE FROM[Horses]
            //WHERE[Id] = @p0;
            //SELECT @@ROWCOUNT;

            //Executed DbCommand(3ms) [Parameters=[@p1 = 'Trigger'(Size = 4000), @p2 = '17'], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Horses] ([Name], [SamuraiId])
            //VALUES(@p1, @p2);

            //SELECT[Id]
            //FROM[Horses]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();

            var horse = _context.Set<Horse>().FirstOrDefault(h => h.Name == "Mr. Ed");
            horse.SamuraiId = 17; //owns Trigger! savechanges will fail
            _context.SaveChanges();

            //Microsoft.Data.SqlClient.SqlException(0x80131904): Cannot insert duplicate key row in object 'dbo.Horses' with unique index 'IX_Horses_SamuraiId'.The duplicate key value is (17).
        }
    }
}
