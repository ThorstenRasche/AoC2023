using System.Diagnostics;
using System.Globalization;
using System.Numerics;

namespace AoC23.Domain;

[Day(16)]
public class Day16Calculator : IDayCalculator
{
    public long CalculatePart1(string filePath)
    {
        var input = File.ReadAllLines(filePath).Select(l => l.ToCharArray()).ToArray();
        return CountTiles(input, 0, -1, Directions.Right);
    }

    public long CalculatePart2(string filePath)
    {
        return T(filePath).Max(t => t);
    }

    private int[] T(string filePath)
    {
        var input = File.ReadAllLines(filePath).Select(l => l.ToCharArray()).ToArray();
        
        var tasks = new List<Task<int>>();

        for (var row = 0; row < input.Length; row++)
        {
            var currentRow = row;
            tasks.Add(Task.Run(() => CountTiles(input, currentRow, -1, Directions.Right)));
            tasks.Add(Task.Run(() => CountTiles(input, currentRow, input[currentRow].Length, Directions.Left)));
        }

        for (var col = 0; col < input[0].Length; col++)
        {
            var currentCol = col;
            tasks.Add(Task.Run(() => CountTiles(input, -1, currentCol, Directions.Down)));
            tasks.Add(Task.Run(() => CountTiles(input, input.Length, currentCol, Directions.Up)));
        }

        return Task.WhenAll(tasks).Result;
    }

    static int CountTiles(char[][] map, int startRow, int startColumn, Directions startDirection)
    {
        var tiles = new Dictionary<(int row, int col), Directions>
        {
            [(startRow, startColumn)] = Directions.None
        };

        var beams = new Queue<(int Row, int Column, Directions Direction)>();
        beams.Enqueue((startRow, startColumn, startDirection));

        while (beams.TryDequeue(out var beam))
        {
            if (tiles.TryGetValue((beam.Row, beam.Column), out Directions tileDirections) &&
                tileDirections.HasFlag(beam.Direction))
            {
                continue;
            }

            tiles[(beam.Row, beam.Column)] = tileDirections | beam.Direction;

            var (row, col) = beam.Direction switch
            {
                Directions.Up => (beam.Row - 1, beam.Column),
                Directions.Down => (beam.Row + 1, beam.Column),
                Directions.Left => (beam.Row, beam.Column - 1),
                Directions.Right => (beam.Row, beam.Column + 1),
                _ => throw new Exception("Invalid direction")
            };

            if (row < 0 || row >= map.Length || col < 0 || col >= map[row].Length)
            {
                continue;
            }

            beam = beam with { Row = row, Column = col };

            switch (map[row][col])
            {
                case '/':
                    beams.Enqueue(beam with
                    {
                        Direction = beam.Direction switch
                        {
                            Directions.Up => Directions.Right,
                            Directions.Down => Directions.Left,
                            Directions.Left => Directions.Down,
                            Directions.Right => Directions.Up,
                            _ => throw new Exception("Invalid direction")
                        }
                    });
                    break;
                case '\\':
                    beams.Enqueue(beam with
                    {
                        Direction = beam.Direction switch
                        {
                            Directions.Up => Directions.Left,
                            Directions.Down => Directions.Right,
                            Directions.Left => Directions.Up,
                            Directions.Right => Directions.Down,
                            _ => throw new Exception("Invalid direction")
                        }
                    });
                    break;
                case '-' when beam.Direction is Directions.Up or Directions.Down:
                    beams.Enqueue(beam with { Direction = Directions.Left });
                    beams.Enqueue(beam with { Direction = Directions.Right });
                    break;
                case '|' when beam.Direction is Directions.Left or Directions.Right:
                    beams.Enqueue(beam with { Direction = Directions.Up });
                    beams.Enqueue(beam with { Direction = Directions.Down });
                    break;
                default: 
                    beams.Enqueue(beam);
                    break;
            }
        }

        return tiles.Count - 1;
    }

    [Flags]
    internal enum Directions
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8
    }
}


