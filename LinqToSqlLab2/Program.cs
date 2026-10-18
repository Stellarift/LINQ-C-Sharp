using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace LinqToSqlLab
{
    // Классы для хранения данных из БД
    public class Country    { public int Id; public string Name=""; public double Area; public string Continent=""; }
    public class Capital    { public int Id; public int CountryId; public string Name=""; public long Population; }
    public class City       { public int Id; public int CountryId; public string Name=""; public long Population; }

    class Program
    {
        // Строка подключения к новой базе данных
        static string cs = "Server=localhost,1433;Database=CountriesHomework;User Id=sa;Password=Ghoul1234;TrustServerCertificate=True;";

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Загружаем данные из БД в списки
            var countries = Load<Country>("SELECT Id, Name, Area, Continent FROM Countries", r => new Country {
                Id = (int)r["Id"], Name = r["Name"].ToString()!, Area = Convert.ToDouble(r["Area"]), Continent = r["Continent"].ToString()! });
            var capitals = Load<Capital>("SELECT Id, CountryId, Name, Population FROM Capitals", r => new Capital {
                Id = (int)r["Id"], CountryId = (int)r["CountryId"], Name = r["Name"].ToString()!, Population = (long)r["Population"] });
            var cities = Load<City>("SELECT Id, CountryId, Name, Population FROM Cities", r => new City {
                Id = (int)r["Id"], CountryId = (int)r["CountryId"], Name = r["Name"].ToString()!, Population = (long)r["Population"] });

            // задание 2
            Console.WriteLine("\n--- Задание 2 ---");

            Console.WriteLine("\n1. Вся информация о странах:");
            foreach (var c in countries)
            {
                var cap = capitals.First(x => x.CountryId == c.Id);
                Console.WriteLine($"{c.Name,-15} | {cap.Name,-15} | Площадь: {c.Area,12:F0} | {c.Continent}");
            }

            Console.WriteLine("\n2. Названия стран:");
            foreach (var c in countries) Console.WriteLine(c.Name);

            Console.WriteLine("\n3. Названия столиц:");
            foreach (var cap in capitals) Console.WriteLine(cap.Name);

            Console.WriteLine("\n4. Города России:");
            var russianCities = cities.Where(ci => ci.CountryId == 1);
            foreach (var ci in russianCities) Console.WriteLine(ci.Name);

            Console.WriteLine("\n5. Столицы с населением > 5 000 000:");
            var bigCapitals = capitals.Where(c => c.Population > 5_000_000);
            foreach (var c in bigCapitals) Console.WriteLine($"{c.Name} — {c.Population}");

            Console.WriteLine("\n6. Европейские страны:");
            var europe = countries.Where(c => c.Continent == "Европа");
            foreach (var c in europe) Console.WriteLine(c.Name);

            Console.WriteLine("\n7. Страны с площадью > 1 000 000:");
            var bigArea = countries.Where(c => c.Area > 1_000_000);
            foreach (var c in bigArea) Console.WriteLine($"{c.Name} — {c.Area:F0}");
            
            // задание 3

            Console.WriteLine("\n--- Задание 3 ---");

            Console.WriteLine("\n8. Столицы, в названии есть 'а' и 'р':");
            var capAandR = capitals.Where(c => c.Name.Contains('а') && c.Name.Contains('р'));
            foreach (var c in capAandR) Console.WriteLine(c.Name);

            Console.WriteLine("\n9. Столицы на 'к':");
            var capK = capitals.Where(c => c.Name.StartsWith("К", StringComparison.OrdinalIgnoreCase));
            foreach (var c in capK) Console.WriteLine(c.Name);

            Console.WriteLine("\n10. Страны с площадью 100 000..1 000 000:");
            var areaRange = countries.Where(c => c.Area >= 100_000 && c.Area <= 1_000_000);
            foreach (var c in areaRange) Console.WriteLine($"{c.Name} — {c.Area:F0}");

            Console.WriteLine("\n11. Страны с населением столицы > 10 000 000:");
            var capPopBig = capitals.Where(c => c.Population > 10_000_000)
                                    .Join(countries, cap => cap.CountryId, co => co.Id, (cap, co) => co.Name);
            foreach (var name in capPopBig) Console.WriteLine(name);

            // ==========================================
            // ЗАДАНИЕ 4
            // ==========================================
            Console.WriteLine("\n--- Задание 4 ---");

            Console.WriteLine("\n12. Топ-5 стран по площади:");
            var top5Area = countries.OrderByDescending(c => c.Area).Take(5);
            foreach (var c in top5Area) Console.WriteLine($"{c.Name} — {c.Area:F0}");

            Console.WriteLine("\n13. Топ-5 столиц по населению:");
            var top5Cap = capitals.OrderByDescending(c => c.Population).Take(5);
            foreach (var c in top5Cap) Console.WriteLine($"{c.Name} — {c.Population}");

            Console.WriteLine("\n14. Страна с самой большой площадью:");
            var maxArea = countries.OrderByDescending(c => c.Area).First();
            Console.WriteLine($"{maxArea.Name} — {maxArea.Area:F0}");

            Console.WriteLine("\n15. Столица с самым большим населением:");
            var maxCap = capitals.OrderByDescending(c => c.Population).First();
            Console.WriteLine($"{maxCap.Name} — {maxCap.Population}");

            Console.WriteLine("\n16. Страна с самой маленькой площадью в Европе:");
            var minEu = countries.Where(c => c.Continent == "Европа").OrderBy(c => c.Area).First();
            Console.WriteLine($"{minEu.Name} — {minEu.Area:F0}");

            Console.WriteLine("\n17. Средняя площадь европейских стран:");
            var avgEu = countries.Where(c => c.Continent == "Европа").Average(c => c.Area);
            Console.WriteLine($"{avgEu:F0}");

            Console.WriteLine("\n18. Топ-3 города Китая по населению:");
            var chinaTop3 = cities.Where(ci => ci.CountryId == 6).OrderByDescending(ci => ci.Population).Take(3);
            foreach (var ci in chinaTop3) Console.WriteLine($"{ci.Name} — {ci.Population}");

            Console.WriteLine("\n19. Общее количество стран:");
            Console.WriteLine(countries.Count());

            Console.WriteLine("\n20. Часть света с максимальным количеством стран:");
            var maxContinent = countries.GroupBy(c => c.Continent).OrderByDescending(g => g.Count()).First();
            Console.WriteLine($"{maxContinent.Key} — {maxContinent.Count()} стран");

            Console.WriteLine("\n21. Количество стран в каждой части света:");
            var byContinent = countries.GroupBy(c => c.Continent)
                                       .Select(g => new { Continent = g.Key, Count = g.Count() });
            foreach (var g in byContinent) Console.WriteLine($"{g.Continent} — {g.Count}");

            Console.WriteLine("\nНажмите Enter для выхода...");
            Console.ReadLine();
        }

        static List<T> Load<T>(string sql, Func<SqlDataReader, T> map)
        {
            var list = new List<T>();
            using var conn = new SqlConnection(cs);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(map(reader));
            return list;
        }
    }
}
