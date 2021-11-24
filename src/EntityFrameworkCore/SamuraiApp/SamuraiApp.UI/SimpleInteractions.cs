using Humanizer;
using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static Humanizer.In;
using static System.Net.Mime.MediaTypeNames;

namespace SamuraiApp.UI
{
    internal class SimpleInteractions
    {
        private static SamuraiContext _context = new SamuraiContext();

        internal static void Interact()
        {
            //AddSamuraisByName("Shimada", "Okamoto", "Kikuchio", "Hayashida");
            //GetSamurais();
            //AddVariousTypes();
            //QueryFilters();
            //QueryAggregates();
            //RetrieveAndUpdateSamurai();
            //RetrieveAndUpdateMultipleSamurais();
            //MultipleDatabaseOperations();
            //RetrieveAndDeleteASamurai();
            //QueryAndUpdateBattles_Disconnected();
        }

        private static void AddVariousTypes()
        {
            _context.AddRange(new Samurai { Name = "Shimada" },
                              new Samurai { Name = "Okamoto" },
                              new Battle { Name = "Battle of Anegawa" },
                              new Battle { Name = "Battle of Nagashino" });
            //_context.Samurais.AddRange(
            //    new Samurai { Name = "Shimada" },
            //    new Samurai { Name = "Okamoto" });
            //_context.Battles.AddRange(
            //    new Battle { Name = "Battle of Anegawa" },
            //    new Battle { Name = "Battle of Nagashino" });
            _context.SaveChanges();
        }

        private static void AddSamuraisByName(params string[] names)
        {
            foreach (string name in names)
            {
                _context.Samurais.Add(new Samurai { Name = name });
            }
            _context.SaveChanges();

            // EF will insert all of the samurais with single command - merge join

            //Executed DbCommand(32ms) [Parameters=[@p0 = 'Shimada'(Size = 4000), @p1 = 'Okamoto'(Size = 4000), @p2 = 'Kikuchio'(Size = 4000), @p3 = 'Hayashida'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //DECLARE @inserted0 TABLE([Id] int, [_Position] [int]);
            //MERGE[Samurais] USING(
            //    VALUES(@p0, 0),
            //    (@p1, 1),
            //    (@p2, 2),
            //    (@p3, 3)) AS i([Name], _Position) ON 1 = 0
            //WHEN NOT MATCHED THEN
            //INSERT([Name])
            //VALUES(i.[Name])
            //OUTPUT INSERTED.[Id], i._Position
            //INTO @inserted0;

            //SELECT[t].[Id] FROM[Samurais] t
            //    INNER JOIN @inserted0 i ON([t].[Id] = [i].[Id])
            //ORDER BY[i].[_Position];
        }

        private static void AddSamurais(Samurai[] samurais)
        {
            //AddRange can take an array or an IEnumerable e.g. List<Samurai>
            _context.Samurais.AddRange(samurais);
            _context.SaveChanges();
        }

        private static void GetSamurais()
        {
            var samurais = _context.Samurais
                .TagWith("ConsoleApp.Program.GetSamurais method")
                .ToList();
            Console.WriteLine($"Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }

            //Executed DbCommand(19ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //--ConsoleApp.Program.GetSamurais method

            //SELECT[s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]
        }

        private static void QueryFilters()
        {
            var name = "Sampson";
            _context.Samurais.Where(s => s.Name == name).ToList();

            //Executed DbCommand(47ms) [Parameters=[@__name_0 = 'Sampson'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]
            //WHERE[s].[Name] = @__name_0


            var filter = "J%";
            _context.Samurais.Where(s => EF.Functions.Like(s.Name, filter)).ToList();

            //Executed DbCommand(31ms) [Parameters=[@__filter_1 = 'J%'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]
            //WHERE[s].[Name] LIKE @__filter_1
        }

        private static void QueryAggregates()
        {
            var name = "Sampson";
            var samurai = _context.Samurais.Find(2);

            //Executed DbCommand(27ms) [Parameters=[@__p_0 = '2'], CommandType = 'Text', CommandTimeout = '30']
            //SELECT TOP(1) [s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]
            //WHERE[s].[Id] = @__p_0
        }

        private static void RetrieveAndUpdateSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.SaveChanges();

            //Executed DbCommand(30ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT TOP(1) [s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]

            //Executed DbCommand(28ms) [Parameters=[@p1 = '17', @p0 = 'ShimadaSan'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //UPDATE[Samurais] SET[Name] = @p0
            //WHERE[Id] = @p1;
            //SELECT @@ROWCOUNT;
        }

