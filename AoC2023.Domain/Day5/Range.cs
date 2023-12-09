namespace AoC2023.Domain.Day5;

public partial class Day5Calculator
{
    record struct Range(long Start, long Length)
    {
        public readonly long End => Start + Length;
    }


}

