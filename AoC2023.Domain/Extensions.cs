using System.Text.RegularExpressions;

namespace AoC23.Domain;

public static class Extensions
{
    public static int CalculateSumOfValidGames(string filePath)
    {
        var maxValues = new Dictionary<string, int>
        {
            ["red"] = 12,
            ["green"] = 13,
            ["blue"] = 14
        };

        return File.ReadLines(filePath)
                   .Select(line => new
                   {
                       GameNumber = int.Parse(line.Split(new[] { ':' }, 2)[0].Split(new[] { ' ' }, 2)[1]),
                       Draws = ParseDraws(line.Split(new[] { ':' }, 2).ElementAtOrDefault(1) ?? string.Empty)
                   })
                   .Where(game => game.Draws.All(draw => draw.Max() <= maxValues.GetValueOrDefault(draw.Key, int.MaxValue)))
                   .Sum(game => game.GameNumber);
    }

    public static ILookup<string, int> ParseDraws(string drawsPart)
    {
        return drawsPart.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .SelectMany(s => s.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        .Select(s => s.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        .ToLookup(
                            parts => parts.Length > 1 ? parts[1] : string.Empty,
                            parts => parts.Length > 0 ? int.Parse(parts[0]) : 0
                        );
    }

    public static bool BordersSpecialCharacter(int startingIndex, int endingIndex, int rowIndex, string[] rows)
    {
        var searchCells = GetSearchCells(startingIndex, endingIndex, rowIndex, rows.Length, rows[0].Length);
        return searchCells.Any(sc => !char.IsNumber(rows[sc.row][sc.col]) && rows[sc.row][sc.col] != '.');
    }

    public static List<(int row, int col)> GetSearchCells(int startingIndex, int endingIndex, int rowIndex, int numRows, int numColumns)
    {
        var searchCells = new List<(int row, int col)>();
        for (int i = rowIndex - 1; i <= rowIndex + 1; i++)
        {
            for (int j = startingIndex - 1; j <= endingIndex + 1; j++)
            {
                if (i >= 0 && i < numRows && j >= 0 && j < numColumns)
                {
                    searchCells.Add((i, j));
                }
            }
        }
        return searchCells;
    }


    public static IEnumerable<(int row, int col, int partNumber)> GetBorderingGearLocations(string[] rows, Match match, int rowIndex)
    {
        var startCol = match.Index;
        var endCol = startCol + match.Length;
        var partNumber = int.Parse(match.Value);

        return Enumerable.Range(startCol - 1, match.Length + 2)
                         .SelectMany(col => Enumerable.Range(rowIndex - 1, 3), (col, row) => (row, col))
                         .Where(rc => rc.row >= 0 && rc.row < rows.Length && rc.col >= 0 && rc.col < rows[0].Length && rows[rc.row][rc.col] == '*')
                         .Select(rc => (rc.row, rc.col, partNumber));
    }

    public static List<int> GetNumbers(string input)
    {
        return Regex.Matches(input, @"\d+")
                    .Select(match => int.Parse(match.Value))
                    .ToList();
    }

}