using System;
using System.Collections.Generic;
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
            
            try 
            { 
                LoadData(); 
                Console.WriteLine("Успешное подключение к базе данных\n"); 
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"Ошибка подключения: {ex.Message}"); 
                return; 
            }

            while (true)
            {
                Console.WriteLine("Меню");
                Console.WriteLine("1. Отчеты (Часть 1)");
                Console.WriteLine("2. Добавить данные (Задание 1)");
                Console.WriteLine("3. Обновить данные (Задание 2)");
                Console.WriteLine("4. Удалить данные (Задание 3)");
                Console.WriteLine("5. Хранимые процедуры (Задание 4)");
                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите действие: ");

                string choice = Console.ReadLine() ?? "";
                if (choice == "0") break;

                try
                {
                    switch (choice)
                    {
                        case "1": MenuReports();
                         break;
                        case "2": MenuAdd();
                         break;
                        case "3": MenuUpdate();
                         break;
                        case "4": MenuDelete();
                         break;
                        case "5": MenuStoredProcedures();
                         break;
                        default: Console.WriteLine("Нет такого пункта.");
                         continue;
                    }
                }
                catch (Exception ex) { Console.WriteLine($"Ошибка: {ex.Message}"); }
                
                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();
                Console.Clear();
                LoadData(); // Обновляем данные из БД после изменений
            }
        }

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

        static void ExecuteSql(string sql)
        {
            using var conn = new SqlConnection(cs);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        // задание 1 2 3
        static void MenuAdd()
        {
            Console.WriteLine("1. Добавить страну\n2. Добавить столицу\n3. Добавить город");
            Console.Write("Выбор: "); string ch = Console.ReadLine() ?? "";

            if (ch == "1") {
                Console.Write("Название: "); string n = Console.ReadLine() ?? "";
                Console.Write("Площадь: "); double a = double.Parse(Console.ReadLine() ?? "0");
                Console.Write("Часть света: "); string c = Console.ReadLine() ?? "";
                ExecuteSql($"INSERT INTO Countries (Name, Area, Continent) VALUES (N'{n}', {a.ToString(System.Globalization.CultureInfo.InvariantCulture)}, N'{c}')");
            } else if (ch == "2") {
                Console.Write("ID страны: "); int id = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Название столицы: "); string n = Console.ReadLine() ?? "";
                Console.Write("Население: "); long p = long.Parse(Console.ReadLine() ?? "0");
                ExecuteSql($"INSERT INTO Capitals (CountryId, Name, Population) VALUES ({id}, N'{n}', {p})");
            } else if (ch == "3") {
                Console.Write("ID страны: "); int id = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Название города: "); string n = Console.ReadLine() ?? "";
                Console.Write("Население: "); long p = long.Parse(Console.ReadLine() ?? "0");
                ExecuteSql($"INSERT INTO Cities (CountryId, Name, Population) VALUES ({id}, N'{n}', {p})");
            }
            Console.WriteLine("Добавлено!");
        }

        static void MenuUpdate()
        {
            Console.WriteLine("1. Обновить страну\n2. Обновить столицу\n3. Обновить город");
            Console.Write("Выбор: "); string ch = Console.ReadLine() ?? "";

            if (ch == "1") {
                Console.Write("ID страны: "); int id = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Новая площадь: "); double a = double.Parse(Console.ReadLine() ?? "0");
                ExecuteSql($"UPDATE Countries SET Area = {a.ToString(System.Globalization.CultureInfo.InvariantCulture)} WHERE Id = {id}");
            } else if (ch == "2") {
                Console.Write("ID столицы: "); int id = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Новое население: "); long p = long.Parse(Console.ReadLine() ?? "0");
                ExecuteSql($"UPDATE Capitals SET Population = {p} WHERE Id = {id}");
            } else if (ch == "3") {
                Console.Write("ID города: "); int id = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Новое население: "); long p = long.Parse(Console.ReadLine() ?? "0");
                ExecuteSql($"UPDATE Cities SET Population = {p} WHERE Id = {id}");
            }
            Console.WriteLine("Обновлено!");
        }

        static void MenuDelete()
        {
            Console.WriteLine("1. Удалить город\n2. Удалить столицу\n3. Удалить страну");
            Console.Write("Выбор: "); string ch = Console.ReadLine() ?? "";

            if (ch == "1") {
                Console.Write("ID города: "); int id = int.Parse(Console.ReadLine() ?? "0");
                ExecuteSql($"DELETE FROM Cities WHERE Id = {id}");
            } else if (ch == "2") {
                Console.Write("ID столицы: "); int id = int.Parse(Console.ReadLine() ?? "0");
                ExecuteSql($"DELETE FROM Capitals WHERE Id = {id}");
            } else if (ch == "3") {
                Console.Write("ID страны: "); int id = int.Parse(Console.ReadLine() ?? "0");
                // Удаляем связанные данные вручную, чтобы не было ошибки внешнего ключа
                ExecuteSql($"DELETE FROM Cities WHERE CountryId = {id}");
                ExecuteSql($"DELETE FROM Capitals WHERE CountryId = {id}");
                ExecuteSql($"DELETE FROM Countries WHERE Id = {id}"); 
            }
            Console.WriteLine("Удалено!");
        }

        // задание 4- хранимые процедуры
        static void MenuStoredProcedures()
        {
            Console.WriteLine("1. Топ-3 стран по населению");
            Console.WriteLine("2. Топ-3 столиц по населению");
            Console.WriteLine("3. Страна с макс. населением");
            Console.WriteLine("4. Город с макс. населением");
            Console.Write("Выбор: "); string ch = Console.ReadLine() ?? "";

            string proc = ch switch { "1" => "sp_Top3CountriesByPop", "2" => "sp_Top3CapitalsByPop", "3" => "sp_CountryWithMaxPop", "4" => "sp_CityWithMaxPop", _ => "" };
            if (string.IsNullOrEmpty(proc)) return;

            using var conn = new SqlConnection(cs);
            conn.Open();
            using var cmd = new SqlCommand(proc, conn) { CommandType = System.Data.CommandType.StoredProcedure };
            using var reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++) Console.Write($"{reader.GetValue(i)} ");
                Console.WriteLine();
            }
        }

        // Отчет
        static void MenuReports()
        {
            Console.WriteLine("1. Полная информация о странах\n2. Города конкретной страны");
            Console.Write("Выбор: "); string ch = Console.ReadLine() ?? "";
            if (ch == "1") {
                foreach (var c in countries) {
                    var cap = capitals.FirstOrDefault(x => x.CountryId == c.Id);
                    Console.WriteLine($"{c.Name,-15} | {cap?.Name,-15} | {c.Area,12:F0} | {c.Continent}");
                }
            } else if (ch == "2") {
                Console.Write("Введите страну: "); string name = Console.ReadLine() ?? "";
                var c = countries.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (c == null) Console.WriteLine("Не найдено.");
                else foreach (var ci in cities.Where(x => x.CountryId == c.Id)) Console.WriteLine($"{ci.Name} — {ci.Population}");
            }
        }
    }
}
