using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;

namespace AoC23.Domain;

[Day(10)]
public class Day10Calculator : IDayCalculator
{
    private (int, int) res;
    public long CalculatePart1(string filePath)
    {
        return Day10.Run(filePath).Item1;
    }

    public long CalculatePart2(string filePath)
    {
        return Day10.Run(filePath).Item2;
    }

    internal class Day10
    {
        private static int Width { get; set; } = 0;
        private static int Height { get; set; } = 0;

        private static string[] gameMap = [];
        private static StringBuilder[] stateMap = [];

        private record struct Point(int X, int Y);

        private static Direction[] AllDirections = { Direction.East, Direction.South, Direction.North, Direction.West };

        private static Point[] AllPoints = [];

        public static (int, int) Run(string fileName)
        {
            gameMap = GetLines(fileName).Select(s => s + ".").ToArray();
            Width = gameMap.First().Length;
            Height = gameMap.Length;
           
            string stateLineTemplate = new StringBuilder().Append((char)CellState.None, Width).ToString();
            stateMap = GetLines(fileName).Select(s => new StringBuilder(stateLineTemplate)).ToArray();
                        
            List<Point> points = new List<Point>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Point point = new Point(x, y);
                    points.Add(point);
                }
            }
            AllPoints = points.ToArray();

            int steps = Part1();
            Console.WriteLine("Part 1:" + steps / 2);

            int countEnclosed = Part2();
            Console.WriteLine("Part 2:" + countEnclosed);

            return (steps / 2, countEnclosed);            

        }

        private static int Part1()
        {            
            int steps = 0;

            Point currentPosition = FindStart();
            Direction lastDirection = Direction.None;
            Tile currentTile = Tile.Start;
            do
            {                
                Direction forbiddenDirection = getOppositeDirection(lastDirection);

                foreach (var direction in AllDirections)
                {
                    if (direction == forbiddenDirection) continue;

                    Point testPosition = Move(currentPosition, direction);
                    Tile testTile = GetGameTile(testPosition);

                    if (CanMove(currentTile, testTile, direction))
                    {
                        MarkInsideIfNotVisited(Move(currentPosition, getOrthogonalDirection(direction)));

                        currentPosition = testPosition;
                        currentTile = testTile;
                        lastDirection = direction;

                        SetCellState(currentPosition, CellState.Visited);
                        MarkInsideIfNotVisited(Move(currentPosition, getOrthogonalDirection(direction)));

                        break;
                    }
                }

                steps++;
            } while (currentTile != Tile.Start);
            return steps;
        }

        private static int Part2()
        {            
            foreach (var point in AllPoints)
            {
                if (GetCellState(point) == CellState.Inside)
                {
                    FloodInside(point);
                }
            }
            
            foreach (var point in AllPoints)
            {
                if (GetCellState(point) != CellState.Visited && GetCellState(point) != CellState.Inside)
                {
                    SetCellState(point, CellState.Outside);
                }
            }
            
            bool inverse = GetCellState(new Point(Width - 1, 0)) == CellState.Inside;
            if (inverse)
            {
                foreach (var point in AllPoints)
                {
                    if (GetCellState(point) == CellState.Inside)
                    {
                        SetCellState(point, CellState.Outside);
                    }
                    else if (GetCellState(point) == CellState.Outside)
                    {
                        SetCellState(point, CellState.Inside);
                    }
                }
            }
          
            int countEnclosed = AllPoints.Select(p => GetCellState(p)).Where(s => s == CellState.Inside).Count();

            return countEnclosed;
        }

        private static Tile GetGameTile(Point point)
        {
            if (point.X < 0 || point.X >= Width) return Tile.Ground;
            if (point.Y < 0 || point.Y >= Height) return Tile.Ground;

            char tileChar = gameMap[point.Y][point.X];
            Tile tile = (Tile)tileChar;
            return tile;
        }

        private static CellState GetCellState(Point point)
        {
            if (point.X < 0 || point.X >= Width) return CellState.Invalid;
            if (point.Y < 0 || point.Y >= Height) return CellState.Invalid;

            return (CellState)stateMap[point.Y][point.X];
        }

        private static void SetCellState(Point point, CellState state)
        {
            if (point.X < 0 || point.X >= Width) return;
            if (point.Y < 0 || point.Y >= Height) return;

            stateMap[point.Y][point.X] = (char)state;
        }

        private static void MarkInsideIfNotVisited(Point point)
        {
            CellState current = GetCellState(point);
            if (current != CellState.Visited)
            {
                SetCellState(point, CellState.Inside);
            }
        }

        private static void FloodInside(Point point)
        {
            MarkInsideIfNotVisited(point);

            var neighbors = AllDirections.Select(d => Move(point, d));

            foreach (var neighbor in neighbors)
            {
                var state = GetCellState(neighbor);
                if (state != CellState.Visited && state != CellState.Inside && state != CellState.Invalid)
                {
                    FloodInside(neighbor);
                }
            }
        }


        private static Point FindStart()
        {
            foreach (var point in AllPoints)
            {
                if (GetGameTile(point) == Tile.Start)
                {
                    return point;
                }
            }

            throw new ApplicationException("No Start");
        }

        private static Point Move(Point point, Direction direction)
        {
            switch (direction)
            {
                case Direction.South:
                    return point with { Y = point.Y + 1 };
                case Direction.West:
                    return point with { X = point.X - 1 };
                case Direction.North:
                    return point with { Y = point.Y - 1 };
                case Direction.East:
                    return point with { X = point.X + 1 };
                default:
                    throw new ArgumentException("Unknown direction", nameof(direction));
            }
        }

        private static bool CanMove(Tile from, Tile to, Direction direction)
        {            
            if (from == Tile.Ground || to == Tile.Ground) return false;

            string fromName = from.ToString();
            string toName = to.ToString();
            string directionName = direction.ToString();
            string oppositeDirectionName = getOppositeDirection(direction).ToString();
            
            bool canGoFromTileInDirection = fromName.Contains(directionName) || from == Tile.Start;
            bool canGoToTileInDirection = toName.Contains(oppositeDirectionName) || to == Tile.Start;

            return canGoFromTileInDirection && canGoToTileInDirection;
        }

        private static Direction getOppositeDirection(Direction direction)
        {
            return direction switch
            {
                Direction.South => Direction.North,
                Direction.West => Direction.East,
                Direction.North => Direction.South,
                Direction.East => Direction.West,
                Direction.None => Direction.None,
                _ => throw new ArgumentException("Direction", nameof(direction))
            };
        }

        private static Direction getOrthogonalDirection(Direction direction)
        {
            return direction switch
            {
                Direction.South => Direction.West,
                Direction.West => Direction.North,
                Direction.North => Direction.East,
                Direction.East => Direction.South,
                Direction.None => Direction.None,
                _ => throw new ArgumentException("Direction", nameof(direction))
            };
        }

        public enum Direction
        {
            None,
            South,
            West,
            North,
            East
        }

        public enum Tile
        {
            Ground = '.',
            Start = 'S',
        }

        public enum CellState
        {
            Invalid = '\0',

            None = ' ',
            Visited = 'V',
            Inside = 'I',
            Outside = 'O',
        }

        private static string[] GetLines(string fileName)
        {
            return File.ReadAllLines(fileName);
        }

    }
}
