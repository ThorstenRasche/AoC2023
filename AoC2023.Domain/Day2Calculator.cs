using System.Drawing;

namespace AoC23.Domain;

[Day(2)]
public class Day2Calculator : IDayCalculator
{
    public int CalculatePart1(string filePath)
    {
        return Extensions.CalculateSumOfValidGames(filePath);
    }

    public int CalculatePart2(string filePath)
    {
        return File.ReadLines(filePath)
                   .Select(line => line.Split(": ")[1].Split("; ")
                                       .SelectMany(s => s.Split(", "))
                                       .Select(s => (Quantity: int.Parse(s.Split(' ')[0]), Color: s.Split(' ')[1]))
                                       .ToLookup(s => s.Color, s => s.Quantity))
                   .Select(draws => new
                   {
                       MaxRed = draws["red"].DefaultIfEmpty(0).Max(),
                       MaxGreen = draws["green"].DefaultIfEmpty(0).Max(),
                       MaxBlue = draws["blue"].DefaultIfEmpty(0).Max()
                   })
                   .Sum(game => game.MaxRed * game.MaxGreen * game.MaxBlue);
    }
}
