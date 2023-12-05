namespace AoC23.Domain;

[Day(1)]
public class Day1Calculator : IDayCalculator
{
    public long CalculatePart1(string filePath)
    {
        return ProcessFile(filePath, false);
    }

    public long CalculatePart2(string filePath)
    {
        return ProcessFile(filePath, true);
    }


    private int ProcessFile(string filePath, bool includeWords)
    {
        var numbers = includeWords
            ? new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                  "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" }
            : new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        return File.ReadLines(filePath).Sum(line =>
        {
            var first = numbers.Select(num => new { Num = num, Index = line.IndexOf(num) })
                               .Where(x => x.Index != -1)
                               .OrderBy(x => x.Index).FirstOrDefault()?.Num;
            var last = numbers.Select(num => new { Num = num, Index = line.LastIndexOf(num) })
                              .Where(x => x.Index != -1)
                              .OrderByDescending(x => x.Index).FirstOrDefault()?.Num;

            return int.Parse(ConvertNumber(first) + ConvertNumber(last));
        });
    }

    private string? ConvertNumber(string? num) => num?.Length > 1 ? num[0].ToString() : num;

}

