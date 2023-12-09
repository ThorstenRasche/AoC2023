namespace AoC2023.Domain.Day5;

public partial class Day5Calculator
{
    record struct Ranges(long Destination, Range Range)
    {
        public readonly long Map(long value) =>
            value >= Range.Start && value < Range.End
            ? Destination + (value - Range.Start)
            : value;

        public readonly Range MapRange(Range range) => new(Destination + (range.Start - Range.Start), range.Length);
    }


}

