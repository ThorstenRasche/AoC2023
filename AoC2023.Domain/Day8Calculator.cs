using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AoC23.Domain;

[Day(8)]
public class Day8Calculator : IDayCalculator
{
    private const string StartNode = "AAA";
    private const string EndNodeSuffix1 = "ZZZ";
    private const string NodePattern1 = @"(\w+) = \((\w+), (\w+)\)";

    private const string NodePattern2 = @"(\w+) = \((\w+), (\w+)\)";
    private const char StartNodeSuffix = 'A';
    private const string EndNodeSuffix2 = "Z";

    public long CalculatePart1(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var moves = lines.First().ToCharArray();
        var regex = new Regex(NodePattern1);
        var nodes = lines.Skip(2)
                         .Select(line => regex.Match(line))
                         .Where(match => match.Success)
                         .ToDictionary(
                             match => match.Groups[1].Value,
                             match => (match.Groups[2].Value, match.Groups[3].Value));
        return moves.PathLength(nodes, StartNode, EndNodeSuffix1);
    }

    public long CalculatePart2(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var moves = lines[0].ToCharArray();
        var regex = new Regex(NodePattern2);
        var nodes = lines.Skip(2)
                         .Select(line => regex.Match(line))
                         .Where(match => match.Success)
                         .ToDictionary(
                             match => match.Groups[1].Value,
                             match => (match.Groups[2].Value, match.Groups[3].Value));
        return nodes.Keys
                    .Where(key => key.EndsWith(StartNodeSuffix))
                    .Select(startNode => moves.PathLength(nodes, startNode, EndNodeSuffix2))
                    .Aggregate(Extensions.LCM);
    }
    
    
}

