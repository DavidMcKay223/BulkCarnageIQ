using BulkCarnageIQ.ConsoleApp.Utility;
using BulkCarnageIQ.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BulkCarnageIQ.ConsoleApp
{
    public class Program
    {
        public static void Main()
        {
            string projectRoot = FileUtility.GetProjectRoot();
            string jsonFilePath = Path.Combine(projectRoot, "Data\\seed_data.json");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BulkCarnageIQ;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            DataSeeder dataSeeder = new DataSeeder(jsonFilePath, options);

            //dataSeeder.ResetDatabase();
            //dataSeeder.SeedDatabaseFromJson();

            //dataSeeder.SaveDatabaseToJson();
        }
    }
}
