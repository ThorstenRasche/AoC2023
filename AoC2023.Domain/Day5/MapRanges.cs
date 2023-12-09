namespace AoC2023.Domain.Day5;

public partial class Day5Calculator
{
    record struct MapRanges(string To, Ranges[] Ranges)
    {
        public readonly long Map(long value)
        {
            var pos = Array.BinarySearch(Ranges, new Ranges(0, new Range(value, 0)), RangesComparer.Instance);

            if (pos < 0)
            {
                pos = ~pos;
                pos -= 1;
            }

            return pos < 0 || pos >= Ranges.Length
                ? value
                : Ranges[pos].Map(value);
        }

        public readonly IEnumerable<Range> MapRange(Range range)
        {
            var pos = Array.BinarySearch(Ranges, new Ranges(0, range), RangesComparer.Instance);

            if (pos < 0)
            {
                pos = ~pos;
                pos -= 1;
            }

            if (pos < 0)
            {
                var splitRange = Ranges[0].Range;

                if (range.End < splitRange.Start)
                {
                    yield return range;
                }
                else
                {
                    yield return new Range(range.Start, splitRange.Start - range.Start);

                    foreach (var m in MapRange(new Range(splitRange.Start, range.End - splitRange.Start)))
                    {
                        yield return m;
                    }
                }
            }
            else if (pos >= Ranges.Length)
            {
                var splitRange = Ranges[^1].Range;
                if (range.Start > splitRange.End)
                {

                    yield return range;
                }
                else
                {

                    yield return new Range(splitRange.End, range.End - splitRange.End);

                    foreach (var m in MapRange(new Range(range.Start, splitRange.End - range.Start)))
                    {
                        yield return m;
                    }
                }
            }
            else
            {
                var splitRange = Ranges[pos].Range;
                if (range.End <= splitRange.End)
                {
                    yield return Ranges[pos].MapRange(range);
                }
                else if (range.Start >= splitRange.End)
                {
                    if (pos + 1 < Ranges.Length && range.End > Ranges[pos + 1].Range.Start)
                    {
                        var next = Ranges[pos + 1].Range;
                        yield return new Range(range.Start, next.Start - range.Start);

                        foreach (var m in MapRange(new Range(next.Start, range.End - next.Start)))
                        {
                            yield return m;
                        }
                    }
                    else
                    {
                        yield return range;
                    }
                }
                else
                {
                    var sr = new Range(range.Start, splitRange.End - range.Start);

                    yield return Ranges[pos].MapRange(sr);

                    foreach (var m in MapRange(new Range(splitRange.End, range.End - splitRange.End)))
                    {
                        yield return m;
                    }
                }
            }
        }
    }


}

