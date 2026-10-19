USE CountriesHomework;
GO

-- 1. Топ-3 стран по населению
CREATE OR ALTER PROCEDURE sp_Top3CountriesByPop
AS
BEGIN
    SELECT TOP 3 c.Name, cap.Population
    FROM Countries c 
    JOIN Capitals cap ON c.Id = cap.CountryId
    ORDER BY cap.Population DESC;
END;
GO

-- 2. Топ-3 столиц по населению
CREATE OR ALTER PROCEDURE sp_Top3CapitalsByPop
AS
BEGIN
    SELECT TOP 3 Name, Population 
    FROM Capitals 
    ORDER BY Population DESC;
END;
GO

-- 3. Страна с самым большим населением
CREATE OR ALTER PROCEDURE sp_CountryWithMaxPop
AS
BEGIN
    SELECT TOP 1 c.Name, cap.Population
    FROM Countries c 
    JOIN Capitals cap ON c.Id = cap.CountryId
    ORDER BY cap.Population DESC;
END;
GO

-- 4. Город с самым большим населением
CREATE OR ALTER PROCEDURE sp_CityWithMaxPop
AS
BEGIN
    SELECT TOP 1 Name, Population 
    FROM Cities 
    ORDER BY Population DESC;
END;
GO