CREATE DATABASE CountriesHomework;
GO

USE CountriesHomework;
GO

-- 1. Таблица стран
CREATE TABLE Countries (
    Id        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name      NVARCHAR(100) NOT NULL,
    Area      FLOAT NOT NULL,
    Continent NVARCHAR(50) NOT NULL
);
GO

-- 2. Таблица столиц
CREATE TABLE Capitals (
    Id          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CountryId   INT NOT NULL,
    Name        NVARCHAR(100) NOT NULL,
    Population  BIGINT NOT NULL,
    FOREIGN KEY (CountryId) REFERENCES Countries (Id)
);
GO

-- 3. Таблица крупных городов
CREATE TABLE Cities (
    Id         INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CountryId  INT NOT NULL,
    Name       NVARCHAR(100) NOT NULL,
    Population BIGINT NOT NULL,
    FOREIGN KEY (CountryId) REFERENCES Countries (Id)
);
GO

-- Данные: страны
INSERT INTO Countries (Name, Area, Continent) VALUES
(N'Россия',       17098246, N'Европа'),
(N'Германия',       357022, N'Европа'),
(N'Франция',        551695, N'Европа'),
(N'Италия',         301340, N'Европа'),
(N'Испания',        505990, N'Европа'),
(N'Китай',         9596961, N'Азия'),
(N'Индия',         3287263, N'Азия'),
(N'Япония',         377975, N'Азия'),
(N'Южная Корея',    100210, N'Азия'),
(N'Египет',        1002450, N'Африка'),
(N'Нигерия',        923768, N'Африка'),
(N'ЮАР',           1221037, N'Африка');
GO

-- Данные: столицы
INSERT INTO Capitals (CountryId, Name, Population) VALUES
(1,  N'Москва',       12600000),
(2,  N'Берлин',        3600000),
(3,  N'Париж',         2100000),
(4,  N'Рим',           2800000),
(5,  N'Мадрид',        3300000),
(6,  N'Пекин',        21500000),
(7,  N'Нью-Дели',     32000000),
(8,  N'Токио',        13900000),
(9,  N'Сеул',          9700000),
(10, N'Каир',         21000000),
(11, N'Абуджа',        3600000),
(12, N'Претория',      2500000);
GO

-- Данные: крупные города
INSERT INTO Cities (CountryId, Name, Population) VALUES
(1,  N'Санкт-Петербург',  5400000),
(1,  N'Новосибирск',      1600000),
(6,  N'Шанхай',          24000000),
(6,  N'Гуанчжоу',        15000000),
(7,  N'Мумбаи',          20000000),
(7,  N'Бангалор',        12000000),
(8,  N'Осака',            2700000),
(9,  N'Пусан',            3400000),
(10, N'Александрия',      5200000),
(2,  N'Гамбург',          1800000),
(3,  N'Марсель',          1600000),
(4,  N'Милан',            3100000);
GO