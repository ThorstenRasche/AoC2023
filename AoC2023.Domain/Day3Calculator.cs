using System.Text.RegularExpressions;

namespace AoC23.Domain;

[Day(3)]
public class Day3Calculator : IDayCalculator
{
    public int CalculatePart1(string filePath)
    {
        var rows = File.ReadAllLines(filePath);
        return rows.SelectMany((row, i) => Regex.Matches(row, @"\d+")
                                                 .Select(match => new { Value = int.Parse(match.Value), match.Index, RowIndex = i }))
                   .Where(x => Extensions.BordersSpecialCharacter(x.Index, x.Index + x.Value.ToString().Length - 1, x.RowIndex, rows))
                   .Sum(x => x.Value);
    }

    public int CalculatePart2(string filePath)
    {
        return File.ReadLines(filePath)
               .SelectMany((line, rowIndex) =>
                   Regex.Matches(line, @"\d+")
                        .Cast<Match>()
                        .SelectMany(match => Extensions.GetBorderingGearLocations(File.ReadLines(filePath).ToArray(), match, rowIndex)))
               .GroupBy(g => (g.row, g.col))
               .Where(g => g.Count() == 2)
               .Sum(g => g.Select(gl => gl.partNumber).Aggregate((a, b) => a * b));
    }
}
