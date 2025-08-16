using BulkCarnageIQ.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.ConsoleApp.Utility
{
    public class DataSeeder
    {
        private readonly string _jsonFilePath;
        private readonly DbContextOptions<AppDbContext> _options;

        public DataSeeder(string jsonFilePath, DbContextOptions<AppDbContext> options)
        {
            _jsonFilePath = jsonFilePath;
            _options = options;
        }

        public void ResetDatabase()
        {
            using (var context = new AppDbContext(_options))
            {
                context.Database.EnsureDeleted();
                context.Database.Migrate();
                Console.WriteLine("Database has been reset successfully!");
            }
        }

        public void SeedDatabaseFromJson()
        {
            using (var context = new AppDbContext(_options))
            {
                var seedData = LoadSeedDataFromJson(_jsonFilePath);
                SeedDatabase(seedData, context);
                Console.WriteLine("Database has been seeded successfully!");
            }
        }

        public void SaveDatabaseToJson()
        {
            using (var context = new AppDbContext(_options))
            {
                SaveSeedDataToJson(_jsonFilePath, GenerateSeedDataFromDatabase(context));
                Console.WriteLine("Database has been saved to JSON successfully!");
            }
        }

        private SeedData LoadSeedDataFromJson(string filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<SeedData>(json);
            }
        }

        private void SeedDatabase(SeedData seedData, AppDbContext context)
        {
            context.Database.EnsureCreated();

            // 1. Add Food Items
            context.FoodItems.AddRange(seedData.FoodItems);
            context.SaveChanges();
        }

        private void SaveSeedDataToJson(string filePath, SeedData seedData)
        {
            string json = JsonConvert.SerializeObject(seedData, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ContractResolver = new InlineResolver()
                });
            File.WriteAllText(filePath, json);
        }

        private SeedData GenerateSeedDataFromDatabase(AppDbContext context)
        {
            var seedData = new SeedData();

            Console.WriteLine("Generating Seed Data from Database!");
            
            Console.WriteLine("1. Load Food Items");
            seedData.FoodItems = context.FoodItems.ToList();

            return seedData;
        }
    }

    class InlineResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) =>
            base.CreateProperties(type, memberSerialization)
                .Where(p => p.Writable).ToList();
    }
}