        private static void RetrieveAndUpdateMultipleSamurais()
        {
            var samurais = _context.Samurais.Skip(1).Take(4).ToList();
            samurais.ForEach(s => s.Name += "San");
            _context.SaveChanges();

            //Executed DbCommand(24ms) [Parameters=[@__p_0 = '1', @__p_1 = '4'], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]
            //ORDER BY(SELECT 1)
            //OFFSET @__p_0 ROWS FETCH NEXT @__p_1 ROWS ONLY

            //Executed DbCommand(5ms) [Parameters=[@p1 = '18', @p0 = 'OkamotoSan'(Size = 4000), @p3 = '19', @p2 = 'KikuchioSan'(Size = 4000), @p5 = '20', @p4 = 'HayashidaSan'(Size = 4000), @p7 = '21', @p6 = 'ShimadaSan'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //UPDATE[Samurais] SET[Name] = @p0
            //WHERE[Id] = @p1;
            //SELECT @@ROWCOUNT;

            //UPDATE[Samurais] SET[Name] = @p2
            //WHERE[Id] = @p3;
            //SELECT @@ROWCOUNT;

            //UPDATE[Samurais] SET[Name] = @p4
            //WHERE[Id] = @p5;
            //SELECT @@ROWCOUNT;

            //UPDATE[Samurais] SET[Name] = @p6
            //WHERE[Id] = @p7;
            //SELECT @@ROWCOUNT;
        }

        private static void MultipleDatabaseOperations()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.Samurais.Add(new Samurai { Name = "Shino" });
            _context.SaveChanges();

            //Executed DbCommand(40ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT TOP(1) [s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]

            // Began transaction with isolation level 'ReadCommitted'.

            //Executed DbCommand(24ms) [Parameters=[@p1 = '17', @p0 = 'ShimadaSanSanSan'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //UPDATE[Samurais] SET[Name] = @p0
            //WHERE[Id] = @p1;
            //SELECT @@ROWCOUNT;

            //Executed DbCommand(2ms) [Parameters=[@p0 = 'Shino'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //INSERT INTO[Samurais] ([Name])
            //VALUES(@p0);
            //SELECT[Id]
            //FROM[Samurais]
            //WHERE @@ROWCOUNT = 1 AND[Id] = scope_identity();

            // Committed transaction.
        }

        private static void RetrieveAndDeleteASamurai()
        {
            var samurai = _context.Samurais.Find(18);
            _context.Samurais.Remove(samurai);
            _context.SaveChanges();

            //Executed DbCommand(34ms) [Parameters=[@__p_0 = '18'], CommandType = 'Text', CommandTimeout = '30']
            //SELECT TOP(1) [s].[Id], [s].[Name]
            //FROM[Samurais] AS[s]
            //WHERE[s].[Id] = @__p_0

            //Executed DbCommand(6ms) [Parameters=[@p0 = '18'], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //DELETE FROM[Samurais]
            //WHERE[Id] = @p0;
            //SELECT @@ROWCOUNT;
        }

        private static void QueryAndUpdateBattles_Disconnected()
        {
            List<Battle> disconnectedBattles;
            using (var context1 = new SamuraiContext())
            {
                disconnectedBattles = _context.Battles.ToList();
            } //context1 is disposed
            disconnectedBattles.ForEach(b =>
            {
                b.StartDate = new DateTime(1570, 01, 01);
                b.EndDate = new DateTime(1570, 12, 1);
            });
            using (var context2 = new SamuraiContext())
            {
                context2.UpdateRange(disconnectedBattles);
                context2.SaveChanges();
            }

            //Executed DbCommand(46ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[b].[BattleId], [b].[EndDate], [b].[Name], [b].[StartDate]
            //FROM[Battles] AS[b]

            //Executed DbCommand(30ms) [Parameters=[@p3 = '1', @p0 = '1570-12-01T00:00:00.0000000', @p1 = 'Battle of Anegawa'(Size = 4000), @p2 = '1570-01-01T00:00:00.0000000'], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //UPDATE[Battles] SET[EndDate] = @p0, [Name] = @p1, [StartDate] = @p2
            //WHERE[BattleId] = @p3;
            //SELECT @@ROWCOUNT;

            //Executed DbCommand(2ms) [Parameters=[@p3 = '2', @p0 = '1570-12-01T00:00:00.0000000', @p1 = 'Battle of Nagashino'(Size = 4000), @p2 = '1570-01-01T00:00:00.0000000'], CommandType = 'Text', CommandTimeout = '30']
            //SET NOCOUNT ON;
            //UPDATE[Battles] SET[EndDate] = @p0, [Name] = @p1, [StartDate] = @p2
            //WHERE[BattleId] = @p3;
            //SELECT @@ROWCOUNT;
        }
    }
}
