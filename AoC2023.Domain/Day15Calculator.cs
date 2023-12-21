using System.Text.RegularExpressions;

namespace AoC23.Domain;

[Day(15)]
public class Day15Calculator : IDayCalculator
{
    public long CalculatePart1(string filePath)
    {
        List<string> input = File.ReadAllLines(filePath).ToList();
        return input[0].Split(',').Sum(CalculateHash);
    }

    public long CalculatePart2(string filePath)
    {
        List<string> input = File.ReadAllLines(filePath).ToList();
        var labelRegex = new Regex(@"[^-=]+");
        var boxes = Enumerable.Range(0, 256).ToDictionary(n => n, _ => new List<Lens>());

        foreach (var step in input[0].Split(','))
        {
            var label = labelRegex.Match(step).Value;
            var instruction = step[label.Length..];
            var boxId = CalculateHash(label);

            PerformInstruction(boxes[boxId], label, instruction);
        }

        return boxes.Sum(GetBoxFocusingPower);
    }

    internal static int CalculateHash(string valueToHash)
    {
        return valueToHash.Aggregate(0, PerformHashStep);
    }
    internal static int PerformHashStep(int current, char value)
    {
        return (current + value) * 17 % 256;
    }
    internal static void PerformInstruction(List<Lens> lenses, string label, string instruction)
    {
        switch (instruction[0])
        {
            case '-':
                lenses.RemoveAll(lens => lens.Label == label);
                break;

            case '=':
                var focalLength = int.Parse(instruction[1..]);
                var existingLens = lenses.FirstOrDefault(lens => lens.Label == label);
                if (existingLens != null)
                {
                    existingLens.FocalLength = focalLength;
                }
                else
                {
                    lenses.Add(new Lens { Label = label, FocalLength = focalLength });
                }
                break;

            default:
                throw new Exception($"Unexpected instruction found: {instruction}");
        }
    }

    internal static int GetBoxFocusingPower(KeyValuePair<int, List<Lens>> box)
    {
        var focusingPower = 0;
        for (var i = 0; i < box.Value.Count; ++i)
        {
            focusingPower += (1 + box.Key) * (1 + i) * box.Value[i].FocalLength;
        }

        return focusingPower;
    }
    internal class Lens
    {
        public string Label { get; set; }
        public int FocalLength { get; set; }
    }
}
