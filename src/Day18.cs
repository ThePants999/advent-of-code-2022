namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
using System.Linq;

public class Day18 : Day
{
    private Space _space = new Space();

    public Day18(AOCEnvironment env) : base("2022", 18, env) { }

    protected override string ExecutePart1()
    {
        foreach (string line in new LineReader(() => new StringReader(Input)))
        {
            string[] chunks = line.Split(',');
            Position3D pos = new Position3D(int.Parse(chunks[0]), int.Parse(chunks[1]), int.Parse(chunks[2]));
            _space.AddCube(pos);
        }

        return _space.SurfaceArea().ToString();
    }

    protected override string ExecutePart2()
    {
        return _space.ExternalSurfaceArea().ToString();
    }

    protected override string? GetExampleInput()
    {
        return @"2,2,2
1,2,2
3,2,2
2,1,2
2,3,2
2,2,1
2,2,3
2,2,4
2,2,6
1,2,5
3,2,5
2,1,5
2,3,5";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "64";
    }

    protected override string? GetExamplePart2Answer()
    {
        return "58";
    }
}

internal class Cube
{
    internal Position3D Position { get; init; }
    private int _openSides;
    internal int OpenSides { get { return _openSides; } }

    internal Cube(Position3D position)
    {
        Position = position;
        _openSides = 6;
    }

    internal void AdjacentCubeCreated()
    {
        _openSides--;
    }
}

internal class Space
{
    private Dictionary<Position3D, Cube> _cubes = new Dictionary<Position3D, Cube>();

    internal Space() { }

    internal void AddCube(Position3D position)
    {
        Cube cube = new Cube(position);

        Position3D[] adjacencies = position.AdjacentPositions();
        foreach (Position3D adjacentPosition in adjacencies)
        {
            Cube? adjacentCube;
            if (_cubes.TryGetValue(adjacentPosition, out adjacentCube))
            {
                adjacentCube.AdjacentCubeCreated();
                cube.AdjacentCubeCreated();
            }
        }
        _cubes.Add(position, cube);
    }

    internal int SurfaceArea()
    {
        return _cubes.Values.Select(cube => cube.OpenSides).Sum();
    }

    internal int ExternalSurfaceArea()
    {
        Space air = new Space();
        int minX = _cubes.Keys.Select(pos => pos.X).Min() - 1;
        int maxX = _cubes.Keys.Select(pos => pos.X).Max() + 1;
        int minY = _cubes.Keys.Select(pos => pos.Y).Min() - 1;
        int maxY = _cubes.Keys.Select(pos => pos.Y).Max() + 1;
        int minZ = _cubes.Keys.Select(pos => pos.Z).Min() - 1;
        int maxZ = _cubes.Keys.Select(pos => pos.Z).Max() + 1;

        Position3D pos = new Position3D(minX, minY, minZ);
        air.AddCube(pos);
        Stack<Position3D> stack = new Stack<Position3D>();
        stack.Push(pos);

        while (stack.Count > 0)
        {
            Position3D current = stack.Pop();

            Position3D[] adjacencies = current.AdjacentPositions();
            foreach (Position3D adjacentPos in adjacencies)
            {
                if (adjacentPos.X >= minX &&
                    adjacentPos.X <= maxX &&
                    adjacentPos.Y >= minY &&
                    adjacentPos.Y <= maxY &&
                    adjacentPos.Z >= minZ &&
                    adjacentPos.Z <= maxZ &&
                    !_cubes.ContainsKey(adjacentPos) &&
                    !air._cubes.ContainsKey(adjacentPos)) {

                    air.AddCube(adjacentPos);
                    stack.Push(adjacentPos);
                }
            }
        }

        int xDim = maxX - minX + 1;
        int yDim = maxY - minY + 1;
        int zDim = maxZ - minZ + 1;
        int outerCubeSurface = (xDim * yDim * 2) + (xDim * zDim * 2) + (yDim * zDim * 2);
        return air.SurfaceArea() - outerCubeSurface;
    }

    internal void Visualize()
    {
        int minX = _cubes.Keys.Select(pos => pos.X).Min();
        int maxX = _cubes.Keys.Select(pos => pos.X).Max();
        int minY = _cubes.Keys.Select(pos => pos.Y).Min();
        int maxY = _cubes.Keys.Select(pos => pos.Y).Max();
        int minZ = _cubes.Keys.Select(pos => pos.Z).Min();
        int maxZ = _cubes.Keys.Select(pos => pos.Z).Max();
        for (int z = minZ; z <= maxZ; z++)
        {
            Console.WriteLine($"----------\nLayer {z}\n\n");
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    if (_cubes.ContainsKey(new Position3D(x, y, z)))
                    {
                        Console.Write("#");
                    } else {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}