using System.ComponentModel;
using static AoC23.Domain.Day10Calculator.Day10;

namespace AoC23.Domain;

[Day(17)]
public class Day17Calculator : IDayCalculator
{
    int[][] map;
    PriorityQueue<Path, int>? queue;
    HashSet<string>? visited;
    public long CalculatePart1(string filePath)
    {
        map = File.ReadAllLines(filePath)
            .Select(s => 
                s.Select(c => 
                    int.Parse(c.ToString()))
                .ToArray())
            .ToArray();

        queue = new PriorityQueue<Path, int>();
        visited = new HashSet<string>();

        queue.Enqueue(new Path(new(0, 0), Direction.Right, 0), 0);

        var totalHeat = 0;

        while (queue.Count > 0)
        {
            var path = queue.Dequeue();

            if (path.Position.Row == map.Length - 1 && path.Position.Col == map[0].Length - 1)
            {
                totalHeat = path.Heat;
                break;
            }

            if (path.StraightLineLength < 3)
            {
                TryMove(path, path.Direction);
            }

            TryMove(path, path.Direction.TurnLeft());
            TryMove(path, path.Direction.TurnRight());
        }

        return totalHeat;
    }

    public long CalculatePart2(string filePath)
    {
        map = File.ReadAllLines(filePath)
            .Select(s => 
                s.Select(c => 
                    int.Parse(c.ToString()))
                .ToArray())
            .ToArray();

        queue = new PriorityQueue<Path, int>();
        visited = new HashSet<string>();
        queue.Clear();
        queue.Enqueue(new Path(new(0, 0), Direction.Right, 0), 0);
        queue.Enqueue(new Path(new(0, 0), Direction.Down, 0), 0);
        var totalHeat = 0;
        visited.Clear();
        while (queue.Count > 0)
        {
            var path = queue.Dequeue();
            if (path.Position.Row == map.Length - 1 && path.Position.Col == map[0].Length - 1 && path.StraightLineLength >= 4)
            {
                totalHeat = path.Heat;
                break;
            }

            if (path.StraightLineLength < 10)
            {
                TryMove(path, path.Direction);
            }

            if (path.StraightLineLength >= 4)
            {
                TryMove(path, path.Direction.TurnLeft());
                TryMove(path, path.Direction.TurnRight());
            }
        }

        return totalHeat;
    }

    void TryMove(Path path, Direction direction)
    {
        var candidate = new Path(path.Position.Move(direction), direction, direction == path.Direction ? path.StraightLineLength + 1 : 1);

        if (candidate.Position.Row < 0 || candidate.Position.Row >= map.Length ||
            candidate.Position.Col < 0 || candidate.Position.Col >= map[0].Length)
        {
            return;
        }

        var key = $"{candidate.Position.Row}," +
            $"{candidate.Position.Col}," +
            $"{candidate.Direction.Row}," +
            $"{candidate.Direction.Col}," +
            $"{candidate.StraightLineLength}";

        if (visited!.Contains(key))
        {
            return;
        }

        visited.Add(key);

        candidate.Heat = path.Heat + map[candidate.Position.Row][candidate.Position.Col];
        queue!.Enqueue(candidate, candidate.Heat);
    }

    internal class Path(Position position, Direction direction, int straightLineLength)
    {
        public readonly Position Position = position;
        public readonly Direction Direction = direction;
        public readonly int StraightLineLength = straightLineLength;
        public int Heat { get; set; }
    }

    internal class Direction(int row, int col)
    {
        public readonly int Row = row;
        public readonly int Col = col;

        public Direction TurnLeft()
        {
            return new Direction(-Col, Row);
        }

        public Direction TurnRight()
        {
            return new Direction(Col, -Row);
        }

        public static Direction Up = new(-1, 0);
        public static Direction Down = new(1, 0);
        public static Direction Left = new(0, -1);
        public static Direction Right = new(0, 1);
    }

    internal class Position(int row, int col)
    {
        public readonly int Row = row;
        public readonly int Col = col;

        public Position Move(Direction dir)
        {
            return new Position(Row + dir.Row, Col + dir.Col);
        }
    }

}
