using System.Drawing;
using System.Text.RegularExpressions;

namespace AoC23.Domain;

[Day(6)]
public class Day6Calculator : IDayCalculator
{
    public long CalculatePart1(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var regex = new Regex(@"\d+");
        var times = regex.Matches(lines[0])
            .Select(r => 
                long.Parse(r.Value))
            .ToList();
        var distances = regex.Matches(lines[1])
            .Select(r => 
                long.Parse(r.Value))
            .ToList();

        var marginOfError = 1l;

        for (var i = 0; i < times.Count; ++i)
        {
            marginOfError *= CalculateNumberOfWinningTimes(times[i], distances[i]);
        }

        return marginOfError;
    }

    public long CalculatePart2(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var time = long.Parse(lines[0]
            .Split(':')[1]
            .Replace(" ", ""));
        var distance = long.Parse(lines[1]
            .Split(':')[1]
            .Replace(" ", ""));

        return CalculateNumberOfWinningTimes(time, distance);
    }

    private static long CalculateNumberOfWinningTimes(long time, long distance)
    {
        var firstWinningTime = (long)Math
            .Ceiling((time - Math
                .Sqrt(time * time - 4 * (distance + 1))) / 2);

        return time - 2 * firstWinningTime + 1;
    }
}
