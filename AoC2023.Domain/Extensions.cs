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

    public static IEnumerable<long> ParseSeeds(this string line)
    {
        return line.Split(": ")[1].Split().Select(long.Parse);
    }

    public static IEnumerable<IEnumerable<(long start, long end, long destination)>> ParseMaps(this string[] lines)
    {
        var currentMap = new List<(long start, long end, long destination)>();
        bool isMapStarted = false;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (isMapStarted)
                {
                    yield return currentMap;
                    currentMap = new List<(long start, long end, long destination)>();
                    isMapStarted = false;
                }
            }
            else if (line.EndsWith("map:"))
            {
                isMapStarted = true;
            }
            else if (isMapStarted)
            {
                var parts = line.Split(' ').Select(long.Parse).ToArray();
                currentMap.Add((parts[1], parts[1] + parts[2], parts[0]));
            }
        }

        if (isMapStarted)
        {
            yield return currentMap;
        }
    }

    public static long[] ToLongs(this string input, string splitter)
    {
        return input.Split(splitter, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
    }

    static long NextTerm(IEnumerable<long> sequence)
    {
        var differences = sequence.Zip(sequence.Skip(1), (a, b) => b - a).ToList();

        if (differences.All(d => d == 0) || !differences.Any())
        {
            return sequence.Last();
        }
        return sequence.Last() + NextTerm(differences);
    }
    public static long ProcessFile(this string filePath, bool reverse) => File.ReadAllLines(filePath)
                   .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(long.Parse))
                   .Select(numbers => reverse ? numbers.Reverse() : numbers)
                   .Select(NextTerm)
                   .Sum();

    public static long PathLength(this char[] moves, Dictionary<string, (string Left, string Right)> nodes, string startNode, string targetSuffix)
    {
        var extendedMoves = Enumerable.Repeat(moves, int.MaxValue / moves.Length).SelectMany(x => x).GetEnumerator();
        var currentNode = startNode;
        long stepCount = 0;
        while (!currentNode.EndsWith(targetSuffix))
        {
            extendedMoves.MoveNext();
            var move = extendedMoves.Current;
            currentNode = move == 'L' ? nodes[currentNode].Left : nodes[currentNode].Right;
            stepCount++;
        }
        return stepCount;
    }

    public static long LCM(long a, long b) => (a / GCD(a, b)) * b;
    public static long GCD(this long a, long b) => b == 0 ? a : GCD(b, a % b);


}