using System.Diagnostics;

namespace AoC23.Domain;

[Day(18)]
public class Day18Calculator : IDayCalculator
{
    public long CalculatePart1(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        return Fill(Dig(ParseDigPlan(lines, readFromColor: false))).Count;
    }

    public long CalculatePart2(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        return Area(ParseDigPlan(lines, readFromColor: true));
    }

    internal static long Area((Direction dir, int len)[] instructions)
    {
        var pos = (y: 0L, x: 0L);
        var points = new (long y, long x)[instructions.Length];
        var perimeter = 0L;

        for (var i = 0; i < instructions.Length; i++)
        {
            points[i] = pos;
            var instruction = instructions[i];
            var (dir, len) = instruction;
            pos = PosFromInstruction(pos, instruction);
            perimeter += len;
        }
        
        var area = 0L;

        for (var i = 0; i < points.Length; i++)
        {
            var nextI = (i + 1) % points.Length;
            var prevI = i - 1 < 0 ? points.Length - 1 : i - 1;
            area += points[i].y * (points[nextI].x - points[prevI].x);
        }

        area = Math.Abs(area) / 2;        
        area += perimeter / 2 + 1;

        return area;

        static (long y, long x) PosFromInstruction((long y, long x) pos, (Direction dir, int len) instruction)
        {
            var (dir, len) = instruction;
            var (dy, dx) = dir switch
            {
                Direction.N => (-len, 0),
                Direction.S => (+len, 0),
                Direction.W => (0, -len),
                Direction.E => (0, +len),
                _ => throw new UnreachableException()
            };

            return (pos.y + dy, pos.x + dx);
        }
    }

    internal static HashSet<(int y, int x)> Fill(HashSet<(int y, int x)> digged)
    {
        var minPos = (y: int.MaxValue, x: int.MaxValue);
        var maxPos = (y: int.MinValue, x: int.MinValue);
        foreach (var (y, x) in digged)
        {
            minPos.y = Math.Min(minPos.y, y);
            minPos.x = Math.Min(minPos.x, x);
            maxPos.y = Math.Max(maxPos.y, y);
            maxPos.x = Math.Max(maxPos.x, x);
        }

        if (Fill((-1, -1), out var filled)
            || Fill((-1, +1), out filled)
            || Fill((+1, +1), out filled)
            || Fill((+1, -1), out filled))
            return filled;

        throw new UnreachableException();

        bool Fill((int y, int x) pos, out HashSet<(int y, int x)> filled)
        {
            filled = new HashSet<(int y, int x)>(digged);

            var queue = new Queue<(int y, int x)>();
            queue.Enqueue(pos);

            while (queue.Count > 0)
            {
                var (y, x) = queue.Dequeue();
                if (filled.Contains((y, x)))
                    continue;

                if (y < minPos.y || y > maxPos.y || x < minPos.x || x > maxPos.x)
                    return false;

                filled.Add((y, x));

                queue.Enqueue((y - 1, x));
                queue.Enqueue((y + 1, x));
                queue.Enqueue((y, x - 1));
                queue.Enqueue((y, x + 1));
            }

            return true;
        }
    }

    internal static HashSet<(int y, int x)> Dig((Direction dir, int len)[] instruction)
    {
        var pos = (y: 0, x: 0);
        HashSet<(int y, int x)> digged = [pos];

        foreach (var (dir, len) in instruction)
        {
            var (dy, dx) = dir switch
            {
                Direction.N => (-1, 0),
                Direction.S => (+1, 0),
                Direction.W => (0, -1),
                Direction.E => (0, +1),
                _ => throw new UnreachableException()
            };

            for (var i = 0; i < len; i++)
            {
                pos = (pos.y + dy, pos.x + dx);
                digged.Add(pos);
            }
        }

        return digged;
    }

    internal static (Direction dir, int len)[] ParseDigPlan(string[] lines, bool readFromColor)
    {
        Func<string, (Direction, int)> parse = readFromColor
            ? ParseInstructionFromColor
            : ParseInstruction;

        return [.. lines.Select(parse)];

        static (Direction dir, int len) ParseInstruction(string line)
        {
            var parts = line.Split(' ');
            var dir = parts[0][0] switch
            {
                'U' => Direction.N,
                'D' => Direction.S,
                'L' => Direction.W,
                'R' => Direction.E,
                _ => throw new UnreachableException()
            };
            var len = parts[1].ToInt();
            return (dir, len);
        }

        static (Direction dir, int len) ParseInstructionFromColor(string line)
        {
            var parts = line.Split(' ');
            var dir = parts[2][7] switch
            {
                '3' => Direction.N,
                '1' => Direction.S,
                '2' => Direction.W,
                '0' => Direction.E,
                _ => throw new UnreachableException()
            };
            var len = parts[2][2..7].ToIntFromHex();
            return (dir, len);
        }


    }
    internal enum Direction
    {
        None,
        N,
        E,
        S,
        W
    }
}
