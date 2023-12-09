namespace AoC2023.Domain.Day5;

public partial class Day5Calculator
{
    class RangesComparer : Comparer<Ranges>
    {
        public override int Compare(Ranges x, Ranges y) => Comparer<long>.Default.Compare(x.Range.Start, y.Range.Start);
        public static RangesComparer Instance { get; } = new RangesComparer();
    }


}

