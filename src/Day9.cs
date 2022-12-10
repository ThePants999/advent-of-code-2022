namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
//using System.Linq;

public class Day9 : Day
{
    Rope _rope = new Rope();

    public Day9(AOCEnvironment env) : base("2022", 9, env) { }

    protected override string ExecutePart1()
    {
        _rope.ApplyInstructions(Input);
        return _rope.Knot1VisitedCount.ToString();
    }

    protected override string ExecutePart2()
    {
        return _rope.Knot9VisitedCount.ToString();
    }

    protected override string? GetExampleInput()
    {
        return @"R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "13";
    }

    protected override string? GetExamplePart2Answer()
    {
        return null;
    }
}

internal enum Directions
{
    Up,
    Down,
    Left,
    Right
}

internal class Position
{
    private int _row;
    private int _col;
    internal int Row { get { return _row; } }
    internal int Col { get { return _col; } }

    internal Position(int row, int col)
    {
        _row = row;
        _col = col;
    }

    internal Position() : this(0, 0) { }

    internal Position Clone()
    {
        return new Position(_row, _col);
    }

    public override bool Equals(object? obj)
    {
        var item = obj as Position;
        if (item == null)
        {
            return false;
        }

        return _row == item._row && _col == item._col;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_row, _col);
    }

    internal void Move(Directions dir)
    {
        switch (dir)
        {
            case Directions.Up:
                _row -= 1;
                break;

            case Directions.Down:
                _row += 1;
                break;

            case Directions.Left:
                _col -= 1;
                break;

            case Directions.Right:
                _col += 1;
                break;
        }
    }

    internal void MoveTowards(Position other)
    {
        if (Math.Abs(_row - other._row) > 1 || Math.Abs(_col - other._col) > 1)
        {
            // Not adjacent, move
            if (_row < other._row)
            {
                _row += 1;
            } else if (_row > other._row)
            {
                _row -= 1;
            }
            if (_col < other._col)
            {
                _col += 1;
            } else if (_col > other._col)
            {
                _col -= 1;
            }
        }
    }
}

internal class Rope
{
    private Position[] _knots = new Position[10];
    private HashSet<Position> _knot1Visited = new HashSet<Position>();
    internal int Knot1VisitedCount { get { return _knot1Visited.Count; } }
    private HashSet<Position> _knot9Visited = new HashSet<Position>();
    internal int Knot9VisitedCount { get { return _knot9Visited.Count; } }

    internal Rope()
    {
        for (int i = 0; i < _knots.Length; i++)
        {
            _knots[i] = new Position();
        }
        _knot1Visited.Add(new Position());
        _knot9Visited.Add(new Position());
    }

    internal void ApplyInstructions(string instructions)
    {
        foreach (string line in new LineReader(() => new StringReader(instructions)))
        {
            string[] chunks = line.Split(' ');
            Directions dir = chunks[0][0] switch
            {
                'U' => Directions.Up,
                'D' => Directions.Down,
                'L' => Directions.Left,
                'R' => Directions.Right,
                _ => throw new InputException($"Invalid direction {chunks[0]}"),
            };
            int count = int.Parse(chunks[1]);
            for (int i = 0; i < count; i++)
            {
                _knots[0].Move(dir);
                for (int knot = 1; knot < _knots.Length; knot++)
                {
                    _knots[knot].MoveTowards(_knots[knot - 1]);
                }
                _knot1Visited.Add(_knots[1].Clone());
                _knot9Visited.Add(_knots[9].Clone());
            }
        }
    }
}