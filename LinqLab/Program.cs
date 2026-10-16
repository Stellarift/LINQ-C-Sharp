using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

public class Student
{
    public string Name { get; set; } = "";
    public string Group { get; set; } = "";
    public int Age { get; set; }
    public double GPA { get; set; }
    public int PassedCredits { get; set; }

    public override string ToString()
    {
        return $"{Name,-10} | {Group,-6} | {Age,2} | {GPA,4:F1} | {PassedCredits,2}";
    }
}

class Program
{
    static void Main(string[] args)
    {
        Task1_QuerySyntax();
        Task2_MethodSyntax();
        Task3_Agreement();
        Task4_Xml();

        Console.WriteLine("\nНажмите Enter для выхода...");
        Console.ReadLine();
    }

    // Источник данных
    static List<Student> GetStudents()
    {
        return new List<Student>
        {
            new Student { Name = "Алексей",  Group = "ИТ-41", Age = 20, GPA = 4.2, PassedCredits = 40 },
            new Student { Name = "Борис",    Group = "ИТ-41", Age = 19, GPA = 3.1, PassedCredits = 35 },
            new Student { Name = "Виктория", Group = "ПИ-42", Age = 21, GPA = 4.8, PassedCredits = 45 },
            new Student { Name = "Григорий", Group = "ИБ-43", Age = 20, GPA = 2.9, PassedCredits = 20 },
            new Student { Name = "Дарья",    Group = "ИТ-41", Age = 22, GPA = 4.5, PassedCredits = 42 },
            new Student { Name = "Елена",    Group = "ПИ-42", Age = 20, GPA = 3.8, PassedCredits = 38 },
            new Student { Name = "Жанна",    Group = "ИБ-43", Age = 23, GPA = 4.9, PassedCredits = 48 },
            new Student { Name = "Игорь",    Group = "ИТ-41", Age = 19, GPA = 2.5, PassedCredits = 15 }
        };
    }

    //1) query-синтаксис
    static void Task1_QuerySyntax()
    {
        Console.WriteLine("1) Студенты ИТ-41 с баллом > 3.5");
        Console.WriteLine("Имя        | Группа | Возраст | Балл | Зачеты");

        var students = GetStudents();

        var query = from s in students
                    where s.Group == "ИТ-41" && s.GPA > 3.5
                    orderby s.Age
                    select s;

        foreach (var s in query)
            Console.WriteLine(s);
    }

    //2) method-синтаксис + анонимный тип
    static void Task2_MethodSyntax()
    {
        Console.WriteLine("\n2) Стипендии (синтаксис методов)");

        var students = GetStudents();

        var result = students
            .Where(s => true)
            .OrderByDescending(s => s.GPA)
            .Select(s => new
            {
                FullName = s.Name,
                Scholarship =
                    s.GPA >= 4.7 ? "Ректорская" :
                    s.GPA >= 4.5 ? "Повышенная" :
                    s.GPA >= 4.0 ? "Стандартная" : "Нет"
            });

        foreach (var item in result)
            Console.WriteLine($"{item.FullName}: {item.Scholarship}");
    }

    //3)агрегация и группировка
    static void Task3_Agreement()
    {
        Console.WriteLine("\n3) Статистика");

        var students = GetStudents();

        // группировка по группе, средний балл
        var groups = students
            .GroupBy(s => s.Group)
            .Select(g => new
            {
                GroupName = g.Key,
                AvgGPA = g.Average(s => s.GPA)
            })
            .OrderBy(g => g.GroupName);

        foreach (var g in groups)
            Console.WriteLine($"Группа {g.GroupName}: Средний балл {g.AvgGPA:F2}");

        // самый старший студент, сдавший ровно 45 зачетов
        var oldest = students
            .Where(s => s.PassedCredits == 45)
            .OrderByDescending(s => s.Age)
            .FirstOrDefault();

        if (oldest != null)
            Console.WriteLine($"\nСтудент с 45 зачетами: {oldest.Name} ({oldest.Age} год)");
        else
            Console.WriteLine("\nСтудентов с 45 зачетами нет.");
    }

    //4) LINQ to XML
    static void Task4_Xml()
    {
        Console.WriteLine("\n4) Отличники из XML");

        // создание XML
        XDocument doc = new XDocument(
            new XElement("university",
                new XElement("course",
                    new XAttribute("name", "Программирование"),
                    new XElement("student", new XAttribute("name", "Кирилл"),  new XAttribute("gra", "3.9")),
                    new XElement("student", new XAttribute("name", "Лариса"), new XAttribute("gra", "4.7")),
                    new XElement("student", new XAttribute("name", "Михаил"), new XAttribute("gra", "3.2")),
                    new XElement("student", new XAttribute("name", "Наталья"),new XAttribute("gra", "4.1"))
                )
            )
        );

        doc.Save("uni.xml");
        Console.WriteLine("Файл uni.xml сохранён.");

        // чтение и фильтрация
        XDocument loaded = XDocument.Load("uni.xml");

        var excellent = loaded
            .Descendants("student")
            .Where(s => (double)s.Attribute("gra")! > 4.0)
            .Select(s => s.Attribute("name")!.Value);

        foreach (var name in excellent)
            Console.WriteLine(name);
    }
}
