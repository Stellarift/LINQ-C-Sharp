using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace LinqToSqlPart2
{
    public class Country    { public int Id; public string Name=""; public double Area; public string Continent=""; }
    public class Capital    { public int Id; public int CountryId; public string Name=""; public long Population; }
    public class City       { public int Id; public int CountryId; public string Name=""; public long Population; }

    class Program
    {
        static string cs = "Server=localhost,1433;Database=CountriesHomework;User Id=sa;Password=Ghoul1234;TrustServerCertificate=True;";

        static List<Country> countries = new();
        static List<Capital> capitals = new();
        static List<City> cities = new();

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1) Подключение
            Console.WriteLine("Подключение к БД...");
            try
            {
                LoadData();
                Console.WriteLine("Успешное подключение к базе данных!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
                return;
            }

            while (true)
            {
                Console.WriteLine("Система отчетов");
                Console.WriteLine("1. Полная информация о странах");
                Console.WriteLine("2. Частичная информация (кол-во стран)");
                Console.WriteLine("3. Информация о конкретной стране");
                Console.WriteLine("4. Города конкретной страны");
                Console.WriteLine("5. Страны на букву");
                Console.WriteLine("6. Столицы на букву");
                Console.WriteLine("7. Топ-3 столицы с наименьшим населением");
                Console.WriteLine("8. Топ-3 страны с наименьшим населением");
                Console.WriteLine("9. Среднее население столиц по частям света");
                Console.WriteLine("10. Топ-3 страны (min население) по частям света");
                Console.WriteLine("11. Топ-3 страны (max население) по частям света");
                Console.WriteLine("12. Среднее население в конкретной стране");
                Console.WriteLine("13. Город с наименьшим населением в стране");
                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите отчет: ");

                string choice = Console.ReadLine() ?? "";
                if (choice == "0") break;

                Console.Write("Вывести на экран (1) или в файл (2)? ");
                bool toFile = Console.ReadLine() == "2";
                string output = "";

                try
                {
                    switch (choice)
                    {
                        case "1": output = ReportAllCountries();
                         break;
                        case "2": output = ReportPartialCountries();
                         break;
                        case "3": output = ReportSpecificCountry(); 
                         break;
                        case "4": output = ReportCitiesOfCountry();
                         break;
                        case "5": output = ReportCountriesByLetter();
                         break;
                        case "6": output = ReportCapitalsByLetter();
                         break;
                        case "7": output = ReportTop3CapitalsMin();
                         break;
                        case "8": output = ReportTop3CountriesMin();
                         break;
                        case "9": output = ReportAvgPopByContinent();
                         break;
                        case "10": output = ReportTop3MinByContinent();
                         break;
                        case "11": output = ReportTop3MaxByContinent();
                         break;
                        case "12": output = ReportAvgPopSpecificCountry();
                         break;
                        case "13": output = ReportMinCityInCountry();
                         break;
                        default: Console.WriteLine("Нет такого пункта.");
                         continue;
                    }

                    if (toFile)
                    {
                        string filename = $"Report_{choice}_{DateTime.Now:HHmmss}.txt";
                        File.WriteAllText(filename, output);
                        Console.WriteLine($"Отчет сохранен в файл: {filename}");
                    }
                    else
                    {
                        Console.WriteLine(output);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();
                Console.Clear();
            }
        }

        // методы загрузки
        static void LoadData()
        {
            countries = Load<Country>("SELECT Id, Name, Area, Continent FROM Countries", r => new Country { Id = (int)r["Id"], Name = r["Name"].ToString()!, Area = Convert.ToDouble(r["Area"]), Continent = r["Continent"].ToString()! });
            capitals = Load<Capital>("SELECT Id, CountryId, Name, Population FROM Capitals", r => new Capital { Id = (int)r["Id"], CountryId = (int)r["CountryId"], Name = r["Name"].ToString()!, Population = (long)r["Population"] });
            cities = Load<City>("SELECT Id, CountryId, Name, Population FROM Cities", r => new City { Id = (int)r["Id"], CountryId = (int)r["CountryId"], Name = r["Name"].ToString()!, Population = (long)r["Population"] });
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

        // 2)Методы отчетов
        static string ReportAllCountries()
        {
            var sb = new System.Text.StringBuilder("Все страны:\n");
            foreach (var c in countries)
            {
                var cap = capitals.FirstOrDefault(x => x.CountryId == c.Id);
                sb.AppendLine($"{c.Name,-15} | Столица: {cap?.Name,-15} | Площадь: {c.Area,12:F0} | {c.Continent}");
            }
            return sb.ToString();
        }

        static string ReportPartialCountries()
        {
            Console.Write("Сколько стран вывести? ");
            if (!int.TryParse(Console.ReadLine(), out int count)) count = 3;
            var sb = new System.Text.StringBuilder($"Первые {count} стран:\n");
            foreach (var c in countries.Take(count))
                sb.AppendLine($"{c.Name} (Площадь: {c.Area:F0})");
            return sb.ToString();
        }

        static string ReportSpecificCountry()
        {
            Console.Write("Введите название страны: ");
            string name = Console.ReadLine() ?? "";
            var c = countries.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (c == null) return "Страна не найдена.";

            var cap = capitals.FirstOrDefault(x => x.CountryId == c.Id);
            return $"Страна: {c.Name}\nСтолица: {cap?.Name}\nНаселение столицы: {cap?.Population}\nПлощадь: {c.Area}\nЧасть света: {c.Continent}";
        }

        static string ReportCitiesOfCountry()
        {
            Console.Write("Введите название страны: ");
            string name = Console.ReadLine() ?? "";
            var c = countries.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (c == null) return "Страна не найдена.";

            var cityList = cities.Where(ci => ci.CountryId == c.Id).ToList();
            if (!cityList.Any()) return "Городов не найдено.";

            var sb = new System.Text.StringBuilder($"Города страны {c.Name}:\n");
            foreach (var ci in cityList)
                sb.AppendLine($"{ci.Name} — {ci.Population}");
            return sb.ToString();
        }

        // 3)Методы отчетов
        static string ReportCountriesByLetter()
        {
            Console.Write("Введите букву: ");
            string letter = Console.ReadLine() ?? "";
            var list = countries.Where(c => c.Name.StartsWith(letter, StringComparison.OrdinalIgnoreCase));
            return string.Join("\n", list.Select(c => c.Name));
        }

        static string ReportCapitalsByLetter()
        {
            Console.Write("Введите букву: ");
            string letter = Console.ReadLine() ?? "";
            var list = capitals.Where(c => c.Name.StartsWith(letter, StringComparison.OrdinalIgnoreCase));
            return string.Join("\n", list.Select(c => c.Name));
        }

        static string ReportTop3CapitalsMin()
        {
            var list = capitals.OrderBy(c => c.Population).Take(3);
            return string.Join("\n", list.Select(c => $"{c.Name} — {c.Population}"));
        }

        static string ReportTop3CountriesMin()
        {
            // Берем население столицы как показатель населения страны
            var list = capitals.OrderBy(c => c.Population).Take(3)
                .Join(countries, cap => cap.CountryId, co => co.Id, (cap, co) => $"{co.Name} (Столица: {cap.Name}, {cap.Population})");
            return string.Join("\n", list);
        }

        static string ReportAvgPopByContinent()
        {
            var list = capitals.Join(countries, cap => cap.CountryId, co => co.Id, (cap, co) => new { co.Continent, cap.Population })
                               .GroupBy(x => x.Continent)
                               .Select(g => $"{g.Key}: {g.Average(x => x.Population):F0}");
            return string.Join("\n", list);
        }

        // 4)Методы отчетов
        static string ReportTop3MinByContinent()
        {
            var list = countries.Join(capitals, co => co.Id, cap => cap.CountryId, (co, cap) => new { co.Continent, co.Name, cap.Population })
                                .GroupBy(x => x.Continent)
                                .SelectMany(g => g.OrderBy(x => x.Population).Take(3).Select(x => $"[{g.Key}] {x.Name} — {x.Population}"));
            return string.Join("\n", list);
        }

        static string ReportTop3MaxByContinent()
        {
            var list = countries.Join(capitals, co => co.Id, cap => cap.CountryId, (co, cap) => new { co.Continent, co.Name, cap.Population })
                                .GroupBy(x => x.Continent)
                                .SelectMany(g => g.OrderByDescending(x => x.Population).Take(3).Select(x => $"[{g.Key}] {x.Name} — {x.Population}"));
            return string.Join("\n", list);
        }

        static string ReportAvgPopSpecificCountry()
        {
            Console.Write("Введите страну: ");
            string name = Console.ReadLine() ?? "";
            var c = countries.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (c == null) return "Страна не найдена.";

            var cityList = cities.Where(ci => ci.CountryId == c.Id);
            if (!cityList.Any()) return "Городов нет.";

            return $"Среднее население городов в {c.Name}: {cityList.Average(ci => ci.Population):F0}";
        }

        static string ReportMinCityInCountry()
        {
            Console.Write("Введите страну: ");
            string name = Console.ReadLine() ?? "";
            var c = countries.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (c == null) return "Страна не найдена.";

            var minCity = cities.Where(ci => ci.CountryId == c.Id).OrderBy(ci => ci.Population).FirstOrDefault();
            return minCity == null ? "Городов нет." : $"Город с мин. населением в {c.Name}: {minCity.Name} ({minCity.Population})";
        }
    }
}
