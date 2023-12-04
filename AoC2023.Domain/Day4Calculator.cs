namespace AoC23.Domain;

[Day(4)]
public class Day4Calculator : IDayCalculator
{
    public int CalculatePart1(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        return lines.Select(card =>
        {
            var parts = card.Split(": ");
            var winningNumbers = Extensions.GetNumbers(parts[1].Split(" | ")[0]);
            var cardNumbers = Extensions.GetNumbers(parts[1].Split(" | ")[1]);
            return new { NumWinners = winningNumbers.Intersect(cardNumbers).Count() };
        })
                .Sum(card => card.NumWinners > 0 ? 1 << (card.NumWinners - 1) : 0);
    }

    public int CalculatePart2(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var cardInstances = new int[lines.Length];
        Array.Fill(cardInstances, 1);

        lines.Select((card, index) =>
        {
            var parts = card.Split(": ");
            var winningNumbers = Extensions.GetNumbers(parts[1].Split(" | ")[0]);
            var cardNumbers = Extensions.GetNumbers(parts[1].Split(" | ")[1]);
            return new { NumWinners = winningNumbers.Intersect(cardNumbers).Count(), Index = index };
        })
            .ToList()
            .ForEach(item =>
            {
                for (int j = item.Index + 1; j <= item.Index + item.NumWinners && j < cardInstances.Length; j++)
                {
                    cardInstances[j] += cardInstances[item.Index];
                }
            });

        return cardInstances.Sum();
    }
}
