using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AoC23.Domain;

namespace AoC2023.Domain.Day5;

[Day(5)]
public partial class Day5Calculator : IDayCalculator
{
    public long CalculatePart1(string filePath)
    {

        var lines2 = File.ReadAllLines(filePath);
        var seeds = lines2[0].ParseSeeds();
        var maps = lines2.ParseMaps();

        return seeds.Select(seed => maps.Aggregate(seed, (currentSeed, map) =>
            map.Where(range => range.start <= currentSeed && currentSeed < range.end)
               .Select(range => range.destination + (currentSeed - range.start))
               .FirstOrDefault(currentSeed)
        )).Min();
    }

    public long CalculatePart2(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var seedRanges = ParseSeedRanges();
        var maps = ParseMaps(lines);

        TraverseMaps(lines, MoveToNextCategory);

        return seedRanges.Min(x => x.Start);

        void MoveToNextCategory(MapRanges map) =>
            seedRanges = seedRanges.SelectMany(x => map.MapRange(x)).ToList();

        List<Range> ParseSeedRanges()
        {
            var n = lines[0].Split(": ")[1].ToLongs(" ");
            List<Range> ranges = [];
            for (int i = 0; i < n.Length; i += 2)
            {
                ranges.Add(new Range(n[i], n[i + 1]));
            }
            return ranges;
        }
    }

    static void TraverseMaps(string[] lines, Action<MapRanges> action)
    {
        var maps = ParseMaps(lines);
        var mapping = "seed";
        while (mapping != "location")
        {
            var map = maps[mapping];
            action(map);
            mapping = map.To;
        }
    }

    static Dictionary<string, MapRanges> ParseMaps(string[] lines)
    {
        Dictionary<string, MapRanges> maps = [];
        for (var i = 2; i < lines.Length; i++)
        {
            var (id, ranges) = ParseMap(ref i);
            maps[id] = ranges;
        }
        return maps;

        (string id, MapRanges ranges) ParseMap(ref int i)
        {
            var mapping = lines[i].Split(" ")[0].Split("-to-");
            i++;
            var ranges = new List<Ranges>();
            while (i < lines.Length && lines[i] != "")
            {
                var n = lines[i].ToLongs(" ");
                ranges.Add(new Ranges(n[0], new Range(n[1], n[2])));
                i++;
            }

            return (mapping[0], new MapRanges(mapping[1], [.. ranges.OrderBy(x => x.Range.Start)]));
        }
    }


}

