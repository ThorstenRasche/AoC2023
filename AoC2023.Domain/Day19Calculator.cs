using System.Diagnostics;

namespace AoC23.Domain;

[Day(19)]
public class Day19Calculator : IDayCalculator
{
    public long CalculatePart1(string filePath)
    {
        var input = File.ReadAllLines(filePath);
        return Part1(input);        
    }

    public long CalculatePart2(string filePath)
    {
        var input = File.ReadAllLines(filePath);
        var res = Part2(input);
        return (long)res;
    }

    int Part1(string[] lines) => Sort(ParseRulesAndParts(lines)).accepted.Sum(p => p.TotalValue);
    ulong Part2(string[] lines) => Combinations(ParseRules(lines)).accepted;

    static (ulong accepted, ulong rejected) Combinations(Dictionary<string, Rules> rules)
    {
        var accepted = 0UL;
        var rejected = 0UL;

        var queue = new Queue<(string, PartCombi)>();
        queue.Enqueue(("in", new PartCombi(new(1, 4000), new(1, 4000), new(1, 4000), new(1, 4000))));

        while (queue.Count > 0)
        {
            var (ruleName, part) = queue.Dequeue();
            foreach (var (nextRuleName, nextPart) in rules[ruleName].NextRules(part))
                switch (nextRuleName)
                {
                    case "A": accepted += nextPart.Count; break;
                    case "R": rejected += nextPart.Count; break;
                    default: queue.Enqueue((nextRuleName, nextPart)); break;
                }
        }

        return (accepted, rejected);
    }

    static (Part[] accepted, Part[] rejected) Sort((Dictionary<string, Rules>, Part[]) input)
    {
        var accepted = new List<Part>();
        var rejected = new List<Part>();
        var (rules, parts) = input;

        foreach (var part in parts)
            for (var ruleName = rules["in"].NextRule(part); ; ruleName = rules[ruleName].NextRule(part))
                if (ruleName == "A")
                {
                    accepted.Add(part);
                    break;
                }
                else if (ruleName == "R")
                {
                    rejected.Add(part);
                    break;
                }

        return ([.. accepted], [.. rejected]);
    }

    static (Dictionary<string, Rules>, Part[]) ParseRulesAndParts(string[] lines)
    {
        var ySplit = lines.Select((l, y) => (l, y)).First(l => l.l == "").y;

        var rules = ParseRules(lines);
        Part[] parts = [.. lines[(ySplit + 1)..].Select(ParsePart)];

        return (rules, [.. parts]);

        static Part ParsePart(string line)
        {
            var split = line[1..^1].Split(",");
            var x = split[0][2..].ToInt();
            var m = split[1][2..].ToInt();
            var a = split[2][2..].ToInt();
            var s = split[3][2..].ToInt();
            return new Part(x, m, a, s);
        }
    }

    static Dictionary<string, Rules> ParseRules(string[] lines)
    {
        var ySplit = lines.Select((l, y) => (l, y)).First(l => l.l == "").y;

        return new Dictionary<string, Rules>(lines[..ySplit].Select(ParseRule));

        static KeyValuePair<string, Rules> ParseRule(string line)
        {
            var nameSplit = line.Split("{");
            var rulesParts = nameSplit[1].Split(",");
            Rule[] rules = [.. rulesParts[..^1].Select(ParseRulePart)];
            return new(nameSplit[0], new Rules(rules, rulesParts[^1][..^1]));
        }

        static Rule ParseRulePart(string line)
        {
            var nextRuleSplit = line.Split(":");
            var part = nextRuleSplit[0][0];
            var @operator = nextRuleSplit[0][1];
            var value = nextRuleSplit[0][2..].ToInt();
            return new Rule(part, @operator, value, nextRuleSplit[^1]);
        }
    }
}

record Rules(Rule[] rules, string lastRule)
{
    public string NextRule(Part part) => rules.FirstOrDefault(r => r.Applies(part))?.nextRule ?? lastRule;

    public IEnumerable<(string, PartCombi)> NextRules(PartCombi part)
    {
        PartCombi? nextCombi = part;
        foreach (var rule in rules)
        {
            var (truePart, falsePart) = rule.Split(nextCombi!);
            if (truePart is not null)
                yield return (rule.nextRule, truePart);
            nextCombi = falsePart;
            if (nextCombi is null)
                yield break;
        }
        if (nextCombi is not null)
            yield return (lastRule, nextCombi);
    }
}
record Rule(char part, char @operator, int value, string nextRule)
{
    public bool Applies(Part p) => (part, @operator) switch
    {
        ('x', '<') when p.x < value => true,
        ('x', '>') when p.x > value => true,
        ('m', '<') when p.m < value => true,
        ('m', '>') when p.m > value => true,
        ('a', '<') when p.a < value => true,
        ('a', '>') when p.a > value => true,
        ('s', '<') when p.s < value => true,
        ('s', '>') when p.s > value => true,
        _ => false
    };

    public (PartCombi? truePart, PartCombi? falsePart) Split(PartCombi p)
    {
        var (left, right) = p.Split(part, @operator, value);
        return @operator is '<' ? (left, right) : (right, left);
    }
}
record Part(int x, int m, int a, int s)
{
    public int TotalValue => x + m + a + s;
}
record PartCombi(PartRange x, PartRange m, PartRange a, PartRange s)
{
    public ulong Count => x.Count * m.Count * a.Count * s.Count;

    public (PartCombi?, PartCombi?) Split(char part, char @operator, int value) => part switch
    {
        'x' => SplitX(@operator, value),
        'm' => SplitM(@operator, value),
        'a' => SplitA(@operator, value),
        's' => SplitS(@operator, value),
        _ => throw new UnreachableException()
    };

    (PartCombi?, PartCombi?) SplitX(char @operator, int value)
    {
        var (x1, x2) = x.Split(@operator, value);
        return (x1 is null ? null : new PartCombi(x1, m, a, s), x2 is null ? null : new PartCombi(x2, m, a, s));
    }
    (PartCombi?, PartCombi?) SplitM(char @operator, int value)
    {
        var (m1, m2) = m.Split(@operator, value);
        return (m1 is null ? null : new PartCombi(x, m1, a, s), m2 is null ? null : new PartCombi(x, m2, a, s));
    }
    (PartCombi?, PartCombi?) SplitA(char @operator, int value)
    {
        var (a1, a2) = a.Split(@operator, value);
        return (a1 is null ? null : new PartCombi(x, m, a1, s), a2 is null ? null : new PartCombi(x, m, a2, s));
    }
    (PartCombi?, PartCombi?) SplitS(char @operator, int value)
    {
        var (s1, s2) = s.Split(@operator, value);
        return (s1 is null ? null : new PartCombi(x, m, a, s1), s2 is null ? null : new PartCombi(x, m, a, s2));
    }
}
record PartRange(int start, int end)
{
    public ulong Count => (ulong)end - (ulong)start + 1;

    public (PartRange?, PartRange?) Split(char @operator, int value)
    {
        if (value < start) return (null, this);
        if (value > end) return (this, null);
        return @operator is '<'
            ? (new PartRange(start, value - 1), new PartRange(value, end))
            : (new PartRange(start, value), new PartRange(value + 1, end));
    }


}
