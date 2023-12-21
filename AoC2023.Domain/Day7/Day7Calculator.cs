using System.Numerics;
using AoC23.Domain;

namespace AoC2023.Domain.Day7;

[Day(7)]
public partial class Day7Calculator : IDayCalculator
{
    public List<string> SplitByNewline(string input, bool blankLines = false, bool shouldTrim = true)
    {
        return input
           .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
           .Where(s => blankLines || !string.IsNullOrWhiteSpace(s))
           .Select(s => shouldTrim ? s.Trim() : s)
           .ToList();
    }

    public long CalculatePart1(string filePath)
    {
        var input = File.ReadAllText(filePath);
        List<Part1Hand> hands = new();
        foreach (var h in SplitByNewline(input))
        {
            var h2 = h.Split();
            hands.Add(new(h2[0], long.Parse(h2[1])));
        }


        List<Part1Hand> p1Hands = new(hands);

        p1Hands.Sort();

        long total = 0;

        for (int i = 0; i < p1Hands.Count; i++)
        {
            total += (i + 1) * p1Hands[i].bid;
        }
        return total;
    }

    public long CalculatePart2(string filePath)
    {
        var input = File.ReadAllText(filePath);
        List<Part2Hand> twoHands = new();
        foreach (var h in SplitByNewline(input))
        {
            var h2 = h.Split();

            twoHands.Add(new(h2[0], long.Parse(h2[1])));
        }
        List<Part2Hand> p2Hands = new(twoHands);

        p2Hands.Sort();

        long total = 0;

        for (int i = 0; i < p2Hands.Count; i++)
        {
            total += (i + 1) * p2Hands[i].bid;
        }
        return total;


    }

    BigInteger Part1Points(string hand) =>
        (PatternValue(hand) << 64) + CardValue(hand, "123456789TJQKA");

    BigInteger Part2Points(string hand)
    {
        var replacement = (
            from ch in hand
            where ch != 'J'
            group ch by ch into g
            orderby g.Count() descending
            select g.Key
        ).FirstOrDefault('J');

        var cv = CardValue(hand, "J123456789TQKA");
        var pv = PatternValue(hand.Replace('J', replacement));
        return (pv << 64) + cv;
    }


    BigInteger CardValue(string hand, string cardOrder) =>
         new BigInteger(hand.Select(ch => (byte)cardOrder.IndexOf(ch)).Reverse().ToArray());

    BigInteger PatternValue(string hand) =>
        new BigInteger(hand.Select(ch => (byte)hand.Count(x => x == ch)).Order().ToArray());
}

