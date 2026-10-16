using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace LinqToSqlLab
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Capital { get; set; } = "";
        public long Population { get; set; }
        public double Area { get; set; }
        public string Continent { get; set; } = "";

        public override string ToString()
        {
            return $"{Name,-15} | {Capital,-15} | {Population,12} | {Area,12:F0} | {Continent}";
        }
    }

    class Program
    {
        static string cs = "Server=localhost,1433;Database=CountriesDB;User Id=sa;Password=Ghoul1234;TrustServerCertificate=True;";

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<Country> countries = LoadCountries();

            // ЗАДАНИЕ 2
            Console.WriteLine("1. Вся информация о странах:");
            foreach (var c in countries) Console.WriteLine(c);

            Console.WriteLine("\n2. Названия стран:");
            foreach (var c in countries) Console.WriteLine(c.Name);

            Console.WriteLine("\n3. Названия столиц:");
            foreach (var c in countries) Console.WriteLine(c.Capital);

            Console.WriteLine("\n4. Европейские страны:");
            var europe = countries.Where(c => c.Continent == "Европа");
            foreach (var c in europe) Console.WriteLine(c.Name);

            Console.WriteLine("\n5. Страны с площадью > 1 000 000:");
            var bigArea = countries.Where(c => c.Area > 1_000_000);
            foreach (var c in bigArea) Console.WriteLine($"{c.Name} — {c.Area:F0}");

            // ЗАДАНИЕ 3
            Console.WriteLine("\n6. Страны, в названии есть 'а' и 'ц':");
            var withAandC = countries.Where(c => c.Name.Contains('а') && c.Name.Contains('ц'));
            foreach (var c in withAandC) Console.WriteLine(c.Name);

            Console.WriteLine("\n7. Страны, начинающиеся на 'а':");
            var startsWithA = countries.Where(c => c.Name.StartsWith("А", StringComparison.OrdinalIgnoreCase));
            foreach (var c in startsWithA) Console.WriteLine(c.Name);

            Console.WriteLine("\n8. Страны с площадью от 100 000 до 1 000 000:");
            var areaRange = countries.Where(c => c.Area >= 100_000 && c.Area <= 1_000_000);
            foreach (var c in areaRange) Console.WriteLine($"{c.Name} — {c.Area:F0}");

            Console.WriteLine("\n9. Страны с населением > 100 000 000:");
            var popBig = countries.Where(c => c.Population > 100_000_000);
            foreach (var c in popBig) Console.WriteLine($"{c.Name} — {c.Population}");

            // ЗАДАНИЕ 4
            Console.WriteLine("\n10. Топ-5 стран по площади:");
            var top5Area = countries.OrderByDescending(c => c.Area).Take(5);
            foreach (var c in top5Area) Console.WriteLine($"{c.Name} — {c.Area:F0}");

            Console.WriteLine("\n11. Топ-5 стран по населению:");
            var top5Pop = countries.OrderByDescending(c => c.Population).Take(5);
            foreach (var c in top5Pop) Console.WriteLine($"{c.Name} — {c.Population}");

            Console.WriteLine("\n12. Страна с самой большой площадью:");
            var maxArea = countries.OrderByDescending(c => c.Area).First();
            Console.WriteLine($"{maxArea.Name} — {maxArea.Area:F0}");

            Console.WriteLine("\n13. Страна с самым большим населением:");
            var maxPop = countries.OrderByDescending(c => c.Population).First();
            Console.WriteLine($"{maxPop.Name} — {maxPop.Population}");

            Console.WriteLine("\n14. Страна с самой маленькой площадью в Африке:");
            var minAreaAfrica = countries
                .Where(c => c.Continent == "Африка")
                .OrderBy(c => c.Area)
                .First();
            Console.WriteLine($"{minAreaAfrica.Name} — {minAreaAfrica.Area:F0}");

            Console.WriteLine("\n15. Средняя площадь стран в Азии:");
            var avgAreaAsia = countries
                .Where(c => c.Continent == "Азия")
                .Average(c => c.Area);
            Console.WriteLine($"Средняя площадь: {avgAreaAsia:F0}");

            Console.WriteLine("\nНажмите Enter...");
            Console.ReadLine();
        }

        static List<Country> LoadCountries()
        {
            var list = new List<Country>();
            using var conn = new SqlConnection(cs);
            conn.Open();
            using var cmd = new SqlCommand("SELECT Id, Name, Capital, Population, Area, Continent FROM Countries", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Country
                {
                    Id         = (int)reader["Id"],
                    Name       = reader["Name"].ToString()!,
                    Capital    = reader["Capital"].ToString()!,
                    Population = (long)reader["Population"],
                    Area       = Convert.ToDouble(reader["Area"]),
                    Continent  = reader["Continent"].ToString()!
                });
            }
            return list;
        }
    }
}
