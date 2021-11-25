using SamuraiApp.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SamuraiApp.UI
{
    internal class RawSqlInteractions
    {
        private static SamuraiContext _context = new SamuraiContext();

        public static void Interact()
        {
            //QuerySamuraiBattleStats();
            //QueryUsingRawSql();
            //QueryRelatedUsingRawSql();
            //QueryUsingRawSqlWithInterpolation();
            //DANGERQueryUsingRawSqlWithInterpolation();
            //QueryUsingFromSqlRawStoredProc();
            //QueryUsingFromSqlIntStoredProc();
            //ExecuteSomeRawSql();
        }

        private static void QuerySamuraiBattleStats()
        {
            // call DB View
            //_context.SamuraiBattleStats.ToList();

            //Executed DbCommand(102ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[s].[EarliestBattle], [s].[Name], [s].[NumberOfBattles]
            //FROM[SamuraiBattleStats] AS[s]

            //_context.SamuraiBattleStats.FirstOrDefault();

            //Executed DbCommand(13ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT TOP(1) [s].[EarliestBattle], [s].[Name], [s].[NumberOfBattles]
            //FROM[SamuraiBattleStats] AS[s]

            //_context.SamuraiBattleStats.FirstOrDefault(b => b.Name == "SampsonSan");

            //Executed DbCommand(30ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT TOP(1) [s].[EarliestBattle], [s].[Name], [s].[NumberOfBattles]
            //FROM[SamuraiBattleStats] AS[s]
            //WHERE[s].[Name] = N'SampsonSan'

            _context.SamuraiBattleStats.Find(2);

            //Unhandled exception. System.NullReferenceException: Object reference not set to an instance of an object.
        }

        private static void QueryUsingRawSql()
        {
            _context.Samurais.FromSqlRaw("Select * from samurais").ToList();

            //Executed DbCommand(36ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //Select* from samurais

            _context.Samurais.FromSqlRaw("Select Id, Name, Quotes, Battles, Horse from Samurais").ToList();

            //Unhandled exception. Microsoft.Data.SqlClient.SqlException(0x80131904): Invalid column name 'Quotes'.
            //    Invalid column name 'Battles'.
            //    Invalid column name 'Horse'.
        }

        private static void QueryRelatedUsingRawSql()
        {
            _context.Samurais.FromSqlRaw("Select Id, Name from Samurais").Include(s => s.Quotes).ToList();

            //Executed DbCommand(51ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //SELECT[s].[Id], [s].[Name], [q].[Id], [q].[SamuraiId], [q].[Text]
            //FROM(
            //    Select Id, Name from Samurais
            //) AS[s]
            //LEFT JOIN[Quotes] AS[q] ON[s].[Id] = [q].[SamuraiId]
            //ORDER BY[s].[Id], [q].[Id]
        }

        private static void QueryUsingRawSqlWithInterpolation()
        {
            // FromSqlInterpolated prevents Sql injection
            string name = "Kikuchyo";
            _context.Samurais.FromSqlInterpolated($"Select * from Samurais Where Name= {name}").ToList();

            //Executed DbCommand(36ms) [Parameters=[p0 = 'Kikuchyo'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //Select* from Samurais Where Name = @p0
        }

        private static void DANGERQueryUsingRawSqlWithInterpolation()
        {
            string name = "Kikuchyo";
            _context.Samurais.FromSqlRaw($"Select * from Samurais Where Name= '{name}'").ToList();

            //Executed DbCommand(27ms) [Parameters=[], CommandType = 'Text', CommandTimeout = '30']
            //Select* from Samurais Where Name = 'Kikuchyo'
        }

        private static void QueryUsingFromSqlRawStoredProc()
        {
            var text = "Happy";
            _context.Samurais.FromSqlRaw("EXEC dbo.SamuraisWhoSaidAWord {0}", text).ToList();

            //Executed DbCommand(95ms) [Parameters=[p0 = 'Happy'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //EXEC dbo.SamuraisWhoSaidAWord @p0
        }

        private static void QueryUsingFromSqlIntStoredProc()
        {
            var text = "Happy";
            _context.Samurais.FromSqlInterpolated($"EXEC dbo.SamuraisWhoSaidAWord {text}").ToList();

            //Executed DbCommand(48ms) [Parameters=[p0 = 'Happy'(Size = 4000)], CommandType = 'Text', CommandTimeout = '30']
            //EXEC dbo.SamuraisWhoSaidAWord @p0
        }


        private static void ExecuteSomeRawSql()
        {
            //var samuraiId = 2;
            //_context.Database.ExecuteSqlRaw("EXEC DeleteQuotesForSamurai {0}", samuraiId);

            //Executed DbCommand(100ms) [Parameters=[@p0 = '2'], CommandType = 'Text', CommandTimeout = '30']
            //EXEC DeleteQuotesForSamurai @p0

            var samuraiId = 2;
            _context.Database.ExecuteSqlInterpolated($"EXEC DeleteQuotesForSamurai {samuraiId}");

            //Executed DbCommand(46ms) [Parameters=[@p0 = '2'], CommandType = 'Text', CommandTimeout = '30']
            //EXEC DeleteQuotesForSamurai @p0
        }
    }
}
