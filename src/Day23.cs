namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
using System.Linq;

public class Day23 : Day
{
    HashSet<Position> _elves = new HashSet<Position>();
    LinkedList<CompassDirections> _directions = new LinkedList<CompassDirections>();

    public Day23(AOCEnvironment env) : base("2022", 23, env) { }

    protected override string ExecutePart1()
    {
        _directions.AddLast(CompassDirections.N);
        _directions.AddLast(CompassDirections.S);
        _directions.AddLast(CompassDirections.W);
        _directions.AddLast(CompassDirections.E);

        string[] rows = Input.Split(Environment.NewLine);
        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row].Length; col++)
            {
                if (rows[row][col] == '#')
                {
                    _elves.Add(new Position(row, col));
                }
            }
        }

        for (int i = 0; i < 10; i++)
        {
            //Console.WriteLine($"== End of Round {i + 1} ==");
            PerformRound();
        }

        int numRows = _elves.Select(elf => elf.Row).Max() - _elves.Select(elf => elf.Row).Min() + 1;
        int numCols = _elves.Select(elf => elf.Col).Max() - _elves.Select(elf => elf.Col).Min() + 1;

        return ((numRows * numCols) - _elves.Count).ToString();
    }

    protected override string ExecutePart2()
    {
        return "";
    }

    private void PerformRound()
    {
        Dictionary<Position, List<Position>> proposals = new Dictionary<Position, List<Position>>();

        foreach (Position elf in _elves)
        {
            // An elf with no adjacent elves at all does not move.
            bool adjacentElf = false;
            foreach (Position pos in elf.AllAdjacentPositions())
            {
                if (_elves.Contains(pos))
                {
                    adjacentElf = true;
                    break;
                }
            }
            if (!adjacentElf)
            {
                continue;
            }

            foreach (CompassDirections direction in _directions)
            {
                Position[] positionsToCheck = direction switch
                {
                    CompassDirections.N => new Position[] {elf.AdjacentPosition(CompassDirections.N), elf.AdjacentPosition(CompassDirections.NE), elf.AdjacentPosition(CompassDirections.NW)},
                    CompassDirections.S => new Position[] {elf.AdjacentPosition(CompassDirections.S), elf.AdjacentPosition(CompassDirections.SE), elf.AdjacentPosition(CompassDirections.SW)},
                    CompassDirections.W => new Position[] {elf.AdjacentPosition(CompassDirections.W), elf.AdjacentPosition(CompassDirections.NW), elf.AdjacentPosition(CompassDirections.SW)},
                    CompassDirections.E => new Position[] {elf.AdjacentPosition(CompassDirections.E), elf.AdjacentPosition(CompassDirections.NE), elf.AdjacentPosition(CompassDirections.SE)},
                    _ => throw new Exception()
                };

                if (!_elves.Contains(positionsToCheck[0]) && !_elves.Contains(positionsToCheck[1]) && !_elves.Contains(positionsToCheck[2]))
                {
                    // No adjacent elves in this direction - propose moving this way.
                    Position proposedPosition = elf.AdjacentPosition(direction);
                    List<Position>? proposalsInThisPosition;
                    if (proposals.TryGetValue(proposedPosition, out proposalsInThisPosition))
                    {
                        // This is the first elf proposing to move here.
                        proposalsInThisPosition.Add(elf);
                    } else {
                        // At least one elf is already proposing to move here; add this one.
                        proposalsInThisPosition = new List<Position>();
                        proposalsInThisPosition.Add(elf);
                        proposals.Add(proposedPosition, proposalsInThisPosition);
                    }
                    break;
                }
            }
        }

        foreach (KeyValuePair<Position, List<Position>> entry in proposals)
        {
            if (entry.Value.Count == 1)
            {
                // Only one elf wanted to move here - make it so.
                System.Diagnostics.Debug.Assert(!_elves.Contains(entry.Key));
                _elves.Remove(entry.Value[0]);
                _elves.Add(entry.Key);
            }
        }

        // Reorder the directions ready for the next round.
        CompassDirections oldPriorityDirection = _directions.First()!;
        _directions.RemoveFirst();
        _directions.AddLast(oldPriorityDirection);

        /*int minRow = _elves.Select(elf => elf.Row).Min();
        int maxRow = _elves.Select(elf => elf.Row).Max();
        int minCol = _elves.Select(elf => elf.Col).Min();
        int maxCol = _elves.Select(elf => elf.Col).Max();
        for (int row = minRow; row <= maxRow; row++)
        {
            for (int col = minCol; col <= maxCol; col++)
            {
                if (_elves.Contains(new Position(row, col)))
                {
                    Console.Write("#");
                } else {
                    Console.Write(".");
                }
            }
            Console.WriteLine();
        }*/
    }

    protected override string? GetExampleInput()
    {
        return @"....#..
..###.#
#...#.#
.#...##
#.###..
##.#.##
.#..#..";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "110";
    }

    protected override string? GetExamplePart2Answer()
    {
        return null;
    }
}