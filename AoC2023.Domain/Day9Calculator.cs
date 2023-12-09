using System.Text.RegularExpressions;

namespace AoC23.Domain;

[Day(9)]
public class Day9Calculator : IDayCalculator
{
    public long CalculatePart1(string filePath)
    {
        return filePath.ProcessFile(false);
    }

    public long CalculatePart2(string filePath)
    {   
        return filePath.ProcessFile(true);
    }
    
}
