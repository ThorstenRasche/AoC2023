using System.Drawing;

namespace AoC23.Domain;

[Day(21)]
public class Day21Calculator : IDayCalculator
{
    public long CalculatePart1(string filePath)
    {
        var input = Input(filePath);
        var gridSize = GridSize(input);

        var start = FindStart(input, gridSize);
        var work = new HashSet<Coord> { start };

        for (int i = 0; i < 64; i++)
        {
            work = Expand(work, input, gridSize);
        }

        return work.Count;
    }

    

    public long CalculatePart2(string filePath)
    {
        var input = Input(filePath);
        var gridSize = GridSize(input);

        var start = FindStart(input, gridSize);
        var (grids, rem) = CalculateGridsAndRemainder(26501365, gridSize);

        var sequence = CalculateSequence(start, gridSize, rem, input);
        var (a, b, c) = CalculateCoefficients(sequence);

        return F(grids, a, b, c);
    }

    private static int GridSize(List<string> input) => input.Count == input[0].Length ? input.Count : throw new ArgumentOutOfRangeException();
    private static List<string> Input(string filePath) => File.ReadLines(filePath).ToList();
    private static long F(long n, int a, int b, int c) => (a * (n * n)) + (b * n) + c;
    Coord FindStart(List<string> input, int gridSize)
    {        
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (input[i][j] == 'S')
                    return new Coord(i, j);
            }
        }

        throw new InvalidOperationException("Startposition nicht gefunden.");
    }

    (int grids, int rem) CalculateGridsAndRemainder(int total, int size) =>        
        (total / size, total % size);

    List<int> CalculateSequence(Coord start, int gridSize, int rem, List<string> input)
    {
        var sequence = new List<int>();
        var work = new HashSet<Coord> { start };
        var steps = 0;

        for (var n = 0; n < 3; n++)
        {
            for (; steps < (n * gridSize) + rem; steps++)
            {
                work = Expand(work, input, gridSize);
            }
            sequence.Add(work.Count);
        }

        return sequence;
    }

    HashSet<Coord> Expand(HashSet<Coord> current, List<string> input, int gridSize)
    {        
        var result = new HashSet<Coord>();
        foreach (var it in current)
        {
            foreach (var dir in new[] { Dir.N, Dir.S, Dir.E, Dir.W })
            {
                var next = it.Move(dir);
                if (IsValid(next, input, gridSize))
                    result.Add(next);
            }
        }
        return result;
    }

    bool IsValid(Coord coord, List<string> input, int gridSize)
    {        
        int x = ((coord.X % gridSize) + gridSize) % gridSize;
        int y = ((coord.Y % gridSize) + gridSize) % gridSize;
        return input[x][y] != '#';
    }

    (int a, int b, int c) CalculateCoefficients(List<int> sequence)
    {        
        var c = sequence[0];
        var aPlusB = sequence[1] - c;
        var fourAPlusTwoB = sequence[2] - c;
        var twoA = fourAPlusTwoB - (2 * aPlusB);
        var a = twoA / 2;
        var b = aPlusB - a;

        return (a, b, c);
    }

    internal enum Dir
    {
        N, 
        S, 
        E, 
        W  
    }

    internal record Coord(int X, int Y)
    {        
        public Coord Move(Dir dir, int dist = 1)
        {
            return dir switch
            {
                Dir.N => new Coord(X - dist, Y), 
                Dir.S => new Coord(X + dist, Y), 
                Dir.E => new Coord(X, Y + dist), 
                Dir.W => new Coord(X, Y - dist), 
                _ => this
            };
        }
    }
}


